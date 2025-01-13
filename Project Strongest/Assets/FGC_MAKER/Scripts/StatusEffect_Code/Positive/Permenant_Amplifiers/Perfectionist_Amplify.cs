using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Perfectionist_Amplify : Amplifiers
{
    [Header("Perfectionist --ONLY-- Debuff")]
    public int _perfectionistDebuff;
    public override void ActivateInstantPassiveAmplify()
    {
        _base._cHealth.SetStunBuffValue(0.25f);
        _base.opponentPlayer._cDamageCalculator.SetBuffMultiplier(0.15f);
        _base.opponentPlayer._cDamageCalculator.SetChipBuffMultiplier(0.35f);
        currentState = ActiveState.Active;
    }
    public override void DeactivateInstantPassiveAmplify()
    {
        _base._cHealth.SetStunBuffValue();
        _base.opponentPlayer._cDamageCalculator.SetBuffMultiplier(-0.15f);
        _base.opponentPlayer._cDamageCalculator.SetChipBuffMultiplier();
        currentState = ActiveState.Inactive;
    }
}
