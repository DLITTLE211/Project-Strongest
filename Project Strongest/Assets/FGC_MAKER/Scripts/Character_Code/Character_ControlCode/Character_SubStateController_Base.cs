using System.Collections;
using System.Collections.Generic;
using FightingGame_FrameData;
using UnityEngine;

public class Character_SubStateController_Base : MonoBehaviour
{
    [SerializeField] protected GameObject personalCamera;
    [SerializeField] protected Camera orthoCamera;
    [SerializeField] protected Animator orthoCameraAnim;
    [SerializeField] protected Character_Base _base;
    [SerializeField] protected Character_Animator _cAnimator;
    protected float startSecondaryIdle = 10f;
    protected IEnumerator SecondIdleAnimRoutine;
    protected bool canPlaySecondIdle;
    public void SetStarterInformation(Character_Base newBase) 
    {
        _base = newBase;
    }
    public virtual void OnRoundReset() 
    {

    }
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
    public void PlayCameraAnimation(string animName = null) 
    {
        if(animName != null || animName != "") 
        {
            PlayCameraAnimationClip(animName);
        }
        else 
        {
            PlayCameraFocusAnimation();
        }
    }
    public void PlayCameraAnimationClip(string animName) 
    {
        orthoCameraAnim.Play(animName, 0,1);
    }
    public void PlayCameraFocusAnimation()
    {
    }
    public virtual void PlayActivateInstallProperties(CustomCallback callback = null)
    {

    }
    public virtual void PlayRoundWinAnimation() 
    {

    }
    public virtual void PlayVictoryWinAnimation()
    {

    }
    public virtual void SetCameraCanvas(Canvas _screenSpaceCanvas) 
    {

    }
}
