using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Rewired;
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

    public bool canChooseStage;
    public void LockinCharacterChoice(Character_Profile chosenProfile)
    {
        profile = chosenProfile;
        
        cursorPage.characterFrame.color = cursorImage.color;
        cursorPage.LockInfo(profile);
        cursorObject.transform.DOScale(0.85f, 0.15f);
    }
    public void UnlockCharacterChoice()
    {
        profile = null;
        if (canChooseStage) 
        {
            canChooseStage = false;
        }
        cursorPage.characterFrame.color = Color.white;
        cursorPage.characterName.text = "Choose Your Character";
        cursorObject.transform.DOScale(1f, 0.15f);
    }

    public void DesyncController()
    {
        isConnected = false;
    }
}
