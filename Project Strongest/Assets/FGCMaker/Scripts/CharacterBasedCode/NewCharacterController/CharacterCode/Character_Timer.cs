using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Character_Timer
{
    public InputLogger inputLogger;
    public float curTime;
    public float startTime;
    public bool hasStarted;
    public bool hasEnded;
    public Queue<Character_ButtonInput> receivedButtons2 = new Queue<Character_ButtonInput>();

    public List<string> logString;
    public void setStartValues(Character_ButtonInput releaseCheck = null)
    {
        if (releaseCheck != null)
        {
            if (releaseCheck.Button_State._state == ButtonStateMachine.InputState.released)
            {
                receivedButtons2.Clear();
            }
        }
        curTime = startTime;
        hasStarted = false;
    }
    public void startTimer()
    {
        hasStarted = true;
    }
    public void TimerCountDown()
    {
        if (hasStarted)
        {
            curTime -= Time.deltaTime;
        }
    }

    public bool timerEnded()
    {
        hasEnded = curTime <= 0;
        return hasEnded;
    }
    public bool sendQueueCount()
    {
        return receivedButtons2.Count > 0;
    }
    public void AddPressedButton(Character_ButtonInput button)
    {
        receivedButtons2.Enqueue(button);
        startTimer();
    }
    public void TrimString()
    {
        List<string> tempString = new List<string>();
        for (int i = 0; i < logString.Count; i++)
        {
            tempString.Add(logString[i]);
        }
        int minStringCount = tempString.Count / 6;
        int maxCount = tempString.Count;
        logString.Clear();
        for (int i = 0; i < minStringCount; i++)
        {
            logString.Add(tempString[i]);
        }
    }
    public void UpdateInputLogger(Character_ButtonInput log)
    {
        if (logString.Count >= 20)
        {
            TrimString();
        }
        switch (log.Button_State._state)
        {
            case ButtonStateMachine.InputState.pressed:
                logString.Insert(0, $"{log.Button_Name}-P");
                break;
            case ButtonStateMachine.InputState.held:
                logString.Insert(0, $"{log.Button_Name}-Ho");
                break;
            case ButtonStateMachine.InputState.released:
                logString.Insert(0, $"{log.Button_Name}-R");
                break;
            case ButtonStateMachine.InputState.directional:
                logString.Insert(0, $"{log.Button_State.directionalInput}");
                break;
        }
        if (logString.Count == 1)
        {
            inputLogger.setFirstItem(logString);
        }
        else
        {
            inputLogger.setNextItemInList(logString);
        }
    }
}
