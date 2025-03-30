using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class CharacterSelect_Page : MonoBehaviour
{
    [SerializeField] private RectTransform _nameplateTransform;
    [SerializeField] private RectTransform _colorPickerTransform;
    public AmplifyController_Object characterAmplify;
    public CharacterSelect_ColorPicker colorPicker;
    public TMP_Text characterName;
    public Image _characterIconImage;
    public bool lockedIn;
    public Character_Profile chosenCharacter;
    public Amplifiers chosenAmplifier;
    [Range(0, 4)] public int colorSelectIndex;
    public bool amplifySelectCooldown;
    public void UpdateInfo(Character_Profile profile)
    {
        _characterIconImage.color = Color.white;
        _characterIconImage.preserveAspect = true;
        _characterIconImage.sprite = profile.CharacterProfileImage;
        characterName.text = profile.CharacterName;
    }
    public void LockInfo(Character_Profile profile)
    {
        lockedIn = true; 
        chosenCharacter = profile;
        _characterIconImage.sprite = profile.CharacterProfileImage;
        characterName.text = $"{profile.CharacterName} \n(Selected)";
        chosenAmplifier = characterAmplify.ReturnChosenAmplifier();
    }
    public void ClearInfo()
    {
        lockedIn = false;
        _characterIconImage.sprite = null;
        characterName.text = "Choose Your Character";
        chosenAmplifier = null;
        chosenCharacter = null;
        colorSelectIndex = 0;
    }
    public void ClearAmplifierData() 
    {
        chosenAmplifier = null;
    }
    public void ClearColorText()
    {
        colorPicker.ClearText();
    }
    public void SetDefaultText() 
    {
        colorSelectIndex = 0;
        colorPicker.UpdateColorChoice(colorSelectIndex);
    }
    public void LockInColorState() 
    {
        colorPicker.LockInColorChoice();
    }
    public void UpdateColorSelectNumberDown()
    {
        if (colorSelectIndex <= 0)
        {
            colorSelectIndex = 0;
        }
        else
        {
            colorSelectIndex--;
        }
        colorPicker.UpdateColorChoice(colorSelectIndex);
    }
    public void UpdateColorSelectNumberUp()
    {
        if (colorSelectIndex >= 4)
        {
            colorSelectIndex = 4;
        }
        else
        {
            colorSelectIndex++;
        }
        colorPicker.UpdateColorChoice(colorSelectIndex);
    }
    public void SetPlayerInfo(float value)
    {
        _characterIconImage.DOFade(value, 1.5f);
        characterName.DOFade(value, 1.5f);
        //characterAmplify.SetAmplifyInfo(value);
    }

    public IEnumerator DelayResetBool() 
    {
        amplifySelectCooldown = true;
        yield return new WaitForSeconds(0.75f);
        amplifySelectCooldown = false;
    }
}
