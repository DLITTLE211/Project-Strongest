using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class VersusMenu_DisplayObject : MonoBehaviour
{
    [SerializeField] private CharacterSelect_Cursor _playerData;
    [SerializeField] private Image _playerImage;
    [SerializeField] private List<TMP_Text> _characterDataText;
    [SerializeField] private List<string> _profileDataMessages;

    public void DisplayChosenPlayerData()
    {
        if(_profileDataMessages != null) 
        {
            _profileDataMessages.Clear();
        }
        else 
        {
            _profileDataMessages = new List<string>();
        }
        SetPlayerInformation();
    }
    public void SetPlayerInformation() 
    {
        _profileDataMessages.Add($"Character Name {_playerData.cursorPage.chosenCharacter.name}");
        _profileDataMessages.Add($"Character Weight {_playerData.cursorPage.chosenCharacter.Height}");
        _profileDataMessages.Add($"Character Height {_playerData.cursorPage.chosenCharacter.Height}");
        _profileDataMessages.Add($"Chosen Amplifier {_playerData.cursorPage.chosenAmplifier.name}");
        _playerImage.sprite = _playerData.cursorPage.chosenCharacter.CharacterProfileImage;
        for (int i = 0; i < _characterDataText.Count; i++) 
        {
            string currentString = SpriteToTextColorUtility.AppendSpriteName(_profileDataMessages[i].ToUpper(), Color.white);
            _characterDataText[i].SetText(currentString);
        }
    }
}
