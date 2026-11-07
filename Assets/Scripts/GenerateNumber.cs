using System.Collections.Generic;
using UnityEngine;

public class GenerateNumber : MonoBehaviour
{
    [SerializeField] TMPro.TMP_Text numberInScreen;
    private int randomNumber;
    public int CurrentNumber => randomNumber;

    void Awake()
    {
        CallNumber();   
        Debug.Log("Generated: " + randomNumber, this);
    }

    public void CallNumber()
    {
        randomNumber = Random.Range(1, 8);
        if(numberInScreen != null) numberInScreen.text = randomNumber.ToString();
    }

    public int EliminateOnLanding(List<int> positions, int startPlayer, int k)
    {
        if (positions == null || positions.Count == 0) return -1;

        int startIndex = positions.IndexOf(startPlayer);
        if (startIndex < 0) startIndex = 0;

        int unfortunateIndex = (startIndex + k - 1) % positions.Count; //Josephus
        int unfortunate = positions[unfortunateIndex];

         Debug.Log($"Empieza en jugador {startPlayer} (índice {startIndex}), " +
                  $"cuenta {k}, vivos {positions.Count} -> cae {unfortunate}");
        
        return unfortunate;
    }
}
