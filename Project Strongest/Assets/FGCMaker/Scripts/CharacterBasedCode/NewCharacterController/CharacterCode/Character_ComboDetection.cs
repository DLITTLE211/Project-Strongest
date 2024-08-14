using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

public class Character_ComboDetection : MonoBehaviour
{
    [SerializeField] private Character_Base _base;
    [SerializeField] private Character_Animator _animator;
    [SerializeField] private int lastInput;
    public bool inStance, inRekka,inSuper ,superMobilityOption;
    private bool canCheckMovement;
    [SerializeField] private AttackInputTypes currentInput;
    [SerializeField] private KeyValuePair<AttackInputTypes,IAttackFunctionality> ActiveFollowUpAttackCheck;
    private string curString;
    private char[] curStringArray;
    private Character_ButtonInput lastAddedinput;
    List<MoveType> followUpInputMoveTypes;
    private void Start()
    {
        canCheckMovement = false;
        lastInput = 5; 
        curString = "";
        currentInput = new AttackInputTypes(new Attack_Input(curString,curStringArray));
        SetFollowUpAttackTypes();
    }
    void SetFollowUpAttackTypes() 
    {
        followUpInputMoveTypes = new List<MoveType>();
        followUpInputMoveTypes.Add(MoveType.String_Normal);
        followUpInputMoveTypes.Add(MoveType.Stance);
        followUpInputMoveTypes.Add(MoveType.Rekka);
    }
    public void SetAnimator(Character_Animator myAnim) 
    {
        _animator = myAnim;
    }
    public void CheckPossibleCombos(Character_ButtonInput newInput)
    {
        StoreNewInput(newInput);
    }
    public void StoreNewInput(Character_ButtonInput input)
    {
        if (input.Button_State._state != ButtonStateMachine.InputState.directional)
        {
            if (_base._cAnimator.inputWindowOpen)
            {
                // SpecialInputVerifier(input);
                //SimpleInputVerifier(input);
                lastAddedinput = input;
                AddToCurrentInput(lastInput,input);
            }
        }
        else
        {
            if (lastInput != input.Button_State.directionalInput && canCheckMovement)
            {
                _base._cAttackTimer.CheckForInput = true;
                _base._cAttackTimer.ResetTimerSuccess();
                lastInput = input.Button_State.directionalInput;
                // SpecialInputVerifier(input);
                AddToCurrentInput(lastInput);
            }
            ExtraMovementVerifier(input);
        }
    }
    void AddToCurrentInput(int direction, Character_ButtonInput attack = null) 
    {
        if (attack != null)
        {
            currentInput.AddAttackInput(direction, _base.pSide.thisPosition._directionFacing,attack,_base._cHurtBox.IsGrounded());
            CompleteMoveListVerifier();
            return;
        }
        currentInput.AddDirectionalInput(direction,_base.pSide.thisPosition._directionFacing);
    }
    void CompleteMoveListVerifier()
    {
        if (ActiveFollowUpAttackCheck.Value != null)
        {

        }
        else
        {
            if (lastAddedinput.Button_State._state == ButtonStateMachine.InputState.pressed)
            {
                if (FindStringEntry(currentInput).Value != null)
                {
                    IAttackFunctionality refAttackType = FindStringEntry(currentInput).Value;
                    MoveType indexMoveType = refAttackType.GetAttackMoveType();
                    if (_base._aManager.MoveTypeHierarchy > indexMoveType)
                    {
                        return;
                    }
                    else
                    {
                        if (followUpInputMoveTypes.Contains(indexMoveType))
                        {
                            ActiveFollowUpAttackCheck = new KeyValuePair<AttackInputTypes, IAttackFunctionality>(currentInput, refAttackType);
                        }
                        refAttackType.PreformAttack();
                    }
                    Debug.Log("attack found");
                }
                Debug.Log("attack not found");
                /* 
                 IAttackFunctionality refAttackType = null;
                 if (_base.CharacterMoveListAttacks.TryGetValue(currentInput, out refAttackType))
                 {
                     MoveType indexMoveType = refAttackType.GetAttackMoveType();
                     if (_base._aManager.MoveTypeHierarchy > indexMoveType)
                     {
                         return;
                     }
                     else
                     {
                         if (followUpInputMoveTypes.Contains(indexMoveType))
                         {
                             ActiveFollowUpAttackCheck = refAttackType;
                         }
                         refAttackType.PreformAttack();
                     }
                 }*/
            }
        }
    }
    public void PrimeCombos()
    {
        PrimeMobility();
        _base.CollectCharacterMovelist();
    }
    void PrimeSpecialMoves()
    {
        #region Super Storage
        for (int i = 0; i < _base.comboList3_0.BasicSuperAttacks.Count; i++)
        {
            _base.CustomSuperAttackList.Add(_base.comboList3_0.BasicSuperAttacks[i]);
        }
        #endregion

        #region Command Throw Storage
        for (int i = 0; i < _base.comboList3_0.CommandThrows.Count; i++)
        {
            _base.CommandThrowAttackList.Add(_base.comboList3_0.CommandThrows[i]);
        }
        #endregion

        #region Counter Storage
        for (int i = 0; i < _base.comboList3_0.CounterAttacks.Count; i++)
        {
            _base.counterAttackList.Add(_base.comboList3_0.CounterAttacks[i]);
        }
        #endregion

        #region Stance Specials Storage
        for (int i = 0; i < _base.comboList3_0.stanceSpecials.Count; i++)
        {
            _base.stanceAttackList.Add(_base.comboList3_0.stanceSpecials[i]);
        }
        #endregion

        #region Rekka Specials Storage
        for (int i = 0; i < _base.comboList3_0.rekkaSpecials.Count; i++)
        {
            _base.rekkaAttackList.Add(_base.comboList3_0.rekkaSpecials[i]);
        }
        #endregion

        #region Basic Special Storage
        for (int i = 0; i < _base.comboList3_0.special_Simple.Count; i++)
        {
            _base.specialMoveList.Add(_base.comboList3_0.special_Simple[i]);
        }
        #endregion
    }
    void PrimeMobility() 
    {
        for (int i = 0; i < _base._extraMoveAsset.MobilityOptions.Count; i++)
        {
            _base._extraMoveControls[i].cTimer = _base._cMobiltyTimer;
            _base._extraMoveControls[i].baseCharacter = _base;
            MobilityAnimation mobilityAnim = _base._extraMoveControls[i].mobilityAnim;
            _base._extraMoveControls[i].mobilityAnim = new MobilityAnimation(_animator.myAnim, mobilityAnim.animClip, mobilityAnim.animName, mobilityAnim.totalWaitTime, mobilityAnim.animLength, mobilityAnim.frameData);

            _base._extraMoveControls[i].TurnInputsToString(_base);
            _base._extraMoveControls[i].ResetCurrentInput();
            _base._extraMoveControls[i].SetAnims(_base._cAnimator);
        }
        canCheckMovement = true;
    }
    void SetSimpleButtons()
    {
        _base.simpleAttackList.Clear();
        for (int i = 0; i < _base.comboList3_0.simpleAttacks.Count; i++)
        {
            _base.simpleAttackList.Add(_base.comboList3_0.simpleAttacks[i]);
        }
        _base.BasicThrows.Clear();
        for (int i = 0; i < _base.comboList3_0.BasicThrows.Count; i++)
        {
            _base.BasicThrows.Add(_base.comboList3_0.BasicThrows[i]);
        }
    }
    /*void SetSpecialButtons()
    {
        _base.specialMoveList.Clear();
        _base.rekkaAttackList.Clear();
        _base.stanceAttackList.Clear();
        _base.counterAttackList.Clear();
        _base.CommandThrowAttackList.Clear();
        _base.CustomSuperAttackList.Clear();

        #region Super Storage
        for (int i = 0; i < _base.comboList3_0.BasicSuperAttacks.Count; i++)
        {
            _base.CustomSuperAttackList.Add(_base.comboList3_0.BasicSuperAttacks[i]);
        }
        #endregion

        #region Command Throw Storage
        for (int i = 0; i < _base.comboList3_0.CommandThrows.Count; i++)
        {
            _base.CommandThrowAttackList.Add(_base.comboList3_0.CommandThrows[i]);
        }
        #endregion

        #region Counter Storage
        for (int i = 0; i < _base.comboList3_0.CounterAttacks.Count; i++)
        {
            _base.counterAttackList.Add(_base.comboList3_0.CounterAttacks[i]);
        }
        #endregion

        #region Stance Specials Storage
        for (int i = 0; i < _base.comboList3_0.stanceSpecials.Count; i++)
        {
            _base.stanceAttackList.Add(_base.comboList3_0.stanceSpecials[i]);
        }
        #endregion

        #region Rekka Specials Storage
        for (int i = 0; i < _base.comboList3_0.rekkaSpecials.Count; i++)
        {
            _base.rekkaAttackList.Add(_base.comboList3_0.rekkaSpecials[i]);
        }
        #endregion

        #region Basic Special Storage
        for (int i = 0; i < _base.comboList3_0.special_Simple.Count; i++)
        {
            _base.specialMoveList.Add(_base.comboList3_0.special_Simple[i]);
        }
        #endregion
    }*/

    void ExtraMovementVerifier(Character_ButtonInput input)
    {
        for (int i = 0; i < _base._extraMoveControls.Count; i++)
        {
            Character_Mobility c = _base._extraMoveControls[i];
            c.ContinueCombo(input, _base, superMobilityOption);
        }
    }
    public void ResetCombos()
    {
        currentInput.ResetComboInfo();

       /* 
        for (int i = 0; i < _base.simpleAttackList.Count; i++)
        {
            _base.comboList3_0.simpleAttacks[i].ResetCombo();
            _base.simpleAttackList[i].ResetCombo();
        }
        for (int i = 0; i < _base.BasicThrows.Count; i++)
        {
            _base.comboList3_0.BasicThrows[i].ResetCombo();
            _base.BasicThrows[i].ResetCombo();
        }
        
        for (int i = 0; i < _base.specialMoveList.Count; i++)
        {
            _base.comboList3_0.special_Simple[i].ResetCombo();
            _base.specialMoveList[i].ResetCombo();
        }
        for (int i = 0; i < _base.rekkaAttackList.Count; i++)
        {
            _base.comboList3_0.rekkaSpecials[i].ResetCombo();
            _base.rekkaAttackList[i].ResetCombo();
        }
        for (int i = 0; i < _base.stanceAttackList.Count; i++)
        {
            _base.comboList3_0.stanceSpecials[i].ResetCombo();
            _base.stanceAttackList[i].ResetCombo();
        }
        for (int i = 0; i < _base.counterAttackList.Count; i++)
        {
            _base.comboList3_0.CounterAttacks[i].ResetCombo();
            _base.counterAttackList[i].ResetCombo();
        }
        for (int i = 0; i < _base.CommandThrowAttackList.Count; i++)
        {
            _base.comboList3_0.CommandThrows[i].ResetCombo();
            _base.CommandThrowAttackList[i].ResetCombo();
        }
        
        for (int i = 0; i < _base.CustomSuperAttackList.Count; i++)
        {
            _base.comboList3_0.BasicSuperAttacks[i].ResetCombo();
            _base.CustomSuperAttackList[i].ResetCombo();
        }
       */
    }
    public void OnSuccessfulSpecialMove(Attack_BaseProperties attack)
    {
        for (int i = 0; i < _base.specialMoveList.Count; i++)
        {
            if (_base.comboList3_0.special_Simple[i].ReturnMoveComplete() && _base.comboList3_0.special_Simple[i].property != attack)
            {
                _base.comboList3_0.special_Simple[i].ResetCombo();
                _base.specialMoveList[i].ResetCombo();
            }
            else { continue; }
        }
        for (int i = 0; i < _base.comboList3_0.rekkaSpecials.Count; i++)
        {
            if (_base.comboList3_0.rekkaSpecials[i].ReturnMoveComplete() && _base.comboList3_0.rekkaSpecials[i].rekkaInput.mainAttackProperty != attack)
            {
                if (_base.comboList3_0.rekkaSpecials[i].rekkaInput._rekkaProperties.Contains(attack)) 
                {
                    continue;
                }
                _base.comboList3_0.rekkaSpecials[i].ResetCombo();
                _base.rekkaAttackList[i].ResetCombo();
            }
            else { continue; }
        }
        for (int i = 0; i < _base.comboList3_0.stanceSpecials.Count; i++)
        {
            if (_base.comboList3_0.stanceSpecials[i].ReturnMoveComplete() && _base.comboList3_0.stanceSpecials[i].stanceStartProperty != attack)
            {
                _base.comboList3_0.stanceSpecials[i].ResetCombo();
                _base.stanceAttackList[i].ResetCombo();
            }
            else { continue; }
        }
        for (int i = 0; i < _base.comboList3_0.CounterAttacks.Count; i++)
        {
            if (_base.comboList3_0.CounterAttacks[i].ReturnMoveComplete() && _base.comboList3_0.CounterAttacks[i].property != attack)
            {
                _base.comboList3_0.CounterAttacks[i].ResetCombo();
                _base.counterAttackList[i].ResetCombo();
            }
            else { continue; }
        }
        for (int i = 0; i < _base.CommandThrowAttackList.Count; i++)
        {
            if (_base.comboList3_0.CommandThrows[i].ReturnMoveComplete() && _base.comboList3_0.CommandThrows[i].property != attack)
            {
                _base.comboList3_0.CommandThrows[i].ResetCombo();
                _base.CommandThrowAttackList[i].ResetCombo();
            }
            else { continue; }
        }
        
        for (int i = 0; i < _base.CustomSuperAttackList.Count; i++)
        {
            if (_base.comboList3_0.BasicSuperAttacks[i].ReturnMoveComplete() && _base.comboList3_0.BasicSuperAttacks[i].property != attack)
            {
                _base.comboList3_0.BasicSuperAttacks[i].ResetCombo();
                _base.CustomSuperAttackList[i].ResetCombo();
            }
            else { continue; }
        }
    }
    private KeyValuePair<AttackInputTypes, IAttackFunctionality> FindStringEntry(AttackInputTypes key)
    {
        if (key.specialMoveTypeInput.attackString == null || key.specialMoveTypeInput.attackString == "")
        {
            throw new KeyNotFoundException();
        }
        KeyValuePair<AttackInputTypes, IAttackFunctionality> entry = new KeyValuePair<AttackInputTypes, IAttackFunctionality>();
        for (int i = 0; i < _base.CharacterMoveListAttacks.Count; i++)
        {
            entry = _base.CharacterMoveListAttacks.ElementAt(i);
            if (entry.Key.specialMoveTypeInput != null)
            {
                string moveInDict = entry.Key.specialMoveTypeInput.attackString;
                string keyRef = key.specialMoveTypeInput.attackString;
                if (moveInDict.Contains(keyRef))
                {
                    moveInDict = moveInDict.Remove(moveInDict.IndexOf(keyRef));
                    keyRef = keyRef.Remove(keyRef.IndexOf(keyRef));
                    if (moveInDict == keyRef)
                    {
                        Debug.Log(entry.Value);
                        return entry;
                    }
                }
            }
            if (entry.Key.normalTypeInput != null)
            {
                if (entry.Key.normalTypeInput[0] == key.currentAttackInput)
                {
                    Debug.Log(entry.Value);
                    return entry;
                }
            }
            continue;
        }

        return entry;
    }
    private bool FindFollowUpEntry(AttackInputTypes key)
    {
        if (key.specialMoveTypeInput.attackString == null || key.specialMoveTypeInput.attackString == "")
        {
            throw new KeyNotFoundException();
        }
        AttackInputTypes entry = ActiveFollowUpAttackCheck.Key;
        if (entry != null)
        {
            for (int i = 0; i < entry.normalTypeInput.Count; i++) 
            {
                if (entry.normalTypeInput[i] == key.currentAttackInput)
                {
                    return true;
                }
                continue;
            }
        }
        return false;
    }
}
[Serializable]
public class AttackInputCustomComparer : IEqualityComparer<AttackInputTypes>
{
    public bool Equals(AttackInputTypes x, AttackInputTypes y)
    {
        if (y.moveType == MoveType.Key)
        {
            if ((int)x.moveType <= (int)MoveType.String_Normal)
            {
                bool startingInputCheck = x.normalTypeInput[0] == y.currentAttackInput;
                bool groundClearanceCheck = false;
                if (x.normalAirAttackInfo == AirAttackInfo.AirOk)
                {
                    groundClearanceCheck = true;
                }
                else
                {
                    groundClearanceCheck = x.normalAirAttackInfo == y.keyGroundCheck;
                }
                bool fullcheck = startingInputCheck && groundClearanceCheck;
                return fullcheck;
            }
            else
            {
                bool fullCheck = x.specialMoveTypeInput.attackString == y.specialMoveTypeInput.attackString;
                return fullCheck;
            }
        }
        else
        {
            if ((int)x.moveType <= (int)MoveType.String_Normal)
            {
                bool startingInputCheck = x.normalTypeInput[0] == y.normalTypeInput[0];
                bool groundClearanceCheck = x.normalAirAttackInfo == y.normalAirAttackInfo;
                bool fullcheck = startingInputCheck && groundClearanceCheck;
                return fullcheck;
            }
            else
            {
                return x.specialMoveTypeInput.attackString == y.specialMoveTypeInput.attackString;
            }
        }
    }
    public int GetHashCode(AttackInputTypes obj)
    {
        if (obj == null) 
        { 
            return 0; 
        }
        int inputHash = 0;
        if (obj.moveType == MoveType.Key)
        {
            if (obj.specialMoveTypeInput.attackStringArray.Length >= 3) 
            {
                inputHash = (int)obj.specialMoveTypeInput.attackString.GetHashCode();
            }
            else 
            {
                inputHash = (int)obj.currentAttackInput.GetHashCode();
            }
        }
        else
        {
            if ((int)obj.moveType <= (int)MoveType.String_Normal)
            {
                inputHash = obj.normalTypeInput[0].GetHashCode();
            }
            else
            {
                 inputHash = obj.specialMoveTypeInput.attackString.GetHashCode();
            }
        }
        obj.hash = inputHash;
        return inputHash;
    }
}