using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Character_SystemButtons : MonoBehaviour
{
    public Character_Base _base;
    public List<Character_ButtonInput> _systemButtons = new List<Character_ButtonInput>();

    public void AddSystemButton(Character_ButtonInput systemButton) 
    {
        _systemButtons.Add(systemButton);
    }
    public void Update()
    {
        if (_systemButtons.Count > 0)
        {
            CheckSystemButtonPressed();
        }
    }
    public void CheckSystemButtonPressed() 
    {
        if (_base._subState != Character_SubStates.Controlled) { return; }
        if (_base.player.GetButtonDown(_systemButtons[0].Button_Element.actionDescriptiveName))
        {
            //PauseButton
            GameManager.instance.PauseGame();
        }
        if (_base.player.GetButtonDown(_systemButtons[1].Button_Element.actionDescriptiveName))
        {
            //CenterButton
            GameManager.instance.TeleportPosition();
        }
        if (_base.player.GetButtonDown(_systemButtons[2].Button_Element.actionDescriptiveName))
        {
            //ShareButton
            Debug.Log(_systemButtons[2].Button_Element.actionDescriptiveName);
        }
    }
}
