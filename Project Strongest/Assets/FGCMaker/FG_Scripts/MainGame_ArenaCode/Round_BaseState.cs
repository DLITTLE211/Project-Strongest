using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Round_BaseState 
{
    public MainGame_RoundSystemController _rSystem;
    public virtual void OnEnter()
    {
    }
    public virtual void OnExit() 
    {
    }
    public virtual void OnGamePause(bool state) { }
    public Round_BaseState(MainGame_RoundSystemController rSystem)
    {
        _rSystem = rSystem;
    }
}
