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
    public Character_Base _base;
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
    Callback endFunc;
    public AfflictionType afflictionType;
    public void SetTextValue() 
    {
        textField.text = $"{affliction.ToString()[0]}{affliction.ToString()[1]}";
    }
    public void ActivateAffliction(Callback _endFunc) 
    {
        if (durationType != DurationType.Permenant)
        {
            endFunc = _endFunc;
            durationSlider.DOValue(0, duration).SetEase(Ease.Linear).OnComplete(() =>
            {
                CallEndFunc();
            });
        }
    }
    public void CallEndFunc() 
    {
        if (endFunc != null)
        {
            endFunc();
            endFunc = null;
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
    public virtual void KillSingleAffliction()
    {
    }
    public virtual void ActivateAffliction(float value,AfflictionType _type = AfflictionType.Damage)
    {
        if (_type != afflictionType) { return; }
    }
    public virtual void ForceEndAffliction() 
    {
    }
}
[Serializable]
public enum AfflictionType 
{
    Damage,
    Stun,
    Meter,
    Mobility,
}
