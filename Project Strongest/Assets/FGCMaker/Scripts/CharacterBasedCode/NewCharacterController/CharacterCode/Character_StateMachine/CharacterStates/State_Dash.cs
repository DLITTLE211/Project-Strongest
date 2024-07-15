using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_Dash : BaseState
{
    public State_Dash(Character_Base playerBase) : base(playerBase)
    {

    }
    public override void OnEnter()
    {
        DebugMessageHandler.instance.DisplayErrorMessage(1, "Enter JumpState");
        if (_base._cAnimator.activatedInput != null)
        {
            if (_base.myRb.velocity.x < _base.DashForce && _base._cAnimator.activatedInput.type == MovementType.ForwardDash)
            {
                _base.myRb.velocity = new Vector3(_base.DashForce, _base.myRb.velocity.y);
            }
            if (_base.myRb.velocity.x > -_base.DashForce && _base._cAnimator.activatedInput.type == MovementType.BackDash)
            {
                _base.myRb.velocity = new Vector3(-_base.DashForce, _base.myRb.velocity.y);
            }
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
        base.OnExit();
    }
}
