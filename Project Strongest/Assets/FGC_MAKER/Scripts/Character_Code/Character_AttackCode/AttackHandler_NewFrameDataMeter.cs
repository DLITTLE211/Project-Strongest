using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using FightingGame_FrameData;

public class AttackHandler_NewFrameDataMeter : MonoBehaviour
{
    [SerializeField] private Character_Base _base;
    [SerializeField] private GameObject _refSingularFrame;
    [SerializeField] private List<AttackHandler_SingularFrame> _refSingularFrameList;
    [SerializeField] private TMP_Text frameDataInformationText;
    FrameType lastFrameDataType;
    string message;
    [SerializeField] private int currentFrame;
    public int FrameIndex { get { return currentFrame; } }
    [SerializeField] private int frameCount;
    public int TrueFrameCount { get { return frameCount; } }
    [SerializeField] private bool _isHitRecovering;
    public bool HitRecovering { get { return _isHitRecovering; } }
    void Start()
    {
        InitializeMeter();
        currentFrame = 0;
        lastFrameDataType = FrameType.Startup;
        message = "";
    }
    public void ResetMessage() 
    {
        message = "";
    }
    public void InitializeMeter()
    {
        for (int i = 0; i < 60; i++)
        {
            GameObject newFrame = Instantiate(_refSingularFrame, this.transform);
            newFrame.name = $"SingleFrame_{i + 1}";
            AttackHandler_SingularFrame singularFrameData = newFrame.GetComponent<AttackHandler_SingularFrame>();
            singularFrameData.InitColors();
            singularFrameData.InitFrame();

            _refSingularFrameList.Add(singularFrameData);
        }
    }

    public void ResetMeterData()
    {
        if (_base.opponentPlayer._aFrameDataMeter.HitRecovering)
        {
            frameDataInformationText.text = message;
            ResetMessage();
        }
        ResetFrames();
        _base.opponentPlayer._aFrameDataMeter.ResetFrames();
        lastFrameDataType = FrameType.Startup;
    }
    public void ResetFrames()
    {
        currentFrame = 0;
        frameCount = 0;
        for (int i = 0; i < _refSingularFrameList.Count; i++)
        {
            _refSingularFrameList[i].InitFrame();
        }
    }
    #region Update Frame On Attack
    public void UpdateFrame(FrameType type)
    {
        if (currentFrame >= _refSingularFrameList.Count)
        {
            currentFrame = 0;
        }
        if (lastFrameDataType != type)
        {
            lastFrameDataType = type;
            message += $"{lastFrameDataType.ToString()}: {currentFrame}/";
            _refSingularFrameList[currentFrame].SetFrame_FrameType(lastFrameDataType, currentFrame+1);
        }
        else
        {
            _refSingularFrameList[currentFrame].SetFrame_FrameType(lastFrameDataType);
        }
        currentFrame++;
        frameCount++;
    }
    public void GetAdvantageValue(FrameData _frameData)
    {
        int lastFrame = currentFrame == 0 ? 0 : currentFrame-1;
        _refSingularFrameList[lastFrame].SetFrame_FrameType(lastFrameDataType, currentFrame );
        GameManager.instance._frameDataCalculator.ReturnFrameDifference(_base.opponentPlayer, _frameData);
    }
    public void SetFrameInformation(FrameData _frameData)
    {
        float frameValue = GameManager.instance._frameDataCalculator.FrameDifference;
        message = $"Startup: {_frameData.startup}/ TotalFrames: {_frameData.recoveryEnd}/";
        string frameDifference = frameValue < 0 ? $"-{Mathf.Abs(frameValue)}" : $"+{Mathf.Abs(frameValue)}";
        string advantageAmount = frameDifference;
        message += $"Advantage: {advantageAmount}";
        frameDataInformationText.text = message;
    }
    #endregion
    public void SetHitRecoveringState(bool state)
    {
        if (!state) 
        {
            int nextFrame = currentFrame + 1;
            nextFrame = nextFrame >= _refSingularFrameList.Count ? 0 : nextFrame;
            _refSingularFrameList[nextFrame].SetFrame_FrameType(FrameType.StunNext, nextFrame);
        }
        if (state && !_isHitRecovering)
        {
            ResetFrames();
            currentFrame = _base.opponentPlayer._aFrameDataMeter.FrameIndex - 1;
            frameCount = currentFrame;
        }
        _isHitRecovering = state;
    }
    public void UpdateFrameOnHit()
    {
        if (_isHitRecovering)
        {
            if (currentFrame >= _refSingularFrameList.Count)
            {
                currentFrame = 0;
            }
            int nextFrame = currentFrame+1;
            nextFrame = nextFrame >= _refSingularFrameList.Count ? 0 : nextFrame;
            _refSingularFrameList[nextFrame].SetFrame_FrameType(FrameType.StunNext);
            _refSingularFrameList[currentFrame].SetFrame_FrameType(FrameType.Stun);
            currentFrame++;
            frameCount++;
        }
    }
}
