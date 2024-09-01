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
    [SerializeField] private AttackInputTypes currentInput;
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
        currentInput = new AttackInputTypes(new Attack_Input(curString, curStringArray));
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
                _base._cAttackTimer.CheckForInput = true;
                // SpecialInputVerifier(input);
                //SimpleInputVerifier(input);
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
            currentInput.AddAttackInput(direction, _base.pSide.thisPosition._directionFacing, attack, _base._cHurtBox.IsGrounded());
            CompleteMoveListVerifier();
            return;
        }
        currentInput.AddDirectionalInput(direction, _base.pSide.thisPosition._directionFacing);
    }
    void CompleteMoveListVerifier()
    {
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
            int followUpAttackIndex = FindFollowUpEntry(ActiveFollowUpAttackCheck.Key, currentInput);
            if (followUpAttackIndex >= 0)
            {
                ActiveFollowUpAttackCheck.Value.DoFollowUpAttack(lastAddedinput,followUpAttackIndex, () => _base.comboList3_0.SetCurrentAttack(ActiveFollowUpAttackCheck));

            }

        }
        else
        {
            if (lastAddedinput.Button_State._state == ButtonStateMachine.InputState.pressed)
            {
                int followUpAttackIndex = FindFollowUpEntry(ActiveFollowUpAttackCheck.Key, currentInput);
                if (ActiveFollowUpAttackCheck.Value.GetAttackMoveType() == MoveType.String_Normal)
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
                else
                {
                    if (followUpAttackIndex > -1)
                    {
                        ActiveFollowUpAttackCheck.Value.DoFollowUpAttack(followUpAttackIndex, () => _base.comboList3_0.SetCurrentAttack(ActiveFollowUpAttackCheck));
                    }
                }
            }
        }
    }
    void FullMovelistCheck() 
    {
        if (lastAddedinput.Button_State._state == ButtonStateMachine.InputState.pressed)
        {
            KeyValuePair<AttackInputTypes, IAttackFunctionality> refAttackType = new KeyValuePair<AttackInputTypes, IAttackFunctionality>(currentInput, null);
            refAttackType = FindStringEntry(currentInput);
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
                        if (keyRef.Contains(movementOnlyString) && keyRef.Contains(attackButton))
                        {
                            try
                            {
                                keyRef = keyRef.Remove(movementOnlyString.IndexOf(movementOnlyString), movementOnlyString.Length);
                                keyRef = keyRef.Remove(keyRef.IndexOf(attackButton), attackButton.Length);
                                return entry;
                            }
                            catch (ArgumentOutOfRangeException) 
                            {
                                continue;
                            }
                        }
                    }
                }
            }
            else
            {
                if (entry.Key.normalAirAttackInfo == key.keyGroundCheck)
                {
                    if (key.currentAttackInput.Contains(entry.Key.normalTypeInput[0]))
                    {
                        key.currentAttackInput = key.currentAttackInput.Remove(key.currentAttackInput.IndexOf(entry.Key.normalTypeInput[0]), entry.Key.normalTypeInput[0].Length);
                        Debug.Log(entry.Value);
                        return entry;
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

    public void PrimeCombos()
    {
        PrimeMobility();
        _base.CollectCharacterMovelist();
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
        if (ActiveFollowUpAttackCheck.Value != null)
        {
            ActiveFollowUpAttackCheck.Value.ResetAttackData();
            ActiveFollowUpAttackCheck = new KeyValuePair<AttackInputTypes, IAttackFunctionality>(currentInput, null);
        }

    }

}