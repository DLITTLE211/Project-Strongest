using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Focus_Amplify : Amplifiers
{
    public override void ActivateEffect(Attack_BaseProperties _currentAttack = null, bool blockedAttack = false)
    {
        if (_currentAttack != null)
        {
            float awardedAmount = 0;
            if (blockedAttack)
            {
                awardedAmount = _currentAttack._meterAwardedOnHit * 0.2f;
            }
            else
            {
                awardedAmount = _currentAttack._meterAwardedOnHit * 0.55f;
            }
            _base._cSuperMeter.AddMeter(awardedAmount);
        }
    }
}
