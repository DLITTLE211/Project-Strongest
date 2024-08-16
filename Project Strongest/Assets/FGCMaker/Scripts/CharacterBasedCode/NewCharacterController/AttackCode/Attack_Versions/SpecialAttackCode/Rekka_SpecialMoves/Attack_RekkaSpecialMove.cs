using System;
using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;

[Serializable]
public class Attack_RekkaSpecialMove : Attack_Special_Rekka  , IAttackFunctionality//IAttack_RekkaFuctionality
{
    [SerializeField] private int curRekkaInput, rekkaInputCount;
    [SerializeField] internal int leewayTime;
    [SerializeField] internal bool inRekkaState;
    [SerializeField] internal List<Attack_BaseProperties> usedRekkas;
    (Attack_BaseInput.MoveInput, Attack_BaseInput.AttackInput) _newinput;
    public Character_Base _curBase;
    private AttackData attackData;

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
        rekkaInput.mainAttackProperty.InputTimer.SetTimerType();
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
    public void DoFollowUpAttack(int attack)
    {
        attackData = new AttackData(_curBase, rekkaInput._rekkaPortion[attack], null, -1, null);
        if (usedRekkas.Contains(attackData.rekkaAttack.individualRekkaAttack._correctInput[0].property))
        {
            Debug.LogError($"Attack: \"{attackData.rekkaAttack.individualRekkaAttack._correctInput[0].property._attackName}\" has already been used. Returning...");
            attackData = null;
            return;
        }
        if (curRekkaInput >= rekkaInputCount)
        {
            Debug.LogError($"Rekka Input Allowance exceeded. Returning");
            attackData = null;
            return;
        }
        curRekkaInput++;
        usedRekkas.Add(attackData.rekkaAttack.individualRekkaAttack._correctInput[0].property);
        attackData.curBase._aManager.ReceiveAttack(attackData.rekkaAttack.individualRekkaAttack._correctInput[0].property);
    }
    public void PreformAttack()
    {
        attackData = new AttackData(_curBase, null, null, -1, rekkaInput.mainAttackProperty);
        attackData.curBase._aManager.ReceiveAttack(rekkaInput.mainAttackProperty);
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
    public void SendCounterHitInfo(Character_Base curBase)
    {
        throw new NotImplementedException();
    }
    public void SendCounterHitInfo(Character_Base curBase, RekkaAttack _rekkaAttack = null)
    {
        if (_rekkaAttack != null)
        {
            curBase._cDamageCalculator.ReceiveCounterHitMultiplier(_rekkaAttack.individualRekkaAttack._correctInput[0].property.counterHitDamageMult);
        }
        else
        {
            curBase._cDamageCalculator.ReceiveCounterHitMultiplier(rekkaInput.mainAttackProperty.counterHitDamageMult);
        }
    }
    public void SendSuccessfulDamageInfo(Character_Base curBase, bool blockedAttack)
    {
        if (attackData.rekkaAttack != null)
        {
            SendCounterHitInfo(curBase, attackData.rekkaAttack);
            if (!blockedAttack)
            {
                curBase._cDamageCalculator.TakeDamage(attackData.rekkaAttack.individualRekkaAttack._correctInput[0].property);
            }
            else
            {
                curBase._cDamageCalculator.TakeChipDamage(attackData.rekkaAttack.individualRekkaAttack._correctInput[0].property);
            }
        }
        else
        {
            SendCounterHitInfo(curBase);
            if (!blockedAttack)
            {
                curBase._cDamageCalculator.TakeDamage(attackData.mainRekka);
            }
            else
            {
                curBase._cDamageCalculator.TakeChipDamage(attackData.mainRekka);
            }
        }
    }
    public void SendSuccessfulDamageInfo(Character_Base curBase, bool blockedAttack, RekkaAttack _rekkaAttack = null)
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
    }
}
