using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;
using System;

public interface IThrowAction 
{
    bool IsCorrectInput(Character_ButtonInput move, Character_ButtonInput attack, Character_Base curBase);
    bool CheckCombo(Character_ButtonInput move, Character_ButtonInput attack, Character_Base curBase);

    void PreformAttack(int curInput, int CurrentAttack, Character_Base curBase);
    void SendSuccessfulDamageInfo(Path_Data _data, Character_Base target, bool blockedAttack);
    void SendCounterHitInfo(Path_Data _data, Character_Base target);
    void SetStarterInformation();
    void SetComboTimer(Character_InputTimer_Attacks timer);
}
[Serializable]
public abstract class ThrowActionBase 
{
    public string ThrowName;
    public Attack_BasicInput _attackInput;
    public List<AttackHandler_Attack> _throwAnimation;
    public abstract bool ContinueCombo(Character_ButtonInput i, Character_ButtonInput j, Character_Base curBase);
    public abstract void CheckButtonInfo(InputAction buttonInfo);
    public abstract void ResetCombo();
    public abstract void ResetMoveCombo();

}
[Serializable]
public class Attack_ThrowBase : ThrowActionBase , IAttackFunctionality //IThrowAction
{
    [SerializeField] private int curInput, curAttack;
    [SerializeField] private int lastDirection;
    [SerializeField] private Character_Base curBase;
    public (Attack_BaseInput.MoveInput, Attack_BaseInput.AttackInput) _newinput;
    AttackData attackData;

    public void DisableCheckable()
    {
        throw new NotImplementedException();
    }

    public MoveType GetAttackMoveType()
    {
        throw new NotImplementedException();
    }
    #region Attack Base Code
    public override void CheckButtonInfo(InputAction buttonInfo)
    {
        throw new System.NotImplementedException();
    }

    public override void ResetCombo()
    {
        curInput = 0;
        curAttack = 0;
        _attackInput._correctInput[0].property.hitConnected = false;
    }

    public override void ResetMoveCombo()
    {
        // _cTimer.ResetTimerSuccess();
    }

    public override bool ContinueCombo(Character_ButtonInput move, Character_ButtonInput attackInput, Character_Base curBase)
    {
        return CheckCombo(move, attackInput, curBase);
    }
    #endregion

    #region Attack Functionality
    public bool CheckCombo(Character_ButtonInput moveInput, Character_ButtonInput attackButton, Character_Base curBase)
    {
        for (int i = 0; i < _attackInput._correctInput.Count; i++)
        {
            _attackInput._correctInput[i].property.InputTimer.CheckForInput = true;
        }
        if (IsCorrectInput(moveInput, attackButton, curBase))
        {
            this._attackInput._correctInput[0].property.InputTimer.ResetTimerSuccess();
            //_cTimer.ResetTimerSuccess();
            attackData = new AttackData(curBase, null, null, -1, null, null, _attackInput._correctInput[curAttack - 1]);
            PreformAttack(curInput, curAttack, curBase);
            if (curInput > _attackInput._correctInput.Count)
            {
                curInput = _attackInput._correctInput.Count;

            }
            else
            {
                curInput++;
                curAttack++;
            }
            return true;
        }
        return false;
    }

    bool ButtonStateCheck(Character_ButtonInput attack)
    {
        return attack.Button_State._state == _attackInput._correctInput[curInput].attackInputState._state;
    }
    bool itemCheck()
    {
        if (curInput >= _attackInput._correctInput.Count)
        {
            ResetCombo();
        }
        bool DirectionInputCheck = _newinput.Item1 == _attackInput._correctInput[curInput].verifyAttackInput.Item1;
        char attackInput = _attackInput._correctInput[curInput].verifyAttackInput.Item2.ToString().ToCharArray()[0];
        bool AttackInputCheck = (char)_newinput.Item2 == attackInput;
        return DirectionInputCheck && AttackInputCheck;
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
        }
        return switchValue;
    }
    public bool IsCorrectInput(Character_ButtonInput move, Character_ButtonInput attack, Character_Base curBase)
    {
        if (move.Button_State.directionalInput != lastDirection)
        {
            switch (curBase.pSide.thisPosition._directionFacing)
            {
                case Character_Face_Direction.FacingLeft:
                    lastDirection = TransfigureDirectionOnSideSwitch(move);
                    break;
                case Character_Face_Direction.FacingRight:
                    lastDirection = move.Button_State.directionalInput;
                    break;
            }
        }
        //DebugMessageHandler.instance.DisplayErrorMessage(3, $"Current Direction Inputted: {lastDirection}");
        _newinput.Item1 = (Attack_BaseInput.MoveInput)lastDirection;
        char buttonInput = attack.Button_Name.ToCharArray()[0];
        _newinput.Item2 = (Attack_BaseInput.AttackInput)buttonInput;
        if (itemCheck() && ButtonStateCheck(attack))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void PreformAttack()
    {
        attackData.curBase._aManager.ReceiveAttack(attackData.normalAttack.property);
        //throw new NotImplementedException();
    }
    public void PreformAttack(int currentInput, int currentAttack, Character_Base curBase)
    {
        curBase._aManager.ReceiveAttack(_attackInput._correctInput[currentInput].property);
    }
    public void SetStarterInformation()
    {
        ResetCombo();
        for (int i = 0; i < _attackInput._correctInput.Count; i++)
        {
            _attackInput._correctInput[i].property.InputTimer.SetTimerType();
        }
        try
        {
            _attackInput.ActivateAttackInfo(ThrowName);
        }
        catch (ArgumentNullException e) { DebugMessageHandler.instance.DisplayErrorMessage(3, $"{e.Message} has taken place. Skipping Step..."); }
    }


    public void SendCounterHitInfo(Character_Base curBase)
    {
        throw new NotImplementedException();
    }
    public void SendCounterHitInfo(Path_Data _data, Character_Base target)
    {
        target._cDamageCalculator.ReceiveCounterHitMultiplier(_attackInput._correctInput[_data._curInputPath].property.counterHitDamageMult);
    }

    public void SendSuccessfulDamageInfo(Character_Base curBase, bool blockedAttack)
    {
        throw new NotImplementedException();
    }
    public void SendSuccessfulDamageInfo(Path_Data _data, Character_Base target, bool blockedAttack = false)
    {
        target._cDamageCalculator.TakeDamage(_attackInput._correctInput[_data._curInputPath].property);
    }

    public void PlayNextAttackAnimation(int currentAnimation) 
    {
        curBase._cAnimator.PlayNextAnimation(Animator.StringToHash(_throwAnimation[currentAnimation].animName), 0, true);
    }

    public void SetComboTimer(Character_InputTimer_Attacks timer)
    {
        for (int i = 0; i < _attackInput._correctInput.Count; i++)
        {
            _attackInput._correctInput[i].property.InputTimer = timer;
        }
        curBase = timer._base;
        _throwAnimation[0].SetAttackAnim(timer._base._cAnimator);
    }
    public void HandleThrowAnimAttackInfo()
    {
        _throwAnimation[0].SetAttackAnim(curBase._cAnimator);
        _throwAnimation[0].AddRequiredCallbacks(curBase);
        _throwAnimation[0].AddCustomCallbacks(_throwAnimation[0]);
        curBase._cAnimator.StartThrowFrameCount(this._attackInput._correctInput[0].property, _throwAnimation[0]);
    }


    #endregion
}
