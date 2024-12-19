using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FightingGame_FrameData;

public class Sliced_Affliction : Affliction
{
    [Range(1, 25), SerializeField] private int frameBetweenDotDamage;
    IEnumerator slicedRoutine;

    public override void ActivateAffliction(float value, AfflictionType _type)
    {
        base.ActivateAffliction(value, _type);
        if (slicedRoutine != null)
        {
            StopCoroutine(slicedRoutine);
            slicedRoutine = null;
        }
        slicedRoutine = SlashCoroutine(value);
        StartCoroutine(slicedRoutine);
    }
    public override void KillSingleAffliction()
    {
        CallEndFunc();
    }
    IEnumerator SlashCoroutine(float damageValue)
    {
        KillSingleAffliction();
        for (int i = 0; i < 8; i++)
        {
            _base.opponentPlayer._cDamageCalculator.ReceiveCustomAfflictionDamage(damageValue);
            yield return new WaitForSeconds(Base_FrameCode.ONE_FRAME * frameBetweenDotDamage);
        }
    }
}
