using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject optionsPanel;

    public void Play()
    {
        SceneManager.LoadScene("GameStart");
    }

    public void OpenOptions()
    {
        optionsPanel.SetActive(true);
    }

    public void PlayChallenge()
    {
        SceneManager.LoadScene("Challenge");
    }

    public void CloseOptions()
    {
        optionsPanel.SetActive(false);
    }
}