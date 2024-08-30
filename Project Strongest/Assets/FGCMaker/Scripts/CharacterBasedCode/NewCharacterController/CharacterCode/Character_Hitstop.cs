using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using DG.Tweening;

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
        float oneFrame = 1 / 60f;
        float actualWaitTime = rateOfIncrease * oneFrame;
        attacker._cAnimator.SetSelfFreeze();
        target._cAnimator.SetSelfFreeze();
        _cameraController.CallCameraShake(rateOfIncrease, lastAttack.hitstopValue);
        while (actualWaitTime > 0)
        {
            actualWaitTime -= (oneFrame);
            yield return new WaitForSeconds(oneFrame);
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
