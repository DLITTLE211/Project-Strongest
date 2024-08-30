using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using Rewired;
using DG.Tweening;

public class Menu_Manager : MonoBehaviour
{
    public Character_AvailableID players;
    [SerializeField] private Player _mainMenuPlayer;
    [SerializeField] private int _mainMenuPlayerID;

    [SerializeField] private MenuButtonHolder FirstMenuButtonLayer;
    [SerializeField] private MenuButtonHolder SecondMenuButtonLayer;
    [SerializeField] private Image _backgroundImage;
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        SetPlayerControllers();
        SetButtonHolderImages();
    }
    void SetPlayerControllers()
    {
        if (ReInput.controllers.GetJoystickNames().Length <= 0)
        {
            return;
        }
        else
        {
            players.InitAvailableIDs();
            players.AddToJoystickNames(ReInput.controllers.GetJoystickNames());
            players.AddUsedID(players.joystickNames[0]);
            SetCharacterSelectCursorState(_mainMenuPlayer, 0);
        }
    }
    void SetCharacterSelectCursorState(Player player, int ID)
    {
        player = ReInput.players.GetPlayer(players.UsedID.Item1[ID]);
        _mainMenuPlayerID = ID;
        player.controllers.AddController(ControllerType.Joystick, players.UsedID.Item1[ID], true);
        player.controllers.maps.LoadMap(ControllerType.Joystick, players.UsedID.Item1[ID],
            $"UI_CanvasController", $"TestPlayer{_mainMenuPlayerID}");
    }
    public void SetButtonHolderImages() 
    {
        FirstMenuButtonLayer.SetImageObject(_backgroundImage);
        SecondMenuButtonLayer.SetImageObject(_backgroundImage);
    }
}

