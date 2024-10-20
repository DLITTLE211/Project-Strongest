using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Round_StateMachine : MonoBehaviour
{
    public Round_CharacterDialogueState _cDState;
    public Round_InitialCountdownState _iCState;
    public Round_ActiveMatchState _aMState;
    public Round_ResultMatchState _rMState;
    public Round_EndScreenState _eSState;

    [SerializeField] private Round_BaseState _curState;
    public Round_BaseState GetCurrentState() 
    {
        return _curState;
    }
    public void SetCurrentState(Round_BaseState newState) 
    {
        _curState?.OnExit();
        _curState = null;
        newState.OnEnter();
        _curState = newState;
    }
    private void Update()
    {
        if (_curState != null)
        {
            _curState.Update();
        }
    }

    public void CallCharacterDialogueState() 
    {
        SetCurrentState(_cDState);
    }
    public void CallInitialTimerState()
    {
        SetCurrentState(_iCState);
    }
    public void CallActiveGameState()
    {
        SetCurrentState(_aMState);
    }
    public void CallResultState()
    {
        SetCurrentState(_rMState);
    }
    public void CallEndScreenState()
    {
        SetCurrentState(_eSState);
    }
}
