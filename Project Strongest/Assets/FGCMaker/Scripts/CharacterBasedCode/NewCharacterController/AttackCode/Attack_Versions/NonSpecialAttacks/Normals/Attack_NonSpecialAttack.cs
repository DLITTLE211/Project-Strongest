using Rewired;
using System;
using UnityEngine;
[System.Serializable]
public class Path_Data 
{
    public int _curInputPath;
    public int _curAttackPath;
    public void SetPathData(int i, int j) 
    {
        _curInputPath = i;
        _curAttackPath = j;
    }
}
[System.Serializable]
public class Attack_NonSpecialAttack : Attack_NonSpecial_Base,  IAttack_BasicFunctionality
{
    public Path_Data _pathdata;
    [SerializeField] private int curInput,curAttack;
    [SerializeField] private int lastDirection;
    public (Attack_BaseInput.MoveInput, Attack_BaseInput.AttackInput) _newinput;
    private Character_Base _base;
    #region Attack Base Code
    public override void CheckButtonInfo(InputAction buttonInfo)
    {
        throw new System.NotImplementedException();
    }

    public override void ResetCombo()
    {
        curInput = 0;
        curAttack = 0;
        for (int i = 0; i < _attackInput._correctInput.Count; i++)
        {
            _attackInput._correctInput[i].property.hitConnected = false;
        }
    }

    public override void ResetMoveCombo()
    {
       // _cTimer.ResetTimerSuccess();
    }

    public override bool ContinueCombo(Character_ButtonInput move, Character_ButtonInput attackInput,Character_Base curBase)
    {
        return CheckCombo(move,attackInput,curBase);
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
            if (this._attackInput._correctInput[0].property._airInfo == AirAttackInfo.AirOnly && _base._cHurtBox.IsGrounded()) 
            {
               // return false;
            }
            if (this._attackInput._correctInput[0].property._airInfo == AirAttackInfo.GroundOnly && !_base._cHurtBox.IsGrounded()) 
            {
                // return false;
            }
            if (_base._cComboDetection.inRekka == true || _base._cComboDetection.inStance == true)
            {
                 return false;
            }
            if (curAttack > 0)
            {
                if (this._attackInput._correctInput[curAttack - 1].property.hitConnected == true)
                {
                    RewardAttack(curBase);
                }
                else
                {
                    return false;
                }
            }
            else 
            {
                RewardAttack(curBase);
            }
            return true;
        }
        return false;
    }
    public void RewardAttack(Character_Base curBase) 
    {
        this._attackInput._correctInput[0].property.InputTimer.ResetTimerSuccess();
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
    public bool IsCorrectInput(Character_ButtonInput move, Character_ButtonInput attack,Character_Base curBase)
    {
        if(move.Button_State.directionalInput != lastDirection) 
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
        if(itemCheck() && ButtonStateCheck(attack)) 
        {
            return true;
        }
        else 
        {
            return false;
        }
    }

    public void PreformAttack(int currentInput, int currentAttack,Character_Base curBase)
    {
        _pathdata.SetPathData(currentInput, currentAttack);
        
        curBase.comboList3_0.UpdatePathData(_pathdata);
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
            _attackInput.ActivateAttackInfo(SpecialAttackName);
        }
        catch(ArgumentNullException e) { DebugMessageHandler.instance.DisplayErrorMessage(3, $"{e.Message} has taken place. Skipping Step..."); }
    }

    public void SendCounterHitInfo(Path_Data _data, Character_Base target)
    {
        target._cDamageCalculator.ReceiveCounterHitMultiplier(_attackInput._correctInput[_data._curInputPath].property.counterHitDamageMult);
    }

    public void SendSuccessfulDamageInfo(Path_Data _data, Character_Base target, bool blockedAttack = false)
    {
        if (!blockedAttack)
        {
            target._cDamageCalculator.TakeDamage(_attackInput._correctInput[_data._curInputPath].property);
        }
        else 
        {
            target._cDamageCalculator.TakeChipDamage(_attackInput._correctInput[_data._curInputPath].property);
        }
    }

    public void SetComboTimer(Character_InputTimer_Attacks timer)
    {
        _base = timer._base;
        for (int i = 0; i < _attackInput._correctInput.Count; i++)
        {
            _attackInput._correctInput[i].property.InputTimer = timer;
        }
    }

    public void UpdateLastDirectional(Character_ButtonInput newDirection)
    {
        throw new System.NotImplementedException();
    }
    #endregion
}
