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
    (Attack_BaseInput.MoveInput, Attack_BaseInput.AttackInput) _newinput;
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
        _curBase._aManager.ReceiveAttack(newProperty, SendAttackOnSucess);
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
    public void SendCounterHitInfo(Character_Base curBase, Attack_BaseProperties followUp = null)
    {
        if (followUp != null)
        {
            curBase._cDamageCalculator.ReceiveCounterHitMultiplier(followUp.counterHitDamageMult);
        }
        else
        {
            curBase._cDamageCalculator.ReceiveCounterHitMultiplier(rekkaInput.mainAttackProperty.counterHitDamageMult);
        }
    }
    public void SendSuccessfulDamageInfo(Character_Base attacker, Character_Base target, bool blockedAttack, Attack_BaseProperties main, Attack_BaseProperties followUp = null)
    {
        if (followUp != null)
        {
            if (!blockedAttack)
            {
                SendCounterHitInfo(_curBase, followUp);
                _curBase._cDamageCalculator.TakeDamage(followUp);
            }
            else
            {
                _curBase._cDamageCalculator.TakeChipDamage(rekkaInput.mainAttackProperty);
            }
        }
        else
        {
            if (!blockedAttack)
            {
                SendCounterHitInfo(_curBase);
                _curBase._cDamageCalculator.TakeDamage(followUp);
            }
            else
            {
                _curBase._cDamageCalculator.TakeChipDamage(rekkaInput.mainAttackProperty);
            }
        }
    }
    /*public void SendSuccessfulDamageInfo(Character_Base curBase, bool blockedAttack, RekkaAttack _rekkaAttack = null)
    {
        if (_rekkaAttack != null)
        {
            SendCounterHitInfo(curBase,_rekkaAttack);
            if (!blockedAttack)
            {
                curBase._cDamageCalculator.TakeDamage(_rekkaAttack.individualRekkaAttack._correctInput[0].property);
            }
            else 
            {
                curBase._cDamageCalculator.TakeChipDamage(_rekkaAttack.individualRekkaAttack._correctInput[0].property);
            }
        }
        else
        {
            SendCounterHitInfo(curBase); 
            if (!blockedAttack)
            {
                curBase._cDamageCalculator.TakeDamage(_rekkaAttack.individualRekkaAttack._correctInput[0].property);
            }
            else
            {
                curBase._cDamageCalculator.TakeChipDamage(_rekkaAttack.individualRekkaAttack._correctInput[0].property);
            }
        }
    }*/
}
