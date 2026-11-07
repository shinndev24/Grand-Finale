using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ChoosePosition : MonoBehaviour
{
    public List<Button> positions;
    public List<NpcMovement> characters;
    public GenerateNumber generator;
    public SlotPresenter presenter;

    public int landed;
    public int positionChosen;

    private List<int> alive = new();
    private Dictionary<int, int> assignment;
    private int playerId;
    private int startPosition;
    private bool roundDone;

    void Start()
    {
        playerId = characters.Find(c => c.isPlayer).id;
        ResetGame();
    }

    public void ResetGame()
    {
        alive.Clear();
        foreach (var c in characters) alive.Add(c.id);

        presenter.ShowAll();          // SOLO aquí se muestran todos
        roundDone = false;
        foreach (var b in positions) b.interactable = true;

        StartRound();
    }

    public void StartRound()
    {
        if (alive.Count == 1)
        {
            Debug.Log("You won! End.");
            return;
        }

        // Repartir a los vivos, jugador en un slot aleatorio también
        var freeSlots = new List<int>();
        for (int s = 1; s <= positions.Count; s++) freeSlots.Add(s);
        ShuffleList(freeSlots);

        assignment = new Dictionary<int, int>();
        var npcs = alive.FindAll(id => id != playerId);
        ShuffleList(npcs);

        // Jugador ocupa el primer slot barajado
        assignment[freeSlots[0]] = playerId;

        // NPCs en los siguientes slots barajados
        for (int i = 0; i < npcs.Count; i++)
            assignment[freeSlots[i + 1]] = npcs[i];

        presenter.PlaceCharacters(assignment);


        presenter.PlaceCharacters(assignment);

        startPosition = Random.Range(1, positions.Count + 1);
        generator.CallNumber();
        roundDone = false;

        foreach (var b in positions) b.interactable = true;

        Debug.Log($"Ronda: {alive.Count} vivos. Empieza en slot {startPosition}, número {generator.CurrentNumber}");
    }

    // El jugador pulsa un slot: intercambia con quien esté ahí
    public void Choose(int chosenSlot)
    {
        if (roundDone) return;
        if (chosenSlot == GetSlotOf(playerId)) return; // ya está ahí, no hace nada

        int playerSlot = GetSlotOf(playerId);

        if (assignment.ContainsKey(chosenSlot))
        {
            int other = assignment[chosenSlot];
            assignment[chosenSlot] = playerId;
            assignment[playerSlot] = other;
            Debug.Log($"Intercambio: jugador a slot {chosenSlot}, {other} a slot {playerSlot}.");
        }
        else
        {
            assignment.Remove(playerSlot);
            assignment[chosenSlot] = playerId;
            Debug.Log($"Jugador se mueve al slot vacío {chosenSlot}.");
        }

        presenter.PlaceCharacters(assignment);
        ResolveRound(); // mover = empezar la ronda
    }

    int GetSlotOf(int id)
    {
        foreach (var kv in assignment)
            if (kv.Value == id) return kv.Key;
        return -1;
    }

    int EliminateFromSlot(int startSlot, int k)
    {
        var occupied = new List<int>();
        for (int s = 1; s <= positions.Count; s++)
            if (assignment.ContainsKey(s) && alive.Contains(assignment[s])) occupied.Add(s);

        int startIndex = occupied.FindIndex(s => s >= startSlot);
        if (startIndex < 0) startIndex = 0;

        int landedSlot = occupied[(startIndex + k - 1) % occupied.Count];
        return assignment[landedSlot];
    }

    void ShuffleList(List<int> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
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
        positionChosen = GetSlotOf(playerId);

        foreach (var b in positions) b.interactable = false;

        landed = EliminateFromSlot(startPosition, generator.CurrentNumber);

        presenter.HideCharacter(landed);
        alive.Remove(landed);

        if (landed == playerId)
        {
            Debug.Log("You died!");
            EditorApplication.isPlaying = false;

            return;
        }

        Debug.Log($"Sobrevives. Muere {landed}.");
        StartRound();
    }
}
