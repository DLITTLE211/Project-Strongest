using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class Menu_CharacterSelectState : Menu_BaseState
{
    [SerializeField] private CharacterSelect_Setup _characterSelect;
    [SerializeField] private CharacterSelect_Page _player1_PlayerPage, _player2_PlayerPage;
    [SerializeField] private CharacterSelect_Cursor _player1_Cursor, _player2_Cursor;
    private bool allowUpdate;
    private bool allowColorUpdate;
    public override void OnEnter()
    {
        allowUpdate = false;
        allowColorUpdate = true;
        _player1_Cursor.cursorPage.ClearColorText();
        _player2_Cursor.cursorPage.ClearColorText();
        _player1_Cursor.UnlockCharacterChoice();
        _player1_PlayerPage.ClearInfo();
        _player2_Cursor.UnlockCharacterChoice();
        _player2_PlayerPage.ClearInfo();
        _characterSelect.CallCharacterSelectObject();
        _characterSelect.SetCharacterSelectObjects();
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
        Messenger.Broadcast<CharacterSelect_Cursor>(Events.TryApplyCharacter, _currentCursor);
    }
    public override void Cancel(CharacterSelect_Cursor _currentCursor) 
    {
        _characterSelect._menuStateMachine.CallPlayerSideState();
    }
    IEnumerator DelayUpdateRoutine(float time)
    {
        allowUpdate = false;
        yield return new WaitForSeconds(time);
        allowUpdate = true;
    }
    public override void CycleLeft(CharacterSelect_Cursor _currentCursor) { }
    public override void CycleRight(CharacterSelect_Cursor _currentCursor) { }
}
