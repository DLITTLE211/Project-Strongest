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
    public List<Selectable> buttonList;
    public List<Vector3> buttonListDirections;
    public void Start()
    {
        Messenger.AddListener<CharacterSelect_Cursor>(Events.TryApplyCharacter, SendCharacterSelected);
    }

    public void SetPosition() 
    {
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
    /*public void GetLeftCursor(Transform Cursor) 
    {
        leftCursor = Cursor;
    }
    public void GetRightCursor(Transform Cursor)
    {
        rightCursor = Cursor;
    }*/
    private void Update()
    {
        //CheckCursorPos();
        
    }
   /* void CheckCursorPos() 
    { 
        if (leftCursor != null && leftCursor.gameObject.activeInHierarchy)
        {
            if (CheckCursorOverlap(leftCursor))
            {
                leftHover = true;
                HighlightSelection(leftCursor.GetComponent<CharacterSelect_Cursor>());
            }
            else
            {
                leftHover = false;
                UnselectButton(leftCursor.GetComponent<CharacterSelect_Cursor>());
            }
        }
        if (rightCursor != null && rightCursor.gameObject.activeInHierarchy)
        {
            if (CheckCursorOverlap(rightCursor))
            {
                rightHover = true;
                HighlightSelection(rightCursor.GetComponent<CharacterSelect_Cursor>());
            }
            else
            {
                rightHover = false;
                UnselectButton(rightCursor.GetComponent<CharacterSelect_Cursor>());
            }
        }
    }*/
    void SetHoverColor() 
    {
        if (!leftHover && !rightHover)
        {
            _hoverState = hoverState.none;
        }
        switch (_hoverState) 
        {
            case hoverState.both:
                hoverImage_Whole.color = Color.white;
                hoverImage_Front.color = Color.white;
                break;
            case hoverState.left:
                hoverImage_Whole.color = Color.red;
                hoverImage_Front.color = Color.red;
                break;
            case hoverState.right:
                hoverImage_Whole.color = Color.blue;
                hoverImage_Front.color = Color.blue;
                break;
            case hoverState.none:
                hoverImage_Whole.color = Color.black;
                hoverImage_Front.color = Color.black;
                break;
        }
    }
    public void HighlightSelection(CharacterSelect_Cursor cursor)
    {
        if (cursor == null)
        {
            _hoverState = hoverState.none;
        }
        else
        {
            if (leftHover && rightHover)
            {
                _hoverState = hoverState.both;
                /*if (hoverImage_Whole.color != Color.white)
                {
                    if (!cursor.cursorPage.characterName.text.Contains("Selected"))
                    {
                        //Messenger.Broadcast<Character_Profile, CharacterSelect_Cursor>(Events.DisplayCharacterInfo, characterProfile, cursor);
                    }
                }*/
            }
            else
            {
                if (cursor.ID == 0)
                {
                    _hoverState = hoverState.left;
                    /*if (hoverImage_Whole.color != Color.red)
                    {
                        if (!cursor.cursorPage.characterName.text.Contains("Selected"))
                        {
                           // Messenger.Broadcast<Character_Profile, CharacterSelect_Cursor>(Events.DisplayCharacterInfo, characterProfile, cursor);
                        }
                    }*/

                }
                else if (cursor.ID == 1)
                {
                    _hoverState = hoverState.right;
                    /*if (hoverImage_Whole.color != Color.blue)
                    {
                        if (!cursor.cursorPage.characterName.text.Contains("Selected"))
                        {
                           // Messenger.Broadcast<Character_Profile, CharacterSelect_Cursor>(Events.DisplayCharacterInfo, characterProfile, cursor);
                        }
                    }*/
                }
            }
        }
        SetHoverColor();
    }
    public void UnselectButton(CharacterSelect_Cursor cursor)
    {
        if (hoverImage_Whole.color != Color.black)
        {
            if (cursor.cursorPage._characterIconImage.sprite == characterProfile.CharacterProfileImage)
            {
                Messenger.Broadcast<int>(Events.ClearCharacterInfo, cursor.ID);
            }
        }
    }
    public void SendCharacterSelected(CharacterSelect_Cursor cursor) 
    {
       /* if (CheckCursorOverlap(cursor.gameObject.transform) && cursor.cursorPage._characterIconImage.sprite == characterProfile.CharacterProfileImage) 
        {
            Messenger.Broadcast<Character_Profile, CharacterSelect_Cursor>(Events.LockinCharacterChoice, characterProfile, cursor);
        }*/
    }
   /* bool CheckCursorOverlap(Transform cursor) 
    {
        float cursorXPos = cursor.GetComponent<CircleCollider2D>().transform.localPosition.x;
        float cursorYPos = cursor.GetComponent<CircleCollider2D>().transform.localPosition.y;

        if (cursorXPos <= buttonSize.x + widthFloat && cursorXPos > buttonSize.x - widthFloat && cursorYPos < buttonSize.y + heightFloatTop && cursorYPos > buttonSize.y - heightFloatBottom)
        {
            return true;
        }
        else 
        {
            return false;
        }
    }*/
    public void OnApplicationQuit()
    {
        Messenger.RemoveListener<CharacterSelect_Cursor>(Events.TryApplyCharacter, SendCharacterSelected);
    }
}