using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class MainMeterController 
{
    public Slider meterSlider;
    public float startValue,currentValue;
    public float maxValue;
    public void SetStartMeterValues(float value)
    {
        startValue = value;
        currentValue = startValue;
        maxValue = startValue;
        meterSlider.maxValue = startValue;
        meterSlider.value = currentValue;

    }
    public void SetCurrentMeterValue(float value)
    {
        meterSlider.value = value;
    }
}
