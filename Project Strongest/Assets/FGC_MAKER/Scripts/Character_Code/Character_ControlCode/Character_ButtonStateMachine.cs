using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Character_ButtonStateMachine 
{
    public enum InputState { pressed, held, released, directional };
    public InputState _state;
    public int directionalInput;
    public void SetDI(int numInput) { directionalInput = numInput; }

    public InputState returnButtonState() { return this._state; }
    public void OnPressed() { _state = InputState.pressed; }
    public void OnHeld() { _state = InputState.held; }
    public void OnDirectional() { _state = InputState.directional; }
    public void OnReleased() { _state = InputState.released; }
}
