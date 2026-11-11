using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public GameObject panel;
    [SerializeField] Image character;
    public TMP_Text lineText;

    void Awake()
    {
        panel.SetActive(false);
    }

    public IEnumerator PlayLine(string line, float duration = 2.5f)
    {
        panel.SetActive(true);
        lineText.text = line;
        yield return new WaitForSeconds(duration);
        panel.SetActive(false);
    }
}
