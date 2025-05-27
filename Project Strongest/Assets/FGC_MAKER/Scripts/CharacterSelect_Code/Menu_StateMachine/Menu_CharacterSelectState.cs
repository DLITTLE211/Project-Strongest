using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu_CharacterSelectState : Menu_BaseState
{
    [SerializeField] private CharacterSelect_Setup _characterSelect;
    [SerializeField] private CSSubMenu_CharacterSelectController _characterSelectController;
    [SerializeField] private CharacterSelect_Page _player1_PlayerPage, _player2_PlayerPage;
    [SerializeField] private CharacterSelect_Cursor _player1_Cursor, _player2_Cursor;
    private bool allowUpdate;
    private bool allowColorUpdate;
    public override void OnEnter()
    {
        allowUpdate = false;
        allowColorUpdate = true;
        for(int i = 0; i < _characterSelect._playerCursors.Count; i++) 
        {
            CharacterSelect_Cursor cursor = _characterSelect._playerCursors[i];
            cursor.cursorPage.ClearColorText();
            cursor.UnlockCharacterChoice();
            cursor.cursorPage.ClearInfo();
            cursor.cursorPage.ClearColorText();
            cursor.cursorPage.ActivateNamePlatePosition();

        }
        _characterSelectController.gameObject.SetActive(true);
        StartCoroutine(DelayUpdateRoutine(1f));
    }
    public override void OnExit()
    {
    }
    public override void OnUpdate()
    {
        if (allowUpdate)
        {
            _characterSelect.CursorController(_player1_Cursor);
            _characterSelect.CursorController(_player2_Cursor);
        }
    }
    public override void Select(CharacterSelect_Cursor _currentCursor) 
    {
        _currentCursor.ApplyCharacterData();
        _characterSelect.CheckIfBothPlayersLockedIn(_currentCursor);
    }
    public override void Cancel(CharacterSelect_Cursor _currentCursor) 
    {
        _characterSelect._menuStateMachine.CallAmplifierSelectState();
        for (int i = 0; i < _characterSelect._playerCursors.Count; i++)
        {
            CharacterSelect_Cursor cursor = _characterSelect._playerCursors[i];
            cursor.cursorPage.ResetNamePlatePosition();
        }
        _characterSelectController.Deactivate();
    }
    IEnumerator DelayUpdateRoutine(float time)
    {
        allowUpdate = false;
        yield return new WaitForSeconds(time);
        _characterSelectController.Activate();
        yield return new WaitForSeconds(time);
        allowUpdate = true;
    }
    public override void CycleLeft(CharacterSelect_Cursor _currentCursor)
    {
        _currentCursor.FindSelectable(0, false);
    }
    public override void CycleRight(CharacterSelect_Cursor _currentCursor)
    {
        _currentCursor.FindSelectable(0, true);
    }
    public override void CycleUp(CharacterSelect_Cursor _currentCursor)
    {
        _currentCursor.FindSelectable(1, true);
    }
    public override void CycleDown(CharacterSelect_Cursor _currentCursor)
    {
        _currentCursor.FindSelectable(1, false);
    }
}
