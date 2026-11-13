using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueSystem : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text lineText;
    [SerializeField] private TMP_Text characterName;
    [SerializeField] private Image portrait;

    [Header("Diálogos")]
    [SerializeField] private DialogueLine[] lines;

    [Header("Escritura")]
    [SerializeField] private float typingSpeed = 0.03f; // segundos por letra

    private bool waitingForClick;
    private bool isTyping;
    private bool skipTyping;

    void Awake() => panel.SetActive(false);

    void Update()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            if (isTyping) skipTyping = true;              // completa el texto
            else if (waitingForClick) waitingForClick = false; // siguiente línea
        }
    }

    public void Play(Action onComplete = null)
    {
        if (lines.Length == 0)
        {
            onComplete?.Invoke();
            return;
        }
        StartCoroutine(Run(onComplete));
    }

    private IEnumerator Run(Action onComplete)
    {
        panel.SetActive(true);

        foreach (var line in lines)
        {
            characterName.text = line.speaker;
            portrait.sprite = line.portrait;
            portrait.enabled = line.portrait != null;

            yield return StartCoroutine(TypeLine(line.text));

            waitingForClick = true;
            while (waitingForClick) yield return null;
        }

        panel.SetActive(false);
        onComplete?.Invoke();
    }

    private IEnumerator TypeLine(string text)
    {
        lineText.text = text;
        lineText.maxVisibleCharacters = 0;
        lineText.ForceMeshUpdate();
        int total = lineText.textInfo.characterCount;

        yield return null; // evita que el clic anterior cuente como skip

        isTyping = true;
        skipTyping = false;

        for (int i = 1; i <= total; i++)
        {
            if (skipTyping) break;
            lineText.maxVisibleCharacters = i;
            yield return new WaitForSeconds(typingSpeed);
        }

        lineText.maxVisibleCharacters = total; // muestra el texto completo
        isTyping = false;                      // permite pasar a la siguiente línea
    }
}