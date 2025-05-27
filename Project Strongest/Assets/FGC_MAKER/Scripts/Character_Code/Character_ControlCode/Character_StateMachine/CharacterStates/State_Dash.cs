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
        _base._aFrameDataMeter.ResetMeterData();
        _base._cHurtBox.ResetExtendedHurtbox();
        _base._cHitboxManager.DisableCurrentHitbox();
        _base._cHurtBox.SetHurboxState();
        if (_base._cHurtBox.IsGrounded())
        {
            _base.myRb.drag = 100000;
        }
        base.OnEnter();
        hitSuperChargeCheck = false;
        DebugMessageHandler.instance.DisplayErrorMessage(1, "Enter DashState");
        if (_base.ReturnMovementInputs().Button_State.directionalInput <= 3)
        {
            _base._cComboDetection.superMobilityOption = true;
        }
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        if (!hitSuperChargeCheck)
        {
            if (_base.ReturnMovementInputs().Button_State.directionalInput <= 3 && !_base._cComboDetection.superMobilityOption)
            {
                hitSuperChargeCheck = true;
                _base._cComboDetection.superMobilityOption = true;
            }
        }
        if(Mathf.Abs(_base._cForce.GetXSpeed()) != _base.DashForce)
        {
            ForceDashSpeed();
        }
    }
    public void ForceDashSpeed() 
    {
        float dashSpeed = _base.DashForce;
        dashSpeed = _base.pSide.thisPosition._directionFacing == Character_Face_Direction.FacingRight ? dashSpeed : -dashSpeed;
        if (_base._cAnimator.activatedInput != null)
        {
            if (_base._cAnimator.activatedInput.GetMovementType() == MovementType.ForwardDash)
            {
                _base.myRb.velocity = new Vector3(dashSpeed, _base.myRb.velocity.y, _base.myRb.velocity.z);
            }
            if (_base._cAnimator.activatedInput.GetMovementType() == MovementType.BackDash)
            {
                _base.myRb.velocity = new Vector3(-dashSpeed, _base.myRb.velocity.y, _base.myRb.velocity.z);
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
