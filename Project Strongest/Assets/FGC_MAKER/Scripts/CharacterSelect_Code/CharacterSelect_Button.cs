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
    public Character_Profile characterProfile;
    public Image hoverImage;
    public BoxCollider2D hoverCollider2D;
    [SerializeField] private Transform leftCursor, rightCursor;
    public enum hoverState {none,left,right,both }
    public hoverState _hoverState;
    public Vector3 buttonSize;
    public float widthFloat;
    public float heightFloatTop;
    public float heightFloatBottom;
    bool leftHover,rightHover;
    public void Start()
    {
        hoverCollider2D = hoverImage.GetComponent<BoxCollider2D>();
        Messenger.AddListener<CharacterSelect_Cursor>(Events.TryApplyCharacter, SendCharacterSelected);
    }

    public void SetPosition() 
    {
        buttonSize = this.GetComponent<Transform>().localPosition;
    }
    public void GetLeftCursor(Transform Cursor) 
    {
        leftCursor = Cursor;
    }
    public void GetRightCursor(Transform Cursor)
    {
        rightCursor = Cursor;
    }
    private void Update()
    {
        CheckCursorPos();
        SetHoverColor();
    }
    void CheckCursorPos() 
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
    }
    void SetHoverColor() 
    {
        if (!leftHover && !rightHover)
        {
            _hoverState = hoverState.none;
        }
        switch (_hoverState) 
        {
            case hoverState.both:
                hoverImage.color = Color.white;
                break;
            case hoverState.left:
                hoverImage.color = Color.red;
                break;
            case hoverState.right:
                hoverImage.color = Color.blue;
                break;
            case hoverState.none:
                hoverImage.color = Color.black;
                break;
        }
    }
    public void HighlightSelection(CharacterSelect_Cursor cursor)
    {
        if (leftHover && rightHover)
        {
            _hoverState = hoverState.both;
            if (hoverImage.color != Color.white)
            {
                if (!cursor.cursorPage.characterName.text.Contains("Selected"))
                {
                    Messenger.Broadcast<Character_Profile, CharacterSelect_Cursor>(Events.DisplayCharacterInfo, characterProfile, cursor);
                }
            }
        }
        else
        {
            if (cursor.ID == 0)
            {
                _hoverState = hoverState.left;
                if (hoverImage.color != Color.red)
                {
                    if (!cursor.cursorPage.characterName.text.Contains("Selected"))
                    {
                        Messenger.Broadcast<Character_Profile, CharacterSelect_Cursor>(Events.DisplayCharacterInfo, characterProfile, cursor);
                    }
                }

            }
            else if (cursor.ID == 1)
            {
                _hoverState = hoverState.right;
                if (hoverImage.color != Color.blue)
                {
                    if (!cursor.cursorPage.characterName.text.Contains("Selected"))
                    {
                        Messenger.Broadcast<Character_Profile, CharacterSelect_Cursor>(Events.DisplayCharacterInfo, characterProfile, cursor);
                    }
                }
            }
        }
    }
    public void UnselectButton(CharacterSelect_Cursor cursor)
    {
        if (hoverImage.color != Color.black)
        {
            if (cursor.cursorPage.characterBackgroundImage.sprite == characterProfile.CharacterProfileImage)
            {
                Messenger.Broadcast<int>(Events.ClearCharacterInfo, cursor.ID);
            }
        }
    }
    public void SendCharacterSelected(CharacterSelect_Cursor cursor) 
    {
        if (CheckCursorOverlap(cursor.gameObject.transform) && cursor.cursorPage.characterBackgroundImage.sprite == characterProfile.CharacterProfileImage) 
        {
            Messenger.Broadcast<Character_Profile, CharacterSelect_Cursor>(Events.LockinCharacterChoice, characterProfile, cursor);
        }
    }
    bool CheckCursorOverlap(Transform cursor) 
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
    }
    public void OnApplicationQuit()
    {
        Messenger.RemoveListener<CharacterSelect_Cursor>(Events.TryApplyCharacter, SendCharacterSelected);
    }
}