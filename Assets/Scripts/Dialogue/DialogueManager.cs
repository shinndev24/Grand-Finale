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
        lineText.maxVisibleCharacters = 0;
        lineText.ForceMeshUpdate();
        int total = lineText.textInfo.characterCount;

        yield return null; // evita que un clic anterior cuente

        // Escritura (un clic la completa)
        int shown = 0;
        float timer = 0f;
        while (shown < total)
        {
            if (Clicked()) break;
            timer += Time.deltaTime;
            while (timer >= 0.03f && shown < total)
            {
                shown++;
                timer -= 0.03f;
            }
            lineText.maxVisibleCharacters = shown;
            yield return null;
        }
        lineText.maxVisibleCharacters = total;

        yield return null; // el clic que completó el texto no debe cerrarlo

        // Espera solo al clic
         while (!Clicked())
            yield return null;

        panel.SetActive(false);
    }

    bool Clicked()
    {
        return Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space);
    }
}
