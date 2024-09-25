using System;
using UnityEngine;

[Serializable]
public class Attack_BasicSpecialMove : Attack_Special_Base, IAttackFunctionality
{
    [SerializeField] private int framesBetweenAttacks;
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
    public void PreformAttack(Callback SendAttackOnSucess)
    {
        _curBase.comboList3_0.ClearFollowUpAttack();
        property.InputTimer.SetTimerType(TimerType.Special);
        _curBase._aManager.ReceiveAttack(property,SendAttackOnSucess);
    }
    public void HandleDamageDealing(Character_Base attacker, Character_Base target, bool blockedAttack, Attack_BaseProperties main, Attack_BaseProperties followUp = null, bool armoredAttack = false)
    {
        SendSuccessfulDamageInfo(attacker, target,blockedAttack,main);
    }
    public void SendCounterHitInfo(Character_Base target, Attack_BaseProperties property)
    {
        target._cDamageCalculator.ReceiveCounterHitMultiplier(property.counterHitDamageMult);
    }
    public void SendSuccessfulDamageInfo(Character_Base attacker, Character_Base target, bool blockedAttack, Attack_BaseProperties main, Attack_BaseProperties followUp = null, bool armoredAttack = false)
    {
        if (!blockedAttack)
        {
            SendCounterHitInfo(target,main);
        }
        target._cDamageCalculator.ReceiveDamage(main,blockedAttack,armoredAttack);
    }
    #endregion
    public MoveType GetAttackMoveType()
    {
        return property._moveType;
    }
    public MoveListAttackInfo CreateMoveListData()
    {
        string attackName = property._attackName;
        string specialMoveInput = attackInput[0].attackString;
        int _meterRequirement = property._meterRequirement;
        MoveListAttackInfo newMoveListAttackInfo = new MoveListAttackInfo(attackName, specialMoveInput, _meterRequirement);

        return newMoveListAttackInfo;
    }
}
