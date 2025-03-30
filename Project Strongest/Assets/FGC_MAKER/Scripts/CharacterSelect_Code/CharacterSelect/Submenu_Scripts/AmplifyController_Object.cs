using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;

public class AmplifyController_Object : MonoBehaviour
{
    [SerializeField] private float offSceenPos;
    [SerializeField] private Vector3 onSceenPos;
    [SerializeField] private Image _amplifierBackground;
    [SerializeField] private RectTransform _amplifierTransform;
    [SerializeField] private Image _amplifierFrameImage;
    [SerializeField] private Image _amplifierBackgroundImage;
    [SerializeField] private Image _amplifierImage;
    [SerializeField] private TMP_Text _amplifierText;
    Tween colorShift;
    Tween imageShift;
    private Amplifiers _chosenAmplifier;
    public int AmplifierIndex;
    public bool ToggleReady;
    public void Activate() 
    {
        _amplifierBackground.DOFade(1f, 0.45f); 
        _amplifierFrameImage.DOFade(1f, 0.45f);
        _amplifierBackgroundImage.DOFade(1f, 0.45f);
        _amplifierImage.DOFade(1f, 0.45f);
        _amplifierText.DOFade(1f, 0.45f);
        _amplifierTransform.DOLocalMove(onSceenPos, 1.5f).SetEase(Ease.OutBack).OnComplete(() =>
        {
            ToggleReady = true;
        });
    }
    public void Deactivate()
    {
        ToggleReady = false;
        _amplifierBackground.DOFade(0f, 0.45f);
        _amplifierFrameImage.DOFade(0f, 0.45f);
        _amplifierBackgroundImage.DOFade(0f, 0.45f);
        _amplifierImage.DOFade(0f, 0.45f).OnComplete(() =>
        {
            _amplifierTransform.DOLocalMoveX(offSceenPos, 0f);
        });
    }
    public void DeactivateInstant()
    {
        ToggleReady = false;
        _amplifierBackground.DOFade(0f, 0);
        _amplifierFrameImage.DOFade(0f, 0);
        _amplifierBackgroundImage.DOFade(0f, 0);
        _amplifierImage.DOFade(0f, 0).OnComplete(() =>
        {
            _amplifierTransform.DOLocalMoveX(offSceenPos, 0f);
        });
    }
    public void SetSelected() 
    {
        string message = $"{_chosenAmplifier.amplifier.ToString().ToUpper()} \nSELECTED";
        _amplifierText.text = SpriteToTextColorUtility.AppendSpriteName(message, _amplifierText.color);
    }
    // Start is called before the first frame update
    public void UpdateAmplifier(Amplifiers newAmplifier, int index, bool overrideReady = false) 
    {
        if (!overrideReady)
        {
            if (!ToggleReady)
            {
                return;
            }
        }

        AmplifierIndex = index;
        string amplifierName = newAmplifier.amplifier.ToString().ToUpper();
        ShiftImage(newAmplifier);
        _amplifierText.text = SpriteToTextColorUtility.AppendSpriteName(amplifierName, _amplifierText.color);
        BackgroundShiftColor(newAmplifier.meterColor);
    }
    public void ShiftImage(Amplifiers newAmp) 
    {
        if (imageShift != null)
        {
            imageShift.Complete();
            imageShift = null;
        }
        imageShift = _amplifierImage.DOFade(0.15f, 0.45f);
        imageShift.Play();
        imageShift.OnComplete(() =>
        {
            imageShift = _amplifierImage.DOFade(1f, 0.35f);
            imageShift.OnStart(() =>
            {
                _amplifierImage.sprite = newAmp.amplifierImage;
                _chosenAmplifier = newAmp;
            });
            imageShift.Play();
        });
    }
    public void BackgroundShiftColor(Color newColor) 
    {
        if(colorShift != null) 
        {
            colorShift.Complete();
            colorShift = null;
        }
        colorShift = _amplifierBackground.DOColor(newColor, 0.55f);
        colorShift.Play();
    }
    public Amplifiers ReturnChosenAmplifier() 
    {
        return _chosenAmplifier;
    }
}
