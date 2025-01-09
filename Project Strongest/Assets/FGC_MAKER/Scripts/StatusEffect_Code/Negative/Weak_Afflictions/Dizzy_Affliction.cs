using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dizzy_Affliction : Affliction
{
    public override void KillSingleAffliction()
    {
        currentState = ActiveState.Inactive;
        _base._cHealth.SetStunBuffValue();
        CallEndFunc();
    }
    public override void ActivateAffliction(float value, AfflictionType _type)
    {
        currentState = ActiveState.Active;
        base.ActivateAffliction(value, _type);
        _base._cHealth.SetStunBuffValue(0.95f);
        _isConsumed = true;
    }
}
