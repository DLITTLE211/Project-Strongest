using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

[Serializable]
public class Round_EndScreenState : Round_BaseState
{
    public Round_EndScreenState(MainGame_RoundSystemController rSystem) : base(rSystem) { }
    [SerializeField] private TMP_Text countDownText;
    [SerializeField] private EndScreen_Controller endingScreen;
    public override void OnEnter()
    {
        countDownText.gameObject.SetActive(true);
        GameManager.instance.stopWatchController.PauseTimer();
        for (int i = 0; i < GameManager.instance.players.totalPlayers.Count; i++)
        {
            GameManager.instance.players.totalPlayers[i].Deactivate();
        }

        if (GameManager.instance.winningCharacter != null)
        {
            GameManager.instance.winningCharacter._cSubStateController.PlayVictoryWinAnimation(CallEndScreen);
            int winningSide = GameManager.instance.winningCharacter._side == 0 ? 1 : 2;
            PulseAndSetText(countDownText, $"Player {winningSide} Wins!!");
        }
        else
        {
            PulseAndSetText(countDownText, $"Tie Game...");
            CallEndScreen();
        }
    }
    public async void CallEndScreen() 
    {
        await ActivateEndScreen();
    }
    public async Task ActivateEndScreen()
    {
        await Task.Delay(1000);
        endingScreen.gameObject.SetActive(true);
        countDownText.text = "";
        countDownText.gameObject.SetActive(false);
        GameManager.instance.SetupEndScreen(endingScreen.SetupEndScreenButtons);
        GameManager.instance.settingsController.SetActiveButton(endingScreen.ReturnTopButton().gameObject);
    }
    public override void OnExit()
    {
        endingScreen.gameObject.SetActive(false);
    }
}
