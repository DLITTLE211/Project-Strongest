using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bloodthirsty_Amplify : Amplifiers
{
    public override void ActivateEffect(Attack_BaseProperties _currentAttack = null, bool blockedAttack = false)
    {
        if (_currentAttack != null)
        {
            int stolenAmount = 0;
            float awardedAmount = 0;
            if (blockedAttack) 
            {
                 stolenAmount = (int)(_currentAttack._meterAwardedOnHit * 0.35f);
                 awardedAmount = _currentAttack._meterAwardedOnHit * 0.2f;
            }
            else 
            {
                 stolenAmount = (int)(_currentAttack._meterAwardedOnHit * 0.65f);
                 awardedAmount = _currentAttack._meterAwardedOnHit * 0.3f;
            }
            _base.opponentPlayer._cSuperMeter.DecreaseMeterAmount(stolenAmount);
            _base._cSuperMeter.AddMeter(awardedAmount);
        }
    }
}
