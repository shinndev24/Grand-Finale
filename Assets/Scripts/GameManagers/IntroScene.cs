using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroScene : MonoBehaviour
{
    public IntroSequence intro;
    public DialogueSystem dialogue;

    void Start()
    {
        intro.Play(() =>
            dialogue.Play(() => SceneManager.LoadScene("Game")));
    }
}