using System;
using UnityEngine;
using System.Collections.Generic;

[Serializable]
public class Attack_RekkaSpecialMove : Attack_Special_Rekka  , IAttackFunctionality
{
    [SerializeField] private int curRekkaInput, rekkaInputCount;
    [SerializeField] internal int leewayTime;
    [SerializeField] internal bool inRekkaState;
    [SerializeField] internal List<Attack_BaseProperties> usedRekkas;
    public Character_Base _curBase;

    #region Starting Information Code
    public void SetStarterInformation(Character_Base _base)
    {
        _curBase = _base;
        TurnInputsToString();
        SetComboTimer();
        SetAttackAnims();
    }
    private void TurnInputsToString()
    {
        curRekkaInput = 0;
        try
        {
            rekkaInput.mainAttackInput.turnStringToArray();
            inRekkaState = false;
            usedRekkas = new List<Attack_BaseProperties>();
        }
        catch (ArgumentNullException e)
        {
            DebugMessageHandler.instance.DisplayErrorMessage(3, $"{e.Message} has taken place. Skipping Step...");
        }
    }

    public void SetComboTimer()
    {
        rekkaInput.mainAttackProperty.InputTimer = _curBase._cAttackTimer;
        for (int i = 0; i < rekkaInput._rekkaPortion.Count; i++)
        {
            rekkaInput._rekkaPortion[i].individualRekkaAttack._correctInput[0].property.InputTimer = _curBase._cAttackTimer;
        }
    }
    public void SetAttackAnims()
    {
        rekkaInput.mainAttackProperty.SetAttackAnims(_curBase._cAnimator);
        for (int i = 0; i < rekkaInput._rekkaPortion.Count; i++)
        {
            rekkaInput._rekkaPortion[i].individualRekkaAttack.ActivateAttackInfo(RekkaSpecialAttack_Name);
        }
    }
    #endregion


    public override void ResetCombo()
    {
        inRekkaState = false; 
        curRekkaInput = 0;
        usedRekkas = new List<Attack_BaseProperties>();
    }
    public void DoFollowUpAttack(int attack, Callback SendAttackOnSucess)
    {
        Attack_BaseProperties newProperty = rekkaInput._rekkaPortion[attack].individualRekkaAttack._correctInput[0].property;
        if (usedRekkas.Contains(newProperty))
        {
            Debug.LogError($"Attack: \"{newProperty._attackName}\" has already been used. Returning...");
            return;
        }
        if (curRekkaInput >= rekkaInputCount)
        {
            Debug.LogError($"Rekka Input Allowance exceeded. Returning");
            return;
        }
        curRekkaInput++;
        usedRekkas.Add(newProperty);
        _curBase._aManager.ReceiveAttack(newProperty, () => RekkaFollowUpFunctions(newProperty, SendAttackOnSucess));
    }
    public void RekkaFollowUpFunctions(Attack_BaseProperties newProperty, Callback SendAttackOnSucess) 
    {
        SendAttackOnSucess();
        _curBase.comboList3_0.SetFollowAttack(newProperty);
    }
    public void PreformAttack(Callback SendAttackOnSucess)
    {
        _curBase._aManager.ReceiveAttack(rekkaInput.mainAttackProperty,SendAttackOnSucess);
        SetRekkaStateTrue();
        ResetCombo();
        rekkaInput.mainAttackProperty.InputTimer.SetTimerType(TimerType.InRekka,leewayTime);
        inRekkaState = true;

    }
    public void ResetAttackData()
    {
        ResetCombo();
    }

    public MoveType GetAttackMoveType()
    {
        return rekkaInput.mainAttackProperty._moveType;
    }


    public void SetRekkaStateTrue() 
    {
        inRekkaState = true;
    }
    public void SendCounterHitInfo(Character_Base target, Attack_BaseProperties main)
    {
        target._cDamageCalculator.ReceiveCounterHitMultiplier(main.counterHitDamageMult);
    }
    public void SendSuccessfulDamageInfo(Character_Base attacker, Character_Base target, bool blockedAttack, Attack_BaseProperties main, Attack_BaseProperties followUp = null, bool armoredAttack = false)
    {
        if (followUp != null)
        {
            if (!blockedAttack)
            {
                SendCounterHitInfo(target, followUp);
            }
            target._cDamageCalculator.ReceiveDamage(followUp,blockedAttack,armoredAttack);
        }
        else
        {
            if (!blockedAttack)
            {
                SendCounterHitInfo(target, main);
            }
            target._cDamageCalculator.ReceiveDamage(main, blockedAttack, armoredAttack);
        }
    }

    public void HandleDamageDealing(Character_Base attacker, Character_Base target, bool blockedAttack, Attack_BaseProperties main, Attack_BaseProperties followUp = null, bool armoredAttack = false)
    {
        SendSuccessfulDamageInfo(attacker, target, blockedAttack, main, followUp);
    }
}
