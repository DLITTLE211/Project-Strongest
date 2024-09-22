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
    [Range(5,30)]public int activeDuration;
    public float damageValue;
    public bool _isConsumed;
    public Slider durationSlider;
    [Header("TempValues")]
    public TMP_Text textField;
    public void SendEffect(Callback endFunc) 
    {
        if (durationType != DurationType.Permenant)
        {
            durationSlider.DOValue(0, (float)activeDuration).OnComplete(() =>
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
        }
    }
}
[Serializable]
public enum AfflictionApplictionType 
{

}
