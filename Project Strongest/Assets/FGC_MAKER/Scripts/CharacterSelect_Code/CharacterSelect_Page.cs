using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class CharacterSelect_Page : MonoBehaviour
{
    [SerializeField] private Transform _nameplateTransform;
    [SerializeField] private Vector3 _startNtPosition,_endNtPosition;

    [SerializeField] private Transform _colorPickerTransform;
    [SerializeField] private Vector3 _startColorPosition, _endColorPosition;

    public AmplifyController_Object characterAmplify;
    public CharacterSelect_ColorPicker colorPicker;
    public TMP_Text characterName;
    public Image _characterIconImage;
    public bool lockedIn;
    public Character_Profile chosenCharacter;
    public Amplifiers chosenAmplifier;
    [Range(0, 4)] public int colorSelectIndex;
    public bool amplifySelectCooldown;
    IEnumerator delayResetRoutine;
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
        string characterNameString = $"{profile.CharacterName}";
        characterName.text = SpriteToTextColorUtility.AppendSpriteName(characterNameString.ToUpper(), characterName.color);
        chosenAmplifier = characterAmplify.ReturnChosenAmplifier();
    }
    public void ClearInfo()
    {
        lockedIn = false;
        _characterIconImage.sprite = null;
        characterName.text = "";
        chosenAmplifier = null;
        chosenCharacter = null;
        colorSelectIndex = 0;
    }
    public void ResetNamePlatePosition()
    {
        Sequence resetSequence = DOTween.Sequence();
        resetSequence.Append(_colorPickerTransform.DOLocalMove(_startColorPosition, 0.55f));
        resetSequence.Append(_nameplateTransform.DOLocalMove(_startNtPosition, 0.45f));
        resetSequence.Play();
    }
    public void ActivateColorPanelPosition()
    {
        _colorPickerTransform.DOLocalMove(_endColorPosition, 1.15f).SetEase(Ease.OutBack);
    }
    public void ActivateNamePlatePosition()
    {
        _nameplateTransform.DOLocalMove(_endNtPosition, 1.15f).SetEase(Ease.OutBack);
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
    }
    public void CallDelayResetBool() 
    {
        if(delayResetRoutine != null) 
        {
            StopCoroutine(delayResetRoutine);
            delayResetRoutine = null;
        }
        delayResetRoutine = DelayResetBool();
        StartCoroutine(delayResetRoutine);
    }
    public IEnumerator DelayResetBool() 
    {
        amplifySelectCooldown = false;
        yield return new WaitForSeconds(0.75f);
        amplifySelectCooldown = true;
    }
}
