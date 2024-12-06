using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Focus_Amplify : Amplifiers
{
    public override void ActivateEffect(Attack_BaseProperties _currentAttack = null)
    {
        if (_currentAttack != null)
        {
            _base._cSuperMeter.AddMeter(_currentAttack._meterAwardedOnHit * 0.35f);
        }
    }
}
