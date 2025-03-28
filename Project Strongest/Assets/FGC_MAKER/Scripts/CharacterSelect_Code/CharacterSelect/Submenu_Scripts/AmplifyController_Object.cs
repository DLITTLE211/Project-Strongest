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
        ShiftImage(newAmplifier.amplifierImage);
        _amplifierText.text = SpriteToTextColorUtility.AppendSpriteName(amplifierName, _amplifierText.color);
        BackgroundShiftColor(newAmplifier.meterColor);
    }
    public void ShiftImage(Sprite newImage) 
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
                _amplifierImage.sprite = newImage;
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
}
