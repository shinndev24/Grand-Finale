using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HintManager : MonoBehaviour
{
    public Text hintText;

    private Dictionary<int, int> previousAssignment = new();

    public void ShowHint(Dictionary<int, int> assignment, int startPosition)
    {
        if (!assignment.ContainsKey(startPosition))
        {
            hintText.text = "El conteo empezará en un lugar incierto...";
            return;
        }

        int hintCharacter = assignment[startPosition];
        if (!previousAssignment.ContainsKey(hintCharacter))
        {
            hintText.text = "El conteo empezará en un lugar incierto...";
            return;
        }

        string name = GetName(hintCharacter);
        hintText.text = $"El conteo empezará donde estuvo {name} la ronda anterior (slot {previousAssignment[hintCharacter]}).";
    }

    public void RememberPositions(Dictionary<int, int> assignment)
    {
        previousAssignment = new Dictionary<int, int>(assignment);
    }

    string GetName(int id)
    {
        // un diccionario id->nombre, o lo buscas en characters
        return "alguien";
    }
}
