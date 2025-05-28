using System.Collections;
using System.Collections.Generic;
using FightingGame_FrameData;
using UnityEngine;
using System;
using DG.Tweening;
using UnityEngine.Rendering;

public class Character_SubStateController_Base : MonoBehaviour
{
    [SerializeField] protected GameObject personalCamera;
    [SerializeField] protected Canvas _screenSpaceCanvas;
    [SerializeField] protected Camera perspectiveCamera;
    [SerializeField] protected Camera orthoCamera;
    [SerializeField] protected Animator orthoCameraAnim;
    [SerializeField] protected Character_Base _base;
    [SerializeField] protected Character_Animator _cAnimator;
    protected float startSecondaryIdle = 10f;
    protected IEnumerator SecondIdleAnimRoutine;
    protected bool canPlaySecondIdle;
    List<string> cameraLayers;
    public void SetStarterInformation(Character_Base newBase) 
    {
        _base = newBase;
        _screenSpaceCanvas = newBase.screenSpaceCanvas;
        cameraLayers = new List<string>();
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
    public void PlayCameraAnimation(CustomCallback func) 
    {
        AnimationClip cameraClip = func._cameraAnimation;
        if (cameraClip != null)
        {
            orthoCameraAnim.enabled = true;
            PlayCameraAnimationClip(func);
        }
    }
    public void EndCameraAnimation()
    {
        ResetCameraCanvas();
    }
    public void PlayCameraAnimationClip(CustomCallback animName) 
    {
        personalCamera.SetActive(true); 
        SetCameraCanvas();
        orthoCamera.cullingMask = LayerMask.GetMask(ReturnLayerMask(animName.renderOpponent));
        personalCamera.transform.position = _base._mainGameCamera.ReturnCameraPos();
        string animationName = $"{animName._cameraAnimation.name}";
        int hash = Animator.StringToHash(animationName);
        orthoCameraAnim.CrossFade(hash, 0, 0);
    }
    string[] ReturnLayerMask(bool renderOpponent) 
    {
        if (renderOpponent) 
        {
            return new List<string> { "Default", "TransparentFX", "Ignore Raycast", "Outlined Objects", "Water", "UI", "Outlined Player1", "Outlined Player2" }.ToArray();
        }
        else 
        {
            if (_base.playerID == 0)
            {
                return new List<string> { "Default", "TransparentFX", "Ignore Raycast", "Outlined Objects", "Water", "UI", "Outlined Player1"}.ToArray();
            }
            else 
            {
                return new List<string> { "Default", "TransparentFX", "Ignore Raycast", "Outlined Objects", "Water", "UI", "Outlined Player2" }.ToArray();
            }
            
        }
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
    public void ResetCameraCanvas() 
    {
        if (personalCamera.activeInHierarchy) 
        {
            personalCamera.SetActive(false);
        }
        if (_screenSpaceCanvas.worldCamera == orthoCamera)
        {
            _base._mainGameCamera.ResetCanvas();
        }
    }
    public void SetCameraCanvas() 
    {
        _screenSpaceCanvas.worldCamera = orthoCamera;
    }
}
