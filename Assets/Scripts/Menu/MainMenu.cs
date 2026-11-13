using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject optionsPanel;
    public GameObject creditsPanel;
    public GameObject journal;

    public void Play()
    {
        SceneManager.LoadScene("GameStart");
    }

    public void OpenOptions()
    {
        optionsPanel.SetActive(true);
    }

    public void OpenCredits()
    {
        creditsPanel.SetActive(true);
    }
    public void OpenJournal()
    {
        journal.SetActive(true);
    }

    public void PlayChallenge()
    {
        SceneManager.LoadScene("Challenge");
    }

    public void CloseOptions()
    {
        optionsPanel.SetActive(false);
    }

    public void CloseCredits()
    {
        creditsPanel.SetActive(false);
    }
    public void CloseJournal()
    {
        journal.SetActive(false);
    }
}