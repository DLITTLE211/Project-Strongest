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
    [SerializeField] private Transform chosenCharacterImageObject;
    [SerializeField] private float startXPos;
    [SerializeField] private float endXPos;
    private void Start()
    {
        endXPos = chosenCharacterImageObject.transform.localPosition.x;
    }
    public void UpdateInfo(Character_Profile profile)
    {
        Vector3 startPos = new Vector3(chosenCharacterImageObject.transform.localPosition.x+ startXPos,0,0);
        chosenCharacterImageObject.transform.localPosition = startPos;
        _characterIconImage.sprite = profile.CharacterProfileImage;
        string nameMessage = $"{profile.CharacterName.ToUpper()}";
        characterName.text = SpriteToTextColorUtility.AppendSpriteName(nameMessage,characterName.color);
        chosenCharacterImageObject.transform.DOLocalMoveX(endXPos, 0.65f);
        chosenCharacter = profile;
        lockedIn = true;
    }
    public void ClearInfo()
    {
        lockedIn = false;
        _characterIconImage.sprite = null;
        characterName.text = "";
        colorSelectIndex = 0;
        Vector3 startPos = new Vector3(chosenCharacterImageObject.transform.localPosition.x + startXPos, 0, 0);
        chosenCharacterImageObject.transform.localPosition = startPos;
        chosenCharacter = null;
    }
    public void ResetNamePlatePosition(bool clearData = false)
    {
        Sequence resetSequence = DOTween.Sequence();
        resetSequence.Append(_colorPickerTransform.DOLocalMove(_startColorPosition, 0.55f));
        resetSequence.Append(_nameplateTransform.DOLocalMove(_startNtPosition, 0.45f).OnComplete(() => 
        {
            if (clearData) 
            {
                ClearInfo();
            }
        }));
        resetSequence.Play();
    }
    public void DeactivateColorPanelPosition()
    {
        _colorPickerTransform.DOLocalMove(_startColorPosition, 1.15f).SetEase(Ease.OutBack);
    }
    public void ActivateColorPanelPosition()
    {
        _colorPickerTransform.DOLocalMove(_endColorPosition, 1.15f).SetEase(Ease.OutBack).OnComplete(() => 
        {
            SetDefaultText();
        });
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
            colorSelectIndex = chosenCharacter._characterSkins.ColorSets.Count-1;
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
            colorSelectIndex = 0;
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
