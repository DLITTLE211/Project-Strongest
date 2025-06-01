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
        slicedRoutine = SlashCoroutine(5.5f);
        StartCoroutine(slicedRoutine);
    }
    public override void KillSingleAffliction()
    {
        CallEndFunc();
    }
    IEnumerator SlashCoroutine(float damageValue)
    {
        for (int i = 0; i < 8; i++)
        {
            _base._cDamageCalculator.ReceiveCustomAfflictionDamage(damageValue);
            yield return new WaitForSeconds(Time.smoothDeltaTime * frameBetweenDotDamage);
        }
        KillSingleAffliction();
    }
}
