using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MainGame_Timer : MonoBehaviour
{
    [SerializeField] private float stopwatchTime;
    [SerializeField] private TMP_Text stopwatchText;
    public bool tickDownStopWatch;
    public void SetStartTimerValues(float startingTime = Mathf.Infinity) 
    {
        stopwatchTime = startingTime;
        if (startingTime == Mathf.Infinity)
        {
            tickDownStopWatch = true;
            stopwatchText.text = "Timer \n \u221E";
            tickDownStopWatch = false;
            return;
        }
        stopwatchText.text = $"Timer \n {startingTime.ToString()}";
    }
    void UpdateTimer() 
    {
        stopwatchTime -= Time.deltaTime;
        stopwatchText.text = $"Timer \n {stopwatchTime.ToString("n0")}";
    }
    public bool ReturnTimerOver() 
    {
        return (int)stopwatchTime == 0;
    }
    private void Update()
    {
        if (tickDownStopWatch && (stopwatchTime > -(1 / 60f) && stopwatchTime != Mathf.Infinity)) 
        {
            UpdateTimer();
        }
    }
}
