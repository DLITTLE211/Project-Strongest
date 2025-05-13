using System.Collections;
using System.Collections.Generic;
using FightingGame_FrameData;
using UnityEngine;

public class Character_SubStateController_Base : MonoBehaviour
{
    [SerializeField] private Character_Base _base;
    [SerializeField] private Character_Animator _cAnimator;
    private float startSecondaryIdle = 10f;
    IEnumerator SecondIdleAnimRoutine;
    private bool canPlaySecondIdle;
    public void PlaySecondaryAnimation()
    {
        if (canPlaySecondIdle)
        {
            if (SecondIdleAnimRoutine != null)
            {
                StopCoroutine(SecondIdleAnimRoutine);
                SecondIdleAnimRoutine = null;
            }
            SecondIdleAnimRoutine = PlaySecondIdleAnimation();
            StartCoroutine(SecondIdleAnimRoutine);
        }
    }
    public virtual bool VerifyInstallState() 
    {
        return true;
    }
    IEnumerator PlaySecondIdleAnimation() 
    {
        float time = 0;
        canPlaySecondIdle = false;
        while (time < startSecondaryIdle) 
        {
            if (!_base.isLockedPause) 
            {
                time += Base_FrameCode.ONE_FRAME;
            }
            yield return new WaitForSeconds(Base_FrameCode.ONE_FRAME);
        }
        _base.TriggerSecondaryIdleAnim();
        yield return new WaitForSeconds(Base_FrameCode.ONE_FRAME);
        canPlaySecondIdle = true;
    }
    public virtual void PlayActivateInstallProperties()
    {

    }
    public virtual void PlayRoundWinAnimation() 
    {

    }
    public virtual void PlayVictoryWinAnimation()
    {

    }
}
