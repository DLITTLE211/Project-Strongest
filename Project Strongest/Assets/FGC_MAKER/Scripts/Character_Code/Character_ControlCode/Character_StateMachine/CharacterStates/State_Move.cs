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
        _cAnim.PlayNextAnimation(moveFHash, 0.05f);
    }
    void HandleBackwardAnimation()
    {
        onBack = true;
        onForward = false;
        _cAnim.PlayNextAnimation(moveBHash, 0.05f);
    }
    public override void OnRecov()
    {
        base.OnRecov();
    }

    public override void OnExit()
    {
        onForward = false;
        onBack = false;
        base.OnExit();
    }
}
