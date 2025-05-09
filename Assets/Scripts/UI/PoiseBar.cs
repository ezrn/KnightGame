using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PoiseBar : MonoBehaviour
{
    public Slider poiseSlider;

    public void SetSlider(float amount)
    {
        poiseSlider.value = amount;
    }

    public void SetSliderMax(float amount)
    {
        poiseSlider.maxValue = amount;
        SetSlider(amount);
    }
}
