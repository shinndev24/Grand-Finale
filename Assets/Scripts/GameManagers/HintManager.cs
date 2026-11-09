using System.Collections.Generic;
using UnityEngine;

public class HintManager : MonoBehaviour
{
    public TMPro.TMP_Text hintText;
    private Dictionary<int, int> previousAssignment = new();

    public int ShowHint(List<NpcMovement> characters)
    {
        // Primera ronda: no hay ronda anterior
        if (previousAssignment.Count == 0)
        {
            hintText.text = "No previous round to recall...";
            return Random.Range(1, 8);  // cualquiera, no hay pista aún
        }

        // Elegir un slot que SÍ tenga pista de la ronda anterior
        var slots = new List<int>(previousAssignment.Keys);
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
