using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using DG.Tweening;

public class MainGame_RoundSystemController : MonoBehaviour
{
    public Player_Round_Signifier p1_Signifiers;
    public Player_Round_Signifier p2_Signifiers;
    [SerializeField] private Round_StateMachine _StateMachine;
    public Round_StateMachine StateMachine { get { return _StateMachine; } }

    public void Initialize()
    {
        _StateMachine.CallCharacterDialogueState();
    }
    public void AwardWin(int side) 
    {
        Callback AwardWin = side == 0 ? p1_Signifiers.AwardRoundWin : p2_Signifiers.AwardRoundWin;
        AwardWin();
    }
    private void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.G)) 
        {
            AwardWin(0);
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            AwardWin(1);
        }*/
    }
}
[Serializable]
public class Player_Round_Signifier
{
    public List<Round_Signifier> win_Signifiers;
    int roundWinCount = -1;
    int maxRoundCount;
    public bool hasWon;
    public void DisableRoundObjects()
    {
        hasWon = false;
        for (int i = 0; i < 5; i++)
        {
            win_Signifiers[i]._roundImage.gameObject.SetActive(false);
        }
    }
    public void SetRoundSignifier(int winCount)
    {
        maxRoundCount = winCount;
        for (int i = 0; i < winCount; i++)
        {
            win_Signifiers[i].SetupRoundSignifiers(); 
        }
    }
    public void AwardRoundWin()
    {
        roundWinCount++;
        if (roundWinCount >= maxRoundCount-1)
        {
            hasWon = true;
            Debug.Log("Player Has Reached Victory");
            win_Signifiers[roundWinCount].WinRound();
            return;
        }
        win_Signifiers[roundWinCount].WinRound();
    }
}
[Serializable]
public class Round_Signifier
{
    public Image _roundImage; 
    public void SetupRoundSignifiers()
    {
        _roundImage.DOFade(0f, 0f);
        _roundImage.DOColor(Color.black, 0.75f);
        _roundImage.gameObject.SetActive(true);
        _roundImage.DOFade(0.5f, 0.75f);
    }
    public void WinRound()
    {
        Color32 gold = new Color32((byte)242f, (byte)210f, (byte)34f, (byte)255f);
        _roundImage.DOColor(gold, 0.75f);
        _roundImage.DOFade(1f, 0.35f);
    }
}
[Serializable]
public class Round_Info 
{
    public int totalRounds;
    public int winningRoundCount;
    public Round_Info(int _winningRoundCount) 
    {
        winningRoundCount = _winningRoundCount;
        totalRounds = (_winningRoundCount * 2)-1;
    }
}
