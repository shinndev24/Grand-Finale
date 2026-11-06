using UnityEngine;

public class GenerateNumber : MonoBehaviour
{
    [SerializeField] TMPro.TMP_Text numberInScreen;
    private int randomNumber;

    void Start()
    {
        CallNumber();
    }
    public void CallNumber()
    {
        randomNumber = Random.Range(1, 7);
        Debug.Log("number = " + randomNumber);

        numberInScreen.text = randomNumber.ToString();
    }
}
