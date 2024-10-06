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
        if (GameManager.instance.winningCharacter != null)
        {
            CallAward(GameManager.instance.winningCharacter);
        }
        else 
        {
            CallAward(GameManager.instance.CallPlayerDeathOnTimerEnd());
        }
    }
    public async void CallAward(Character_Base _Base) 
    {
        await OnWin_AwardPoint(_Base);
    }
    public async Task OnWin_AwardPoint(Character_Base _Base)
    {
        _rSystem.AwardWin(_Base._side);
        if (_rSystem.p1_Signifiers.hasWon)
        {
            await Task.Delay(1000);
            _rSystem.StateMachine.CallEndScreenState();
            return;
        }
        else if (_rSystem.p2_Signifiers.hasWon)
        {
            await Task.Delay(1000);
            _rSystem.StateMachine.CallEndScreenState();
            return;
        }
        else 
        {
            await Task.Delay(1000);
            _rSystem.StateMachine.CallInitialTimerState();
            return;
        }
    }
    public override void OnExit()
    {
        base.OnExit();
    }
}
