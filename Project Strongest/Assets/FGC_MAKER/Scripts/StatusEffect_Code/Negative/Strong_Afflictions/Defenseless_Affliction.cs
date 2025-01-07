using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Defenseless_Affliction : Affliction
{
    public override void ActivateAffliction(float value, AfflictionType _type)
    {
        base.ActivateAffliction(value, _type);
        _base._cBlockHandler.isDefenseless = true; 
    }
    public override void ForceEndAffliction()
    {
        _base._cBlockHandler.isDefenseless = false;
    }
}
