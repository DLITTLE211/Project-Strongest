using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_AttackDetection : MonoBehaviour
{
    [SerializeField] private Character_Base _base;
    [SerializeField] private Character_ComboDetection _comboDetection;
    public void CheckButtonPressed(List<Character_ButtonInput> buttonSet)
    {
        if (_base._subState != Character_SubStates.Controlled) { return; }
        foreach (Character_ButtonInput action in buttonSet)
        {
            if (_base.player.GetButtonUp(action.Button_Element.actionId))
            {
                action.Button_State.OnReleased();
                _base._timer.AddPressedButton(action);
                _base.widget.ValidateButtonReleaseInput(action);
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
                            action.Button_State.OnHeld();
                            _base.widget.ValidateButtonHoldInput(action);
                            _base._timer.AddPressedButton(action);
                            break;
                    case ButtonStateMachine.InputState.held:
                            _base._timer.AddPressedButton(action);
                            break;
                    default:
                        DebugMessageHandler.instance.DisplayErrorMessage(2, $"Invalid Input State Detected { action.Button_State}");
                        break;
                }
            }
        }
        if (_base.player.GetButtonUp(_base.blockButton.Button_Element.actionId))
        {
            _base.blockButton.Button_State.OnReleased();
            _base._timer.AddPressedButton(_base.blockButton);
            _base.widget.ValidateButtonReleaseInput(_base.blockButton);
        }
        if (_base.player.GetButton(_base.blockButton.Button_Element.actionId))
        {
            switch (_base.blockButton.Button_State._state)
            {
                case ButtonStateMachine.InputState.released:
                    _base.blockButton.Button_State.OnPressed();
                    returnButtonPressName(_base.blockButton);
                    break;
                case ButtonStateMachine.InputState.pressed:
                    _base.blockButton.Button_State.OnHeld();
                    _base.widget.ValidateButtonHoldInput(_base.blockButton);
                    _base._timer.AddPressedButton(_base.blockButton);
                    break;
                case ButtonStateMachine.InputState.held:
                    _base._timer.AddPressedButton(_base.blockButton);
                    break;
            }
        }
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
