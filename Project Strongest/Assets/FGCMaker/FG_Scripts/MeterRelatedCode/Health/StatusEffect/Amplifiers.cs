using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;
using FightingGame_FrameData;

[Serializable]
public class Amplifiers : StatusEffect 
{
    [Header("Amplifier Type")]
    public Effect_Amplify amplifier;
    public DurationType durationType;
    public FillType fillType;
    public ActiveState currentState = ActiveState.Inactive;
    [Space(15)]

    [Header("Fill Meter Variables")]
    public float fillRateInFrames;
    [Range(1, 20)]public int fillRate;
    [Range(10, 50)] public float activeDuration;
    [Space(15)]


    [Header("Percent Bonus Upon Activation")]
    [Range(5, 45)] public float percentBonus;
    [Space(15)]
    [Header("Perfectionist --ONLY-- Debuff")]
    public int _perfectionistDebuff;
    private void Start()
    {
        if(fillType == FillType.Instant) 
        {
            fillRate = 1;
            fillRateInFrames = 1f;
        }
        else 
        {
            fillRateInFrames = Base_FrameCode.ONE_FRAME * (1/fillRate);
        }
    }
    public void SendEffect()
    {
    }
}
public enum FillType 
{
    Instant,
    Standard,
}
