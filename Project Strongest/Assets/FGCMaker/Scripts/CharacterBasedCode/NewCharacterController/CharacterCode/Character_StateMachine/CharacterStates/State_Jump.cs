using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_Jump : BaseState
{
    public State_Jump(Character_Base playerBase) : base(playerBase)
    {

    }
    public override void OnEnter()
    {
        _base._cHurtBox.SetHitboxSize(HurtBoxSize.Standing);
        DebugMessageHandler.instance.DisplayErrorMessage(1, "Enter JumpState");
        _cAnim.PlayNextAnimation(jumpHash, _crossFade);
        _baseForce.SetWalkForce(_base.ReturnMovementInputs());
    }
    public override void OnRecov()
    {
        base.OnRecov();
    }
    public override void OnUpdate()
    {
        base.OnUpdate();
    }

    public override void OnExit()
    {
        base.OnExit();
    }
}
