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
