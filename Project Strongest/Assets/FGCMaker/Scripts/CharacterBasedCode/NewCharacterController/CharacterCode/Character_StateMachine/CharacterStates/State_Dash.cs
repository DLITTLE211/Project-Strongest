using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class State_Dash : BaseState
{
    bool hitSuperChargeCheck;
    public State_Dash(Character_Base playerBase) : base(playerBase)
    {
    }
    public override async void OnEnter()
    {
        hitSuperChargeCheck = false;
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
        await WaitToChargeSuperMobility();
    }
    async Task WaitToChargeSuperMobility()
    {
        float OneFrame = 1 / 60f;
        await ActivateSuperMobility(OneFrame);
    }

    async Task ActivateSuperMobility(float OneFrame)
    {
        float waitTime = 2 * OneFrame;
        int timeInMS = (int)(waitTime * 1000f);
        await Task.Delay(timeInMS);
        if (_base.ReturnMovementInputs().Button_State.directionalInput <= 3)
        {
            _base._cComboDetection.superMobilityOption = true;
        }
    }
    public override async void OnUpdate()
    {
        base.OnUpdate();
        if (!hitSuperChargeCheck)
        {
            if (_base.ReturnMovementInputs().Button_State.directionalInput <= 3)
            {
                hitSuperChargeCheck = true;
                await WaitToChargeSuperMobility();
            }
        }
    }
    public override void OnRecov()
    {
        base.OnRecov();
    }

    public override async void OnExit()
    {
        base.OnExit();
    }
}
