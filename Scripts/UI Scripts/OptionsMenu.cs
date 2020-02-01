using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class OptionsMenu : MonoBehaviour
{
    public Slider MusicSlider;
    public Slider EffectsSlider;
    public Text MusicVolume;
    public Text EffectsVolume;

    private Slider[] sliders;
    private float[] options;

    private void OnEnable()
    {

        sliders = new Slider[2] { MusicSlider, EffectsSlider };

        options = SaveManager.LoadOptions();

        if (options != null)
        {
            for (int i = 0; i < sliders.Length; i++)
            {
                sliders[i].value = options[i];
            }
        }
        else
        {
            for (int i = 0; i < sliders.Length; i++)
            {
                sliders[i].value = 100;
            }
        }
    }
    public void MusicSliderValueChange()
    {
        MusicVolume.text = MusicSlider.value.ToString("0");
    }
    public void EffectsSliderValueChange()
    {
        EffectsVolume.text = EffectsSlider.value.ToString("0");
    }
    public void ApplySettings()
    {
        options = new float[2];
        options[0] = MusicSlider.value;
        options[1] = EffectsSlider.value;

        SaveManager.SaveOptions(options);
    }
}
