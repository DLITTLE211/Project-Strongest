using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;
using System;

[Serializable]
public abstract class ThrowActionBase 
{
    public string ThrowName;
    public Attack_BasicInput _attackInput;
    public List<AttackHandler_Attack> _throwAnimation;
    public abstract void ResetCombo();

}
[Serializable]
public class Attack_ThrowBase : ThrowActionBase , IAttackFunctionality 
{
    [SerializeField] private int curInput, curAttack;
    [SerializeField] private int lastDirection;
    [SerializeField] private Character_Base _curBase;
    public void SetStarterInformation(Character_Base _base)
    {
        _curBase = _base; 
        ResetCombo();
        try
        {
            _attackInput.ActivateAttackInfo(ThrowName);
        }
        catch (ArgumentNullException e) { DebugMessageHandler.instance.DisplayErrorMessage(3, $"{e.Message} has taken place. Skipping Step..."); }
        SetComboTimer();

    }
    public void SetComboTimer()
    {
        for (int i = 0; i < _attackInput._correctInput.Count; i++)
        {
            _attackInput._correctInput[i].property.InputTimer = _curBase._cAttackTimer;
        }
        _throwAnimation[0].SetAttackAnim(_curBase._cAnimator);
    }
    public MoveListAttackInfo CreateMoveListData()
    {
        string attackName = _attackInput._correctInput[0].property._attackName;
        string specialMoveInput = _attackInput._correctInput[0]._correctSequence;
        int _meterRequirement = _attackInput._correctInput[0].property._meterRequirement;
        MoveListAttackInfo newMoveListAttackInfo = new MoveListAttackInfo(attackName, specialMoveInput, _meterRequirement);

        return newMoveListAttackInfo;
    }
    public MoveType GetAttackMoveType()
    {
        return _attackInput._correctInput[0].property._moveType;
    }
    #region Attack Base Code

    public override void ResetCombo()
    {
        _attackInput._correctInput[0].property.hitConnected = false;
    }

    #endregion

    #region Attack Functionality

    public void PreformAttack(Callback SendAttackOnSucess)
    {
        Attack_BaseProperties newAttack = _attackInput._correctInput[0].property;
        _curBase.comboList3_0.ClearFollowUpAttack();
        _curBase._aManager.ReceiveAttack(newAttack, SendAttackOnSucess);
        _attackInput._correctInput[0].property.InputTimer.SetTimerType(TimerType.Throw);
    }

    public void SendSuccessfulDamageInfo(Character_Base attacker, Character_Base target, bool blockedAttack, Attack_BaseProperties main, Attack_BaseProperties followUp = null, bool armoredAttack = false)
    {
        if (_attackInput._correctInput[0].property.rawAttackDamage > 0)
        {
            target._cDamageCalculator.ReceiveDamage(_attackInput._correctInput[0].property,blockedAttack,false);
        }
        SendCounterHitInfo(target, null);
    }

    public void SendCounterHitInfo(Character_Base target, Attack_BaseProperties followUp)
    {
        target._cDamageCalculator.ReceiveCounterHitMultiplier(_attackInput._correctInput[0].property.counterHitDamageMult);
    }

    public void HandleThrowAnimAttackInfo()
    {
        _throwAnimation[0].SetAttackAnim(_curBase._cAnimator);
        _throwAnimation[0].AddRequiredCallbacks(_curBase);
        _throwAnimation[0].AddCustomCallbacks(_throwAnimation[0]);
        _curBase._cAnimator.StartThrowFrameCount(this._attackInput._correctInput[0].property, _throwAnimation[0]);
    }
    public void HandleDamageDealing(Character_Base attacker, Character_Base target, bool blockedAttack, Attack_BaseProperties main, Attack_BaseProperties followUp = null, bool armoredAttack = false)
    {
        SendSuccessfulDamageInfo(attacker, target, blockedAttack, main);
        HandleThrowAnimAttackInfo();
    }
    #endregion
}
