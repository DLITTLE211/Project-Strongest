using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weakened_Affliction : Affliction
{
    public override void ActivateAffliction(float value, AfflictionType _type)
    {
        base.ActivateAffliction(value, _type);
        _base._cDamageCalculator.SetBuffMultiplier(0.35f);
    }
    public override void ForceEndAffliction()
    {
        _base._cDamageCalculator.SetBuffMultiplier();
    }
}
