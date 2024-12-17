using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dizzy_Affliction : Affliction
{
    public override void KillSingleAffliction()
    {
        _base._cHealth.SetStunBuffValue();
        CallEndFunc();
    }
    public override void ActivateAffliction(float value)
    {
        _base._cHealth.SetStunBuffValue(0.35f);
    }
}
