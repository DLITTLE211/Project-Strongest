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
    public void SetStartMeterValues(float _maxValue, float _startValue)
    {
        startValue = _startValue;
        currentValue = _startValue;
        maxValue = _maxValue;
        meterSlider.maxValue = maxValue;
        meterSlider.value = currentValue;

    }
    public void SetCurrentMeterValue(float value)
    {
        meterSlider.value = value;
    }
}
