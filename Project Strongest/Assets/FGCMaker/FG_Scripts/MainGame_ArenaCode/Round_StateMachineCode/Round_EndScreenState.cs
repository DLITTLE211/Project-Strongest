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
    [SerializeField] private GameObject endingScreen;
    [SerializeField] private GameObject topButton;
    public override void OnEnter()
    {
        countDownText.gameObject.SetActive(true); 
        for (int i = 0; i < GameManager.instance.players.totalPlayers.Count; i++)
        {
            GameManager.instance.players.totalPlayers[i].Deactivate();
        }
        if (GameManager.instance.winningCharacter != null) 
        {
            countDownText.text = $"Player {GameManager.instance.winningCharacter.playerID +1} Wins!!";
        }
        CallEndScreen();
    }
    public async void CallEndScreen() 
    {
        await ActivateEndScreen();
    }
    public async Task ActivateEndScreen()
    {
        await Task.Delay(1000);
        endingScreen.SetActive(true);
        GameManager.instance.settingsController.SetActiveButton(topButton);
    }
    public override void OnExit()
    {
        endingScreen.SetActive(false);
    }
}
