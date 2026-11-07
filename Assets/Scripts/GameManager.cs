using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public List<NpcMovement> characters;
    public SlotPresenter presenter;
    public HintManager hints;
    public RoundTimer timer;

    public int landed;

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

        assignment = DealPositions();
        presenter.PlaceCharacters(assignment);

        startPosition = Random.Range(1, 8);

        hints.ShowHint(assignment, startPosition);

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

    public void Choose(int chosenSlot) // el jugador pulsa un slot
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

    public void Confirm() // botón: no moverse
    {
        if (roundDone) return;
        ResolveRound();
    }

    void ResolveRound()
    {
        if (roundDone) return;
        roundDone = true;
        timer.StopTimer();

        landed = EliminateFromSlot(startPosition);
        presenter.HideCharacter(landed);
        alive.Remove(landed);

        if (landed == playerId)
        {
            Debug.Log("You died!");
            UnityEditor.EditorApplication.isPlaying = false;
            return;
        }

        hints.RememberPositions(assignment);
        StartRound();
    }

    int EliminateFromSlot(int startSlot)
    {
        var occupied = new List<int>();
        for (int s = 1; s <= 7; s++)
            if (assignment.ContainsKey(s) && alive.Contains(assignment[s])) occupied.Add(s);

        int startIndex = occupied.FindIndex(s => s >= startSlot);
        if (startIndex < 0) startIndex = 0;

        int landedSlot = occupied[(startIndex + 6 - 1) % occupied.Count]; // k = 6 fijo
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
