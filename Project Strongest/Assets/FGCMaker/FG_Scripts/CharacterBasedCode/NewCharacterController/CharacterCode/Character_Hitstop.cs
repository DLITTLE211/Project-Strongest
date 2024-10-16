using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using DG.Tweening;
using FightingGame_FrameData;

public class Character_Hitstop : MonoBehaviour
{
    [SerializeField] private InGameCameraController _cameraController;
    [SerializeField] private Character_Animator p1, p2;
    IEnumerator hitStopSequence;
    private void Start()
    {
        hitStopSequence = null;
    }
    public void TriggerHitStop(Attack_BaseProperties lastAttack, float rateOfIncrease, Character_Base attacker, Character_Base target,Callback func)
    {
        if (hitStopSequence != null) 
        {
            StopCoroutine(hitStopSequence);
            hitStopSequence = null;
        }
        hitStopSequence = HandleHitStop(lastAttack, rateOfIncrease, attacker, target, func);
        StartCoroutine(hitStopSequence);
    }
    IEnumerator HandleHitStop(Attack_BaseProperties lastAttack, float rateOfIncrease, Character_Base attacker, Character_Base target, Callback func)
    {
        float actualWaitTime = rateOfIncrease * Base_FrameCode.ONE_FRAME;
        attacker._cAnimator.SetSelfFreeze();
        target._cAnimator.SetSelfFreeze();
        _cameraController.CallCameraShake(rateOfIncrease, lastAttack.attackMainStunValues.hitstopValue);
        while (actualWaitTime > 0)
        {
            actualWaitTime -= (Base_FrameCode.ONE_FRAME);
            yield return new WaitForSeconds(Base_FrameCode.ONE_FRAME);
        }
        yield return new WaitForSeconds(actualWaitTime);
        attacker._cAnimator.SetSelfUnfreeze();
        target._cAnimator.SetSelfUnfreeze();
        hitStopSequence = null;
        if (func != null)
        {
            func();
        }
    }
}
