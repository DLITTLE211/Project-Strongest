using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Round_StateMachine : MonoBehaviour
{
    [SerializeField] private Round_CharacterDialogueState _cDState;
    [SerializeField] private Round_InitialCountdownState _iCState;
    [SerializeField] private Round_ActiveMatchState _aMState;
    [SerializeField] private Round_ResultMatchState _rMState;
    [SerializeField] private Round_EndScreenState _eSState;

    [SerializeField] private Round_BaseState _curState;
    public void DefineStates() 
    {
        _cDState = new Round_CharacterDialogueState();
        _iCState = new Round_InitialCountdownState();
        _aMState = new Round_ActiveMatchState();
        _rMState = new Round_ResultMatchState();
        _eSState = new Round_EndScreenState();
    }
    public Round_BaseState GetCurrentState() 
    {
        return _curState;
    }
    public void SetCurrentState(Round_BaseState newState) 
    {
        _curState?.OnExit();
        _curState = newState;
        _cDState?.OnEnter();
    }
}
