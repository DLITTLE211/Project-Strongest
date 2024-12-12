using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Might_Amplify : Amplifiers
{
    public override void ActivatePassiveAmplify()
    {
        _base.opponentPlayer._cDamageCalculator.SetChipBuffMultiplier(0.05f);
    }
    public override void ResetAmplifier()
    {
        _base.opponentPlayer._cDamageCalculator.SetChipBuffMultiplier();
    }
}
