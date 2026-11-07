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

        startPosition = Random.Range(1, positions.Count + 1);
        generator.CallNumber();
        roundDone = false;

        foreach (var b in positions) b.interactable = true;

        Debug.Log($"Ronda: {alive.Count} vivos. Empieza en slot {startPosition}, número {generator.CurrentNumber}");
    }

    public void Choose(int chosenSlot)
    {
        if (roundDone) return;
        roundDone = true;
        positionChosen = chosenSlot;

        foreach (var b in positions) b.interactable = false;

        // 1. El jugador ocupa el slot elegido
        assignment = new Dictionary<int, int>();
        assignment[chosenSlot] = playerId;

        // 2. Los NPCs vivos se barajan en los slots libres
        var npcs = alive.FindAll(id => id != playerId);
        ShuffleList(npcs);

        int index = 0;
        for (int s = 1; s <= positions.Count; s++)
        {
            if (s == chosenSlot) continue;
            if (index >= npcs.Count) break;
            assignment[s] = npcs[index++];
        }

        // 3. Colocar sin volver a mostrar a nadie
        presenter.PlaceCharacters(assignment);

        StartCoroutine(ResolveAfterDelay());
    }

    IEnumerator ResolveAfterDelay()
    {
        yield return new WaitForSeconds(0.6f);

        landed = EliminateFromSlot(startPosition, generator.CurrentNumber);

        presenter.HideCharacter(landed);
        alive.Remove(landed);

        if (landed == playerId)
        {
            Debug.Log("You died!");
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
            yield break;
        }

        Debug.Log($"Sobrevives. Muere {landed}.");
        yield return new WaitForSeconds(0.4f);
        StartRound();
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
}
