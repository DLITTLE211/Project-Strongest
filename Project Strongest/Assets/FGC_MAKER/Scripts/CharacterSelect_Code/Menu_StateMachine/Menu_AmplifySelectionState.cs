using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu_AmplifySelectionState : Menu_BaseState
{
    [SerializeField] private GameObject _amplifyControllerObject;
    [SerializeField] private CSSubMenu_AmplifyPageController _amplifyController;
    [SerializeField] private CharacterSelect_Setup _characterSelect;
    [Space(15)]
    [Header("Player1 Info")]
    [SerializeField] private CharacterSelect_Cursor _player1_Cursor;
    [Space(15)]
    [Header("Player1 Info")]
    [SerializeField] private CharacterSelect_Cursor _player2_Cursor;
    IEnumerator ToggleActivationRoutine;
    bool toggleReady;
    void CheckClearState() 
    {
        if (ToggleActivationRoutine != null)
        {
            StopCoroutine(ToggleActivationRoutine);
            ToggleActivationRoutine = null;
        }
    }
    public override void OnEnter()
    {
        toggleReady = false;
        CheckClearState();
        ToggleActivationRoutine = ActivateAmplifyObject();
        StartCoroutine(ToggleActivationRoutine);
    }
    IEnumerator ActivateAmplifyObject()
    {
        _amplifyControllerObject.SetActive(true);
        yield return new WaitForSeconds(0.65f);
        _amplifyController.Activate();
        ToggleActivationRoutine = null;
        yield return new WaitForSeconds(0.5f);
        toggleReady = true;
        _player1_Cursor.cursorPage.amplifySelectCooldown = true;
        _player2_Cursor.cursorPage.amplifySelectCooldown = true;
    }
    public override void OnExit()
    {
        toggleReady = false;
        CheckClearState();
        ToggleActivationRoutine = DeactivateAmplifyObject();
        StartCoroutine(ToggleActivationRoutine);
    }
    IEnumerator DeactivateAmplifyObject()
    {
        _amplifyController.Deactivate();
        yield return new WaitForSeconds(1.15f);
        _amplifyControllerObject.SetActive(false);
        ToggleActivationRoutine = null;
    }
    public override void OnUpdate()
    {
        if (toggleReady)
        {
            _characterSelect.CursorController(_player1_Cursor);
            _characterSelect.CursorController(_player2_Cursor);
        }
    }
    public override void Select(CharacterSelect_Cursor _currentCursor)
    {
        if(!toggleReady)
        {
            return;
        }
        _currentCursor.cursorPage.characterAmplify.SetSelected();
        _currentCursor.cursorPage.chosenAmplifier = _currentCursor.cursorPage.characterAmplify.ReturnChosenAmplifier(); 
        _characterSelect.CheckAmplifiersFilled();
        //_currentCursor.amplifierController.SetCurrentAmplifier();
        // _characterSelect.CheckControllerState();
    }
    public override void Cancel(CharacterSelect_Cursor _currentCursor)
    {
        if (!toggleReady)
        {
            return;
        }
        if(_currentCursor.cursorPage.chosenAmplifier != null) 
        {
            _currentCursor.cursorPage.ClearAmplifierData();
        }
        else
        {
            _menuStateMachine.CallPlayerSideState();
        }
    }

    public override void CycleLeft(CharacterSelect_Cursor _currentCursor) 
    {
        if (_currentCursor.cursorPage.amplifySelectCooldown)
        {
            _currentCursor.cursorPage.CallDelayResetBool();
            _amplifyController.CyclePlayerAmplifierDown(_currentCursor.ID);
        }
    }
    public override void CycleRight(CharacterSelect_Cursor _currentCursor)
    {
        if (_currentCursor.cursorPage.amplifySelectCooldown)
        {
            _currentCursor.cursorPage.CallDelayResetBool();
            _amplifyController.CyclePlayerAmplifierUp(_currentCursor.ID);
        }
    }
}
