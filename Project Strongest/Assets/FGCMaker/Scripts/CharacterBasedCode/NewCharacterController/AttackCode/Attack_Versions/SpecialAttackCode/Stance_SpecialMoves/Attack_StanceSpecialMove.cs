using System;
using UnityEngine;
using System.Collections.Generic;

[Serializable]
public class Attack_StanceSpecialMove : Attack_Special_Stance, IAttackFunctionality
{
    [SerializeField] private int curInput;
    [SerializeField] internal bool inStanceState;
    [SerializeField] internal int stanceHeldTime;
    [SerializeField] private Character_Base _curBase;
    AttackData newAttackData;

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
    public void DoFollowUpAttack(int attack, Callback SendAttackOnSucess)
    {
        newAttackData.curBase._aManager.ReceiveAttack(newAttackData.stanceAttack._stanceButtonInput._correctInput[newAttackData.stanceCurInput].property, SendAttackOnSucess);
       
        
        //newAttackData.curBase._aManager.ReceiveAttack(stanceStartProperty, SendAttackOnSucess);
    }
    public void PreformAttack(Callback SendAttackOnSucess)
    {
        newAttackData.curBase._aManager.ReceiveAttack(stanceStartProperty, SendAttackOnSucess);
        stanceStartProperty.InputTimer.SetTimerType(TimerType.InStance,(stanceHeldTime * (1/60f)));
        SetStanceStateTrue();
    }
    public void ResetAttackData()
    {
        ResetCombo();
    }
    public override void ResetCombo()
    {
        _curBase._cAnimator._lastAttackState = Character_Animator.lastAttackState.nullified;
        _curBase._cAnimator.myAnim.SetBool("Stance_Release", false);
        inStanceState = false;
    }


    public void SendSuccessfulDamageInfo(Character_Base curBase, bool blockedAttack)
    {
        throw new NotImplementedException();
    }

    public void SendCounterHitInfo(Character_Base curBase)
    {
        throw new NotImplementedException();
    }

    public void SendCounterHitInfo(Character_Base curBase, StanceAttack _stanceMove = null)
    {
        if (inStanceState || _stanceMove != null)
        {
            curBase._cDamageCalculator.ReceiveCounterHitMultiplier(_stanceMove._stanceButtonInput._correctInput[0].property.counterHitDamageMult);
        }
        else
        {
            curBase._cDamageCalculator.ReceiveCounterHitMultiplier(stanceStartProperty.counterHitDamageMult);
        }
    }

    public void SendSuccessfulDamageInfo(Character_Base curBase, bool blockedAttack, StanceAttack _stanceMove = null)
    {
        if (inStanceState || _stanceMove != null)
        {
            SendCounterHitInfo(curBase, _stanceMove);
            if (!blockedAttack)
            {
                curBase._cDamageCalculator.TakeDamage(_stanceMove._stanceButtonInput._correctInput[0].property);
            }
            else
            {
                curBase._cDamageCalculator.TakeChipDamage(_stanceMove._stanceButtonInput._correctInput[0].property);
            }
        }
        else
        {
            SendCounterHitInfo(curBase);
            if (!blockedAttack)
            {
                curBase._cDamageCalculator.TakeDamage(stanceStartProperty);
            }
            else
            {
                curBase._cDamageCalculator.TakeChipDamage(stanceStartProperty);
            }
        }
    }
    #endregion

    public MoveType GetAttackMoveType()
    {
        return stanceStartProperty._moveType;
    }
}