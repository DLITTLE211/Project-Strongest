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
    public bool colorChosen;
    public void LockinCharacterChoice(Character_Profile chosenProfile)
    {
        profile = chosenProfile;
        
        cursorPage.LockInfo(profile);
        cursorObject.transform.DOScale(0.5f, 0.15f);
    }
    public void SetColorLockState(bool state) 
    {
        colorChosen = state;
    }
    public void UnlockCharacterChoice()
    {
        profile = null;
        if (canChooseStage) 
        {
            canChooseStage = false;
        }
        cursorPage.characterName.text = "Choose Your Character";
        cursorObject.transform.DOScale(0.65f, 0.15f);
    }

    public void DesyncController()
    {
        isConnected = false;
    }
}
