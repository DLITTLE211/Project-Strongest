using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Character_AmplifyController : MonoBehaviour
{
    [SerializeField] private Amplifiers chosenAmplifier;
    [SerializeField] private Slider _amplifySlider;
    public bool allowFill;
    public void SetChosenAmplifier(Amplifiers _chosenAmplifier)
    {
        chosenAmplifier = _chosenAmplifier;
    }
    private void Update()
    {
        if (allowFill && _amplifySlider.value != _amplifySlider.maxValue) 
        {
            AllowFillMeter();
        }
    }
    public void AllowFillMeter() 
    {
        _amplifySlider.value += chosenAmplifier.fillRateInFrames;
    }
    public void DrainMeter()
    {
        if (chosenAmplifier.durationType != DurationType.Permenant)
        {
            _amplifySlider.DOValue(0f, chosenAmplifier.activeDuration);
        }
    }
}
