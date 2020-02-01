using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderToText : MonoBehaviour
{
    public Slider sliderUI;

    private void OnEnable()
    {
        GetComponent<Text>().text = sliderUI.value.ToString("0.00");
    }
    public void ShowSliderValue()
    {
        GetComponent<Text>().text = sliderUI.value.ToString("0.00");
    }
}