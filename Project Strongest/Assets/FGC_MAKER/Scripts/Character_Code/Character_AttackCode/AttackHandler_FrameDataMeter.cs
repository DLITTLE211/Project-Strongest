using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using DG.Tweening;
using TMPro;

public class AttackHandler_FrameDataMeter : MonoBehaviour
{
    [SerializeField] private FrameMeterHolder _frameMeterHolder;
    // Start is called before the first frame update
    private void Start()
    {
        _frameMeterHolder.SetFrameImageColor();
    }
    public void SetSliderInformation(FrameData _frameData) 
    {
        _frameMeterHolder.ActivateFrameDisplay(_frameData);
    }
}
[Serializable]
public class FrameMeterHolder 
{
    public List<FrameData_Meter> FrameDataMeter;
    public TMP_Text frameText;
    FrameData _frameData;
    public void SetFrameImageColor()
    {
        for (int i = 0; i < FrameDataMeter.Count; i++)
        {
            FrameDataMeter[i].SetImageColor();
        }
    }
    public void ActivateFrameDisplay(FrameData frameData)
    {
        _frameData = frameData;
        Sequence meterDataSequence = DOTween.Sequence();
        FrameDataMeter[0].SetMeterInformation(_frameData.recoveryEnd, _frameData.startup);
        FrameDataMeter[1].SetMeterInformation(_frameData.recoveryEnd, _frameData.active);
        FrameDataMeter[2].SetMeterInformation(_frameData.recoveryEnd, _frameData.inactive);
        FrameDataMeter[3].SetMeterInformation(_frameData.recoveryEnd, _frameData.recoveryEnd);
        for (int i = 0; i < FrameDataMeter.Count;i++) 
        {
            if (i == FrameDataMeter.Count - 1)
            {
                FrameDataMeter[i].RunTween(SetText);
            }
            else 
            {
                FrameDataMeter[i].RunTween();
            }
        }
    }
    public void SetText() 
    {
        frameText.text = $"Startup Frames: {_frameData.startup}/ Active Frames: {_frameData.active}/ Recovery Frames: -{_frameData.totalRecovery}/ Total Frames: {_frameData.recoveryEnd}";
    }
}
[Serializable]
public class FrameData_Meter 
{
    public Slider _frameMeter;
    public MeterType _meterType;
    public Image _meterFillImage;
    int topValue;
    Tween activeTween;
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
    public void SetMeterInformation(int maxValue, int setValue)
    {
        if(activeTween != null) 
        {
            activeTween.Kill();
            activeTween = null;
        }
        _frameMeter.maxValue = maxValue;
        topValue = setValue;
        _frameMeter.value = 0;
    }
    public void RunTween(Callback func = null)
    {
        activeTween = _frameMeter.DOValue(topValue, 0.5f);
        activeTween.Play();
        if (func != null) { func(); }
    }
}
public enum MeterType 
{
    Startup,
    Active,
    Inactive,
    Recovery,
}
