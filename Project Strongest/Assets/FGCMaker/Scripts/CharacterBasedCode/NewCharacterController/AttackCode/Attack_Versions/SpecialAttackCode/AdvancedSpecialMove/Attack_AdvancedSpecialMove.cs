using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Rewired;

public interface IAdvancedSpecialAction
{
    #region Function Summary
    /// <summary>
    /// Checks if Button input received is the same with Current Attack information
    /// </summary>
    /// <returns></returns>
    #endregion
    bool IsCorrectInput(Character_ButtonInput testInput, Character_Base _curBase, int curInput, Character_ButtonInput attackInput = null);
    #region Function Summary
    /// <summary>
    /// Checks combo information against inputs provided
    /// </summary>
    /// <returns></returns>
    #endregion
    bool CheckCombo(Character_ButtonInput Input, Character_Base curBase, Character_ButtonInput attackInput = null);
    #region Function Summary
    /// <summary>
    /// Upon Successful attack, sends information to character animator to do action
    /// </summary>
    /// <returns></returns>
    #endregion
    void PreformAttack(Character_Base curBase);
    #region Function Summary
    /// <summary>
    /// On Hit, will send damage numbers to hit target
    /// </summary>
    /// <returns></returns>
    #endregion
    void SendSuccessfulDamageInfo(Character_Base curBase, bool blockedAttack);
    #region Function Summary
    /// <summary>
    /// On Hit, will send counter hit damage properties to 
    /// </summary>
    /// <returns></returns>
    #endregion
    void SendCounterHitInfo(Character_Base curBase);
    #region Function Summary
    /// <summary>
    /// Sets attack timer information to this attack timer information
    /// </summary>
    /// <returns></returns>
    #endregion
    void SetComboTimer(Character_InputTimer_Attacks timer);
}
[Serializable]
public abstract class AdvancedSpecialBase
{
    public string specialMoveName;
    public Attack_Input attackInput;
    public ButtonStateMachine attackInputState;
    public Attack_BaseProperties property;
    public List<AttackHandler_Attack> _customAnimation;
    #region Function Summary
    /// <summary>
    /// Continues searching for the next input for potential combo inputs if true
    /// </summary>
    /// <param name="input"></param>
    /// <param name="curBase"></param>
    /// <returns></returns>
    #endregion
    public abstract bool ContinueCombo(Character_ButtonInput input, Character_Base curBase);
    #region Function Summary
    /// <summary>
    /// Turns inputs into readable strings for combo detection
    /// </summary>
    /// <param name="input"></param>
    /// <param name="curBase"></param>
    /// <returns></returns>
    #endregion
    public abstract void TurnInputsToString();
    #region Function Summary
    /// <summary>
    /// Reset combo upon timer ending
    /// </summary>
    /// <param name="input"></param>
    /// <param name="curBase"></param>
    /// <returns></returns>
    #endregion
    public abstract void ResetCombo();
    #region Function Summary
    /// <summary>
    /// Resets combo only on successful complete movement portion of attack
    /// </summary>
    /// <param name="input"></param>
    /// <param name="curBase"></param>
    /// <returns></returns>
    #endregion
    public abstract void ResetMoveCombo();



}
[Serializable]
public class Attack_AdvancedSpecialMove : AdvancedSpecialBase , IAdvancedSpecialAction
{
    [SerializeField] private int curInput;
    [SerializeField] private int movementPortionLength;
    [SerializeField] private char finalAttackButton;
    [SerializeField] private bool moveComplete;
    [SerializeField] private Character_Base curBase;
    [SerializeField] private int framesBetweenAttacks;

    #region Attack Base Code
    public override bool ContinueCombo(Character_ButtonInput input, Character_Base curBase)
    {
        return CheckCombo(input, curBase);
    }
    public override void ResetCombo()
    {
        curInput = 0;
        moveComplete = false;
    }
    public override void ResetMoveCombo()
    {
        property.InputTimer.ResetTimerSuccess();
    }
    public override void TurnInputsToString()
    {
        curInput = 0;
        property.InputTimer.SetTimerType();
        try
        {
            attackInput.turnStringToArray();
            movementPortionLength = attackInput.attackStringArray.Length - 1;
            finalAttackButton = attackInput.attackStringArray[attackInput.attackStringArray.Length - 1];
        }
        catch (ArgumentNullException e)
        {
            DebugMessageHandler.instance.DisplayErrorMessage(3, $"{e.Message} has taken place. Skipping Step...");
        }
    }
    #endregion

    #region Attack Functionality Code
    public bool CheckCombo(Character_ButtonInput Input, Character_Base curBase, Character_ButtonInput attackButton = null)
    {
        property.InputTimer.CheckForInput = true;
        if (IsCorrectInput(Input, curBase, curInput))
        {
            curInput++;
            property.InputTimer.ResetTimerSuccess();
            if (curInput >= movementPortionLength && moveComplete == false)
            {
                moveComplete = true;
                ResetMoveCombo();
            }
            if (curInput >= movementPortionLength + 1 && moveComplete == true)
            {
                PreformAttack(curBase);
                ResetCombo();
            }
            return true;
        }
        else { return false; }
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
    public bool IsCorrectInput(Character_ButtonInput testInput, Character_Base _curBase, int curInput, Character_ButtonInput attackButton = null)
    {
        switch (moveComplete)
        {
            case false:
                bool moveInput = false;
                if (_curBase.pSide.thisPosition._directionFacing == Character_Face_Direction.FacingRight)
                {
                    moveInput = attackInput.attackStringArray[curInput].ToString() == testInput.Button_State.directionalInput.ToString();
                }
                else
                {
                    moveInput = attackInput.attackStringArray[curInput].ToString() == TransfigureDirectionOnSideSwitch(testInput).ToString();
                }

                // DebugMessageHandler.instance.DisplayErrorMessage(3, $"Current Direction Inputted: {TransfigureDirectionOnSideSwitch(testInput)}");
                bool moveState = testInput.Button_State._state == ButtonStateMachine.InputState.directional;
                bool thisMove = moveInput && moveState;
                if (thisMove)

                { return thisMove; }
                else

                { return false; }
            case true:
                bool buttonInput = finalAttackButton.ToString() == testInput.Button_Name.ToString();
                bool CorrectState = testInput.Button_State._state == attackInputState._state;
                bool thisAttack = buttonInput && CorrectState;
                if (thisAttack)

                { return thisAttack; }
                else

                { return false; }
        }
    }
    public void PreformAttack(Character_Base curBase)
    {
        curBase._aManager.ReceiveAttack(property);
    }
    public void SendCounterHitInfo(Character_Base target)
    {
        target._cDamageCalculator.ReceiveCounterHitMultiplier(property.counterHitDamageMult);
    }
    public void PlayNextAttackAnimation(int currentAnimation)
    {
        curBase._cAnimator.PlayNextAnimation(Animator.StringToHash(_customAnimation[currentAnimation].animName), 0, true);
    }
    public void SendSuccessfulDamageInfo(Character_Base target, bool blockedAttack)
    {
        SendCounterHitInfo(target);
        if (!blockedAttack)
        {
            target._cDamageCalculator.TakeDamage(property);
        }
        else
        {
            target._cDamageCalculator.TakeChipDamage(property);
        }
    }
    public void SetComboTimer(Character_InputTimer_Attacks timer)
    {
        property.InputTimer = timer;
        curBase = timer._base;
    }
    #endregion
}
