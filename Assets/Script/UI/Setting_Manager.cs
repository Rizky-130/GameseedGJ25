using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Setting_Manager : MonoBehaviour
{
    public Slider volumeSlider;
    public Slider brightnessSlider;

    public Image brightnessOverlay;
    public float maxOverlayAlpha = 0.85f;

    private const string VolumeKey = "Settings_Volume";
    private const string BrightnessKey = "Settings_Brightness";

    void Start()
    {
        float savedVolume = PlayerPrefs.GetFloat(VolumeKey, 1f);
        float savedBrightness = PlayerPrefs.GetFloat(BrightnessKey, 1f);

        if (volumeSlider != null)
        {
            volumeSlider.value = savedVolume;
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        }

        if (brightnessSlider != null)
        {
            brightnessSlider.value = savedBrightness;
            brightnessSlider.onValueChanged.AddListener(OnBrightnessChanged);
        }

        ApplyVolume(savedVolume);
        ApplyBrightness(savedBrightness);
    }
 
    public void OnVolumeChanged(float value)
    {
        ApplyVolume(value);
        PlayerPrefs.SetFloat(VolumeKey, value);
        PlayerPrefs.Save();
    }

    public void OnBrightnessChanged(float value)
    {
        ApplyBrightness(value);
        PlayerPrefs.SetFloat(BrightnessKey, value);
        PlayerPrefs.Save();
    }

    void ApplyVolume(float value)
    {
        AudioListener.volume = value;
    }

    void ApplyBrightness(float value)
    {
        if (brightnessOverlay == null)
            return;

        float alpha = (1f - value) * maxOverlayAlpha;

        Color c = brightnessOverlay.color;
        c.a = alpha;
        brightnessOverlay.color = c;
    }
}