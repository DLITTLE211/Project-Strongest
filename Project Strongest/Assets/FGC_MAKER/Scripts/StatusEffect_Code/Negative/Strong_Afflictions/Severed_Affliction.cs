using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Severed_Affliction : Affliction
{
    public override void ActivateAffliction(float value, AfflictionType _type)
    {
        base.ActivateAffliction(value, _type);
        _base._cSuperMeter.SetMeterDebuffPercent(0f);
    }
    public override void ForceEndAffliction()
    {
        _base._cSuperMeter.ResetMeterDebuffPercent();
    }
}
