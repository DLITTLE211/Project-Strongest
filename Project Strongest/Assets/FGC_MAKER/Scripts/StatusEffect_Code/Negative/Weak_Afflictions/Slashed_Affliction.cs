using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FightingGame_FrameData;


public class Slashed_Affliction : Affliction
{
    [Range(1,25),SerializeField] private int frameBetweenDotDamage;
    IEnumerator slashedRoutine;
    public override void ActivateAffliction(float value, AfflictionType _type)
    {
        base.ActivateAffliction(value, _type);
        if (slashedRoutine != null) 
        {
            StopCoroutine(slashedRoutine);
            slashedRoutine = null;
        }
        slashedRoutine = SlashCoroutine(value);
        StartCoroutine(slashedRoutine);
    }
    public override void KillSingleAffliction()
    {
        CallEndFunc();
    }
    IEnumerator SlashCoroutine(float damageValue) 
    {
        for(int i = 0; i < 5; i++)
        {
            _base.opponentPlayer._cDamageCalculator.ReceiveCustomAfflictionDamage(damageValue);
            yield return new WaitForSeconds(Base_FrameCode.ONE_FRAME * frameBetweenDotDamage);
        }
        KillSingleAffliction();
    }
}
