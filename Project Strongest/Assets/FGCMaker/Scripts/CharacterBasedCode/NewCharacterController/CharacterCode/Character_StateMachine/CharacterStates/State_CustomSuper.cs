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
        Attack_BaseProperties hitboxProperty = _base._cHitboxManager.GetActiveHitBox().hitboxProperties;
        _base._cComboDetection.inSuper = true;
        _base._cAttackTimer.SetTimerType(TimerType.Super);
        _cAnim.lastAttack = hitboxProperty;

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

        _base._cAttackTimer.SetTimerType(TimerType.Normal);
        base.OnExit();
    }
}
