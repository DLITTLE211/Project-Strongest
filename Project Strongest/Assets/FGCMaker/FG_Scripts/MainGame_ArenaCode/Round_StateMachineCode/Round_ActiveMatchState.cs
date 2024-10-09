using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System;

[Serializable]
public class Round_ActiveMatchState : Round_BaseState
{
    public MainGame_Timer _gameTimer;
    bool checkTimer;
    public Round_ActiveMatchState(MainGame_RoundSystemController rSystem) : base(rSystem) { }
    public override void OnEnter()
    {
        checkTimer = true;
        _gameTimer.tickDownStopWatch = true;
        for (int i = 0; i < GameManager.instance.players.totalPlayers.Count; i++) 
        {
            GameManager.instance.players.totalPlayers[i].Activate();
        }
    }
    public override void Update() 
    {
        if (checkTimer && _gameTimer.ReturnTimerOver()) 
        {
            checkTimer = false;
            _rSystem.StateMachine.CallResultState();
        }
    }
    public override void OnGamePause(bool state) 
    {
        _gameTimer.tickDownStopWatch = state;
    }
    public override void OnExit()
    {
        
    }
}
