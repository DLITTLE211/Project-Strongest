using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class Menu_StateMachine : MonoBehaviour
{
    public Menu_TitleState _mTState;
    public Menu_MainMenuState _mMMState;
    public Menu_PlayerSideState _mPState;
    public Menu_CharacterSelectState _mCState;
    public Menu_RoundSelectState _mRState;
    public Menu_StageSelectState _mSState;

    [SerializeField] private Menu_BaseState _curState;
    public Menu_BaseState GetCurrentState()
    {
        return _curState;
    }
    public void SetCurrentState(Menu_BaseState newState)
    {
        if (_curState != newState)
        {
            _curState?.OnExit();
            _curState = null;
            newState.OnEnter();
            _curState = newState;
        }
    }
    private void Update()
    {
        if (_curState != null)
        {
            _curState.OnUpdate();
        }
    }

    public void CallTitleState()
    {
        SetCurrentState(_mTState);
    }
    public void CallMainMenuState()
    {
        SetCurrentState(_mMMState);
    }
    public void CallPlayerSideState()
    {
        SetCurrentState(_mPState);
    }
    public void CallCharacterSelectState()
    {
        SetCurrentState(_mCState);
    }
    public void CallRoundSelectState()
    {
        SetCurrentState(_mRState);
    }
    public void CallStageSelectState()
    {
        SetCurrentState(_mSState);
    }
}
