using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using FightingGame_FrameData;

public class Character_InputTimer_Attacks : Character_InputTimer
{
    [SerializeField] protected float _subAttackTimer;
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
    public void CloseCurrentSubAttackWindow() 
    {
        _subAttackTimer = 0;
        _base.comboList3_0.CloseSubAttackWindow();
    }
    public void SetStartingValues(float newTime)
    {
        _base._cComboDetection.inStance = false;
        _base._cComboDetection.inRekka = false;
        StartFrameCountTimer = 0.4f;
        FrameCountTimer = newTime;
        CheckForInput = false;
    }
    public void ResetTimerSuccess()
    {
        FrameCountTimer = StartFrameCountTimer;
    }

    public void ResetTimeOnSpecialMove(float time, float _subAttackTime = 0f)
    {
        _frameCountTimer = time;
        if(_subAttackTime > 0f) 
        {
            _subAttackTimer = _subAttackTime;
        }
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
    public void SetTimerType(TimerType newType = TimerType.Normal, float newTime = 0.4f, float subAttackTime = 0f) 
    {
        if (newType != TimerType.Normal)
        {
            if (newType == TimerType.InRekka)
            {
                SetStartRekkaTimerValues(newTime, subAttackTime);
                _type = newType;
                return;

            }
            if (newType == TimerType.InStance)
            {
                SetStartStanceTimerValues(newTime, subAttackTime);
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
            if (subAttackTime > 0f)
            {
                _subAttackTimer = subAttackTime;
            }
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
    public void ClearAttackLanded(Attack_CancelInfo cancelInfo = null)
    {
        if (cancelInfo != null)
        {
            if (cancelInfo.nextAvailableAttackRoute.HasFlag(Cancel_State.NotCancellable))
            {
                SetTimerType(TimerType.Normal);
                FrameCountTimer = -1 / 60f;
                CountDownTimer();
            }
        }
        if (superLanded) 
        {
            superLanded = false;
        }
        if (throwLanded)
        {
            throwLanded = false;
        }
    }

    void SetStartStanceTimerValues(float mainTime,float stanceTime) 
    {
        _base._cComboDetection.inStance = true;
        if (stanceTime > 0)
        {
            ResetTimeOnSpecialMove(mainTime,stanceTime);
            permanentStance = false;
        }
        else 
        {
            permanentStance = true;
        }
    }
    void SetStartRekkaTimerValues(float mainTime, float rekkaTime)
    {
        float newRekkaSetTime = (rekkaTime * (1 / 60f));
        float newMainTime = (mainTime * (1 / 60f));
        ResetTimeOnSpecialMove(newMainTime, newRekkaSetTime);
    }

    public void CountDownTimer()
    {
        switch (_type)
        {
            case TimerType.InRekka:
                if (FrameCountTimer <= -Base_FrameCode.ONE_FRAME)
                {
                    SetTimerType();
                    ResetTimer();
                }
                else
                {
                    FrameCountTimer -= Base_FrameCode.ONE_FRAME;
                }
                if (_subAttackTimer < 0)
                {
                    CloseCurrentSubAttackWindow();
                }
                else
                {
                    _subAttackTimer -= Base_FrameCode.ONE_FRAME;
                }
                break;
            default:
                if (FrameCountTimer <= -Base_FrameCode.ONE_FRAME)
                {
                    ResetTimer();
                }
                else
                {
                    FrameCountTimer -= Base_FrameCode.ONE_FRAME;
                }
                if (_subAttackTimer < 0)
                {
                    CloseCurrentSubAttackWindow();
                }
                else
                {
                    _subAttackTimer -= Base_FrameCode.ONE_FRAME;
                }
                break;
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
