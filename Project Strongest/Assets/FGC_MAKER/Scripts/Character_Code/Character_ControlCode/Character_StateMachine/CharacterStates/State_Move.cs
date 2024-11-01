using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class State_Move : BaseState
{
    bool onBack, onForward;
    public State_Move(Character_Base playerBase) : base(playerBase)
    {
    }
    public override async void OnEnter()
    {
        base.OnEnter();
        _base._cHurtBox.ResetExtendedHurtbox();
        DebugMessageHandler.instance.DisplayErrorMessage(1, "Enter MoveState");
        if (_base.pSide.thisPosition._directionFacing == Character_Face_Direction.FacingRight)
        {
            CheckRightFaceState();
        }
        if (_base.pSide.thisPosition._directionFacing == Character_Face_Direction.FacingLeft)
        {
            CheckLeftFaceState();
        }
        float fourFrameWaitTime = 4 * (1 / 60f);
        int fourtimeInMS = (int)(fourFrameWaitTime * 1000f);
        await Task.Delay(fourtimeInMS);
        _baseForce.SetWalkForce(_base.ReturnMovementInputs());

    }
    public override void OnUpdate()
    {
        if (_base.pSide.thisPosition._directionFacing == Character_Face_Direction.FacingRight)
        {
            CheckRightFaceState();
        }
        if (_base.pSide.thisPosition._directionFacing == Character_Face_Direction.FacingLeft)
        {
            CheckLeftFaceState();
        }
        _baseForce.SetWalkForce(_base.ReturnMovementInputs());
        base.OnUpdate();
    }
    void CheckRightFaceState()
    {
        if (_base.ReturnMovementInputs().Button_State.directionalInput == 4)
        {
            if (!onBack)
            {
                HandleBackwardAnimation();
            }
        }
        if (_base.ReturnMovementInputs().Button_State.directionalInput == 6)
        {
            if (!onForward)
            {
                HandleForwardAnimation();
            }
        }
    }
    void CheckLeftFaceState() 
    {
        if (_base.ReturnMovementInputs().Button_State.directionalInput == 4)
        {
            if (!onForward)
            {
                HandleForwardAnimation();
            }
        }
        if (_base.ReturnMovementInputs().Button_State.directionalInput == 6)
        {
            if (!onBack)
            {
                HandleBackwardAnimation();
            }
        }
    }
    void HandleForwardAnimation() 
    {
        onForward = true;
        onBack = false;
        _cAnim.PlayNextAnimation(moveFHash, 2 * (1 / 60f));
    }
    void HandleBackwardAnimation()
    {
        onBack = true;
        onForward = false;
        _cAnim.PlayNextAnimation(moveBHash, 2 * (1 / 60f));
    }
    public override void OnRecov()
    {
        base.OnRecov();
    }

    public override void OnExit()
    {
        onForward = false;
        onBack = false;

        ITransition nextTransition = _base._cStateMachine._playerState.GetTransition();
        if (nextTransition.To == _base._cStateMachine.idleStateRef)
        {
            if (_base._subState == Character_SubStates.Controlled)
            {
                if (_base.ReturnMovementInputs().Button_State.directionalInput == 5)
                {
                    _cAnim.PlayNextAnimation(groundIdleHash, 2 * (1 / 60f));
                }
            }
            else 
            {
                _cAnim.PlayNextAnimation(groundIdleHash, 2 * (1 / 60f));
            }
        }
        base.OnExit();
    }
}
