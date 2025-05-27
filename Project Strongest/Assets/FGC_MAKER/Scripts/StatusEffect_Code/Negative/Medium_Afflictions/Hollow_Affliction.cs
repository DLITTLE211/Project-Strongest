using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hollow_Affliction : Affliction
{
    public override void ActivateAffliction(float value, AfflictionType _type)
    {
        currentState = ActiveState.Active;
        base.ActivateAffliction(value, _type);
        _base.opponentPlayer._cDamageCalculator.SetBuffMultiplier(-0.85f);
    }
    public override void ForceEndAffliction()
    {
        currentState = ActiveState.Inactive;
        _base.opponentPlayer._cDamageCalculator.SetBuffMultiplier(-0.85f);
    }
}
