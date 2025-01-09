using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Immobile_Affliction : Affliction
{
    public override void ActivateAffliction(float value, AfflictionType _type)
    {
        currentState = ActiveState.Active;
        base.ActivateAffliction(value, _type);
        _base.ImmobileAffliction(0.65f);
    }
    public override void ForceEndAffliction()
    {
        currentState = ActiveState.Inactive;
        _base.ResetImmobileAffliction();
    }
}
