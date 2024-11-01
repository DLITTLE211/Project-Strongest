using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu_CharacterColorSelectState : Menu_BaseState
{
    [SerializeField] private CharacterSelect_Setup _characterSelect;
    [SerializeField] private CharacterSelect_Page _player1_PlayerPage, _player2_PlayerPage;
    [SerializeField] private CharacterSelect_Cursor _player1_Cursor, _player2_Cursor;
    private bool allowUpdate;
    private bool allowColorUpdate;
    public override void OnEnter()
    {
        allowUpdate = false;
        allowColorUpdate = false;
        StartCoroutine(DelayColorUpdateRoutine(0.275f));
        StartCoroutine(DelayUpdateRoutine(0.45f));
        _player1_Cursor.cursorPage.SetDefaultText();
        _player2_Cursor.cursorPage.SetDefaultText();
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
        _currentCursor.SetColorLockState(true);
        _currentCursor.cursorPage.LockInColorState();
        _characterSelect.CheckGameModeSet();
    }
    public override void Cancel(CharacterSelect_Cursor _currentCursor)
    {
        _currentCursor.SetColorLockState(false);
        _characterSelect._menuStateMachine.CallCharacterSelectState();
    }
    public override void CycleLeft(CharacterSelect_Cursor _currentCursor)
    {
        if (allowColorUpdate)
        {
            _currentCursor.cursorPage.UpdateColorSelectNumberDown();
            StartCoroutine(DelayColorUpdateRoutine(0.275f));
        }
    }
    public override void CycleRight(CharacterSelect_Cursor _currentCursor)
    {
        if (allowColorUpdate)
        {
            _currentCursor.cursorPage.UpdateColorSelectNumberUp();
            StartCoroutine(DelayColorUpdateRoutine(0.275f));
        }
    }
    IEnumerator DelayColorUpdateRoutine(float time)
    {
        allowColorUpdate = false;
        yield return new WaitForSeconds(time);
        allowColorUpdate = true;
    }
    IEnumerator DelayUpdateRoutine(float time)
    {
        allowUpdate = false;
        yield return new WaitForSeconds(time);
        allowUpdate = true;
    }
}
