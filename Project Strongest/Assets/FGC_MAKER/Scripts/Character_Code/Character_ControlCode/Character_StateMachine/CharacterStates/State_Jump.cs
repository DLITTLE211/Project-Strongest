using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_Jump : BaseState
{
    private Character_MobilityOption _lastMobilityAction;
    public Character_MobilityOption LastMobilityAction { get { return _lastMobilityAction; } }
    public State_Jump(Character_Base playerBase) : base(playerBase)
    {

    }
    public override void OnEnter()
    {
        _lastMobilityAction = _base._cAnimator.activatedInput;
        DebugMessageHandler.instance.DisplayErrorMessage(1, "Enter JumpState");
        //_cAnim.PlayNextAnimation(jumpHash, _crossFade);
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
