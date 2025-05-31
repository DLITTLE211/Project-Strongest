using System;
using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

[Serializable]
public class Attack_StanceSpecialMove : Attack_Special_Stance, IAttackFunctionality
{
    [SerializeField] private int curInput;
    [SerializeField] internal bool inStanceState;
    public int stanceHeldTime;
    public int AttackResetTime;
    public AfflictionSet attackAfflictionSet;

    [SerializeField] private Character_Base _curBase;
    enum StanceState {KillStance, AttackStance }
    StanceState _curStanceState;
    public void SetStarterInformation(Character_Base _base)
    {
        _curBase = _base;
        TurnInputsToString();
        SetComboTimer();
        SetAttackAnims();
    }

    public void TurnInputsToString()
    {
        curInput = 0;
        try
        {
            for (int i = 0; i < stanceInput._stanceInput.Count; i++)
            {
                stanceInput._stanceInput[i].turnStringToArray();
            }
        }
        catch (ArgumentNullException e)
        {
            DebugMessageHandler.instance.DisplayErrorMessage(3, $"{e.Message} has taken place. Skipping Step...");
        }
    }
    public void SetComboTimer()
    {
        stanceStartProperty.InputTimer = _curBase._cAttackTimer;
        if (stanceInput.stanceAttack._stanceButtonInput._correctInput.Count > 0)
        {
            stanceInput.stanceAttack._stanceButtonInput._correctInput[0].property.InputTimer = _curBase._cAttackTimer;
        }
        if (stanceInput.stanceKill._stanceButtonInput._correctInput.Count > 0)
        {
            stanceInput.stanceKill._stanceButtonInput._correctInput[0].property.InputTimer = _curBase._cAttackTimer;
        }
    }

    public void SetAttackAnims()
    {
        stanceStartProperty.SetAttackAnims(_curBase._cAnimator);
        for (int i = 0; i < stanceInput.stanceAttack._stanceButtonInput._correctInput.Count; i++)
        {
            stanceInput.stanceAttack._stanceButtonInput._correctInput[i].SetInnerAttackAnimations(_curBase._cAnimator);
            stanceInput.stanceAttack._stanceButtonInput.ActivateAttackInfo(stanceInput.stanceAttack._stanceButtonInput._correctInput[i].property._attackName);
        }
        for (int i = 0; i < stanceInput.stanceKill._stanceButtonInput._correctInput.Count; i++)
        {
            stanceInput.stanceKill._stanceButtonInput._correctInput[i].SetInnerAttackAnimations(_curBase._cAnimator);
            stanceInput.stanceKill._stanceButtonInput.ActivateAttackInfo(stanceInput.stanceKill._stanceButtonInput._correctInput[i].property._attackName);
        }
    }

    #region Interface Code
    public void SetStanceStateTrue()
    {
        inStanceState = true;
    }
    public Attack_CancelInfo GetCancelInfoType()
    {
        if (!inStanceState)
        {
            return stanceStartProperty.cancelProperty;
        }
        else 
        {
            if (_curStanceState == StanceState.AttackStance) 
            {
                return stanceInput.stanceAttack._stanceButtonInput._correctInput[curInput].property.cancelProperty;
            }
            if (_curStanceState == StanceState.KillStance) 
            {
                return stanceInput.stanceKill._stanceButtonInput._correctInput[0].property.cancelProperty;
            }
            return stanceStartProperty.cancelProperty;
        }
    }
    public void DoFollowUpKill(int kill)
    {
        _curStanceState = StanceState.KillStance;
           Attack_BaseProperties newAttack = stanceInput.stanceKill._stanceButtonInput._correctInput[kill-1].property;
        newAttack.InputTimer.SetTimerType(TimerType.Normal, 1 / 60f);
        _curBase._aManager.ClearAttacks();
        _curBase._cComboDetection.ResetCombos();
        newAttack.InputTimer.CheckForInput = true;
    }
    public void DoFollowUpAttack(Character_ButtonInput buttonInput, int attack, Callback SendAttackOnSucess) 
    {
        if (buttonInput.Button_State._state == ButtonStateMachine.InputState.held) 
        {
            Debug.LogError("Current Button Input is Still held. Returning...");
            return;
        }
        if (!(attack > stanceInput.stanceAttack._stanceButtonInput._correctInput.Count-1))
        {
            curInput = attack;
               Attack_BaseProperties newAttack = stanceInput.stanceAttack._stanceButtonInput._correctInput[attack].property;
            _curStanceState = StanceState.AttackStance;
            float stanceTime = stanceHeldTime * (1 / 60f);
            float attackResetTime = AttackResetTime * (1 / 60f);
            newAttack.InputTimer.SetTimerType(TimerType.Normal);
            _curBase._aManager.ReceiveAttack(newAttack, () => StanceFollowUpFunctions(newAttack, SendAttackOnSucess), stanceInput.stanceAttack.attackAfflictionSet);
            ResetCombo();
        }
        else 
        {
            DoFollowUpKill(attack);
        }
    }
    public void StanceFollowUpFunctions(Attack_BaseProperties newProperty, Callback SendAttackOnSucess)
    {
        SendAttackOnSucess();
        _curBase.comboList3_0.SetFollowAttack(newProperty);
    }
    public void PreformAttack(Callback SendAttackOnSucess)
    {
        _curBase.comboList3_0.ClearFollowUpAttack();
        _curBase._aManager.ReceiveAttack(stanceStartProperty, SendAttackOnSucess, attackAfflictionSet);
        float stanceTime = stanceHeldTime * (1 / 60f);
        float attackResetTime = AttackResetTime * (1 / 60f);
        stanceStartProperty.InputTimer.SetTimerType(TimerType.InStance, attackResetTime, stanceTime);
        SetStanceStateTrue();
    }
    public void ResetAttackData()
    {
        ResetCombo();
    }
    public override void ResetCombo()
    {
        if (!_curBase.awaitCondition)
        {
            _curBase.awaitCondition = true;
        }
        _curBase._cAnimator._lastAttackState = lastAttackState.nullified;
         inStanceState = false;
    }


    public void SendSuccessfulDamageInfo(Character_Base attacker, Character_Base target, bool blockedAttack, Attack_BaseProperties main, Attack_BaseProperties followUp = null, bool armoredAttack = false)
    {
        if (followUp != null)
        {
            if (!blockedAttack)
            {
                SendCounterHitInfo(target, followUp);
            }
            target._cDamageCalculator.ReceiveDamage(followUp, blockedAttack, armoredAttack);
        }
        else
        {
            if (!blockedAttack)
            {
                SendCounterHitInfo(target,main);
            }
            target._cDamageCalculator.ReceiveDamage(followUp, blockedAttack, armoredAttack);
        }
    }

    public void CloseSubAttackWindow()
    {
        ResetCombo();
    }
    public void SendCounterHitInfo(Character_Base target, Attack_BaseProperties main)
    {
        target._cDamageCalculator.ReceiveCounterHitMultiplier(main.counterHitDamageMult);
    }
    #endregion

    public MoveType GetAttackMoveType()
    {
        return stanceStartProperty._moveType;
    }

    public void HandleDamageDealing(Character_Base attacker, Character_Base target, bool blockedAttack, Attack_BaseProperties main, Attack_BaseProperties followUp = null, bool armoredAttack = false)
    {
        SendSuccessfulDamageInfo(attacker, target, blockedAttack, main,followUp);
    }
    public MoveListAttackInfo CreateMoveListData()
    {
        string attackName = stanceStartProperty._attackName;
        string specialMoveInput = stanceInput._stanceInput[0].attackString;
        float _meterRequirement = stanceStartProperty._meterRequirement;
        MoveListAttackInfo newMoveListAttackInfo = new MoveListAttackInfo(attackName, specialMoveInput, _meterRequirement);

        return newMoveListAttackInfo;
    }
}