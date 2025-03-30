using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu_AmplifySelectionState : Menu_BaseState
{
    [SerializeField] private CSSubMenu_AmplifyPageController _amplifyController;
    [SerializeField] private CharacterSelect_Setup _characterSelect;
    /*[Space(15)]
    [Header("Player 1 Info")]
    [SerializeField] private ChooseSide_Object player1;
    [SerializeField] private CharacterSelect_Page _player1_PlayerPage;
    [SerializeField] private CharacterSelect_Cursor _player1_Cursor;
    [Space(15)]
    [Header("Player 2 Info")]
    [SerializeField] private ChooseSide_Object player2;
    [SerializeField] private CharacterSelect_Page _player2_PlayerPage;
    [SerializeField] private CharacterSelect_Cursor _player2_Cursor;*/

    public override void OnEnter()
    {
        _amplifyController.Activate();
    }
    public override void OnExit()
    {
        _amplifyController.Deactivate();
    }
    public override void Select(CharacterSelect_Cursor _currentCursor)
    {
        //_currentCursor.amplifierController.SetCurrentAmplifier();
        _characterSelect.CheckControllerState();
    }
    public override void Cancel(CharacterSelect_Cursor _currentCursor)
    {
        //_currentCursor.amplifierController.ClearAmplifier();
    }

    public override void CycleLeft(CharacterSelect_Cursor _currentCursor) 
    {
        _amplifyController.CyclePlayerAmplifierDown(_currentCursor.ID);
    }
    public override void CycleRight(CharacterSelect_Cursor _currentCursor) 
    {
        _amplifyController.CyclePlayerAmplifierUp(_currentCursor.ID);
    }
}
