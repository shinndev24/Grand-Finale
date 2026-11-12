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
    [SerializeField] private Image portrait;

    [Header("Diálogos")]
    [SerializeField] private DialogueLine[] lines;

    private bool waitingForClick;

    void Awake() => panel.SetActive(false);

    void Update()
    {
        if (waitingForClick && (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space)))
            waitingForClick = false;
    }

    public void Play(Action onComplete = null)
    {
        if (lines.Length == 0) return;
        StartCoroutine(Run(onComplete));
    }

    private IEnumerator Run(Action onComplete)
    {
        panel.SetActive(true);

        foreach (var line in lines)
        {
            portrait.sprite = line.portrait;
            portrait.enabled = line.portrait != null;
            lineText.text = line.text;

            yield return null;
            waitingForClick = true;
            while (waitingForClick) yield return null;
        }

        panel.SetActive(false);
        onComplete?.Invoke();
    }
}
