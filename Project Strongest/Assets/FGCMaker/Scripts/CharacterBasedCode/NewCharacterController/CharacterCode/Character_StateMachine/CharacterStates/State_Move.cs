using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_Move : BaseState
{
    public State_Move(Character_Base playerBase) : base(playerBase)
    {

    }
    public override void OnEnter()
    {
        DebugMessageHandler.instance.DisplayErrorMessage(1, "Enter MoveState");
        if (_base.ReturnMovementInputs().Button_State.directionalInput == 4)
        {
            _cAnim.PlayNextAnimation(moveBHash, _crossFade);
            //_baseAnim.CrossFade(moveBHash, _crossFade);
        }
        if (_base.ReturnMovementInputs().Button_State.directionalInput == 6)
        {
            _cAnim.PlayNextAnimation(moveFHash, _crossFade);
           // _baseAnim.CrossFade(moveFHash, _crossFade);
        }
        _baseForce.SetWalkForce(_base.ReturnMovementInputs());

    }
    public override void OnFixedUpdate()
    {
        _baseForce.SetWalkForce(_base.ReturnMovementInputs());
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
