using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paralysis_Affliction : Affliction
{
    public override void ActivateAffliction(float value, AfflictionType _type)
    {
        base.ActivateAffliction(value, _type);
        _base.character_MobilityOptions.SetParalyzed(true);
    }
    public override void ForceEndAffliction()
    {
        _base.character_MobilityOptions.SetParalyzed(false);
    }
}
