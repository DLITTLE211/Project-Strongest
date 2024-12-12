using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Iron_Amplify : Amplifiers
{
    public override void ActivatePassiveAmplify()
    {
        _base._cDamageCalculator.SetDefensiveMultiplier(0.25f);
    }
    public override void ResetAmplifier()
    {
        _base._cDamageCalculator.SetDefensiveMultiplier();
    }
}
