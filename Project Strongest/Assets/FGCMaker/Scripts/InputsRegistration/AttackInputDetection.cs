using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;
using UnityEngine.UI;
using TMPro;

public class AttackInputDetection : MonoBehaviour
{
    public ControllerWidgetHandler handler;
    public InputTimer _Timer;
    [Header("Rewired Components")]
    public Player player;
    [SerializeField]
    private int PlayerID;
    private ControllerMap map;
    [Header("Player Attack Buttons")]
    public List<ButtonInput> buttons;
    private IList<InputAction> actions;
    // Start is called before the first frame update
    void Start()
    {
        initButtons();
        _Timer.inputLogger.ResetAllText();
        _Timer.setStartValues();
    }
    void initButtons()
    {
        PlayerID = 0;
        player = ReInput.players.GetPlayer(PlayerID);
        player.controllers.maps.LoadMap(ControllerType.Joystick, PlayerID, "TestPlayer", "TestPlayer", true);
        CheckActiveButtons();
    }
    void CheckActiveButtons()
    {
        map = player.controllers.maps.GetMap(0);
        actions = ReInput.mapping.Actions;
        for (int i = 2; i < actions.Count; i++)
        {
          //  Messenger.Broadcast<InputAction>(Events.SendButtonInformation, actions[i]);
        }
        for (int i = 2; i < actions.Count-3; i++) 
        {
            buttons[i - 2].SetButton(actions[i]);
            
        }
    }
    // Update is called once per frame
    void Update()
    {
        CheckAllButtons();
        _Timer.TimerCountDown();
      
        if (_Timer.timerEnded())
        {
            if (_Timer.receivedButtons2.Count > 0)
            {
                returnButtonPressName(_Timer.receivedButtons2.Dequeue());
            }
        }
    }
    void CheckAllButtons()
    {

    }
    public void returnButtonPressName(ButtonInput buttonInput)
    {
        Messenger.Broadcast<ButtonInput>(Events.SendAttackInput,buttonInput);
        _Timer.UpdateInputLogger(buttonInput);
        _Timer.setStartValues(buttonInput);
    }
}

[System.Serializable]
public class InputTimer
{
    public InputLogger inputLogger;
    public float curTime;
    public float startTime;
    public bool hasStarted;
    public bool hasEnded;
    public Queue<ButtonInput> receivedButtons2 = new Queue<ButtonInput>();

    public List<string> logString;
    public void setStartValues(ButtonInput releaseCheck = null) 
    {
        if (releaseCheck != null)
        {
            if (releaseCheck.buttonState._state == ButtonStateMachine.InputState.released)
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
    public void AddPressedButton(ButtonInput button)
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
    public void UpdateInputLogger(ButtonInput log)
    {
        if (logString.Count >= 20)
        {
            TrimString();
        }
        switch (log.buttonState._state)
        {
            case ButtonStateMachine.InputState.pressed:
                logString.Insert(0, $"{log.inputAction.name}-P");
                break;
            case ButtonStateMachine.InputState.held:
                logString.Insert(0, $"{log.inputAction.name}-Ho");
                break;
            case ButtonStateMachine.InputState.released:
                logString.Insert(0, $"{log.inputAction.name}-R");
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
