using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RoundTimer : MonoBehaviour
{
    public float decisionTime = 7f;
    public Text timerText;

    private IEnumerator coroutine;
    private System.Action onTimeout;

    public void StartTimer(System.Action onTimeout)
    {
        this.onTimeout = onTimeout;
        if (coroutine != null) StopCoroutine(coroutine);
        coroutine = Timer();
        StartCoroutine(coroutine);
    }

    public void StopTimer()
    {
        if (coroutine != null) StopCoroutine(coroutine);
    }

    IEnumerator Timer()
    {
        float t = decisionTime;
        while (t > 0)
        {
            t -= Time.deltaTime;
            if (timerText != null) timerText.text = Mathf.CeilToInt(t).ToString();
            yield return null;
        }
        onTimeout?.Invoke();
    }
}
