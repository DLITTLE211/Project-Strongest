using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FightingGame_FrameData;

public class TeenBaki_SubstateController : Character_SubStateController_Base
{
    public bool demonActivation;
    public VictoryAnimation _victoryAnimation;
    [SerializeField] private GameObject _sodaBottle;

    public override void PlayActivateInstallProperties(CustomCallback callback = null)
    {
        demonActivation = true;
    }
    public override void OnRoundReset()
    {
        demonActivation = false;
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
    public override bool VerifyInstallState()
    {
        return demonActivation;
    }

    IEnumerator PlayAnimSequence(VictoryAnimation _currentAction)
    {
        float frameCount = 0;
        _cAnimator.SetCanTransitionIdle(false);
        bool pointHit = false;
        float waitTime = Base_FrameCode.ONE_FRAME;
        float endingFrame = _victoryAnimation._animLength;
        _cAnimator.PlayNextAnimation(_victoryAnimation._animHash, 0.25f);
        while (frameCount <= endingFrame)
        {
            #region Mobility Anim Checks
            if (frameCount > _victoryAnimation.activatePointInFrames && !pointHit)
            {
                pointHit = true;
                _sodaBottle.gameObject.SetActive(true);
            }
            frameCount += waitTime;
            yield return new WaitForSeconds(waitTime);
            #endregion
        }
    }
}
