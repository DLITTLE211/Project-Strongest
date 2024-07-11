using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_AttackDetection : MonoBehaviour
{
    [SerializeField] private Character_Base _base;
    [SerializeField] private Character_ComboDetection _comboDetection;
    public void CheckButtonPressed() 
    {
        if (_base._subState != Character_SubStates.Controlled) { return; }
        foreach (Character_ButtonInput action in _base.attackButtons)
        {
            if (_base.player.GetButtonUp(action.Button_Element.actionId))
            {
                action.Button_State.OnReleased();
                _base._timer.AddPressedButton(action);
                _base.widget.ValidateButtonReleaseInput(action);

                if (action.Button_Name == "A" || action.Button_Name == "B")
                {
                    if (CheckSingleButtons(_base.attackButtons[0], ButtonStateMachine.InputState.held))//_base.attackButtons[0].Button_State._state != ButtonStateMachine.InputState.held)
                    {
                        _base.attackButtons[0].Button_State.OnReleased();
                        _base.attackButtons[4].Button_State.OnReleased();
                        _base.widget.ValidateButtonReleaseInput(_base.attackButtons[4]);
                        _base._timer.AddPressedButton(_base.attackButtons[4]);
                    }
                    if (CheckSingleButtons(_base.attackButtons[1], ButtonStateMachine.InputState.held))//_base.attackButtons[1].Button_State._state != ButtonStateMachine.InputState.held)
                    {

                        _base.attackButtons[1].Button_State.OnReleased();
                        _base.attackButtons[4].Button_State.OnReleased();
                        _base.widget.ValidateButtonReleaseInput(_base.attackButtons[4]);
                        _base._timer.AddPressedButton(_base.attackButtons[4]);
                    }
                    continue;
                }
                if (action.Button_Name == "C" || action.Button_Name == "D")
                {
                    if (CheckSingleButtons(_base.attackButtons[2], ButtonStateMachine.InputState.held))//_base.attackButtons[6].Button_State._state != ButtonStateMachine.InputState.held)
                    {
                        _base.attackButtons[2].Button_State.OnReleased();
                        _base.attackButtons[9].Button_State.OnReleased();
                        _base.widget.ValidateButtonReleaseInput(_base.attackButtons[9]);
                        _base._timer.AddPressedButton(_base.attackButtons[9]);
                    }
                    if (CheckSingleButtons(_base.attackButtons[3], ButtonStateMachine.InputState.held))//_base.attackButtons[7].Button_State._state != ButtonStateMachine.InputState.held)
                    {
                        _base.attackButtons[3].Button_State.OnReleased();
                        _base.attackButtons[9].Button_State.OnReleased();
                        _base.widget.ValidateButtonReleaseInput(_base.attackButtons[9]);
                        _base._timer.AddPressedButton(_base.attackButtons[9]);
                    }
                    continue;
                }
                continue;
            }
            if (_base.player.GetButton(action.Button_Element.actionId))
            {
                switch (action.Button_State._state)
                {
                    case ButtonStateMachine.InputState.released:
                        action.Button_State.OnPressed();
                        returnButtonPressName(action);
                        break;
                    case ButtonStateMachine.InputState.pressed:
                        if (CheckTwoButtonsPressed(_base.attackButtons[0], _base.attackButtons[1], ButtonStateMachine.InputState.pressed))
                        {
                            _base.attackButtons[4].Button_State.OnHeld();
                            _base._timer.AddPressedButton(_base.attackButtons[4]);
                            _base.widget.ValidateButtonHoldInput(_base.attackButtons[4]);
                            break;

                        }
                        if (CheckTwoButtonsPressed(_base.attackButtons[2], _base.attackButtons[3], ButtonStateMachine.InputState.pressed))
                        {
                            _base.attackButtons[9].Button_State.OnHeld();
                            _base._timer.AddPressedButton(_base.attackButtons[9]);
                            _base.widget.ValidateButtonHoldInput(_base.attackButtons[9]);
                            break;
                        }
                        else
                        {
                            action.Button_State.OnHeld();
                            _base.widget.ValidateButtonHoldInput(action);
                            _base._timer.AddPressedButton(action);
                            break;
                        }
                    case ButtonStateMachine.InputState.held:
                        if (CheckTwoButtonsPressed(_base.attackButtons[0], _base.attackButtons[1], ButtonStateMachine.InputState.held))
                        {
                            _base.attackButtons[4].Button_State.OnHeld();
                            _base.widget.ValidateButtonHoldInput(_base.attackButtons[4]);
                            _base._timer.AddPressedButton(_base.attackButtons[4]);
                            break;

                        }
                        if (CheckTwoButtonsPressed(_base.attackButtons[2], _base.attackButtons[3], ButtonStateMachine.InputState.held))
                        {
                            _base.attackButtons[9].Button_State.OnHeld();
                            _base.widget.ValidateButtonHoldInput(_base.attackButtons[9]);
                            _base._timer.AddPressedButton(_base.attackButtons[9]);
                            break;
                        }
                        else
                        {
                            _base._timer.AddPressedButton(action);
                            break;
                        }
                    default:
                        DebugMessageHandler.instance.DisplayErrorMessage(2, $"Invalid Input State Detected { action.Button_State}");
                        break;
                }
            }
        }
    }
    bool CheckTwoButtonsPressed(Character_ButtonInput ButtonOne, Character_ButtonInput ButtonTwo, ButtonStateMachine.InputState state) 
    {
        return ButtonOne.Button_State._state == state && ButtonTwo.Button_State._state == state;
    }
    bool CheckSingleButtons(Character_ButtonInput ButtonOne, ButtonStateMachine.InputState state)
    {
        return ButtonOne.Button_State._state != state;
    }
    public void CallReturnButton() 
    {
        if (_base._subState != Character_SubStates.Controlled) { return; }
        if (_base._timer.timerEnded())
        {
            if (_base._timer.receivedButtons2.Count > 0)
            {
                returnButtonPressName(_base._timer.receivedButtons2.Dequeue());
            }
        }
    }
    public void returnButtonPressName(Character_ButtonInput buttonInput)
    {
        _base._timer.UpdateInputLogger(buttonInput);
        _base._timer.setStartValues(buttonInput);
        _comboDetection.CheckPossibleCombos(buttonInput);
    }
}
