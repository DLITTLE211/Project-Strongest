using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class VersusMenu_DisplayObject : MonoBehaviour
{
    [SerializeField] private PlayerData_Positioning playerDataObject;
    [SerializeField] private CharacterSelect_Cursor _playerData;
    [SerializeField] private Image _playerImage;
    [SerializeField] private List<TMP_Text> _characterDataText;
    [SerializeField] private List<string> _profileDataMessages;

    public void DisplayChosenPlayerData(Callback func = null)
    {
        DontDestroyOnLoad(this);
        if (_profileDataMessages != null) 
        {
            _profileDataMessages.Clear();
        }
        else 
        {
            _profileDataMessages = new List<string>();
        }
        SetPlayerInformation(func);
    }
    public void SetPlayerInformation(Callback func) 
    {
        if (_playerData.cursorPage.chosenCharacter != null) 
        {
            Character_Profile currentProfile = _playerData.cursorPage.chosenCharacter;
            _profileDataMessages.Add($"Character Name {currentProfile.name}");
            _profileDataMessages.Add($"Character Weight {currentProfile.Height}lbs");
            _profileDataMessages.Add($"Character Height {currentProfile.Height}\"");
            _profileDataMessages.Add($"Chosen Amplifier {currentProfile.name}");
            _playerImage.sprite = _playerData.cursorPage.chosenCharacter.CharacterProfileImage;
        }
        else
        {
            _profileDataMessages.Add($"Character Name Filip Toddingson");
            _profileDataMessages.Add($"Character Weight Yo Mama");
            _profileDataMessages.Add($"Character Height 99\"");
            _profileDataMessages.Add($"Chosen Amplifier Viagra");
        }

        for (int i = 0; i < _characterDataText.Count; i++) 
        {
            string currentString = SpriteToTextColorUtility.AppendSpriteName(_profileDataMessages[i].ToUpper(), Color.white);
            _characterDataText[i].SetText(currentString);
        }
        FadeInPlayerData(func);
    }
    public void FadeInPlayerData(Callback func)
    {
        playerDataObject._objectTransform.DOLocalMoveX(playerDataObject.xStartPos, 0f);
        playerDataObject._objectTransform.DOLocalMoveX(playerDataObject.xEndPos,1.15f);
        playerDataObject.playerinformationObject.DOLocalMoveX(playerDataObject.xInfoStartPos, 0f);
        playerDataObject.playerinformationObject.DOLocalMoveX(playerDataObject.xInfoEndPos, 1.15f);
        if (func != null)
        {
            func();
        }
    }
    public void OpenVersusSide() 
    {
        playerDataObject._objectTransform.DOLocalMoveX(playerDataObject.xStartPos, 1.15f);
        playerDataObject.playerinformationObject.DOLocalMoveX(playerDataObject.xInfoStartPos, 1.15f);
    }
}
[Serializable]
public class PlayerData_Positioning 
{
    public Transform _objectTransform;
    public float xStartPos, xEndPos;
    public Transform playerinformationObject;
    public float xInfoStartPos, xInfoEndPos;
}