using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_Jump : BaseState
{
    private Character_Mobility _lastMobilityAction;
    public Character_Mobility LastMobilityAction { get { return _lastMobilityAction; } }
    public State_Jump(Character_Base playerBase) : base(playerBase)
    {

    }
    public override void OnEnter()
    {
        _lastMobilityAction = _base._cAnimator.activatedInput;
        _base._cHurtBox.SetHitboxSize(HurtBoxSize.Standing);
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
        ITransition nextTransition = _base._cStateMachine._playerState.GetTransition();

        if (nextTransition.To == _base._cStateMachine.idleStateRef)
        {
            _base._cAnimator.PlayNextAnimation(0, 0, true, "ForceIdle");
        }
        if (nextTransition.To == _base._cStateMachine.crouchStateRef)
        {
            _base._cAnimator.PlayNextAnimation(0, 0, true, "ForceCrouch");
        }
        base.OnExit();
    }
}
