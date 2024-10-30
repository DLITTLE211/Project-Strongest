using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class CharacterSelect_Page : MonoBehaviour
{
    public Image characterFrame,characterBackgroundImage;
    public TMP_Text characterName;
    public CharacterSelect_AmplifySelecter characterAmplify;
    public CharacterSelect_ColorPicker colorPicker;
    public bool lockedIn;
    public Character_Profile chosenCharacter;
    public Amplifiers chosenAmplifier;
    [Range(0, 4)] public int colorSelectIndex;
    public bool amplifySelectCooldown;
    public void UpdateInfo(Character_Profile profile)
    {
        characterBackgroundImage.color = Color.white;
        characterBackgroundImage.preserveAspect = true;
        characterBackgroundImage.sprite = profile.CharacterProfileImage;
        characterName.text = profile.CharacterName;
    }
    public void LockInfo(Character_Profile profile)
    {
        lockedIn = true; 
        chosenCharacter = profile;
        characterBackgroundImage.sprite = profile.CharacterProfileImage;
        characterName.text = $"{profile.CharacterName} \n(Selected)";
        chosenAmplifier = characterAmplify.chosenAmplifier;
    }
    public void ClearInfo()
    {
        lockedIn = false;
        characterBackgroundImage.color = Color.black;
        characterBackgroundImage.preserveAspect = false;
        characterBackgroundImage.sprite = null;
        characterName.text = "Choose Your Character";
        chosenAmplifier = null;
        chosenCharacter = null;
        colorSelectIndex = 0;
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
        characterBackgroundImage.DOFade(value, 1.5f);
        characterName.DOFade(value, 1.5f);
        characterAmplify.SetAmplifyInfo(value);
    }

    public IEnumerator DelayResetBool() 
    {
        amplifySelectCooldown = true;
        yield return new WaitForSeconds(0.75f);
        amplifySelectCooldown = false;
    }
}
