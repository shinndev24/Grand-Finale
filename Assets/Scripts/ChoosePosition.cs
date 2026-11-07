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
    private Dictionary<int, int> previousAssignment = new();  // id -> slot de la ronda anterior
    public Text hintText;   // el Text donde sale la pista


    private int playerId;
    private int startPosition;
    private bool roundDone;
    public float decisionTime = 7f;
    private IEnumerator timerCoroutine;
    public Text timerText;

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

        // 1. Repartir a los vivos, jugador en un slot aleatorio también
        var freeSlots = new List<int>();
        for (int s = 1; s <= positions.Count; s++) freeSlots.Add(s);
        ShuffleList(freeSlots);

        assignment = new Dictionary<int, int>();
        var npcs = alive.FindAll(id => id != playerId);
        ShuffleList(npcs);

        assignment[freeSlots[0]] = playerId;
        for (int i = 0; i < npcs.Count; i++)
            assignment[freeSlots[i + 1]] = npcs[i];

        presenter.PlaceCharacters(assignment);

        // 2. AHORA sortear la posición de salida
        startPosition = Random.Range(1, positions.Count + 1);

        // 3. Leer quién está en la salida y mostrar la pista
        int hintCharacter = assignment.ContainsKey(startPosition)
            ? assignment[startPosition]
            : -1;

        string hint;
        if (hintCharacter < 0 || !previousAssignment.ContainsKey(hintCharacter))
            hint = "El conteo empezará en un lugar incierto...";
        else
            hint = $"El conteo empezará donde estuvo {GetCharacterName(hintCharacter)} la ronda anterior (slot {previousAssignment[hintCharacter]}).";

        if (hintText != null) hintText.text = hint;

        generator.CallNumber();
        roundDone = false;

        foreach (var b in positions) b.interactable = true;

        if (timerCoroutine != null) StopCoroutine(timerCoroutine);
        timerCoroutine = Timer();
        StartCoroutine(timerCoroutine);

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
        Debug.Log($"startPosition={startPosition}, assignment keys={string.Join(",", new List<int>(assignment.Keys))}, prevCount={previousAssignment.Count}");
        if (roundDone) return;
        if (timerCoroutine != null) StopCoroutine(timerCoroutine);
        roundDone = true;
        positionChosen = GetSlotOf(playerId);

        foreach (var b in positions) b.interactable = false;

        landed = EliminateFromSlot(startPosition, generator.CurrentNumber);

        presenter.HideCharacter(landed);
        alive.Remove(landed);

        previousAssignment = new Dictionary<int, int>(assignment);

        if (landed == playerId)
        {
            Debug.Log("You died!");
            EditorApplication.isPlaying = false;

            return;
        }

        Debug.Log($"Sobrevives. Muere {landed}.");
        StartRound();
    }

    IEnumerator Timer()
    {
        float t = decisionTime;
        while (t > 0)
        {
            t -= Time.deltaTime;
            if (timerText != null) timerText.text = Mathf.CeilToInt(t).ToString();
            yield return null;
        }
        ResolveRound();   // se acabó el tiempo: no se mueve, se resuelve
    }

    string GetCharacterName(int id)
    {
        var c = characters.Find(x => x.id == id);
        return c != null ? c.displayName : "alguien";
    }


}
