using System;
using UnityEngine;

[Serializable]
public class Attack_BasicSpecialMove : Attack_Special_Base, IAttackFunctionality
{
    [SerializeField] private int framesBetweenAttacks;
    private AttackData attackData;
    private Character_Base _curBase;
    #region Attack Base Code
    public void SetStarterInformation(Character_Base _base)
    {
        _curBase = _base;
        TurnInputsToString();
        SetComboTimer();
    }
    public void TurnInputsToString()
    {
        property.InputTimer.SetTimerType();
        try
        {
            for (int i = 0; i < attackInput.Count; i++)
            {
                attackInput[i].turnStringToArray();
            }
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
    public void PreformAttack(Character_Base curBase)
    {
        curBase._aManager.ReceiveAttack(property);
    }
    public void PreformAttack()
    {
        attackData = new AttackData(_curBase);
        attackData.curBase._aManager.ReceiveAttack(property);
    }
    public void SendCounterHitInfo(Character_Base target)
    {
        target._cDamageCalculator.ReceiveCounterHitMultiplier(property.counterHitDamageMult);
    }
    public void SendSuccessfulDamageInfo(Character_Base target, bool blockedAttack)
    {
        if (!blockedAttack)
        {
            SendCounterHitInfo(target);
            target._cDamageCalculator.TakeDamage(property);
        }
        else
        {
            target._cDamageCalculator.TakeChipDamage(property);
        }
    }
    #endregion
    public MoveType GetAttackMoveType()
    {
        return property._moveType;
    }
}
