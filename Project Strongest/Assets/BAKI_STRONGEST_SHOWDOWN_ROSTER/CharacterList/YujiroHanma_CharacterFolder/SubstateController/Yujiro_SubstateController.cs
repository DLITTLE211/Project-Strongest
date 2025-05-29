using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FightingGame_FrameData;
using System.Linq;

public class Yujiro_SubstateController : Character_SubStateController_Base
{
    public bool _demonActivation;
    public IntroAnimationSequence _introAnimation;
    public VictoryAnimation _victoryAnimation;
    [SerializeField] private GameObject _shirt;
    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.I)) 
        {
            PlayActivateInstallProperties();
        }
        if (Input.GetKeyUp(KeyCode.O))
        {
            OnRoundReset();
        }
    }

    public override void PlayActivateInstallProperties(CustomCallback callback = null)
    {
        _demonActivation = true;
        _shirt.SetActive(!_demonActivation);
        SetAnimClipsOnChange(1);
    }
    void SetAnimClipsOnChange(float inInstall)
    {
        _cAnimator.myAnim.SetFloat("InInstall", inInstall);
        _cAnimator.shadowAnim.SetFloat("InInstall", inInstall);
    }
    public override void OnRoundReset()
    {
        _demonActivation = false;
        _shirt.SetActive(!_demonActivation);
        SetAnimClipsOnChange(0);
    }
    public override void PlayIntroAnimation()
    {
        StartCoroutine(PlayIntroSequence(_introAnimation, timeBetweenAnims, base.PlayIntroAnimation));
    }

    public override void PlayVictoryWinAnimation(Callback endFunc)
    {
        _base.Deactivate();
        
        _base._cStateMachine.enabled = false;
        _victoryAnimation._animName = _victoryAnimation._animationClip.name;
        _victoryAnimation.activatePointInFrames = _victoryAnimation.activatePoint * Base_FrameCode.ONE_FRAME;
        _victoryAnimation._animLength = _victoryAnimation._animationClip.length;
        _victoryAnimation._animHash = Animator.StringToHash(_victoryAnimation._animName);
        StartCoroutine(PlayVictoryAnimSequence(endFunc));
    }

    IEnumerator PlayVictoryAnimSequence(Callback endFunc)
    {
        _cAnimator.SetCanTransitionIdle(false);
        if (!_base.opponentPlayer._cDamageCalculator.isDead) 
        {
            PlayCameraAnimationClip(_victoryAnimation._cameraAnimationClip.name, false);
        }
        else
        {
            PlayCameraAnimationClip(_victoryAnimation._cameraAnimationClip.name, false, true);
        }
        _cAnimator.PlayNextAnimation(_victoryAnimation._animHash, 0.25f);
        yield return new WaitForSeconds(_victoryAnimation._animLength+0.25f);
        endFunc();
    }
    public override bool VerifyInstallState()
    {
        return _demonActivation;
    }

}
