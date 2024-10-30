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
    [Range(1, 5)] public int colorSelectSlider;
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
        colorSelectSlider = 1;
    }
    public void ClearColorText()
    {
        colorPicker.ClearText();
    }
    public void SetDefaultText() 
    {
        colorSelectSlider = 1;
        colorPicker.UpdateColorChoice(colorSelectSlider);
    }
    public void UpdateColorSelectNumberDown()
    {
        if (colorSelectSlider <= 1)
        {
            colorSelectSlider = 1;
        }
        else
        {
            colorSelectSlider--;
        }
        colorPicker.UpdateColorChoice(colorSelectSlider);
    }
    public void UpdateColorSelectNumberUp()
    {
        if (colorSelectSlider >= 5)
        {
            colorSelectSlider = 5;
        }
        else
        {
            colorSelectSlider++;
        }
        colorPicker.UpdateColorChoice(colorSelectSlider);
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
