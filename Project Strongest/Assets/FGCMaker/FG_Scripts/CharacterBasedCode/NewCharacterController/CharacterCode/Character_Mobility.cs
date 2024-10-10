using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public class Character_Mobility : IMobility
{
    internal Character_Base baseCharacter;
    public string movementName;
    public MovementType type;
    public int curInput;
    public List<MovementInputs> _movementInputs;
    public MobilityAnimation mobilityAnim;
    public Character_InputTimer_Mobility cTimer;
    public int movementPriority;
    public bool isSuperVariant;
    public bool activeMove;
    public Character_Mobility Clone() 
    {
        return new Character_Mobility(baseCharacter,movementName,type,curInput,
            _movementInputs,mobilityAnim,cTimer,movementPriority,isSuperVariant,activeMove);
    }
    public Character_Mobility(Character_Base _base, string _movementName, MovementType _type, int _curInput,
        List<MovementInputs> _moveInputs, MobilityAnimation _mobAnim, Character_InputTimer_Mobility _cTimer,
        int _movementPriority, bool _superVariant, bool _activeMove) 
    {
        baseCharacter = _base;
        movementName = _movementName;
        type = _type;
        curInput = _curInput;
        _movementInputs = _moveInputs;
        mobilityAnim = _mobAnim;
        cTimer = _cTimer;
        movementPriority = _movementPriority;
        isSuperVariant = _superVariant;
        activeMove = _activeMove;

    }
    public void TurnInputsToString(Character_Base _base)
    {
        Messenger.AddListener(Events.ResetMoveOnTimer, ResetCurrentInput);
        for (int i = 0; i < _movementInputs.Count; i++) 
        {
            _movementInputs[i].SetInputToArray();
        }
        cTimer.SetStartingValues();
        movementName = type.ToString();
        curInput = 0;
    }
    public bool CheckMovement(Character_ButtonInput movement, Character_Base curBase, bool superPropAvaiable)
    {
        cTimer.CheckForInput = true;
        if (IsCorrectInput(movement, curInput))
        {
            curInput++;
            foreach (MovementInputs curMove in _movementInputs)
            {
                if (superPropAvaiable)
                {
                    if (isSuperVariant == false)
                    {
                        continue;
                    }
                    if (curInput >= curMove.movementString.Length)
                    {
                        activeMove = true;
                        PlayAnimation(this, curBase);
                        ResetCurrentInput();
                        return true;
                    }
                    else
                    {
                        ResetOnSuccess();
                        continue;
                    }
                }
                else 
                {
                    if (movementPriority != 2)
                    {
                        if (isSuperVariant == true)
                        {
                            continue;
                        }
                    }
                    if (curInput >= curMove.movementString.Length)
                    {
                        activeMove = true;
                        PlayAnimation(this, curBase);
                        ResetCurrentInput();
                        return true;
                    }
                    else
                    {
                        ResetOnSuccess();
                        continue;
                    }
                }
            }
        }
        return false;
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
    public bool IsCorrectInput(Character_ButtonInput newInput, int curInput)
    {
        for (int i = 0; i < _movementInputs.Count; i++)
        {
            try
            {
                bool moveInput = false;
                if (baseCharacter.pSide.thisPosition._directionFacing == Character_Face_Direction.FacingRight)
                {
                    moveInput = _movementInputs[0].stringCharArray[curInput].ToString() == newInput.Button_State.directionalInput.ToString();
                }
                else
                {
                    moveInput = _movementInputs[0].stringCharArray[curInput].ToString() == TransfigureDirectionOnSideSwitch(newInput).ToString();
                }
                if (moveInput == true)
                {
                    return true;
                }
                else
                {
                    continue;
                }
            }
            catch (IndexOutOfRangeException) { continue; }
        }
        return false;
    }
    public bool ContinueCombo(Character_ButtonInput movement, Character_Base curBase, bool superPropAvailable)
    {
        return CheckMovement(movement, curBase, superPropAvailable);
    }

    public void PlayAnimation(Character_Mobility _mobilityAnim, Character_Base curBase)
    {
        if (curBase._cHurtBox.IsGrounded() == true && curBase._cForce.CanSendForce())
        {
            if (curBase._cAnimator._lastMovementState == lastMovementState.nullified)
            {
                if (mobilityAnim._customMobilityCallBacks.Count <= 0)
                {
                    mobilityAnim.ReAddCustomCallbacks();
                }
                curBase._aManager.ResetMoveHierarchy();
                curBase._cAnimator.SetActivatedInput(_mobilityAnim);
            }
        }
    }
    public void ResetCurrentInput()
    {
        curInput = 0;
        mobilityAnim.frameData.ResetExtraFrames();
    }

    public void ResetOnSuccess()
    {
        cTimer.ResetTimerSuccess();

        if (mobilityAnim._customMobilityCallBacks.Count <= 0)
        {
            mobilityAnim.ReAddCustomCallbacks();
        }
    }

    public void SetAnims(Character_Animator animator)
    {   
        mobilityAnim.SetMobilityAnim(animator);
    }

    public void ClearAnimatorAndBase() 
    {
        baseCharacter = null;
        mobilityAnim.playerAnim = null;
    }
}

[Serializable]
public class MovementInputs
{
    public string movementString;       // directional input in string format
    public char[] stringCharArray;          // broken pieces of directional input
    public bool canCheckInput;
    public bool completedString;
    public void SetInputToArray()
    {
        stringCharArray = movementString.ToCharArray();
    }
}
[Serializable]
public enum MovementType
{
    Jump,
    BackJump,
    ForwardJump,
    NeutralSuperJump,
    BackSuperJump,
    ForwardSuperJump,
    ForwardDash,
    BackDash,
    Empty,
}
[Serializable]
public class MobilityAnimation 
{
    public Animator playerAnim;
    public List<AnimationClip> animClip;
    public List<string> animName;
    public float totalWaitTime;

    public List<float> animLength;
    public FrameData frameData;
    public List<CustomCallback> _customMobilityCallBacks;

    public MobilityAnimation(Animator _anim, List<AnimationClip> _animClip, List<string> _animName, float _totalWaitTime, List<float> _animLength, FrameData _frameData) 
    {
        playerAnim = _anim;
        animClip = _animClip;
        animName = _animName;
        totalWaitTime = _totalWaitTime;
        animLength = _animLength;
        frameData = _frameData;
    }

    public void AddCustomCallbacks()
    {
        _customMobilityCallBacks = new List<CustomCallback>();
        for (int i = 0; i < frameData._extraPoints.Count; i++)
        {
            frameData._extraPoints[i].hitFrameBool = false;
            CustomCallback customCallback = new CustomCallback(frameData._extraPoints[i].call, frameData._extraPoints[i].hitFramePoints,
                frameData._extraPoints[i].hitFrameBool, frameData._extraPoints[i].camPos,
                frameData._extraPoints[i].camRotation, frameData._extraPoints[i].Force,
                frameData._extraPoints[i].projectileSpeed);
            _customMobilityCallBacks.Add(customCallback);
        }
    }
    public void SetMobilityAnim(Character_Animator _playerAnim)
    {
        //playerAnim = _playerAnim.myAnim;
        
        animLength = new List<float>();
        animName = new List<string>();
        totalWaitTime = 0;
        for (int i = 0; i < animClip.Count; i++) 
        {
            if (animClip != null)
            {
                animName.Add(animClip[i].name);
                animLength.Add(animClip[i].length);
                frameData.SetRecoveryFrames(60f, animLength[i]);
            }
            else
            {
                Debug.LogError($"Animation Clip is Empty on {this}... Add AnimationClip");
            }
        }
        if (animClip.Count > 1)
        {
            for (int i = 0; i < animClip.Count; i++)
            {
                if (i == 1)
                {
                    float scaledWaitTime = animClip[i].length / 0.45f;
                    totalWaitTime += scaledWaitTime;
                }
                else
                {
                    totalWaitTime += animClip[i].length;
                }
            }
        }
        AddCustomCallbacks();
    }
    public void ReAddCustomCallbacks() 
    {
        AddCustomCallbacks();
    }
}
