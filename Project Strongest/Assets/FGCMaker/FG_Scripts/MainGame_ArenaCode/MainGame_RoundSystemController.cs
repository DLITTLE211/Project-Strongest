using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MainGame_RoundSystemController : MonoBehaviour
{
    public void Initialize(Round_Info currentRoundInfo)
    {
        Debug.Log(currentRoundInfo.totalRounds);
        Debug.Log(currentRoundInfo.winningRoundCount);
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
