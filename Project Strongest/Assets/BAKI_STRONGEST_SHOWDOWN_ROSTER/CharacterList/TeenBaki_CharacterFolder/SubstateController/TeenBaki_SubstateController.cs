using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FightingGame_FrameData;

public class TeenBaki_SubstateController : Character_SubStateController_Base
{
    public bool demonActivation;
    public IntroAnimationSequence _introAnimation;
    public VictoryAnimation _victoryAnimation;
    [SerializeField] private GameObject _sodaBottle;

    public override void PlayActivateInstallProperties(CustomCallback callback = null)
    {
        demonActivation = true;
    }
    public override void OnRoundReset()
    {
        _sodaBottle.gameObject.SetActive(false);
        demonActivation = false;
    }
    public override void PlayIntroAnimation()
    {
        _sodaBottle.gameObject.SetActive(false);
        StartCoroutine(PlayIntroSequence(_introAnimation, timeBetweenAnims, base.PlayIntroAnimation));
    }
    public override bool VerifyInstallState()
    {
        return demonActivation;
    }
    public override void PlayVictoryWinAnimation(Callback endFunc)
    {
        _base.Deactivate();
        _base._cStateMachine.enabled = false;
        _victoryAnimation._animName = _victoryAnimation._animationClip.name;
        _victoryAnimation.activatePointInFrames = _victoryAnimation.activatePoint * Time.smoothDeltaTime;
        _victoryAnimation._animLength = _victoryAnimation._animationClip.length;
        _victoryAnimation._animHash = Animator.StringToHash(_victoryAnimation._animName);
        StartCoroutine(PlayAnimSequence(endFunc));
    }

    IEnumerator PlayAnimSequence(Callback endFunc)
    {
        float frameCount = 0;
        _cAnimator.SetCanTransitionIdle(false);
        bool pointHit = false;
        float waitTime = Time.smoothDeltaTime;
        float endingFrame = _victoryAnimation._animLength;
        PlayCameraAnimationClip(_victoryAnimation._cameraAnimationClip.name,false);
        _cAnimator.PlayNextAnimation(_victoryAnimation._animHash, 0.25f);
        while (frameCount <= endingFrame)
        {
            #region Bottle Activation Check
            if (frameCount > _victoryAnimation.activatePointInFrames && !pointHit)
            {
                pointHit = true;
                _sodaBottle.gameObject.SetActive(true);
            }
            frameCount += waitTime;
            yield return new WaitForSeconds(waitTime);
            #endregion
        }
        endFunc();
    }
}
