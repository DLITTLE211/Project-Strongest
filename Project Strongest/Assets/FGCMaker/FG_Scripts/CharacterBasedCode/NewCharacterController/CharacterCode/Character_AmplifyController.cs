using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Character_AmplifyController : MonoBehaviour
{
    [SerializeField] private Amplifiers chosenAmplifier;
    [SerializeField] private Slider _amplifySlider;
    public void SetChosenAmplifier(Amplifiers _chosenAmplifier)
    {
        chosenAmplifier = _chosenAmplifier;
    }
    public void AllowFillMeter() 
    {
        _amplifySlider.DOValue(1f,chosenAmplifier.fillTime);
    }
    public void DrainMeter()
    {
        if (chosenAmplifier.durationType != DurationType.Permenant)
        {
            _amplifySlider.DOValue(0f, chosenAmplifier.activeDuration);
        }
    }
}
