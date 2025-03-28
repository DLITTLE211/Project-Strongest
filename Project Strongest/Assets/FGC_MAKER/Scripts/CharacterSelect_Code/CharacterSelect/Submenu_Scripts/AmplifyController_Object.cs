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
        _amplifierImage.DOFade(0f, 0.45f);
        _amplifierText.DOFade(0f, 0.45f).OnComplete(() =>
        {
            _amplifierTransform.DOLocalMoveX(offSceenPos, 0f);
        });
    }
    // Start is called before the first frame update
    public void UpdateAmplifier(Amplifiers newAmplifier,Callback updateAmplifierNumber) 
    {
        if (!ToggleReady) 
        {
            return;
        }
        string amplifierName = newAmplifier.amplifier.ToString();
        _amplifierText.text = SpriteToTextColorUtility.AppendSpriteName(amplifierName, _amplifierText.color);
        BackgroundShiftColor(newAmplifier.meterColor);
        updateAmplifierNumber();
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
