using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class Character_AmplifyController : MonoBehaviour
{
    [SerializeField] private Character_Base _base;
    [SerializeField] private Amplifiers chosenAmplifier;
    [SerializeField] private Slider _amplifySlider;
    [SerializeField] private TMP_Text _amplifyText;
    public bool allowFill;
    public void SetChosenAmplifier(Amplifiers _chosenAmplifier)
    {   
        if (_chosenAmplifier != null)
        {
            _amplifySlider.value = 0f;
            chosenAmplifier = _chosenAmplifier;
            _amplifyText.text = $"{_chosenAmplifier.amplifier.ToString()}";
            allowFill = true;
            _chosenAmplifier.SetFillVariables();
            _amplifySlider.targetGraphic.color = chosenAmplifier.meterColor;
        }
    }
    private void Update()
    {
        if (chosenAmplifier != null)
        {
            if (allowFill)
            {
                AllowFillMeter();
            }
        }
    }
    public void AllowFillMeter() 
    {
        if(_amplifySlider.value == _amplifySlider.maxValue) 
        {
            allowFill = false;
            return;
        }
        IState currenState = _base._cStateMachine._playerState.current.State;
        if(currenState == _base._cStateMachine.hitStateRef) 
        {
            return;
        }
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
