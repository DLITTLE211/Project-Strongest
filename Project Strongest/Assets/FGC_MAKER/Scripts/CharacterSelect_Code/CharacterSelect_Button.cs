using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Rewired;

public class CharacterSelect_Button : MonoBehaviour
{
    [SerializeField] private CharacterSelect_CharacterButton selectionState;
    public CharacterSelect_CharacterButton SelectionState { get { return selectionState; } }
    public Character_Profile characterProfile;
    public Image hoverImage_Whole;
    public Image hoverImage_Front;
    public enum hoverState {none,left,right,both }
    public hoverState _hoverState;
    public Vector3 buttonSize;
    public float widthFloat;
    public float heightFloatTop;
    public float heightFloatBottom;
    bool leftHover,rightHover;
    [SerializeField] private List<CharacterSelect_Cursor> playerCursors;
    public List<Selectable> buttonList;
    public List<Vector3> buttonListDirections;
    public void Start()
    {
       // Messenger.AddListener<CharacterSelect_Cursor>(Events.TryApplyCharacter, SendCharacterSelected);
    }

    public void SetPosition() 
    {
        playerCursors = new List<CharacterSelect_Cursor>();
        buttonList = new List<Selectable>();
        buttonSize = this.GetComponent<Transform>().localPosition;
    }
    public void SetNavTransforms()
    {
        Navigation newNav = new Navigation();
        newNav.mode = Navigation.Mode.Explicit;
        for (int i = 0; i < 4; i++) 
        {
            Selectable newSelectableButton = null;
            switch (i) 
            {
                case 0:
                    newSelectableButton = selectionState.FindSelectable(buttonListDirections[i]);
                    newNav.selectOnUp = newSelectableButton;
                    break;
                case 1:
                    newSelectableButton = selectionState.FindSelectable(buttonListDirections[i]);
                    newNav.selectOnRight = newSelectableButton;
                    break;
                case 2:
                    newSelectableButton = selectionState.FindSelectable(buttonListDirections[i]);
                    newNav.selectOnDown = newSelectableButton;
                    break;
                case 3:
                    newSelectableButton = selectionState.FindSelectable(buttonListDirections[i]);
                    newNav.selectOnLeft = newSelectableButton;
                    break;
            }
            if (newSelectableButton != null)
            {
                Debug.Log(newSelectableButton.name);
                buttonList.Add(newSelectableButton);
            }
            else 
            {
                buttonList.Add(null);
            }
        }
        selectionState.navigation = newNav;
    }
    public void ClearPlayerCursorCount() 
    {
        playerCursors.Clear();
    }
    void SetHoverColor(int ID) 
    {
        if (playerCursors.Count == 0 || ID== -1)
        {
            hoverImage_Whole.color = Color.white;
            hoverImage_Front.color = Color.white;
            return;
        }
        if (playerCursors.Count == 2) 
        {
            hoverImage_Whole.color = Color.red;
            hoverImage_Front.color = Color.blue;
            return;
        }
        else 
        {
            if(ID == 0) 
            {
                hoverImage_Whole.color = Color.red;
                hoverImage_Front.color = Color.red;
            }
            else 
            {
                hoverImage_Whole.color = Color.blue;
                hoverImage_Front.color = Color.blue;
            }
        }
    }
    public void UnhighlightSelection(CharacterSelect_Cursor cursor) 
    {
        if (cursor == null)
        {
            _hoverState = hoverState.none;
        }
        else
        {
            if (playerCursors.Contains(cursor))
            {
                playerCursors.Remove(cursor);
                if (playerCursors.Count == 0)
                {
                    SetHoverColor(-1);
                }
                else
                {
                    SetHoverColor(playerCursors[0].ID);
                }
            }
        }
    }
    public void HighlightSelection(CharacterSelect_Cursor cursor)
    {
        if (cursor == null)
        {
            _hoverState = hoverState.none;
            SetHoverColor(-1);
        }
        else
        {
            if (!playerCursors.Contains(cursor)) 
            {
                if (playerCursors.Count == 0)
                {
                    playerCursors.Add(cursor);
                }
                else
                {
                    playerCursors.Insert(cursor.ID, cursor);
                }
            }
            SetHoverColor(cursor.ID);
        }
    }
}