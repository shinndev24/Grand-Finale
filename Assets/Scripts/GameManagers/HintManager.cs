using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HintManager : MonoBehaviour
{
    public TMPro.TMP_Text hintText;
    private Dictionary<int, int> previousAssignment = new();

    public int ShowHint(List<NpcMovement> characters, List<int> alive)
    {
        if (previousAssignment.Count == 0)
        {
            hintText.text = "No previous round to recall...";
            return Random.Range(1, 8);
        }

        // Solo slots cuyo personaje siga vivo
        var slots = new List<int>();
        foreach (var kv in previousAssignment)
            if (alive.Contains(kv.Value)) slots.Add(kv.Key);

        // Si no queda ninguno (no debería), cae al primero disponible
        if (slots.Count == 0) slots.Add(previousAssignment.Keys.First());

        int startPosition = slots[Random.Range(0, slots.Count)];
        int hintCharacter = previousAssignment[startPosition];
        string name = characters.Find(c => c.id == hintCharacter)?.displayName ?? "someone";
        hintText.text = $"Counting will start from where the {name} was located in the previous round";
        return startPosition;
    }


    public void RememberPositions(Dictionary<int, int> assignment)
    {
        previousAssignment = new Dictionary<int, int>(assignment);
    }
}
