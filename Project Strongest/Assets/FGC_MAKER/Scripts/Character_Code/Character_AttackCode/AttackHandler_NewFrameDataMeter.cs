using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using FightingGame_FrameData;

public class AttackHandler_NewFrameDataMeter : MonoBehaviour
{
    [SerializeField] private GameObject mainObject;
    [SerializeField] private Character_Base _base;
    [SerializeField] private GameObject _refSingularFrame;
    [SerializeField] private Transform frameMeterParent;
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

    public void SetObjectState(bool state)
    {
        mainObject.SetActive(state);
    }
    public void SetupMeterData() 
    {
        if (GameManager.instance._gameModeSet.gameMode != GameMode.Training)
        {
            return;
        }
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
        if (GameManager.instance._gameModeSet.gameMode != GameMode.Training)
        {
            return;
        }
        for (int i = 0; i < 60; i++)
        {
            GameObject newFrame = Instantiate(_refSingularFrame, frameMeterParent);
            newFrame.name = $"SingleFrame_{i + 1}";
            AttackHandler_SingularFrame singularFrameData = newFrame.GetComponent<AttackHandler_SingularFrame>();
            singularFrameData.InitColors();
            singularFrameData.InitFrame();

            _refSingularFrameList.Add(singularFrameData);
        }
    }

    public void ResetMeterData()
    {
        if (GameManager.instance._gameModeSet.gameMode != GameMode.Training)
        {
            return;
        }
        if (_base.opponentPlayer._aFrameDataMeter.HitRecovering)
        {
            frameDataInformationText.text = message;
            ResetMessage();
        }
        ResetFrames();
        if (_base.opponentPlayer._aFrameDataMeter._isHitRecovering) 
        {
            _base.opponentPlayer._aFrameDataMeter._isHitRecovering = false;
        }
        _base.opponentPlayer._aFrameDataMeter.ResetFrames();
        lastFrameDataType = FrameType.Startup;
    }
    public void ResetFrames()
    {
        if (GameManager.instance._gameModeSet.gameMode != GameMode.Training)
        {
            return;
        }
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
        if (GameManager.instance._gameModeSet.gameMode != GameMode.Training)
        {
            return;
        }
        if (currentFrame >= _refSingularFrameList.Count)
        {
            currentFrame = 0;
        }
        if (lastFrameDataType != type)
        {
            lastFrameDataType = type;
            message += $"{lastFrameDataType.ToString()}: {currentFrame}/";
            _refSingularFrameList[currentFrame].SetFrame_FrameType(lastFrameDataType, currentFrame + 1);
        }
        else
        {
            _refSingularFrameList[currentFrame].SetFrame_FrameType(lastFrameDataType);
        }
        int prevFrame = currentFrame - 1;
        prevFrame = prevFrame < 0 ? _refSingularFrameList.Count - 1 : prevFrame;
        _refSingularFrameList[prevFrame].DisablePreviousFrameHighlight();

        currentFrame++;
        frameCount++;
    }
    public void GetAdvantageValue(FrameData _frameData)
    {
        if (GameManager.instance._gameModeSet.gameMode != GameMode.Training)
        {
            return;
        }
        int lastFrame = currentFrame == 0 ? 0 : currentFrame-1;
        _refSingularFrameList[lastFrame].SetFrame_FrameType(lastFrameDataType, currentFrame);
        GameManager.instance._frameDataCalculator.ReturnFrameDifference(_base.opponentPlayer, _frameData);
    }
    public void SetFrameInformation(FrameData _frameData)
    {
        if (GameManager.instance._gameModeSet.gameMode != GameMode.Training)
        {
            return;
        }
        float frameValue = GameManager.instance._frameDataCalculator.FrameDifference;
        message = $"Startup: {_frameData.startup}/ TotalFrames: {_frameData.recoveryEnd}/";
        string attackingframeDifference = frameValue < 0 ? $"-{Mathf.Abs(frameValue)}" : $"+{Mathf.Abs(frameValue)}";
        string hitframeDifference = frameValue < 0 ? $"+{Mathf.Abs(frameValue)}" : $"-{Mathf.Abs(frameValue)}";
        string advantageAmount = attackingframeDifference;
        _base.opponentPlayer._aFrameDataMeter.frameDataInformationText.text = $"Disadvantage: {hitframeDifference}";
        message += $"Advantage: {advantageAmount}";
        frameDataInformationText.text = message;
    }
    #endregion
    public void SetHitRecoveringState(bool state)
    {
        if (GameManager.instance._gameModeSet.gameMode != GameMode.Training)
        {
            return;
        }
        if (!state) 
        {
            frameCount++;
            try
            {
                int nextFrame = currentFrame + 1;
                nextFrame = nextFrame >= _refSingularFrameList.Count ? 0 : nextFrame;
                _refSingularFrameList[nextFrame].SetFrame_FrameType(FrameType.StunNext, nextFrame + 1);
                _refSingularFrameList[currentFrame].SetFrame_FrameType(FrameType.Stun);
                int prevFrame = currentFrame - 1;
                prevFrame = prevFrame < 0 ? _refSingularFrameList.Count - 1 : prevFrame;
                _refSingularFrameList[prevFrame].DisablePreviousFrameHighlight();
            }
            catch (ArgumentOutOfRangeException)
            {
                if (currentFrame >= _refSingularFrameList.Count || currentFrame < 0)
                {
                    currentFrame = 0;
                }
                _refSingularFrameList[currentFrame].SetFrame_FrameType(FrameType.Stun);
            }
        }
        if (state)
        {
            if (!_isHitRecovering)
            {
                ResetFrames();
                currentFrame = _base.opponentPlayer._aFrameDataMeter.FrameIndex - 1;
                frameCount = currentFrame;
            }
            int prevFrame = currentFrame - 1;
            prevFrame = prevFrame < 0 ? _refSingularFrameList.Count - 1 : prevFrame;
            _refSingularFrameList[prevFrame].DisablePreviousFrameHighlight();
            frameCount++;
        }
        _isHitRecovering = state;
    }
    public void UpdateFrameOnHit(FrameType _type)
    {
        if (GameManager.instance._gameModeSet.gameMode != GameMode.Training)
        {
            return;
        }
        if (_isHitRecovering)
        {
            if (currentFrame >= _refSingularFrameList.Count || currentFrame < 0)
            {
                currentFrame = 0;
            }
            lastFrameDataType = _type;
            try
            {
                int nextFrame = currentFrame + 1;
                nextFrame = nextFrame >= _refSingularFrameList.Count ? 0 : nextFrame;
                _refSingularFrameList[nextFrame].SetFrame_FrameType(FrameType.StunNext);
                _refSingularFrameList[currentFrame].SetFrame_FrameType(_type);
                int prevFrame = currentFrame - 1;
                prevFrame = prevFrame < 0 ? _refSingularFrameList.Count - 1 : prevFrame;
                _refSingularFrameList[prevFrame].DisablePreviousFrameHighlight();
                currentFrame++;
                frameCount++;
            }
            catch (ArgumentOutOfRangeException) 
            {
                _refSingularFrameList[currentFrame].SetFrame_FrameType(_type);
            }
        }
    }
}
