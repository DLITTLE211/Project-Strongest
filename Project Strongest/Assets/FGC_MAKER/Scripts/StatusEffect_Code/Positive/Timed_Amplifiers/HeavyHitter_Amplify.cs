using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeavyHitter_Amplify : Amplifiers
{
    public override void ActivatePassiveAmplify()
    {
        _base.opponentPlayer._cDamageCalculator.SetBuffMultiplier(0.15f);
    }
}
