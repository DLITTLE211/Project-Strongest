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
    [SerializeField] private bool _isHitRecovering;
    public bool HitRecovering { get { return _isHitRecovering; } }
    void Start()
    {
        InitializeMeter();
        currentFrame = 0;
        lastFrameDataType = FrameType.Init;
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
            singularFrameData.InitFrame();
            _refSingularFrameList.Add(singularFrameData);
        }
    }

    public void ResetMeterData()
    {
        if (_base.opponentPlayer._aFrameDataMeter.HitRecovering)
        {
            frameDataInformationText.text = message;
            message = "";
        }
        ResetFrames();
        lastFrameDataType = FrameType.Init;
    }
    public void ResetFrames()
    {
        currentFrame = 0;
        for (int i = 0; i < _refSingularFrameList.Count; i++)
        {
            _refSingularFrameList[i].InitFrame();
        }
    }
    #region Update Frame On Attack
    public void UpdateFrame(bool initHit, bool startUpHit, bool ActiveHit, bool recoveryHit, bool lastFrame)
    {
        if(currentFrame >= _refSingularFrameList.Count - 1) 
        {
            currentFrame = 0;
        }
        if (!lastFrame)
        {
            FrameType currentFrameDataType = GetCurrentFrameType(initHit, startUpHit, ActiveHit, recoveryHit);
            if (lastFrameDataType != currentFrameDataType) 
            {
                lastFrameDataType = currentFrameDataType;
                _refSingularFrameList[currentFrame].SetFrame_FrameType(lastFrameDataType, currentFrame+1);
                if (currentFrameDataType == FrameType.Reset)
                {
                    float frameValue = GameManager.instance._frameDataCalculator.ReturnFrameDifference(_base.opponentPlayer);
                    string frameDifference = frameValue < 0 ? $"-{Mathf.Abs(frameValue)}" : $"+{Mathf.Abs(frameValue)}";
                    string advantageAmount = frameDifference;
                    message += $"Advantage: {advantageAmount}";
                    frameDataInformationText.text = message;
                }
                else
                {
                    message += $"{currentFrameDataType.ToString()}: {currentFrame + 1}/";
                }
            }
            else
            {
                _refSingularFrameList[currentFrame].SetFrame_FrameType(lastFrameDataType);
            }
            currentFrame++;
        }
        else
        {
            frameDataInformationText.text = message;
        }
    }
    public FrameType GetCurrentFrameType(bool initHit, bool startUpHit, bool ActiveHit, bool recoveryHit) 
    {
        if (recoveryHit) 
        {
            return FrameType.Reset;
        }
        if (ActiveHit)
        {
            return FrameType.Recovery;
        }
        if (startUpHit)
        {
            return FrameType.Active;
        }
        if (initHit)
        {
            return FrameType.Startup;
        }
        return FrameType.Init;
    }
    #endregion
    public void SetHitRecoveringState(bool state) 
    {
        _isHitRecovering = state;
        if (state)
        {
            ResetFrames();
            currentFrame = _base.opponentPlayer._aFrameDataMeter.FrameIndex-1;
        }
    }
    public void UpdateFrameOnHit()
    {
        if (_isHitRecovering)
        {
            if (currentFrame >= _refSingularFrameList.Count - 1)
            {
                currentFrame = 0;
            }
            _refSingularFrameList[currentFrame].SetFrame_FrameType(FrameType.Recovery);
            currentFrame++;
        }
    }
}
