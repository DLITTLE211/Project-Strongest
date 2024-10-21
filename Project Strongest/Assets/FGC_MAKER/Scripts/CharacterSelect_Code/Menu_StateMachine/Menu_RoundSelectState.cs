using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class Menu_RoundSelectState : Menu_BaseState
{
    [SerializeField] private CharacterSelect_StageSelect _stageSelect;
    [SerializeField] private CharacterSelect_Setup _characterSelect;
    [SerializeField] private CharacterSelect_Cursor _player1_Cursor, _player2_Cursor;
    private bool allowUpdate;
    public override void OnEnter()
    {
        _stageSelect.ActivateRoundSelector(_characterSelect.currentSet.gameMode);
        StartCoroutine(DelayUpdateRoutine(1f));
    }
    public override void OnExit()
    {
        allowUpdate = false;
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
        _characterSelect._menuStateMachine.CallStageSelectState();
    }
    public override void Cancel(CharacterSelect_Cursor _currentCursor) 
    {
        _stageSelect.DisableRoundSelectorObject();
        _characterSelect._menuStateMachine.CallCharacterSelectState();
    }
    public override void CycleLeft(CharacterSelect_Cursor _currentCursor)
    {
        if (allowUpdate)
        {
            _stageSelect.ToggleDown();
            StartCoroutine(DelayUpdateRoutine(0.25f));
        }
    }
    public override void CycleRight(CharacterSelect_Cursor _currentCursor)
    {
        if (allowUpdate)
        {
            _stageSelect.ToggleUp();
            StartCoroutine(DelayUpdateRoutine(0.25f));
        }
    }
    IEnumerator DelayUpdateRoutine(float time) 
    {
        allowUpdate = false;
        yield return new WaitForSeconds(time);
        allowUpdate = true;
    }
}
