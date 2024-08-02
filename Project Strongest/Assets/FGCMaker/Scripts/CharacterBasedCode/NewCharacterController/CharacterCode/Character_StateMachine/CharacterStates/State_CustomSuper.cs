using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_CustomSuper : BaseState
{
    public State_CustomSuper(Character_Base playerBase) : base(playerBase)
    {

    }
    public override void OnEnter()
    {
        DebugMessageHandler.instance.DisplayErrorMessage(1, "Enter ThrowState");
    }
    public override void OnUpdate()
    {
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
