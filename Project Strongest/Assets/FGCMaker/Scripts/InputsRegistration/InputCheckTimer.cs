using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputCheckTimer : MonoBehaviour
{
    public float frameCountTimer, startframeCountTimer;
    public bool checkForInput;
    public void SetStartingValues()
    {
        frameCountTimer = startframeCountTimer;
        checkForInput = false;
    }
    public bool isTimerCompleted()
    {
        return (frameCountTimer <= 0);
    }
    public void reUpTimer()
    {
        checkForInput = true;
        if (frameCountTimer >= startframeCountTimer) { frameCountTimer = startframeCountTimer; }
        else 
        {
            frameCountTimer += Time.deltaTime;
            if (frameCountTimer >= startframeCountTimer)
            {
                frameCountTimer = startframeCountTimer;
            }
        }
    }
    public void ResetTimerSuccess()
    {
        frameCountTimer = startframeCountTimer;
    }
    public void ResetTimer()
    {
        frameCountTimer = startframeCountTimer;
        checkForInput = false;
        //Messenger.Broadcast(Events.ResetOnTimer);
       // Messenger.Broadcast(Events.ResetMoveOnTimer);
    }
    private void Start()
    {
        ResetTimer();
    }
    private void Update()
    {
        TimerTickDown();
    }
    public void TimerTickDown() 
    {
        if (checkForInput)
        {
            CountDownTimer();
        }
    }
    void CountDownTimer() 
    {
        if (frameCountTimer > 0)
        {
           frameCountTimer -= Time.deltaTime;
        }
        else
        {
           ResetTimer();
        }
    }
  
}
