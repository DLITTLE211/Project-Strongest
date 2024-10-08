using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Character_InputTimer_Attacks : Character_InputTimer
{
    public Character_Base _base;
    public TimerType _type;
    private bool throwLanded, superLanded;
    private bool permanentStance;
    // Start is called before the first frame update
    public void ResetTimer()
    {
        FrameCountTimer = StartFrameCountTimer;
        CheckForInput = false;
        _base._cComboDetection.ResetCombos();
        _base._aManager.ClearAttacks();
    }
    public void SetStartingValues(float newTime)
    {
        _base._cComboDetection.inStance = false;
        _base._cComboDetection.inRekka = false;
        StartFrameCountTimer = newTime;
        FrameCountTimer = StartFrameCountTimer;
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
    // Update is called once per frame
    private void Update()
    {
        if (_base._cHurtBox.IsGrounded())
        {
            TimerTickDown();
        }
      
    }
    public void TimerTickDown()
    {
        if (_base._cComboDetection.inStance)
        {
            if (!permanentStance)
            {
                if (CheckForInput && _base._cAnimator.inputWindowOpen && !throwLanded && !superLanded)
                {
                    CountDownTimer();
                    return;
                }
            }
            return;
        }
        else
        {
            if (CheckForInput && _base._cAnimator.inputWindowOpen && !throwLanded && !superLanded)
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
                _type = newType;
                return;

            }
            if (newType == TimerType.InStance)
            {
                SetStartStanceTimerValues(newTime);
                _type = newType;
                return;
            }
            if (newType == TimerType.Super || newType == TimerType.Throw || newType == TimerType.Special)
            {
                SetStartingValues(newTime);
                _type = newType;
                return;
            }
        }
        else
        {
            SetStartingValues(newTime);
            _type = newType;
            return;
        }
    }
    public void PauseTimerOnThrowSuccess()
    {
        throwLanded = true;
    }
    public void PauseTimerOnSuperSuccess()
    {
        superLanded = true;
    }
    public void ClearSuperLanded()
    {
        SetTimerType(TimerType.Normal);
        superLanded = false;
        FrameCountTimer = -1 / 60f;
        CountDownTimer();
    }
    public void ClearThrowLanded()
    {
        SetTimerType(TimerType.Normal);
        throwLanded = false;
        FrameCountTimer = -1 / 60f;
        CountDownTimer();
    }
    public void ClearAttackLanded()
    {
        SetTimerType(TimerType.Normal);
        FrameCountTimer = -1 / 60f;
        CountDownTimer();
        if (superLanded) 
        {
            superLanded = false;
        }
        if (throwLanded)
        {
            throwLanded = false;
        }
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
        float newSetTime = (time * (1 / 60f));
        ResetTimeOnSpecialMove(newSetTime);
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
    public bool ReturnTimerLessThan(float timeComparison) 
    {
        return FrameCountTimer <= timeComparison;
    }
    public bool ReturnInThrowAnim() 
    {
        return throwLanded;
    }
}
[Serializable]
public enum TimerType 
{
    Normal,
    Throw,
    Special,
    InRekka,
    InStance,
    Super,
}
