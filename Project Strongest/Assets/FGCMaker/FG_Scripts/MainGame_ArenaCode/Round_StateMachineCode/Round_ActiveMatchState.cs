using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System;

[Serializable]
public class Round_ActiveMatchState : Round_BaseState
{
    public MainGame_Timer _gameTimer;
    public Round_ActiveMatchState(MainGame_RoundSystemController rSystem) : base(rSystem) { }
    public override void OnEnter()
    {
        GameManager.instance.settingsController.SetTeleportPositions();
        _gameTimer.tickDownStopWatch = true;
    }
    public override void OnGamePause(bool state) 
    {
        _gameTimer.tickDownStopWatch = state;
    }
    public override void OnExit()
    {
        
    }
}
