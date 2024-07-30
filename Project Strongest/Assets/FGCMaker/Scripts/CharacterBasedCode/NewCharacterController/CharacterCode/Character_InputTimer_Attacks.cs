using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Character_InputTimer_Attacks : Character_InputTimer
{
    public Character_Base _base;
    public TimerType _type;
    private bool throwLanded;
    private bool permanentStance;
    // Start is called before the first frame update
    public void ResetTimer()
    {
        
        if (throwLanded)
        {
            throwLanded = false;
        }
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

    public void ResetTimeOnSpecialMove(float time)
    {
        _frameCountTimer = time;
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
        if (_base._cComboDetection.inStance)
        {
            if (!permanentStance)
            {
                if (CheckForInput && _base._cAnimator.inputWindowOpen && !throwLanded)
                {
                    CountDownTimer();
                    return;
                }
            }
            return;
        }
        else
        {
            if (CheckForInput && _base._cAnimator.inputWindowOpen && !throwLanded)
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
                SetStartStanceTimerValues(newTime);
            }
        }
        else
        {
            SetStartingValues(newTime);
        }
        _type = newType;
    }
    public void PauseTimerOnThrowSuccess()
    {
        throwLanded = true;
    }
    public void ClearThrowLanded()
    {
        throwLanded = false;
        FrameCountTimer = 0;
        CountDownTimer();
    }

    void SetStartStanceTimerValues(float time) 
    {
        _base._cComboDetection.inStance = true;
        if (time > 0)
        {
            ResetTimeOnSpecialMove((time));
            permanentStance = false;
        }
        else 
        {
            permanentStance = true;
        }
    }
    void SetStartRekkaTimerValues(float time)
    {
        ResetTimeOnSpecialMove((time * (1/60f)));
    }

    public void CountDownTimer()
    {
        if (_type == TimerType.Normal ^ _type == TimerType.InRekka)
        {
            if (FrameCountTimer <= -1 / 60f)
            {
                if (_type == TimerType.InRekka)
                {
                    SetTimerType();
                }
                ResetTimer();
            }
            else
            {
                FrameCountTimer -= 1 / 60f;
            }
        }
        else
        {
            if (FrameCountTimer <= -1 / 60f)
            {
                ResetTimer();
            }
            else
            {
                FrameCountTimer -= 1 / 60f;
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
