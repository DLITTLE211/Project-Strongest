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
    [SerializeField] private TMP_Text countDownText;
    private int threeSecondCountDown;
    public async override void OnEnter()
    {
        GameManager.instance.settingsController.SetTeleportPositions();
        await Task.Delay(WaitingTime * 100);
        GameManager.instance.stopWatchController.SetStartTimerValues();
        for (int i = 0; i < GameManager.instance.players.totalPlayers.Count; i++)
        {
            GameManager.instance.players.totalPlayers[i]._cHealth.SetStartingHealthValues(); 
            GameManager.instance.players.totalPlayers[i]._cHealth.stunController.SetStartStunValues();
            GameManager.instance.players.totalPlayers[i]._cSuperMeter.SetStartValue();
        }
        GameManager.instance.stopWatchController.tickDownStopWatch = false;
        countDownText.gameObject.SetActive(true);
        countDownText.text = "";
        countDownText.DOFade(1f,0f);
        await CountDownTask();
    }
    
    public async Task CountDownTask() 
    {
        for (int i = 3; i > 0; i--) 
        {
            threeSecondCountDown = i;
            countDownText.text = $"{threeSecondCountDown}";
            countDownText.transform.DOScale(0f, 0f);
            countDownText.transform.DOScale(1f, 0.3f).SetEase(Ease.InOutBounce);
            countDownText.transform.DOScale(0.75f, 0.3f);
            await Task.Delay(WaitingTime * 100);
        }
        countDownText.text = $"FIGHT!!";
        countDownText.transform.DOScale(0f, 0f);
        countDownText.transform.DOScale(1f, 0.3f).SetEase(Ease.InOutBounce);
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
