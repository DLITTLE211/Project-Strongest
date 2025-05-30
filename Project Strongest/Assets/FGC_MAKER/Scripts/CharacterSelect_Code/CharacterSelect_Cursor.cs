using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Rewired;
using System;
using DG.Tweening;

public class CharacterSelect_Cursor : MonoBehaviour
{
    public Player curPlayer;
    public int ID;
    public GameObject cursorObject;
    public Image cursorImage;
    public TMP_Text cursorText;
    public bool isConnected;
    public Character_Profile profile;
    public CharacterSelect_Page cursorPage;
    public int ChosenPlayerSide;
    [SerializeField] public float xVal, yVal;
    [SerializeField, Range(0f, 1f)] public float xYield, yYield;
    [SerializeField] private CharacterSelect_Button highlightedButton;
    public bool canChooseStage;
    public bool colorChosen;
    public bool allowChange;
    IEnumerator ChangeCharacterRoutine;

    public void ClearAll()
    {
        SetColorLockState(false);
        ClearHighlightedButton();
        UnlockCharacterChoice();
        if (cursorPage != null)
        {
            cursorPage.ClearInfo();
            cursorPage.chosenAmplifier = null;
        }
    }
    public void SetColorLockState(bool state) 
    {
        colorChosen = state;
    }
    public void SetHighlightedButton(CharacterSelect_Button _newButton) 
    {
        highlightedButton = _newButton;
    }
    public void ClearHighlightedButton() 
    {
        highlightedButton = null;
    }
    public void AllowInitialChange() 
    {
        allowChange = true;
    }
    public void UnlockCharacterChoice()
    {
        profile = null;
        if (canChooseStage) 
        {
            canChooseStage = false;
        }
        if (cursorPage != null)
        {
            cursorPage.characterName.text = "Choose Your Character";
        }
        cursorObject.transform.DOScale(0.65f, 0.15f);
    }
    public void ApplyCharacterData() 
    {
        profile = highlightedButton.characterProfile;
        cursorPage.UpdateInfo(profile);
    }
    public void FindSelectable(int direction, bool positive)
    {
        Button tryNewButton = null;
        Selectable buttonInDirection = null;
        switch (direction)
        {
            case 0:
                buttonInDirection = positive == true ? highlightedButton.SelectionState.navigation.selectOnRight : highlightedButton.SelectionState.navigation.selectOnLeft;
                break;
            case 1:
                buttonInDirection = positive == true ? highlightedButton.SelectionState.navigation.selectOnUp : highlightedButton.SelectionState.navigation.selectOnDown;
                break;
        }
        tryNewButton = buttonInDirection?.gameObject.GetComponent<Button>();
        if (tryNewButton != null)
        {
            CharacterSelect_Button regCharacterButton = tryNewButton.GetComponentInParent<CharacterSelect_Button>();
            if (regCharacterButton != null)
            {
                ChangeHighlightedButton(regCharacterButton);
                SetDelayRoutine();
            }
            else
            {
                //TODO
            }
        }
    }
    
    public void SetDelayRoutine()
    {
        if (ChangeCharacterRoutine != null)
        {
            StopCoroutine(ChangeCharacterRoutine);
            ChangeCharacterRoutine = null;
        }
        ChangeCharacterRoutine = DelayResetChangeRoutine();
        StartCoroutine(ChangeCharacterRoutine);
    }
    IEnumerator DelayResetChangeRoutine()
    {
        allowChange = false;
        yield return new WaitForSeconds(0.2f);
        allowChange = true;
    }
    public void ChangeHighlightedButton(CharacterSelect_Button newButton) 
    {
        highlightedButton.UnhighlightSelection(this);
        highlightedButton = newButton;
        highlightedButton.HighlightSelection(this);
    }
    public void DesyncController()
    {
        isConnected = false;
    }
}
