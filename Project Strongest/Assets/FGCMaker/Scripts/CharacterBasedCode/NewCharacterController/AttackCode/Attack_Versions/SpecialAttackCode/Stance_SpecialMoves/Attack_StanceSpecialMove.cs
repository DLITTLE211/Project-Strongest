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
    [SerializeField] internal int stanceHeldTime;
    [SerializeField] private Character_Base _curBase;
    //AttackData newAttackData;

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
            stanceInput._stanceInput.turnStringToArray();
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
    #region Stance Class Code
   /* public void PreformAttack(Character_Base curBase, StanceAttack action = null, int stanceAttackActionint = -1)
    {
        if (action != null)
        {
            curBase._aManager.ReceiveAttack(action._stanceButtonInput._correctInput[stanceAttackActionint].property);
        }
        else
        {
            SetStanceStateTrue();
            curBase._aManager.ReceiveAttack(stanceStartProperty);
        }
    }*/
    #endregion

    #region Interface Code
    public void SetStanceStateTrue()
    {
        inStanceState = true;
    }
    public void DoFollowUpKill(int kill)
    {
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
            Attack_BaseProperties newAttack = stanceInput.stanceAttack._stanceButtonInput._correctInput[attack].property;
           
            _curBase._aManager.ReceiveAttack(newAttack, () => StanceFollowUpFunctions(newAttack, SendAttackOnSucess));
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
        _curBase._aManager.ReceiveAttack(stanceStartProperty, SendAttackOnSucess);
        stanceStartProperty.InputTimer.SetTimerType(TimerType.InStance,(stanceHeldTime * (1/60f)));
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
        _curBase._cAnimator._lastAttackState = Character_Animator.lastAttackState.nullified;
         inStanceState = false;
    }


    public void SendSuccessfulDamageInfo(Character_Base attacker, Character_Base target, bool blockedAttack, Attack_BaseProperties main, Attack_BaseProperties followUp = null, bool armoredAttack = false)
    {
        if (followUp != null)
        {
            if (!blockedAttack)
            {
                SendCounterHitInfo(target, followUp);
                //target._cDamageCalculator.TakeDamage(followUp);
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
}