using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_Move : BaseState
{
    bool onBack, onForward;
    public State_Move(Character_Base playerBase) : base(playerBase)
    {

    }
    public override void OnEnter()
    {
        DebugMessageHandler.instance.DisplayErrorMessage(1, "Enter MoveState");
        if (_base.ReturnMovementInputs().Button_State.directionalInput == 4)
        {
            onForward = false;
            onBack = true;
            _cAnim.PlayNextAnimation(moveBHash, _crossFade);
            //_baseAnim.CrossFade(moveBHash, _crossFade);
        }
        if (_base.ReturnMovementInputs().Button_State.directionalInput == 6)
        {
            onForward = true;
            onBack = false;
            _cAnim.PlayNextAnimation(moveFHash, _crossFade);
           // _baseAnim.CrossFade(moveFHash, _crossFade);
        }
        _baseForce.SetWalkForce(_base.ReturnMovementInputs());

    }
    public override void OnUpdate()
    {
        if (_base.ReturnMovementInputs().Button_State.directionalInput == 4)
        {
            if (onBack == false)
            {
                onForward = false;
                onBack = true;
                _cAnim.PlayNextAnimation(moveBHash, 0);
            }
        }
        if (_base.ReturnMovementInputs().Button_State.directionalInput == 6 )
        {
            if (onForward == false)
            {
                onForward = true;
                onBack = false;
                _cAnim.PlayNextAnimation(moveFHash, 0);
            }
        }
        _baseForce.SetWalkForce(_base.ReturnMovementInputs());
        base.OnUpdate();
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
