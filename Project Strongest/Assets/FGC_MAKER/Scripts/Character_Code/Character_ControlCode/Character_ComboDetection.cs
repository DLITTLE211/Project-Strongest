using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

public class Character_ComboDetection : MonoBehaviour
{
    [SerializeField] private Character_Base _base;
    [SerializeField] private Character_Animator _animator;
    [SerializeField] private int lastInput;
    public bool inStance, inRekka, inSuper, superMobilityOption;
    private bool canCheckMovement;
    [SerializeField] private AttackInputTypes currentAttackInput;
    [SerializeField] private AttackInputTypes currentMobilityInput;
    [SerializeField] private KeyValuePair<AttackInputTypes, IAttackFunctionality> ActiveFollowUpAttackCheck;
    private string curString;
    private char[] curStringArray;
    private Character_ButtonInput lastAddedinput;
    List<MoveType> followUpInputMoveTypes;
    private void Start()
    {
        canCheckMovement = false;
        lastInput = 5;
        curString = "";
        currentAttackInput = new AttackInputTypes(new Attack_Input(curString, curStringArray));
        currentMobilityInput = new AttackInputTypes(new Attack_Input(curString, curStringArray));
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
        if (_base.ReturnIfPaused())
        {
            return;
        }
        if (input.Button_State._state != ButtonStateMachine.InputState.directional)
        {
            if (_base._cAnimator.inputWindowOpen)
            {
                _base._cAttackTimer.CheckForInput = true;
                lastAddedinput = input;
                AddToCurrentInput(lastInput, input);
            }
        }
        else
        {
            if (lastInput != input.Button_State.directionalInput && canCheckMovement)
            {
                _base._cAttackTimer.CheckForInput = true;
                if (ActiveFollowUpAttackCheck.Value == null)
                {
                    _base._cAttackTimer.ResetTimerSuccess();
                }
                lastInput = input.Button_State.directionalInput;
                AddToCurrentInput(lastInput);
            }
        }
    }
    void AddToCurrentInput(int direction, Character_ButtonInput attack = null)
    {
        currentAttackInput.ClearFirstIndex();
        currentMobilityInput.ClearFirstIndex();
        if (attack != null)
        {
            currentAttackInput.AddAttackInput(direction, _base.pSide.thisPosition._directionFacing, attack, _base._cHurtBox.IsGrounded());
            CompleteMoveListVerifier();
            return;
        }
        currentAttackInput.AddDirectionalInput(direction, _base.pSide.thisPosition._directionFacing);
        currentMobilityInput.AddDirectionalInput(direction, _base.pSide.thisPosition._directionFacing);
        CompleteMobilityVerifier();
    }
    void CompleteMoveListVerifier()
    {
        if (_base._cStateMachine._playerState.current.State == _base._cStateMachine.standBlockRef || _base._cStateMachine._playerState.current.State == _base._cStateMachine.crouchBlockRef)
        {
            return;
        }
        if (ActiveFollowUpAttackCheck.Value != null)
        {
            ActiveContinuedMoveCheck();
        }
        else
        {
            FullMovelistCheck();
        }
    }

    #region Attack Verification Code
    void ActiveContinuedMoveCheck() 
    {
        if (ActiveFollowUpAttackCheck.Value.GetAttackMoveType() == MoveType.Stance)
        {
            int followUpAttackIndex = FindFollowUpEntry(ActiveFollowUpAttackCheck.Key, currentAttackInput);
            if (followUpAttackIndex >= 0)
            {
                ActiveFollowUpAttackCheck.Value.DoFollowUpAttack(lastAddedinput,followUpAttackIndex, () => _base.comboList3_0.SetCurrentAttack(ActiveFollowUpAttackCheck));
            }
        }
        else
        {
            if (lastAddedinput.Button_State._state == ButtonStateMachine.InputState.pressed)
            {
                int followUpAttackIndex = FindFollowUpEntry(ActiveFollowUpAttackCheck.Key, currentAttackInput);
                MoveType currentFollowupMoveType = ActiveFollowUpAttackCheck.Value.GetAttackMoveType();
                try
                {
                    if (currentFollowupMoveType == MoveType.String_Normal)
                    {
                        if (followUpAttackIndex > -1)
                        {
                            ActiveFollowUpAttackCheck.Value.DoFollowUpAttack(followUpAttackIndex, () => _base.comboList3_0.SetCurrentAttack(ActiveFollowUpAttackCheck));
                        }
                        else
                        {
                            FullMovelistCheck();
                        }
                    }
                    if (currentFollowupMoveType == MoveType.Rekka)
                    {
                        IAttackFunctionality newInputtedSpecial = SpecialMoveOnlyCheck();
                        if (newInputtedSpecial != null && newInputtedSpecial != ActiveFollowUpAttackCheck.Value)
                        {
                            return;
                        }
                        if (followUpAttackIndex > -1)
                        {
                            ActiveFollowUpAttackCheck.Value.DoFollowUpAttack(followUpAttackIndex, () => _base.comboList3_0.SetCurrentAttack(ActiveFollowUpAttackCheck));
                        }
                        else
                        {
                            FullMovelistCheck();
                        }
                    }
                }
                catch (NullReferenceException) 
                {
                    Debug.LogError("Null Ref Caught. Check Line"); 
                    ResetCombos();
                    return;
                }
            }
        }
    }
    IAttackFunctionality SpecialMoveOnlyCheck() 
    {
        if (lastAddedinput.Button_State._state == ButtonStateMachine.InputState.pressed)
        {
            KeyValuePair<AttackInputTypes, IAttackFunctionality> refAttackType = new KeyValuePair<AttackInputTypes, IAttackFunctionality>(currentAttackInput, null);
            refAttackType = SpecialMovePreliminaryCheck(currentAttackInput);
            if (refAttackType.Value != null && refAttackType.Value != _base.comboList3_0.ReturnCurrentAttack())
            {
                MoveType indexMoveType = refAttackType.Value.GetAttackMoveType();
                if (_base._aManager.MoveTypeHierarchy > indexMoveType)
                {
                    Debug.LogError($"Attack level of new attack , \"{indexMoveType}\" is too low!");
                    return null;
                }
                else
                {
                    if (refAttackType.Value != ActiveFollowUpAttackCheck.Value)
                    {
                        refAttackType.Value.PreformAttack(() => _base.comboList3_0.SetCurrentAttack(refAttackType));
                        if (followUpInputMoveTypes.Contains(refAttackType.Value.GetAttackMoveType()))
                        {
                            ActiveFollowUpAttackCheck = new KeyValuePair<AttackInputTypes, IAttackFunctionality>(refAttackType.Key, refAttackType.Value);
                        }
                        else
                        {
                            ResetCombos();
                        }
                    }
                    Debug.Log("attack found");
                    return refAttackType.Value;
                }
            }
            Debug.Log("attack not found");
            return null;
        }
        return null;

    }
    void FullMovelistCheck() 
    {
        if (lastAddedinput.Button_State._state == ButtonStateMachine.InputState.pressed)
        {
            KeyValuePair<AttackInputTypes, IAttackFunctionality> refAttackType = new KeyValuePair<AttackInputTypes, IAttackFunctionality>(currentAttackInput, null);
            refAttackType = FindStringEntry(currentAttackInput);
            if (refAttackType.Value != null)
            {
                MoveType indexMoveType = refAttackType.Value.GetAttackMoveType();
                if (_base._aManager.MoveTypeHierarchy > indexMoveType)
                {
                    Debug.LogError($"Attack level of new attack , \"{indexMoveType}\" is too low!");
                    return;
                }
                else
                {
                    refAttackType.Value.PreformAttack(() => _base.comboList3_0.SetCurrentAttack(refAttackType));
                    if (followUpInputMoveTypes.Contains(indexMoveType))
                    {
                        ActiveFollowUpAttackCheck = new KeyValuePair<AttackInputTypes, IAttackFunctionality>(refAttackType.Key, refAttackType.Value);
                    }
                    else
                    {
                        ResetCombos();
                    }
                }
                Debug.Log("attack found");
            }
            Debug.Log("attack not found");
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
            if ((int)entry.Value.GetAttackMoveType() > 3)
            {
                if (entry.Key.specialMoveTypeInput != null)
                {
                    string moveInDict = entry.Key.specialMoveTypeInput.attackString;
                    string keyRef = key.specialMoveTypeInput.attackString;
                    if (entry.Key.specialMoveTypeInput.attackString.Length > 0)
                    {
                        string attackButton = moveInDict.Substring(moveInDict.Length - 1);
                        string movementOnlyString = moveInDict.Remove(moveInDict.Length - 1);
                        if (keyRef.Contains(movementOnlyString))
                        {
                            int motionEndIndex = keyRef.IndexOf(movementOnlyString[0]);
                            keyRef = keyRef.Substring(motionEndIndex);
                            if (keyRef.Contains(attackButton)) 
                            {
                                keyRef = keyRef.Remove(keyRef.IndexOf(attackButton), attackButton.Length);
                                key.specialMoveTypeInput.attackString = keyRef;
                                return entry;
                            }
                        }
                        continue;
                    }
                    continue;
                }
                continue;
            }
            else
            {
                if ((int)entry.Value.GetAttackMoveType() <= 3)
                {
                    if (entry.Key.normalAirAttackInfo == key.keyGroundCheck)
                    {
                        if (key.currentAttackInput.Contains(entry.Key.normalTypeInput[0]))
                        {
                            string normalButton = entry.Key.normalTypeInput[0].Substring(entry.Key.normalTypeInput[0].Length - 1);
                            int normalButtonIndex = key.specialMoveTypeInput.attackString.IndexOf(normalButton);
                            string clearAttackButton = key.specialMoveTypeInput.attackString.Remove(normalButtonIndex);
                            //key.specialMoveTypeInput.attackString = clearAttackButton;
                            key.currentAttackInput = "";
                            Debug.Log(entry.Value);
                            return entry;
                        }
                        continue;
                    }
                    continue;
                }
            }
            continue;
        }
        entry = new KeyValuePair<AttackInputTypes, IAttackFunctionality>(key, null);
        return entry;
    }
    private KeyValuePair<AttackInputTypes, IAttackFunctionality> SpecialMovePreliminaryCheck(AttackInputTypes key) 
    {
        KeyValuePair<AttackInputTypes, IAttackFunctionality> entry = new KeyValuePair<AttackInputTypes, IAttackFunctionality>();
        for (int i = 0; i < _base.CharacterMoveListAttacks.Count; i++)
        {
            entry = _base.CharacterMoveListAttacks.ElementAt(i);
            if ((int)entry.Value.GetAttackMoveType() > 3)
            {
                if (entry.Key.specialMoveTypeInput != null)
                {
                    string moveInDict = entry.Key.specialMoveTypeInput.attackString;
                    string keyRef = key.specialMoveTypeInput.attackString;
                    if (entry.Key.specialMoveTypeInput.attackString.Length > 0)
                    {
                        string attackButton = moveInDict.Substring(moveInDict.Length - 1);
                        string movementOnlyString = moveInDict.Remove(moveInDict.Length - 1);
                        if (keyRef.Contains(movementOnlyString))
                        {
                            int motionEndIndex = keyRef.IndexOf(movementOnlyString[0]);
                            keyRef = keyRef.Substring(motionEndIndex);
                            if (keyRef.Contains(attackButton))
                            {
                                return entry;
                            }
                        }
                    }
                }
            }
            continue;
        }
        entry = new KeyValuePair<AttackInputTypes, IAttackFunctionality>(key, null);
        return entry;
    }
    private int FindFollowUpEntry(AttackInputTypes keyComparison, AttackInputTypes keyInput)
    {
        if (keyInput.currentAttackInput == null)
        {
            throw new KeyNotFoundException();
        }
        AttackInputTypes entry = keyComparison;
        bool OrderedSequence = ActiveFollowUpAttackCheck.Value.GetAttackMoveType() == MoveType.String_Normal ? true : false;
        if (OrderedSequence)
        {
            int stringComparison = ActiveFollowUpAttackCheck.Value.GetFollowUpAttackInt();
            if (stringComparison <= -1)
            {
                return -1;
            }
            try
            {
                string stringCheck = keyComparison.normalTypeInput[stringComparison];

                if (keyInput.currentAttackInput.Contains(stringCheck))
                {
                    keyInput.currentAttackInput = "";
                    return 1;
                }
            }
            catch (ArgumentOutOfRangeException)
            {
                ActiveFollowUpAttackCheck.Value.ResetAttackData();
                return -1;
            }
        }
        else
        {
            for (int i = 0; i < entry.normalTypeInput.Count; i++)
            {
                string KeyString = keyInput.currentAttackInput;
                string Comparison = entry.normalTypeInput[i];
                if (Comparison.Contains("0"))
                {
                    KeyString = KeyString.Remove(0, 1);
                    Comparison = Comparison.Remove(0, 1);
                    if (KeyString.Contains(Comparison))
                    {
                        keyInput.currentAttackInput = "";
                        return i;
                    }
                }
                else
                {
                    if (keyInput.currentAttackInput.Contains(entry.normalTypeInput[i]))
                    {
                        return i;
                    }
                }
                continue;
            }
        }
        return -1;
    }

    #endregion

    #region Mobility Verification Code
    void CompleteMobilityVerifier()
    {
        if (_base._cStateMachine._playerState.current.State == _base._cStateMachine.standBlockRef || _base._cStateMachine._playerState.current.State == _base._cStateMachine.crouchBlockRef)
        {
            return;
        }
        Character_MobilityOption curMobility = ExtraMovementVerifier(currentMobilityInput);
        if (curMobility != null)
        {
            curMobility.PerformMobilityAction();
            Debug.Log("Mobility found");
        }
        Debug.Log("Mobility not found");
    }

    Character_MobilityOption ExtraMovementVerifier(AttackInputTypes mobilityInput)
    {
        _base._cMobiltyTimer.CheckForInput = true;
        for (int i = 0; i < _base.character_MobilityOptions.Mobility.Count; i++)
        {
            Character_MobilityOption entry = _base.character_MobilityOptions.Mobility[i];
            string moveInDict = entry.mobilityInput.attackString;
            string keyRef = mobilityInput.specialMoveTypeInput.attackString;
            if (keyRef.Contains(moveInDict))
            {
                if (entry._requiresCharge && superMobilityOption == entry._requiresCharge)
                {
                    mobilityInput.specialMoveTypeInput.attackString = "";
                    return entry;
                }
                else if (!entry._requiresCharge)
                {
                    mobilityInput.specialMoveTypeInput.attackString = "";
                    return entry;
                }
                else
                {
                    continue;
                }
            }
            continue;
        }
        return null;
    }
    #endregion

    public void PrimeCombos()
    {
        PrimeMobility();
        _base.CollectCharacterMovelist();
    }
    void PrimeMobility()
    {
        for (int i = 0; i < _base.character_MobilityOptions.Mobility.Count; i++)
        {
            Character_MobilityOption curMobility = _base.character_MobilityOptions.Mobility[i];
            curMobility.SetStarterInformation(_base);
        }
        canCheckMovement = true;
    }
    public void ResetCombos()
    {
        currentAttackInput.ResetComboInfo();
        if (ActiveFollowUpAttackCheck.Value != null)
        {
            ActiveFollowUpAttackCheck.Value.ResetAttackData();
            ActiveFollowUpAttackCheck = new KeyValuePair<AttackInputTypes, IAttackFunctionality>(currentAttackInput, null);
        }
    }
    public void ResetMobilityString()
    {
        currentMobilityInput.ResetComboInfo();
    }

}