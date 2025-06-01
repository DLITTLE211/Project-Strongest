using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using FightingGame_FrameData;

public class Character_StunController : MonoBehaviour
{
    [SerializeField] private Character_Base _base;
    public MainMeterController stunMeter;
    IEnumerator RecoveryRoutine;
    public Gradient stunMeterColor;
    public Image stunMeterImage;
    public float clearStunTimeGate;
    public float stunRecoverTime;
    public bool canRecover;
    public bool IsSTUNNED;
    Callback endStunSequenceFunc;
    // Start is called before the first frame update
    void Start()
    {
        endStunSequenceFunc = null;
        SetStartStunValues();
    }
    public void SetStartStunValues() 
    {
        stunMeter.currentValue = 0;
        stunMeter.SetCurrentMeterValue(stunMeter.currentValue);
    }
    private void Update()
    {
        checkStunGradiet();
    }
    public void KillRegenTween() 
    {
        if(endStunSequenceFunc != null) 
        {
            endStunSequenceFunc();
            endStunSequenceFunc = null;
        }
        if(RecoveryRoutine != null) 
        {
            if (IsSTUNNED) 
            {
                IsSTUNNED = false;
                DOTween.Complete(stunMeter.meterSlider);
            }
            else 
            {
                DOTween.Kill(stunMeter.meterSlider);
            }
            StopCoroutine(RecoveryRoutine);
            RecoveryRoutine = null;
        }
    }
    public void ApplyStun(float stunAmount) 
    {
        KillRegenTween();
        stunMeter.currentValue += stunAmount;
        stunMeter.SetCurrentMeterValue(stunMeter.currentValue);
        if(stunMeter.currentValue >= stunMeter.maxValue) 
        {
            IsSTUNNED = true;
            _base._cHurtBox.SetHurboxState(HurtBoxType.Invincible);
        }
        else
        {
            RecoveryRoutine = RecoverStunWaitTime();
            StartCoroutine(RecoveryRoutine);
        }
    }
    void checkStunGradiet() 
    {
        float stunValue = stunMeter.currentValue / stunMeter.maxValue;
        stunMeterImage.color = stunMeterColor.Evaluate(stunValue);
    }
    void CheckMeterValue() 
    {
        if (stunMeter.currentValue != stunMeter.meterSlider.value)
        {
            stunMeter.currentValue = stunMeter.meterSlider.value;
        }
    }
    public void ActivateStunDelay(Callback onFinishFunc) 
    {
        RecoveryRoutine = FullyStunRoutine(onFinishFunc);
        StartCoroutine(RecoveryRoutine);
    }
    IEnumerator FullyStunRoutine(Callback onFinishFunc)
    {
        _base.Deactivate();
        endStunSequenceFunc = onFinishFunc;
        float totalStunTime = Time.smoothDeltaTime * 174f;
        _base._cHurtBox.SetHurboxState();
        int dizzyAnimation = Animator.StringToHash("DizzyState");
        _base._cAnimator.PlayNextAnimation(dizzyAnimation,0f,true);
        stunMeter.meterSlider.DOValue(0, totalStunTime).OnUpdate(CheckMeterValue).OnComplete(() =>
        {
            SetFinalStunValue();
        });
        yield return new WaitForSeconds(totalStunTime);
        _base.Activate();
        IsSTUNNED = false;
        endStunSequenceFunc();
        endStunSequenceFunc = null; 
    }
    IEnumerator RecoverStunWaitTime()
    {
        yield return new WaitForSeconds(clearStunTimeGate);
        recoverStun();
    }
    public void recoverStun()
    {
        if (stunMeter.currentValue <= 0)
        {
            canRecover = false;
            return;
        }
        else
        {
            canRecover = true;
            stunMeter.meterSlider.DOValue(0, stunRecoverTime).OnUpdate(CheckMeterValue).OnComplete(() =>
            {
                SetFinalStunValue();
            });
        }
    }
    public void SetFinalStunValue()
    {
        stunMeter.currentValue = 0;
        stunMeter.SetCurrentMeterValue(0);
        canRecover = false;
    }
}
