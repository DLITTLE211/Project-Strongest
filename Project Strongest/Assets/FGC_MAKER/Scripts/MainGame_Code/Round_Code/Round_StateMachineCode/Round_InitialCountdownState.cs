using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

[Serializable]
public class Round_InitialCountdownState : Round_BaseState
{
    public Round_InitialCountdownState(MainGame_RoundSystemController rSystem) : base(rSystem) { }
    [Range(1,10),SerializeField] private int WaitingTime;
    [SerializeField] private GameObject overlayCanvas;
    [SerializeField] private TMP_Text countDownText;
    private int threeSecondCountDown;
    
    public async override void OnEnter()
    {
        overlayCanvas.SetActive(true);
        GameManager.instance.stopWatchController.tickDownStopWatch = false;
        GameManager.instance.settingsController.SetTeleportPositions();
        GameManager.instance.stopWatchController.SetStartTimerValues();
        for (int i = 0; i < GameManager.instance.players.totalPlayers.Count; i++)
        {
            GameManager.instance.players.totalPlayers[i].InitialReset(true);
            GameManager.instance.players.totalPlayers[i]._cSubStateController.OnRoundReset();
        }
        GameManager.instance.winningCharacter = null;
        await Task.Delay(WaitingTime * 100);
        countDownText.DOFade(1f, 0f);
        countDownText.text = "";
        countDownText.gameObject.SetActive(true);
        await CountDownTask();
    }
    
    public async Task CountDownTask()
    {
        if (_rSystem.FinalRound())
        {
            PulseAndSetText(countDownText, $"Final Round!!");
        }
        else
        {
            PulseAndSetText(countDownText, $"Round {_rSystem.currentRound}!!");
        }
        await Task.Delay(1000);
        for (int i = 3; i > 0; i--) 
        {
            threeSecondCountDown = i;
            PulseAndSetText(countDownText, $"{threeSecondCountDown}");
            await Task.Delay(7 * 100);
        }
        PulseAndSetText(countDownText, $"FIGHT!!");
        await Task.Delay(WaitingTime * 100);
        _rSystem.StateMachine.CallActiveGameState();
    }
    public override void OnExit()
    {
        countDownText.DOFade(0f, 0.5f).OnComplete(() =>
        {
            countDownText.text = "";
            countDownText.gameObject.SetActive(false);
        });
    }
}
