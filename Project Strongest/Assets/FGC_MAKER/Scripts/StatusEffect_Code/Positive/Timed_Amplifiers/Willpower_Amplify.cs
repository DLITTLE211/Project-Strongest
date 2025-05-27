using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Willpower_Amplify : Amplifiers
{
    public override void ActivatePassiveAmplify()
    {
        _base._cHealth.SetStunBuffValue(0.25f);
    }
    public override void ResetAmplifier()
    {
        _base._cHealth.SetStunBuffValue();
    }
}
