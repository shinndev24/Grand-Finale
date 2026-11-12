using UnityEngine;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    public Slider slider;

    void Start()
    {
        float saved = PlayerPrefs.GetFloat("volume", 1f); // 1 = volumen máximo por defecto
        slider.value = saved;
        AudioListener.volume = saved;
        slider.onValueChanged.AddListener(SetVolume);
    }

    void SetVolume(float value)
    {
        AudioListener.volume = value;
        PlayerPrefs.SetFloat("volume", value);
    }
}