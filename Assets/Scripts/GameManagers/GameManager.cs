using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public List<NpcMovement> characters;
    public SlotPresenter presenter;
    public HintManager hints;
    public RoundTimer timer;
    public GenerateNumber generator;   // ← arrástralo en el inspector
    public Animation curtain;

    public int landed;

    private List<int> alive = new();
    private Dictionary<int, int> assignment;
    private int playerId;
    private int startPosition;
    private int k;
    private bool roundDone;

    void Start()
    {
        playerId = characters.Find(c => c.isPlayer).id;
        ResetGame();
    }

    void ResetGame()
    {
        alive.Clear();
        foreach (var c in characters) alive.Add(c.id);
        presenter.ShowAll();
        StartRound();
    }

    public void StartRound()
    {
        if (alive.Count == 1) { Debug.Log("You won! End."); return; }

        if (landed != 0) presenter.HideCharacter(landed);
        curtain.Play("Curtain up");

        assignment = DealPositions();
        presenter.PlaceCharacters(assignment);

        startPosition = hints.ShowHint(characters);
        k = generator.Roll();   // ← el MISMO numero que se ve en pantalla

        Debug.Log($"--- NUEVA RONDA ---");
        Debug.Log($"Vivos al empezar: {string.Join(", ", alive)}");
        Debug.Log($"Reparto (slot -> id): {string.Join(" | ", assignment.Select(kv => $"{kv.Key}->{kv.Value}"))}");
        Debug.Log($"El jugador es el id {playerId} y esta en el slot {GetSlotOf(playerId)}.");
        Debug.Log($"Numero de la ronda (k): {k}");

        roundDone = false;
        timer.StartTimer(ResolveRound);
    }

    Dictionary<int, int> DealPositions()
    {
        var freeSlots = new List<int> { 1, 2, 3, 4, 5, 6, 7 };
        Shuffle(freeSlots);

        var npcs = alive.FindAll(id => id != playerId);
        Shuffle(npcs);

        var result = new Dictionary<int, int> { { freeSlots[0], playerId } };
        for (int i = 0; i < npcs.Count; i++)
            result[freeSlots[i + 1]] = npcs[i];
        return result;
    }

    public void Choose(int chosenSlot)
    {
        if (roundDone) return;
        int playerSlot = GetSlotOf(playerId);
        if (chosenSlot == playerSlot) return;

        if (assignment.ContainsKey(chosenSlot))
        {
            int other = assignment[chosenSlot];
            assignment[chosenSlot] = playerId;
            assignment[playerSlot] = other;
        }
        else
        {
            assignment.Remove(playerSlot);
            assignment[chosenSlot] = playerId;
        }
        presenter.PlaceCharacters(assignment);

        ResolveRound();
    }

    public void Confirm()
    {
        if (roundDone) return;
        ResolveRound();
    }

    void ResolveRound()
    {
        if (roundDone) return;
        roundDone = true;
        timer.StopTimer();

        Debug.Log($"startPosition={startPosition}, k={k}");

        landed = EliminateFromSlot(startPosition, k);
        Debug.Log($"landed={landed}");
        alive.Remove(landed);

        if (landed == playerId)
        {
            Debug.Log("You died!");
            UnityEditor.EditorApplication.isPlaying = false;
            return;
        }
        StartCoroutine(RevealAndNext());
    }

    IEnumerator RevealAndNext()
    {
        curtain.Play("Curtain down");
        yield return new WaitForSeconds(1f);

        presenter.RevealDeath(landed);
        yield return new WaitForSeconds(1f);

        hints.RememberPositions(assignment);
        StartRound();
    }

    int EliminateFromSlot(int startSlot, int step)
    {
        var occupied = new List<int>();
        for (int s = 1; s <= 7; s++)
            if (assignment.ContainsKey(s) && alive.Contains(assignment[s]))
                occupied.Add(s);

        Debug.Log("===== ANTES DEL CONTEO =====");
        Debug.Log($"Numero de la ronda (k): {step}");
        Debug.Log($"La pista dice que empieza en el slot: {startSlot}");
        Debug.Log($"Slots ocupados, en orden: {string.Join(", ", occupied)}");
        Debug.Log($"Hay {occupied.Count} personajes vivos en total.");

        int startIndex = occupied.FindIndex(s => s >= startSlot);
        if (startIndex < 0) startIndex = 0;

        Debug.Log($"El conteo empieza en el slot {occupied[startIndex]}, que es la posicion 1.");
        Debug.Log($"Contamos {step} posiciones desde ahi, dando vueltas si hace falta.");

        int landedIndex = (startIndex + step - 1) % occupied.Count;
        int landedSlot = occupied[landedIndex];

        Debug.Log($"Prediccion: el conteo cae en el slot {landedSlot}.");
        Debug.Log($"En ese slot esta el id {assignment[landedSlot]}.");
        Debug.Log($"Prediccion: muere el id {assignment[landedSlot]}.");

        Debug.Log("===== DESPUES DEL CONTEO =====");
        Debug.Log($"Resultado real: muere el id {assignment[landedSlot]} en el slot {landedSlot}.");
        Debug.Log($"Vivos antes: {string.Join(", ", alive)}");
        Debug.Log($"Vivos despues: {string.Join(", ", alive.FindAll(x => x != assignment[landedSlot]))}");
        Debug.Log("=============================");

        return assignment[landedSlot];
    }

    int GetSlotOf(int id)
    {
        foreach (var kv in assignment)
            if (kv.Value == id) return kv.Key;
        return -1;
    }

    void Shuffle(List<int> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}
