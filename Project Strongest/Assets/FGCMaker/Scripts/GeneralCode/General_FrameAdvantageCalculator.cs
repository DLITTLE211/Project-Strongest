using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class General_FrameAdvantageCalculator : MonoBehaviour
{
    public CharacterFrameData player1, player2;
    private void Start()
    {
        if (player1.character != null && player2.character != null)
        {
            player1.characterName = player1.character.gameObject.name;
            player2.characterName = player2.character.gameObject.name;
        }
        Messenger.AddListener<int,string>(Events.SendReturnTime, ReceiveIdleReturnTimes);
        Messenger.AddListener(Events.ClearLastTime, ClearLastReturnTime);
    }
    public void ReceiveIdleReturnTimes(int returnTime, string objectName)
    {
        if (objectName == player1.characterName)
        {
            player1.idleReturnTime = returnTime;
            player1.isTimeSet = true;
        }
        if (objectName == player2.characterName)
        {
            player2.idleReturnTime = returnTime;
            player2.isTimeSet = true;
        }

        if (player1.isTimeSet && player2.isTimeSet)
        {
            CheckFrameData();
        }
    }
    void CheckFrameData() 
    {
        int finalFrameDataAmount = player1.idleReturnTime - player2.idleReturnTime;
        UpdateFrameDataText((finalFrameDataAmount) * -1, player1._frameDataText);
        UpdateFrameDataText((finalFrameDataAmount) * 1, player2._frameDataText);
    }
    public void ClearLastReturnTime()
    {
        player1.idleReturnTime = 0;
        player1.isTimeSet = false;
        player2.idleReturnTime = 0;
        player2.isTimeSet = false;
    }
    public void UpdateFrameDataText(float advantage, TMP_Text targetText)
    {
        string message = advantage >= 0 ? $"Frame Advantage: {(advantage).ToString("n0")}" : $"Frame Disadvantage: {(advantage).ToString("n0")}";
        targetText.text = message;
    }
}
[Serializable]
public class CharacterFrameData 
{
    public Character_Base character;
    public string characterName;
    public bool isTimeSet;
    public int idleReturnTime;
    public TMP_Text _frameDataText;
}