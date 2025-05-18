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
    public List<string> logString;
    public Queue<Character_ButtonInput> receivedButtons2 = new Queue<Character_ButtonInput>();

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
        logString = new List<string>();
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
        string convertedInput = ConvertNewInput(log);
        ButtonStateMachine.InputState buttonState = log.Button_State._state;
        if (logString.Count >= 19)
        {
            TrimString();
        }
        bool numSize = false;
        if(buttonState == ButtonStateMachine.InputState.directional) 
        {
            if(log.Button_State.directionalInput == 0) 
            {
                return;
            }
            if (CheckNewInputState(lastDirectionalInput.ToString(), log.Button_State.directionalInput.ToString()))
            {
                ResetCount();
                lastDirectionalInput = log.Button_State.directionalInput;
                string newTopString = $"{convertedInput} {ConvertCount()}";
                logString.Insert(0, newTopString);
            }
            else
            {
                IncreaseCount();
                string newTopString = $"{convertedInput} {ConvertCount()}";
                logString[0] = newTopString;
            }
            numSize = true;
        }
        else 
        {

        }
        /*switch (log.Button_State._state)
        {
            case ButtonStateMachine.InputState.pressed:
                if(CheckNewInputState(lastAttackInput, log.Button_Name)) 
                {
                    logString.Insert(0, ($"{log.Button_Name}").ToUpper());
                }
                else 
                {
                    logString[0] = ($"{log.Button_Name}").ToUpper();
                }
                lastAttackInput = log.Button_Name;
                break;
            case ButtonStateMachine.InputState.held:
                if (CheckNewInputState(lastAttackInput, log.Button_Name))
                {
                    logString.Insert(0, ($"{log.Button_Name}").ToUpper());
                }
                lastAttackInput = log.Button_Name;
                break;
            case ButtonStateMachine.InputState.released:
                if (CheckNewInputState(lastAttackInput, log.Button_Name))
                {
                    logString.Insert(0, ($"{log.Button_Name}").ToUpper());
                }
                lastAttackInput = log.Button_Name;
                break;
            case ButtonStateMachine.InputState.directional:
                if (CheckNewInputState(lastDirectionalInput.ToString(), log.Button_State.directionalInput.ToString()))
                {
                    logString.Insert(0, $"{log.Button_State.directionalInput}");
                }
                lastDirectionalInput = log.Button_State.directionalInput;
                numSize = true;
                break;
        }*/
        if (logString.Count == 1)
        {
            inputLogger.setFirstItem(logString,numSize);
        }
        else
        {
            inputLogger.setNextItemInList(logString, numSize);
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
        count = 1;
    }
}
