using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_SecondIdle : BaseState
{
    public State_SecondIdle(Character_Base playerBase) : base(playerBase) { }
    int lastInput;
    public override void OnEnter()
    {
        if (_base._subState == Character_SubStates.Controlled)
        {
            lastInput = _base.ReturnMovementInputs().Button_State.directionalInput;
        }
        if (_base.ReturnMovementInputs().Button_State.directionalInput <= 3)
        {
            _cAnim.PlayNextAnimation(secondaryCrouchHash, 2 * (1 / 60f));
        }
        else 
        {
            _cAnim.PlayNextAnimation(secondaryIdleHash, 2 * (1 / 60f));
        }
        _base.CallWaitAnimFinish(3.75f);
    }

    public override void OnUpdate()
    {
        if (_base.ReturnMovementInputs().Button_State.directionalInput != lastInput)
        {
            _base.allowSecondIdleAnim = false;
        }
        base.OnUpdate();
    }
    public override void OnExit()
    {
        base.OnExit();
    }
}
