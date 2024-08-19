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
    /*public void PreformAttack(Character_Base curBase, Callback SendAttackOnSucess)
    {
        curBase._aManager.ReceiveAttack(property);
    }*/
    public void PreformAttack(Callback SendAttackOnSucess)
    {
        _curBase._aManager.ReceiveAttack(property,SendAttackOnSucess);
    }
    public void HandleDamageDealing(Character_Base attacker, Character_Base target, bool blockedAttack, Attack_BaseProperties main, Attack_BaseProperties followUp = null)
    {
        SendSuccessfulDamageInfo(attacker, target,blockedAttack,main);
    }
    public void SendCounterHitInfo(Character_Base target, Attack_BaseProperties property)
    {
        target._cDamageCalculator.ReceiveCounterHitMultiplier(property.counterHitDamageMult);
    }
    public void SendSuccessfulDamageInfo(Character_Base attacker, Character_Base target, bool blockedAttack, Attack_BaseProperties main, Attack_BaseProperties followUp = null)
    {
        if (!blockedAttack)
        {
            SendCounterHitInfo(target,main);
            target._cDamageCalculator.TakeDamage(main);
        }
        else
        {
            target._cDamageCalculator.TakeChipDamage(main);
        }
    }
    #endregion
    public MoveType GetAttackMoveType()
    {
        return property._moveType;
    }

}
