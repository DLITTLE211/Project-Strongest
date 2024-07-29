using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Character_InputTimer_Attacks : Character_InputTimer
{
    public Character_Base _base;
    public TimerType _type;
    // Start is called before the first frame update
    public void ResetTimer()
    {
        FrameCountTimer = StartFrameCountTimer;
        CheckForInput = false;
        _base._cComboDetection.ResetCombos();
        _base._aManager.ClearAttacks();
    }
    public void SetStartingValues(float newTime = 0.4f)
    {
        _base._cComboDetection.inStance = false;
        _base._cComboDetection.inRekka = false;
        StartFrameCountTimer = newTime;
        FrameCountTimer = StartFrameCountTimer;
        CheckForInput = false;
    }
    public void HaltTimer()
    {
        CheckForInput = false;
    }
    public void ResetTimerSuccess()
    {
        FrameCountTimer = StartFrameCountTimer;
    }

    public void ResetTimeOnRekka(float time)
    {
        FrameCountTimer = time;
    }

    public void ResumeTimer()
    {
        CheckForInput = true;
    }
    // Update is called once per frame
    private void Update()
    {
        TimerTickDown();
    }
    public void TimerTickDown()
    {
        if (!_base._cComboDetection.inStance)
        {
            if (CheckForInput && _base._cAnimator.inputWindowOpen)
            {
                CountDownTimer();
            }
        }
    }
    public void SetTimerType(TimerType newType = TimerType.Normal, float newTime = 0.4f) 
    {
        if (newType != TimerType.Normal)
        {
            if (newType == TimerType.InRekka)
            {
                SetStartRekkaTimerValues(newTime);
            }
            if (newType == TimerType.InStance)
            {
                SetStartStanceTimerValues();
            }
        }
        else
        {
            SetStartingValues(newTime);
        }
        _type = newType;
    }

    void SetStartStanceTimerValues() 
    {
        _base._cComboDetection.inStance = true;
    }
    void SetStartRekkaTimerValues(float time)
    {
        ResetTimeOnRekka((time * (1/60f)));
    }

    public void CountDownTimer()
    {
        if (_type == TimerType.Normal ^ _type == TimerType.InRekka)
        {
            if (FrameCountTimer <= 0f)
            {
                if (_type == TimerType.InRekka)
                {
                    //_base._cAnimator.ClearLastAttack();
                    SetTimerType();
                }
                ResetTimer();
            }
            else
            {
                FrameCountTimer -= 1 / 60f;
               // FrameCountTimer = _frameCountTimer;
            }
        }
        else 
        {
            if (FrameCountTimer <= 0f)
            {
                ResetTimer();
            }
            else
            {
                FrameCountTimer -= 1 / 60f;
               // FrameCountTimer = _frameCountTimer;
            }
        }
    }
    public bool ReturnTimerIsZero() 
    {
        return FrameCountTimer <= 0f;
    }
}
[Serializable]
public enum TimerType 
{
    Normal,
    InRekka,
    InStance,
}
