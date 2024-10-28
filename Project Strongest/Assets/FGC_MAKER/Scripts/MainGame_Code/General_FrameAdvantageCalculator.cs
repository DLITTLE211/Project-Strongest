using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class General_FrameAdvantageCalculator : MonoBehaviour
{
    public float ReturnFrameDifference(Character_Base _hitPlayer)
    {
        float attackingPlayerRecovery = _hitPlayer.opponentPlayer._cAnimator._lastAnim._frameData.recoveryAmount;
        float hitPlayerHitstun = _hitPlayer._cHitController.currentHitstun;
        float frameDifference = -(attackingPlayerRecovery - hitPlayerHitstun);
        return frameDifference;
    }
}