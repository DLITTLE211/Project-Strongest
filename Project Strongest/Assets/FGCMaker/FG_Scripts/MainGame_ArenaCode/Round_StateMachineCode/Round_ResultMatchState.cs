using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System;

[Serializable]
public class Round_ResultMatchState : Round_BaseState
{
    public Round_ResultMatchState(MainGame_RoundSystemController rSystem) : base(rSystem) { }
    public override void OnEnter()
    {
        base.OnEnter();
    }
    public void OnWin_AwardPoint(Character_Base _Base)
    {
        _rSystem.AwardWin(_Base._side);
    }
    public override void OnExit()
    {
        base.OnExit();
    }
}
