using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class Menu_StageSelectState : Menu_BaseState
{
    [SerializeField] private CSSubMenu_StageSelectController _stageSelectController;
    [SerializeField] private CharacterSelect_Setup _characterSelect;
    [SerializeField] private CharacterSelect_Cursor _player1_Cursor, _player2_Cursor;
    private bool allowUpdate;
    public override void OnEnter()
    {
        _stageSelectController.Activate();
        StartCoroutine(DelayUpdateRoutine(0.85f));
    }
    public override void OnExit()
    {
        //_stageSelect.ClearStageSelect();
        allowUpdate = false;
    }
    public override void OnUpdate()
    {
        if (_player1_Cursor.canChooseStage)
        {
            _characterSelect.CursorController(_player1_Cursor);
            return;
        }
        if(_player2_Cursor.canChooseStage)
        {
            _characterSelect.CursorController(_player2_Cursor);
            return;
        }
    }
    public override void Select(CharacterSelect_Cursor _currentCursor)
    {
        if (_currentCursor.canChooseStage)
        {
            if(_stageSelectController.stageSelected)
            {
                _stageSelectController.roundCountSelected = true;
                _characterSelect.CallStageSelected();
                return;
            }
            _characterSelect.SetChosenStage(_stageSelectController._chosenStage);
            _stageSelectController.stageSelected = true;
            _stageSelectController.ActivateRoundSelectObject();       
        }
    }
    public override void Cancel(CharacterSelect_Cursor _currentCursor)
    {
        if (_currentCursor.canChooseStage)
        {
            if (_stageSelectController.roundCountSelected)
            {
                _stageSelectController.roundCountSelected = false;
                _stageSelectController.DeactivateRoundSelectObject();
                return;
            }
            if (_stageSelectController.stageSelected)
            {
                _stageSelectController.stageSelected = false;
                _characterSelect.SetChosenStage(null);
                _stageSelectController.Deactivate();
                _menuStateMachine.CallCharacterSelectState();
            }
        }
    }
    public override void CycleLeft(CharacterSelect_Cursor _currentCursor)
    {
        if (allowUpdate)
        {
            _stageSelectController.ToggleRight();
            StartCoroutine(DelayUpdateRoutine(0.95f));
        }
    }
    public override void CycleRight(CharacterSelect_Cursor _currentCursor)
    {
        if (allowUpdate)
        {
            _stageSelectController.ToggleLeft();
            StartCoroutine(DelayUpdateRoutine(0.95f));
        }
    }
    IEnumerator DelayUpdateRoutine(float time)
    {
        allowUpdate = false;
        yield return new WaitForSeconds(time);
        allowUpdate = true;
    }
}
