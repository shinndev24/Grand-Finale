using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroScene : MonoBehaviour
{
    public DialogueSystem dialogue;

    void Start()
    {
        dialogue.Play(() => SceneManager.LoadScene("Game"));
    }
}