using System.Collections;
using System.Collections.Generic;
using FightingGame_FrameData;
using UnityEngine;
using System;

public class Character_SubStateController_Base : MonoBehaviour
{
    protected static readonly int secondaryIdleHash = Animator.StringToHash("Idle_2");
    protected static readonly int secondaryCrouchHash = Animator.StringToHash("Crouch_2");

    public float time;
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

    public bool introAnimationComplete;
    public bool introAudioComplete;

    public List<float> timeBetweenAnims;
    public void SetStarterInformation(Character_Base newBase) 
    {
        _base = newBase;
        _screenSpaceCanvas = newBase.screenSpaceCanvas;
        cameraLayers = new List<string>();
        introAudioComplete = false;
        canPlaySecondIdle = true;
    }
    public virtual void OnRoundReset() 
    {

    }
    public void ResetBools()
    {
        introAnimationComplete = false;
        introAudioComplete = false;
    }
    public void PlaySecondaryAnimation(Callback replayBasicIdle)
    {
        if (canPlaySecondIdle)
        {
            if (SecondIdleAnimRoutine != null)
            {
                StopCoroutine(SecondIdleAnimRoutine);
                SecondIdleAnimRoutine = null;
            }
            SecondIdleAnimRoutine = PlaySecondIdleAnimation(replayBasicIdle);
            StartCoroutine(SecondIdleAnimRoutine);
        }
    }
    public virtual bool VerifyInstallState() 
    {
        return true;
    }
    IEnumerator PlaySecondIdleAnimation(Callback replayBasicIdle) 
    {
        time = 0;
        canPlaySecondIdle = false;
        while (time < startSecondaryIdle) 
        {
            if (!_base.isLockedPause) 
            {
                time += (1f/ Base_FrameCode.ONE_FRAME);
            }
            yield return new WaitForSeconds((1f/ Base_FrameCode.ONE_FRAME));
        }
        if (allowPlaySecondIdle())
        {
            _base.TriggerSecondaryIdleAnim();
            CallSecondaryAnim();
            yield return new WaitForSeconds(0.2f);
            AnimatorClipInfo clipInfo = _base._cAnimator.myAnim.GetCurrentAnimatorClipInfo(0)[0];
            if (clipInfo.clip != null)
            {
                float length = clipInfo.clip.length;
                yield return new WaitForSeconds(length);
            }
            replayBasicIdle();
        }
        _base.allowSecondIdleAnim = false;
        canPlaySecondIdle = true;
    }
    bool allowPlaySecondIdle() 
    {
        return _base._cStateMachine.At_2Idle() && _base.allowSecondIdleAnim;
    }
    void CallSecondaryAnim() 
    {
        if (_base._subState == Character_SubStates.Controlled)
        {
            if (_base.ACTIVATED)
            {
                int lastInput = _base.ReturnMovementInputs().Button_State.directionalInput;
                int hashToPlay = lastInput <= 3 ? secondaryCrouchHash : secondaryIdleHash;
                _base._cAnimator.PlayNextAnimation(hashToPlay, 2 * (1 / 60f));
            }
        }
        else
        {
            if (_base.ACTIVATED)
            {
                _base._cAnimator.PlayNextAnimation(secondaryIdleHash, 2 * (1 / 60f));
            }
        }
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
    public void PlayCameraAnimationClip(string animName, bool renderCanvas = false, bool renderOpponent = false)
    {
        personalCamera.SetActive(true);
        if (renderCanvas)
        {
            SetCameraCanvas();
        }
        orthoCamera.cullingMask = LayerMask.GetMask(ReturnLayerMask(renderOpponent));
        personalCamera.transform.position = _base._mainGameCamera.ReturnCameraPos();
        int hash = Animator.StringToHash(animName);
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
    public virtual void PlayIntroAnimation()
    {
        introAnimationComplete = true;
        ResetCameraCanvas();
    }

    public IEnumerator PlayIntroSequence(IntroAnimationSequence sequence, List<float> delayTimeAfterCompletion, Callback endFunc)
    {
        bool allowTalkPointInBetween = false;
        if (sequence.introAnimationClips.Count > 1)
        {
            allowTalkPointInBetween = true;
        }
        for (int i = 0; i < sequence.introAnimationClips.Count; i++)
        {
            AnimationClip currentClip = sequence.introAnimationClips[i];
            _base._cAnimator.PlayNextAnimation(Animator.StringToHash(currentClip.name), 0);
            PlayCameraAnimationClip(sequence._cameraAnimationClips[i].name);
            yield return new WaitForSeconds(sequence.introAnimationClips[i].length);
            if (allowTalkPointInBetween)
            {
                if (i == sequence.introAnimationClips.Count - 1)
                {
                    StartCoroutine(PlayIntroDialogue(sequence, delayTimeAfterCompletion[i],endFunc));
                    yield return new WaitForSeconds(delayTimeAfterCompletion[i]);
                }
                else
                {
                    StartCoroutine(PlayIntroDialogue(sequence, delayTimeAfterCompletion[i], null));
                }
                yield return new WaitUntil(() => introAudioComplete);
            }
        }
        if (!allowTalkPointInBetween) 
        {
            StartCoroutine(PlayIntroDialogue(sequence, delayTimeAfterCompletion[0], endFunc));
            yield return new WaitUntil(() => introAudioComplete);
            yield break;
        }
        endFunc();
    }
    public IEnumerator PlayIntroDialogue(IntroAnimationSequence sequence, float delayAfterCompletion, Callback endFunc = null)
    {
        if (sequence.introDialogue != null && !introAudioComplete)
        {
            _base._cAudioManager.PlayNextAudioClip(sequence.introDialogue);
            yield return new WaitForSeconds(sequence.introDialogue.nextPlayedClip.length+0.25f);
            introAudioComplete = true;
        }
        else
        {
            introAudioComplete = true;
        }
        if (endFunc != null)
        {
            introAudioComplete = true;
            yield return new WaitForSeconds(delayAfterCompletion);
            endFunc();
        }
    }
    public virtual void PlayVictoryWinAnimation(Callback endFunc)
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
[Serializable]
public class IntroAnimationSequence
{
    public List<AnimationClip> introAnimationClips;
    public List<AnimationClip> _cameraAnimationClips;
    public AudioClipData introDialogue;
}