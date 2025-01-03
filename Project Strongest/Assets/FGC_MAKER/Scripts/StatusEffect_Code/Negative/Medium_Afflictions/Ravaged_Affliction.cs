using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ravaged_Affliction : Affliction
{
    public override void ActivateAffliction(float value, AfflictionType _type)
    {
        base.ActivateAffliction(value, _type);
        _base.opponentPlayer._cHitstun.IncreaseHitstunOnAffliction(0.17f);
    }
    public override void ForceEndAffliction()
    {
        _base.opponentPlayer._cHitstun.ResetHitstunIncrease();
    }
}
