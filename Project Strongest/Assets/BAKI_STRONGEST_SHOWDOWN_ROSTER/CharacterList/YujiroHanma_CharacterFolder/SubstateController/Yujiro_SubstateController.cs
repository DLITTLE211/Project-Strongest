using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FightingGame_FrameData;
using System.Linq;

public class Yujiro_SubstateController : Character_SubStateController_Base
{
    public bool _demonActivation;

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
    public override void PlayVictoryWinAnimation()
    {
        _base.Deactivate();
        
        _base._cStateMachine.enabled = false;
        _victoryAnimation._animName = _victoryAnimation._animationClip.name;
        _victoryAnimation.activatePointInFrames = _victoryAnimation.activatePoint * Base_FrameCode.ONE_FRAME;
        _victoryAnimation._animLength = _victoryAnimation._animationClip.length;
        _victoryAnimation._animHash = Animator.StringToHash(_victoryAnimation._animName);
        StartCoroutine(PlayAnimSequence(_victoryAnimation));
    }

    IEnumerator PlayAnimSequence(VictoryAnimation _currentAction)
    {
        float frameCount = 0;
        _cAnimator.SetCanTransitionIdle(false);
        bool pointHit = false;
        float waitTime = Base_FrameCode.ONE_FRAME;
        float endingFrame = _victoryAnimation._animLength;
        _cAnimator.PlayNextAnimation(_victoryAnimation._animHash, 0.25f);
        orthoCameraAnim.Play($"{_victoryAnimation._cameraAnimationClip.name}", 0, 1);
        while (frameCount <= endingFrame)
        {
            #region Mobility Anim Checks
            if (frameCount > _victoryAnimation.activatePointInFrames && !pointHit)
            {
                pointHit = true;
            }
            frameCount += waitTime;
            yield return new WaitForSeconds(waitTime);
            #endregion
        }
    }
    public override bool VerifyInstallState()
    {
        return _demonActivation;
    }

}
