using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_Amplify : BaseState
{
    public State_Amplify(Character_Base playerBase) : base(playerBase)
    {

    }
    public override void OnEnter()
    {
        base.OnEnter();
        _base._aFrameDataMeter.ResetMeterData();
        _base._amplifyController.ActivateAmplify();
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
