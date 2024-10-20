using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;
using System;

[System.Serializable]
public class ExtraMovementControl : MovementInput
{
    public Character_InputTimer_Mobility checkTimer;
    public void turnInputsToString(Character_Base _base)
    {
        checkTimer = _base._cMobiltyTimer;
        try
        {
           // Messenger.AddListener(Events.ResetMoveOnTimer, ResetCurrentInput);
            for (int i = 0; i < possibleInputs.Count; i++)
            {
                possibleInputs[i].SetInputToArray();
            }
            curInput = 0;
        }
        catch (ArgumentNullException e) { DebugMessageHandler.instance.DisplayErrorMessage(2, $"{e} has taken place. Skipping step..."); }

        checkTimer.SetStartingValues();
    }
    public void SetupAnimations(Animator animator) 
    {
        myNeutralAnim.SetNeutralAnim(animator);
    }

    public bool MovementCheck(Character_ButtonInput Input,Character_Base curBase)
    {
        return false;
        /*checkTimer.CheckForInput = true;
        if (isCorrectInput(Input, curInput))
        {
            //Include check for different attack styles. either here or in separate script/location
            curInput++;
            for (int i = 0; i < possibleInputs.Count; i++) 
            {
                if (curInput >= possibleInputs[i].defaultStringPieces.Length && possibleInputs[i].possibleActiveInput == true)
                {
                    ResetCurrentInput();
                    PlayAnimation(curBase);
                    return true;
                }
                else
                {
                    setNextMoveCombo();
                    Messenger.Broadcast(Events.ClearLastInput);
                    continue;
                }
            }
            return true;
        }
        else
        {
            ResetCurrentInput();
            return false;
        }*/
    }

    public void PlayAnimation(Character_Base curBase) 
    {
        //curBase._cAnimator.SetActivatedInput(this);
    }
    public void CallAction( MovementInput playAnim, Character_Base curBase) 
    {
        //curBase._cForce.HandleExtraMovement(playAnim);
    }
    public bool continueCombo(Character_ButtonInput i,Character_Base curBase)
    {
        return MovementCheck(i, curBase);
    }
    public void setNextMoveCombo()
    {
        checkTimer.ResetTimerSuccess();
    }
    public void ResetCurrentInput()
    {
        curInput = 0;
        for (int i = 0; i < possibleInputs.Count; i++) 
        {
           // possibleInputs[i].possibleActiveInput = false;
        }
    }
}
[System.Serializable]
public class MovementInput
{
    [SerializeField] public string movementName;
    public MovementType mtype;
    public int movementPriority;
    public int curInput = 0;
    public List<MovementInputs> possibleInputs;
    public NeutralAnim myNeutralAnim;
   /* public bool isCorrectInput(Character_ButtonInput inputTest, int curInput)
    {
        for (int i = 0; i < possibleInputs.Count; i++) 
        {
            try
            {
                bool correctDirectionalInput = possibleInputs[i].defaultStringPieces[curInput].ToString() == inputTest.Button_State.directionalInput.ToString();

                if (correctDirectionalInput)
                {
                    possibleInputs[i].possibleActiveInput = correctDirectionalInput;
                    return correctDirectionalInput;
                }
                else
                {
                    possibleInputs[i].possibleActiveInput = false;
                    continue;
                }
            }
            catch (IndexOutOfRangeException) 
            {
                continue;
            }
        }
        return false;
    }*/
}

