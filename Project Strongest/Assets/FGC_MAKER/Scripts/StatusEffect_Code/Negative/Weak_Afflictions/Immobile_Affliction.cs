using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Immobile_Affliction : Affliction
{
    public override void ActivateAffliction(float value)
    {
        _base.ImmobileAffliction(0.65f);
    }
    public override void ForceEndAffliction()
    {
        _base.ResetImmobileAffliction();
    }
}
