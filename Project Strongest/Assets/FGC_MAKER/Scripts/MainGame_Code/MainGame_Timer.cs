using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class MainGame_Timer : MonoBehaviour
{
    [SerializeField] private float stopwatchTime;
    [SerializeField] private TMP_Text stopwatchText;
    public bool tickDownStopWatch;
    public void SetStartTimerValues(float startingTime = 99) 
    {
        stopwatchTime = startingTime;
        if (startingTime == Mathf.Infinity)
        {
            tickDownStopWatch = true;
            stopwatchText.text = "\u221E";
            tickDownStopWatch = false;
            return;
        }
        stopwatchText.text = $"{startingTime.ToString()}";
    }
    void UpdateTimer() 
    {
        stopwatchTime -= Time.deltaTime;
        string currentTime = (int)stopwatchTime < 0 ? (0).ToString("n0") : ((int)stopwatchTime).ToString("n0");
        string timerMessage = SpriteToTextColorUtility.AppendSpriteName($"Timer \n {currentTime}", Color.white);
        stopwatchText.text = timerMessage;// $"Timer \n {currentTime}";
    }
    public bool ReturnTimerOver() 
    {
        return (int)stopwatchTime == 0;
    }
    private void Update()
    {
        if (tickDownStopWatch && (stopwatchTime > -1 && stopwatchTime != Mathf.Infinity)) 
        {
            UpdateTimer();
        }
    }
}
