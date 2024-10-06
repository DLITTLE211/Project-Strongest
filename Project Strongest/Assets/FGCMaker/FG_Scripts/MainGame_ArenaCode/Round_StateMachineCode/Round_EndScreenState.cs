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
        for (int i = 0; i < GameManager.instance.players.totalPlayers.Count; i++)
        {
            GameManager.instance.players.totalPlayers[i].Deactivate();
        }
        countDownText.DOFade(1f, 0f);
        countDownText.gameObject.SetActive(true);
        countDownText.transform.DOScale(0f, 0f);
        countDownText.transform.DOScale(1f, 0.3f);
        countDownText.transform.DOScale(0.75f, 0.3f);
        if (GameManager.instance.winningCharacter != null)
        {
            countDownText.text = $"Player {GameManager.instance.winningCharacter.playerID + 1} Wins!!";
        }
        else 
        {
            countDownText.text = $"Tie Game...";
        }
        CallEndScreen();
    }
    public async void CallEndScreen() 
    {
        await ActivateEndScreen();
    }
    public async Task ActivateEndScreen()
    {
        await Task.Delay(2500);
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
