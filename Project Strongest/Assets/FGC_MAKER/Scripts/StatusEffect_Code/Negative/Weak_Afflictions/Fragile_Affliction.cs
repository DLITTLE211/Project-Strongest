using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fragile_Affliction : Affliction
{
    public override void ActivateAffliction(float value)
    {
        _base._cDamageCalculator.SetChipBuffMultiplier(0.45f);
    }
    public override void ForceEndAffliction()
    {
        _base._cDamageCalculator.SetChipBuffMultiplier(0.45f);
    }
}
