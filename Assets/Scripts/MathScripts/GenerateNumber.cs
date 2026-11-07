using UnityEngine;

public class GenerateNumber : MonoBehaviour
{
    [SerializeField] TMPro.TMP_Text numberInScreen;
    private int randomNumber;
    public int CurrentNumber => randomNumber;

    public int Roll()
    {
        randomNumber = Random.Range(1, 8);
        if (numberInScreen != null) numberInScreen.text = randomNumber.ToString();
        Debug.Log($"[GenerateNumber] Numero sorteado: {randomNumber}");
        return randomNumber;
    }

    public void Clear()
    {
        randomNumber = 0;
        if (numberInScreen != null) numberInScreen.text = "";
    }
}
