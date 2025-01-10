using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Yielded_Affliction : Affliction
{
    public override void ActivateAffliction(float value, AfflictionType _type)
    {
        currentState = ActiveState.Active;
        base.ActivateAffliction(value, _type);
        _base._amplifyController.allowFill = false;
    }
    public override void ForceEndAffliction()
    {
        currentState = ActiveState.Inactive;
        _base._amplifyController.allowFill = true;
    }
}
