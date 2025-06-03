using System;
using UnityEngine;
using System.Collections.Generic;

[Serializable]
public class Attack_RekkaSpecialMove : Attack_Special_Rekka  , IAttackFunctionality
{
    [SerializeField] private int curRekkaInput, rekkaInputCount;
    [SerializeField] internal int rekkaLeewayTime;
    [SerializeField] internal int AttackResetTime;
    public AfflictionSet attackAfflictionSet;
    public int LeewayTime 
    {
        get { return rekkaLeewayTime; } 
        set { rekkaLeewayTime = value; } 
    }
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
            for (int i = 0; i < rekkaInput.mainAttackInput.Count; i++)
            {
                rekkaInput.mainAttackInput[i].turnStringToArray();
            }
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
    public  Attack_BaseProperties ReturnIndexedRekka(int index) 
    {
        return rekkaInput._rekkaProperties[index]; 
    }
    public bool ReturnUsedRekkas(Attack_BaseProperties property)
    {
        return usedRekkas.Contains(property);
    }
    public void DoFollowUpAttack(int attack, Callback SendAttackOnSucess)
    {
        Attack_BaseProperties newProperty = rekkaInput._rekkaPortion[attack].individualRekkaAttack._correctInput[0].property;
        if (usedRekkas.Contains(newProperty))
        {
            Debug.LogError($"Attack: \"{newProperty._attackName}\" has already been used. Returning...");
            return;
        }
        usedRekkas.Add(newProperty);
        if (curRekkaInput >= rekkaInputCount)
        {
            Debug.LogError($"Rekka Input Allowance exceeded. Returning");
            return;
        }
        curRekkaInput++;
        rekkaInput.mainAttackProperty.InputTimer.SetTimerType(TimerType.InRekka, AttackResetTime, rekkaLeewayTime);
        _curBase._aManager.ReceiveAttack(newProperty, () => RekkaFollowUpFunctions(newProperty, SendAttackOnSucess), rekkaInput._rekkaPortion[attack].attackAfflictionSet);
    }
    public void RekkaFollowUpFunctions(Attack_BaseProperties newProperty, Callback SendAttackOnSucess) 
    {
        SendAttackOnSucess();
        _curBase.comboList3_0.SetFollowAttack(newProperty);
        if (!newProperty.cancelProperty.nextAvailableAttackRoute.HasFlag(Cancel_State.Rekka_Input_FollowUp))// != Cancel_State.Rekka_Input_FollowUp)
        {
            _curBase._cAttackTimer.SetTimerType(TimerType.Special,0.7f);
            _curBase._cAttackTimer.CheckForInput = true;
        }
    }
    public void PreformAttack(Callback SendAttackOnSucess)
    {
        _curBase.comboList3_0.ClearFollowUpAttack();
        ResetCombo();
        _curBase._aManager.ReceiveAttack(rekkaInput.mainAttackProperty,SendAttackOnSucess, attackAfflictionSet);
        SetRekkaStateTrue();
        rekkaInput.mainAttackProperty.InputTimer.SetTimerType(TimerType.InRekka,AttackResetTime, rekkaLeewayTime);
        inRekkaState = true;
    }
    public Attack_CancelInfo GetCancelInfoType()
    {
        if(usedRekkas.Count == 0) 
        {
            return rekkaInput.mainAttackProperty.cancelProperty;
        }
        else 
        {
            return rekkaInput._rekkaPortion[curRekkaInput].individualRekkaAttack._correctInput[0].property.cancelProperty;
        }
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
    public void CloseSubAttackWindow()
    {
        ResetCombo();
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
    public MoveListAttackInfo CreateMoveListData()
    {
        string attackName = rekkaInput.mainAttackProperty._attackName;
        string specialMoveInput = rekkaInput.mainAttackInput[0].attackString;
        float _meterRequirement = rekkaInput.mainAttackProperty._meterRequirement;
        MoveListAttackInfo newMoveListAttackInfo = new MoveListAttackInfo(attackName, specialMoveInput, _meterRequirement);

        return newMoveListAttackInfo;
    }
    public void HandleDamageDealing(Character_Base attacker, Character_Base target, bool blockedAttack, Attack_BaseProperties main, Attack_BaseProperties followUp = null, bool armoredAttack = false)
    {
        SendSuccessfulDamageInfo(attacker, target, blockedAttack, main, followUp);
    }
}
