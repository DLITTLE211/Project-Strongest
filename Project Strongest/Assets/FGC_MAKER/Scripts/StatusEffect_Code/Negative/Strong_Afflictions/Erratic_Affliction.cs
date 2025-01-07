using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Erratic_Affliction : Affliction
{
    public override void ActivateAffliction(float value, AfflictionType _type)
    {
        base.ActivateAffliction(value, _type);
        _base._cADetection.isErratic = true;
    }
    public override void ForceEndAffliction()
    {
        _base._cADetection.isErratic = false;
    }
}
