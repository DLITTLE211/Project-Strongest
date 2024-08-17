using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Rewired;
[Serializable]
public abstract class AdvancedSpecialBase
{
    public string specialMoveName;
    public Attack_Input attackInput;
    public ButtonStateMachine attackInputState;
    public Attack_BaseProperties property;
    public List<AttackHandler_Attack> _customAnimation;
    public abstract void ResetCombo();
}
[Serializable]
public class Attack_AdvancedSpecialMove : AdvancedSpecialBase, IAttackFunctionality 
{
    [SerializeField] private int movementPortionLength;
    [SerializeField] private char finalAttackButton;
    [SerializeField] private Character_Base _curBase;
    [SerializeField] private int framesBetweenAttacks;
    public int currentCustomAnim;
    private AttackData newAttackData;
    #region Attack Base Code

    public void SetStarterInformation(Character_Base _base)
    {
        _curBase = _base;
        TurnInputsToString();
        SetComboTimer();
    }
    public void TurnInputsToString()
    {
        try
        {
            attackInput.turnStringToArray();
        }
        catch (ArgumentNullException e)
        {
            DebugMessageHandler.instance.DisplayErrorMessage(3, $"{e.Message} has taken place. Skipping Step...");
        }
    }
    public void SetComboTimer()
    {
        property.InputTimer = _curBase._cAttackTimer;
    }
    #endregion

    public override void ResetCombo()
    {
        currentCustomAnim = 0;
    }
    #region Attack Functionality Code
    public void PreformAttack(Callback SendAttackOnSucess)
    {
        newAttackData = new AttackData(_curBase);
        ResetCombo();
        newAttackData.curBase._aManager.ReceiveAttack(property, SendAttackOnSucess);
    }
    public void SendCounterHitInfo(Character_Base target)
    {
        target._cDamageCalculator.ReceiveCounterHitMultiplier(property.counterHitDamageMult);
    }
    public void SendSuccessfulDamageInfo(Character_Base target, bool blockedAttack)
    {
        SendCounterHitInfo(target);
        if (!blockedAttack)
        {
            target._cDamageCalculator.TakeDamage(property);
        }
        else
        {
            target._cDamageCalculator.TakeChipDamage(property);
        }
    }
    public void HandleCounterAnimAttackInfo()
    {
        _customAnimation[0].SetAttackAnim(_curBase._cAnimator);
        _customAnimation[0].AddRequiredCallbacks(_curBase);
        _customAnimation[0].AddCustomCallbacks(_customAnimation[0]);
        _curBase._cAnimator.StartThrowFrameCount(property, _customAnimation[0]);
    }
    public void HandleSuperMultipleAnimAttackInfo()
    {
        if (_customAnimation.Count > 0)
        {
            _customAnimation[currentCustomAnim].SetAttackAnim(_curBase._cAnimator);
            _customAnimation[currentCustomAnim].AddRequiredCallbacks(_curBase);
            _customAnimation[currentCustomAnim].AddCustomCallbacks(_customAnimation[currentCustomAnim]);
            _curBase._cAnimator.StartSuperFrameCount(property, currentCustomAnim, _customAnimation.Count - 1, _customAnimation[currentCustomAnim], () => PlayNextCustomAnim());
            return;
        }
        return;
    }
    public void PlayNextCustomAnim()
    {
        currentCustomAnim++;
        HandleSuperMultipleAnimAttackInfo();
    }
    public MoveType GetAttackMoveType()
    {
        return property._moveType;
    }
    #endregion
}
