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
    public bool lockedIn;
    public Character_Profile chosenCharacter;
    public Amplifiers chosenAmplifier;
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
