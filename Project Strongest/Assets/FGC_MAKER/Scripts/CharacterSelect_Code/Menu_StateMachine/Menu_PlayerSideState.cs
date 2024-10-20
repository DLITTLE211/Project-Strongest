using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Menu_PlayerSideState : Menu_BaseState
{
    [SerializeField] private CharacterSelect_Setup _characterSelect;
    [SerializeField] private CharacterSelect_StageSelect _stageSelecter;
    [SerializeField] private GameObject mainObjectHolder;
    [SerializeField] private GameObject SideSelectionObject;
    [SerializeField] private TMP_Text advisoryMessage;
    [SerializeField] private CharacterSelect_ChosenSideController sideController;
    [SerializeField] private ChooseSide_Object player1;
    [SerializeField] private ChooseSide_Object player2;
    [SerializeField] private CharacterSelect_Page _player1_PlayerPage, _player2_PlayerPage;
    [SerializeField] private CharacterSelect_Cursor _player1_Cursor, _player2_Cursor;
    IEnumerator activateRoutine;
    public override void OnEnter()
    {
        _stageSelecter.ClearStageSelect();
        _stageSelecter.DisableRoundSelectorObject();
        if (activateRoutine != null)
        {
            StopCoroutine(activateRoutine);
            activateRoutine = null;
        }
        activateRoutine = ActivateSideController();
        StartCoroutine(activateRoutine);
        SetCharacterSelectEnterInfo();
    }
    IEnumerator ActivateSideController() 
    {
        yield return new WaitForSeconds(2f);
        SideSelectionObject.SetActive(true);
        advisoryMessage.gameObject.SetActive(false);

        player1.InitSideIterator();
        player2.InitSideIterator();

        SetCursorStartPosition(_player1_Cursor.transform, -270f);
        SetCursorStartPosition(_player2_Cursor.transform, 335f);

        _stageSelecter.ResetValues();

        _characterSelect.AddControllerCounter();
        _characterSelect.CheckControllerState();
        _characterSelect.SetPlayerControllers();
    }
    void SetCharacterSelectEnterInfo()
    {
        mainObjectHolder.SetActive(true);
        
    }
    void SetCursorStartPosition(Transform cursor, float xPos)
    {
        cursor.localPosition = new Vector3(xPos, cursor.transform.localPosition.y, cursor.localPosition.z);
    }
    public override void OnExit()
    {

    }
    public override void OnUpdate()
    {
        _characterSelect.CursorController(_player1_Cursor);
        _characterSelect.CursorController(_player2_Cursor);
    }
    public override void Select(CharacterSelect_Cursor _currentCursor) { }
    public override void Cancel(CharacterSelect_Cursor _currentCursor) { }
    public override void CycleLeft(CharacterSelect_Cursor _currentCursor) { }
    public override void CycleRight(CharacterSelect_Cursor _currentCursor) { }
}
