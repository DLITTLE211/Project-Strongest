using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Character_Timer
{
    public GameObject mainObject;
    public InputLogger inputLogger;
    public float curTime;
    public float startTime;
    public bool hasStarted;
    public bool hasEnded;
    public int count;

    public int lastDirectionalInput;
    public string lastAttackInput;
    public int newDirectionalInput;
    public string newAttackInput;

    public List<string> logString;
    public Queue<Character_ButtonInput> receivedButtons2 = new Queue<Character_ButtonInput>();
    string mainString;
    public void SetObjectState(bool state)
    {
        mainObject.SetActive(state);
    }
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
    public void SetLogString()
    {
        lastDirectionalInput = -1;
        lastAttackInput = "";
        newDirectionalInput = -1;
        newAttackInput = "";

        logString = new List<string>();
        mainString = "";
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
    public void UpdateInputLogger(Character_ButtonInput log)
    {
        string convertedInput = ConvertNewInput(log);
        ButtonStateMachine.InputState buttonState = log.Button_State._state;
        string mainString = "";
        if (buttonState == ButtonStateMachine.InputState.directional)
        {
            if (log.Button_State.directionalInput == 0)
            {
                return;
            }
            newDirectionalInput = log.Button_State.directionalInput;
        }
        else
        {
            if (log.Button_State._state == ButtonStateMachine.InputState.released)
            {
                newAttackInput = "";
            }
            else
            {
                newAttackInput = log.Button_Name;
            }
        }
        SendLogData();
    }
    public void SendLogData()
    {
        mainString = "";
        bool numSize = false;
        bool newEntryRequired = false;
        if (lastDirectionalInput != newDirectionalInput) 
        {
            ResetCount();
            lastDirectionalInput = newDirectionalInput;
            string directionString = "";
            if (lastDirectionalInput == 5) 
            {
                directionString = $"";
            }
            else 
            {
                directionString = $"{ConvertLastInput(lastDirectionalInput.ToString())}";
            }
            mainString += directionString;
            numSize = true;
            newEntryRequired = true;
        }
        else 
        {
            IncreaseCount();
            string directionString = "";
            if (lastDirectionalInput == 5)
            {
                directionString = $"";
            }
            else
            {
                directionString = $"{ConvertLastInput(lastDirectionalInput.ToString())}";
            }
            mainString += directionString;
        }
        if(lastAttackInput != newAttackInput) 
        {
            ResetCount();
            lastAttackInput = newAttackInput;
            string attackString = $"{ConvertLastInput(lastAttackInput.ToString())}";
            mainString += attackString;
            newEntryRequired = true;
            numSize = true;
        }
        else
        {
            string directionString = $"{ConvertLastInput(lastAttackInput.ToString())}";
            mainString += directionString;
        }
        mainString += $"{ConvertCount()}";
        if (newEntryRequired)
        {
            logString.Insert(0, mainString);
        }
        else 
        {
            logString[0] = mainString;
        }
        if (logString.Count > 19)
        {
            logString.RemoveAt(logString.Count - 1);
        }
        if (count < 100)
        {
            inputLogger.SetTextLog(logString,numSize);
            return;
        }
    }
    string ConvertNewInput(Character_ButtonInput newInput) 
    {
        if(newInput.Button_State._state == ButtonStateMachine.InputState.directional) 
        {
            int newDirection = newInput.Button_State.directionalInput;
            string directionalString = newDirection.ToString();
            return SpriteToTextColorUtility.AppendSpriteName($"{directionalString.ToUpper()}", Color.white);
        }
        else 
        {
            string buttonString = newInput.Button_Name;
            return SpriteToTextColorUtility.AppendSpriteName($"{buttonString.ToUpper()}", Color.white);
        }
    }
    string ConvertLastInput(string newInput)
    {
        return SpriteToTextColorUtility.AppendSpriteName($"{newInput.ToUpper()}", Color.white);
    }
    void IncreaseCount() 
    {
        count++;
    }
    string ConvertCount()
    {
        return SpriteToTextColorUtility.AppendSpriteName($"( {count} )", Color.white);
    }
    bool CheckNewInputState(string lastInput, string newInput) 
    {
        if(lastInput != newInput) 
        {
            return true;
        }
        return false;
    }
    void ResetCount() 
    {
        count= 1; 
    }
}
