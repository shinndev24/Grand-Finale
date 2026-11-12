using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public List<NpcMovement> characters;
    public SlotPresenter presenter;
    public HintManager hints;
    public RoundTimer timer;
    public GenerateNumber generator;
    public Animation curtain;
    public DialogueManager dialogue;
    public int landed;

    private List<int> alive = new();
    private Dictionary<int, int> assignment;
    private int playerId;
    private int startPosition;
    private int k;
    private bool roundDone;
    private bool revealing;

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
        Canvas.ForceUpdateCanvases();
        StartRound();
    }

    void StartRound()
    {
        if (alive.Count == 1) { Debug.Log("You won! End."); return; }

        if (landed != 0) presenter.HideCharacter(landed);

        revealing = false;
        curtain.Play("Curtain up");

        assignment = DealPositions();
        presenter.PlaceCharacters(assignment);

        startPosition = hints.ShowHint(characters, alive);
        k = generator.Roll();

        roundDone = false;
        timer.StartTimer(ResolveRound);
    }


    void ResolveRound()
    {
        if (roundDone) return;
        roundDone = true;
        timer.StopTimer();

        landed = EliminateFromSlot(startPosition, k);
        alive.Remove(landed);

        if (landed == playerId)
        {
            Debug.Log("You died!");
            SceneManager.LoadScene("EndGame");
            return;
        }

        StartCoroutine(RevealPhase());
    }

    IEnumerator RevealPhase()
    {
        revealing = true;

        curtain.Play("Curtain down");
        yield return new WaitForSeconds(1f);

        presenter.RevealDeath(landed);
        curtain.Play("Curtain up");
        yield return new WaitForSeconds(1f);

        yield return dialogue.PlayLine(PickLine(), 2.5f);

        curtain.Play("Curtain down");
        yield return new WaitForSeconds(1f);

        hints.RememberPositions(assignment);
        StartRound();
    }
    string PickLine()
    {
        string[] lines =
        {
            "Guess this one wasn't prepared",
            "Well? Who wants to go next?",
            "The show must go on!",
            "That didn't seem to be good for their stomach.."
        };
        return lines[Random.Range(0, lines.Length)];
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
        if (roundDone || revealing) return;
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
        if (roundDone || revealing) return;
        ResolveRound();
    }

    int EliminateFromSlot(int startSlot, int step)
    {
        var occupied = new List<int>();
        for (int s = 1; s <= 7; s++)
        {
            if (assignment.ContainsKey(s) && alive.Contains(assignment[s]))
            {
                occupied.Add(s);
            }
        }

        int startIndex = occupied.FindIndex(s => s >= startSlot);
        if (startIndex < 0) startIndex = 0;

        int landedIndex = (startIndex + step - 1) % occupied.Count;
        int landedSlot = occupied[landedIndex];

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
