using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hollow_Affliction : Affliction
{
    public override void ActivateAffliction(float value, AfflictionType _type)
    {
        base.ActivateAffliction(value, _type);
        _base._cDamageCalculator.SetChipBuffMultiplier(0.45f);
    }
    public override void ForceEndAffliction()
    {
        _base._cDamageCalculator.SetChipBuffMultiplier(0.45f);
    }
}
