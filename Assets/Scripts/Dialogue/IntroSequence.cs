using System;
using System.Collections;
using TMPro;
using UnityEngine;

[Serializable]
public class IntroSlide
{
    public GameObject visual;           // Fabio, Valerie, Jack...
    [TextArea(2, 5)] public string text;
}

public class IntroSequence : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject textPanel;  // panel de texto compartido
    [SerializeField] private TMP_Text introText;

    [Header("Diapositivas")]
    [SerializeField] private IntroSlide[] slides;

    [Header("Escritura")]
    [SerializeField] private float typingSpeed = 0.04f;

    void Awake()
    {
        textPanel.SetActive(false);
        foreach (var s in slides)
            if (s.visual != null) s.visual.SetActive(false);
    }

    public void Play(Action onComplete = null)
    {
        if (slides.Length == 0)
        {
            onComplete?.Invoke();
            return;
        }
        StartCoroutine(Run(onComplete));
    }

    private IEnumerator Run(Action onComplete)
    {
        textPanel.SetActive(true);

        foreach (var slide in slides)
        {
            if (slide.visual != null) slide.visual.SetActive(true); // reinicia su animación

            yield return TypeText(slide.text);

            yield return null; // el clic que completó el texto no debe avanzar
            while (!Clicked()) yield return null;

            if (slide.visual != null) slide.visual.SetActive(false);
        }

        textPanel.SetActive(false);
        onComplete?.Invoke();
    }

    private IEnumerator TypeText(string text)
    {
        introText.text = text;
        introText.maxVisibleCharacters = 0;
        introText.ForceMeshUpdate();
        int total = introText.textInfo.characterCount;

        yield return null; // evita que un clic anterior cuente

        int shown = 0;
        float timer = 0f;
        while (shown < total)
        {
            if (Clicked()) break; // un clic completa el texto
            timer += Time.deltaTime;
            while (timer >= typingSpeed && shown < total)
            {
                shown++;
                timer -= typingSpeed;
            }
            introText.maxVisibleCharacters = shown;
            yield return null;
        }
        introText.maxVisibleCharacters = total;
    }

    private bool Clicked()
    {
        return Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space);
    }
}