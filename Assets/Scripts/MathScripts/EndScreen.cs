using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndScreen : MonoBehaviour
{
    public TMP_Text titleText;
    public TMP_Text roundsText;
    public TMP_Text gradeText;
    public TMP_Text phraseText;

    void Start()
    {
        titleText.text = GameResults.won ? "VICTORY" : "YOU DIED";
        roundsText.text = "Rounds survived: " + GameResults.roundsSurvived + " / " + GameResults.totalRounds;
        gradeText.text = "Final grade: " + GetGrade();
        phraseText.text = GetPhrase();
    }

    string GetGrade()
    {
        if (GameResults.won) return "S";
        if (GameResults.roundsSurvived == 0) return "F";
        if (GameResults.roundsSurvived <= 2) return "C";
        if (GameResults.roundsSurvived <= 4) return "B";
        return "A";
    }

    string GetPhrase()
    {
        if (GameResults.won) return "Should I change the formula?";
        if (GameResults.roundsSurvived == 0) return "You didn't even get to enjoy the show.";
        if (GameResults.roundsSurvived <= 2) return "Bad luck... or bad math?";
        if (GameResults.roundsSurvived <= 4) return "Not bad, not bad...";
        return "One more round, just one more!";
    }

    public void GoToMenu()
    {
        Debug.Log("Botón pulsado");
        SceneManager.LoadScene("MainMenu");
    }
}