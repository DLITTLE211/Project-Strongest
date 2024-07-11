using System;
using UnityEngine;
using System.Collections.Generic;

[Serializable]
public class Attack_StanceSpecialMove : Attack_Special_Stance, IAttack_StanceFuctionality
{
    [SerializeField] private int curInput;
    [SerializeField] private int movementPortionLength;
    [SerializeField] private char finalAttackButton;
    [SerializeField] private bool moveComplete;
    [SerializeField] internal bool inStanceState;
    (Attack_BaseInput.MoveInput, Attack_BaseInput.AttackInput) _newinput;


    #region Stance Class Code
    public override bool ContinueCombo(Character_ButtonInput input, Character_Base curBase, Character_ButtonInput attackInput)
    {
        if (attackInput == input)
        {
            return CheckCombo(input, curBase);
        }
        else
        {
            return CheckCombo(input, curBase, attackInput);
        }
    }
    int TransfigureDirectionOnSideSwitch(Character_ButtonInput move)
    {
        int switchValue = 5;
        switch (move.Button_State.directionalInput)
        {
            case 9:
                switchValue = 7;
                break;
            case 6:
                switchValue = 4;
                break;
            case 3:
                switchValue = 1;
                break;
            case 7:
                switchValue = 9;
                break;
            case 4:
                switchValue = 6;
                break;
            case 1:
                switchValue = 3;
                break;
            default:
                switchValue = move.Button_State.directionalInput;
                break;
        }
        return switchValue;
    }
    public bool IsCorrectInput(Character_ButtonInput testInput, Character_Base _curBase, int curInput, Character_ButtonInput attackInput = null)
    {
        if (moveComplete == false)
        {
            bool moveInput = false;
            if (_curBase.pSide.thisPosition._directionFacing == Character_Face_Direction.FacingRight)
            {
                moveInput = stanceInput._stanceInput.attackStringArray[curInput].ToString() == testInput.Button_State.directionalInput.ToString();
            }
            else
            {
                moveInput = stanceInput._stanceInput.attackStringArray[curInput].ToString() == TransfigureDirectionOnSideSwitch(testInput).ToString();
            }

            // DebugMessageHandler.instance.DisplayErrorMessage(3, $"Current Direction Inputted: {TransfigureDirectionOnSideSwitch(testInput)}");
            bool moveState = testInput.Button_State._state == ButtonStateMachine.InputState.directional;
            bool thisMove = moveInput && moveState;
            return thisMove;
        }
        else
        {
            bool buttonInput = finalAttackButton.ToString() == attackInput.Button_Name.ToString();
            bool CorrectState = attackInput.Button_State._state == stanceInput._stanceInputState._state;
            bool thisAttack = buttonInput && CorrectState;
            return thisAttack;
        }
    }


    public bool CheckCombo(Character_ButtonInput Input, Character_Base curBase, Character_ButtonInput attackInput = null)
    {
        stanceStartProperty.InputTimer.CheckForInput = true;
        #region Non-StanceInput Check
        if (!inStanceState)
        {
            if (!moveComplete)
            {
                if (IsCorrectInput(Input, curBase, curInput))
                {
                    curInput++;
                    stanceStartProperty.InputTimer.ResetTimerSuccess();
                    if (curInput >= movementPortionLength && moveComplete == false)
                    {
                        moveComplete = true;
                        ResetMoveCombo();
                    }
                }
                return false;
            }
            else
            {
                if (IsCorrectInput(Input, curBase, curInput, attackInput))
                {
                    curInput++;
                    stanceStartProperty.InputTimer.ResetTimerSuccess();
                    if (curInput >= movementPortionLength + 1 && moveComplete == true)
                    {
                        PreformAttack(curBase);
                    }
                }
                return false;
            }
        }

        #endregion

        #region Stance Input Check
        else
        {
            if (attackInput.Button_State._state == ButtonStateMachine.InputState.held)
            {
                inStanceState = true;
                return false;
            }
            else
            {
                if (IsStanceInputCorrect(Input, curBase, stanceInput.stanceAttack, attackInput))
                {
                    inStanceState = false;
                    PreformAttack(curBase, stanceInput.stanceAttack);
                    return true;
                }
                if (IsStanceInputCorrect(Input, curBase, stanceInput.stanceKill, attackInput))
                {
                    
                    stanceStartProperty.InputTimer._base._cAttackTimer.SetTimerType(TimerType.Normal,0.1f);
                    PreformAttack(curBase, stanceInput.stanceKill);
                    ResetCombo();
                    return true;
                }
            }
        }
        #endregion
        return false;
    }
    bool ButtonStateCheck(Character_ButtonInput attack, StanceAttack stanceAttack)
    {
        return attack.Button_State._state == stanceAttack._stanceButtonInput._correctInput[0].attackInputState._state;
    }
    bool itemCheck(StanceAttack stanceAttack)
    {
        bool DirectionInputCheck = false;
        if (stanceAttack._stanceButtonInput._correctInput[0].verifyAttackInput.Item1 == (Attack_BaseInput.MoveInput)0)
        {
            DirectionInputCheck = true;
        }
        else 
        {
            DirectionInputCheck = _newinput.Item1 == stanceAttack._stanceButtonInput._correctInput[0].verifyAttackInput.Item1;
        }
        char attackInput = stanceAttack._stanceButtonInput._correctInput[0].verifyAttackInput.Item2.ToString().ToCharArray()[0];
        bool AttackInputCheck = (char)_newinput.Item2 == attackInput;
        return DirectionInputCheck && AttackInputCheck;
    }
    public bool IsStanceInputCorrect(Character_ButtonInput testInput, Character_Base _curBase, StanceAttack stanceAttack, Character_ButtonInput attackInput = null) 
    {
        int lastDirection = testInput.Button_State.directionalInput;
        _newinput.Item1 = (Attack_BaseInput.MoveInput)lastDirection;
        char buttonInput = attackInput.Button_Name.ToCharArray()[0];
        _newinput.Item2 = (Attack_BaseInput.AttackInput)buttonInput;
        if (ButtonStateCheck(attackInput, stanceAttack) && itemCheck(stanceAttack))
        {
            return true;
        }
        return false;
    }

    public override void TurnInputsToString()
    {
        curInput = 0;
        moveComplete = false;
        try
        {
            stanceInput._stanceInput.turnStringToArray();
            movementPortionLength = stanceInput._stanceInput.attackStringArray.Length - 1;
            finalAttackButton = stanceInput._stanceInput.attackStringArray[stanceInput._stanceInput.attackStringArray.Length - 1];
            inStanceState = false;
        }
        catch (ArgumentNullException e)
        {
            DebugMessageHandler.instance.DisplayErrorMessage(3, $"{e.Message} has taken place. Skipping Step...");
        }
    }
    public void SetAttackAnims(Character_Animator animator)
    {
        stanceStartProperty.SetAttackAnims(animator);
        stanceInput.stanceAttack._stanceButtonInput.ActivateAttackInfo();
        stanceInput.stanceKill._stanceButtonInput.ActivateAttackInfo();
    }
    public override void ResetCombo()
    {
        stanceStartProperty.InputTimer._base._cAnimator._lastAttackState = Character_Animator.lastAttackState.nullified;
       curInput = 0;
        inStanceState = false;
        moveComplete = false;
    }

    public override void ResetMoveCombo()
    {
        stanceStartProperty.InputTimer.ResetTimerSuccess();
    }
    public void PreformAttack(Character_Base curBase, StanceAttack action = null)
    {
        if (action != null)
        {
            curBase._aManager.ReceiveAttack(action._stanceButtonInput._correctInput[0].property);
        }
        else
        {
            SetStanceStateTrue();
            curBase._aManager.ReceiveAttack(stanceStartProperty);
        }
    }
#endregion

    #region Interface Code
    public void SetStanceStateTrue() 
    {
        inStanceState = true;
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

    public void SetComboTimer(Character_InputTimer_Attacks timer)
    {
        stanceStartProperty.InputTimer = timer;
        stanceInput.stanceAttack._stanceButtonInput._correctInput[0].property.InputTimer = timer;
        stanceInput.stanceKill._stanceButtonInput._correctInput[0].property.InputTimer = timer;
    }
    #endregion
}
