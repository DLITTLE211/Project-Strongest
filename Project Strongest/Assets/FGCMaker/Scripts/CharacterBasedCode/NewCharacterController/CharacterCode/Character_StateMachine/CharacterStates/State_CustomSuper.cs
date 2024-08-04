using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_CustomSuper : BaseState
{
    public State_CustomSuper(Character_Base playerBase) : base(playerBase)
    {

    }
    public override void OnEnter()
    {
        if (_base._cAnimator.lastAttack._moveType == MoveType.Super && _cAnim.customSuperHit)
        {
            _base._cComboDetection.inSuper = true;
            _base._cAttackTimer.PauseTimerOnSuperSuccess();
        }
    }
    public override void OnUpdate()
    {
        base.OnUpdate();
    }
    public override void OnRecov()
    {
        base.OnRecov();
    }

    public override void OnExit()
    {
        _base._cComboDetection.inSuper = false;
        base.OnExit();
    }
}
