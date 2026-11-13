using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroScene : MonoBehaviour
{
    public IntroSequence intro;
    public DialogueSystem dialogue;

    public AudioSource presentationMusic;
    public AudioSource dialogueMusic;

    public GameObject transitionScreen;   // tu pantalla con la animación
    public float transitionTime = 2f;     // duración de tu animación en segundos

    void Start()
    {
        transitionScreen.SetActive(false);
        dialogueMusic.Stop();
        presentationMusic.Play();

        intro.Play(() => StartCoroutine(Transition()));
    }

    IEnumerator Transition()
    {
        presentationMusic.Stop();
        transitionScreen.SetActive(true);   // al activarse, su animación empieza
        yield return new WaitForSeconds(transitionTime);
        transitionScreen.SetActive(false);

        dialogueMusic.Play();
        dialogue.Play(() => SceneManager.LoadScene("Game"));
    }

    public void Skip()
    {
        SceneManager.LoadScene("Game");
    }
}