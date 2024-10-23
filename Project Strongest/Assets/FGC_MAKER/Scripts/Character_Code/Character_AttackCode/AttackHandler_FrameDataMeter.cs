using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using DG.Tweening;
using TMPro;

public class AttackHandler_FrameDataMeter : MonoBehaviour
{
    [SerializeField] private FrameDate_Meter _startUpSlider;
    [SerializeField] private FrameDate_Meter _activeSlider;
    [SerializeField] private FrameDate_Meter _inactiveSlider;
    [SerializeField] private FrameDate_Meter _recoverySlider;
    // Start is called before the first frame update

    public void SetSliderInformation(FrameData _frameData) 
    {
        _startUpSlider.SetFrameMeter(_frameData.recoveryEnd, _frameData.startup);
        _activeSlider.SetFrameMeter(_frameData.recoveryEnd, _frameData.active);
        _inactiveSlider.SetFrameMeter(_frameData.recoveryEnd, _frameData.inactive);
        _recoverySlider.SetFrameMeter(_frameData.recoveryEnd, _frameData.recoveryEnd);
    }
}
[Serializable]
public class FrameDate_Meter 
{
    public Slider _frameMeter;
    public TMP_Text frameNumberText;
    public MeterType _meterType;
    public Image _meterFillImage;
    public void SetImageColor() 
    {
        switch (_meterType) 
        {
            case MeterType.Startup:
                _meterFillImage.color = Color.white;
                break;
            case MeterType.Active:
                _meterFillImage.color = Color.cyan;
                break;
            case MeterType.Inactive:
                _meterFillImage.color = Color.grey;
                break;
            case MeterType.Recovery:
                _meterFillImage.color = Color.red;
                break;
        }
    }
    public void SetFrameMeter(int maxValue, int value) 
    {
        _frameMeter.maxValue = maxValue;
        _frameMeter.value = 0;
        _frameMeter.DOValue(value, 0.5f).OnComplete(() => 
        {
            frameNumberText.text = $"{_meterType}: {value}";;
        });
    }
}
public enum MeterType 
{
    Startup,
    Active,
    Inactive,
    Recovery,
}
