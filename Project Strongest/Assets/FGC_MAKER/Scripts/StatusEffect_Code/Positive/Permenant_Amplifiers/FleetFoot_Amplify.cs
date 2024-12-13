using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FleetFoot_Amplify : Amplifiers
{
    public override void ActivateInstantPassiveAmplify()
    {
        _base.FleetFootAmplifyBuff(0.65f,0.75f);
        currentState = ActiveState.Active;
    }
}
