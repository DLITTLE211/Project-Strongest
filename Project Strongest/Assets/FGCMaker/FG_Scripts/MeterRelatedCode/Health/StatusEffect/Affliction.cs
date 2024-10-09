using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;
using FightingGame_FrameData;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;

[Serializable]
public class Affliction : StatusEffect
{
    public Effect_Affliction affliction;
    public DurationType durationType;
    public ActiveState currentState;
    [SerializeField,Range(5,30)]private int activeDuration;
    public float duration;
    public float damageValue;
    public bool _isConsumed;
    public Slider durationSlider;
    [Header("TempValues")]
    public TMP_Text textField;
    public void SetTextValue() 
    {
        textField.text = $"{affliction.ToString()[0]}{affliction.ToString()[1]}";
    }
    public void ActivateAffliction(Callback endFunc) 
    {
        if (durationType != DurationType.Permenant)
        {
            durationSlider.DOValue(0, duration).SetEase(Ease.Linear).OnComplete(() =>
            {
                endFunc();
            });
        }
    }
    public void SetDurationValues() 
    {
        if(durationType == DurationType.Permenant) 
        {
            activeDuration = -1;
            duration = 9999;
        }
        else 
        {
            duration = activeDuration;
        }
    }
}
[Serializable]
public enum AfflictionApplictionType 
{

}
