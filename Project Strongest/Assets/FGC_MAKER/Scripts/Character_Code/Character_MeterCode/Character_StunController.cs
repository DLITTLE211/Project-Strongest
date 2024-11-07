using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Character_StunController : MonoBehaviour
{
    public MainMeterController stunMeter;
    IEnumerator RecoveryRoutine;
    public Gradient stunMeterColor;
    public Image stunMeterImage;
    public float clearStunTimeGate;
    public float stunRecoverTime;
    public bool canRecover;
    // Start is called before the first frame update
    void Start()
    {
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
        if(RecoveryRoutine != null) 
        {
            StopCoroutine(RecoveryRoutine);
            RecoveryRoutine = null;
        }
        DOTween.Kill(stunMeter.meterSlider);
    }
    public void ApplyStun(float stunAmount) 
    {
        KillRegenTween();
        stunMeter.currentValue += stunAmount;
        stunMeter.SetCurrentMeterValue(stunMeter.currentValue);
        RecoveryRoutine = RecoverStunWaitTime();
        StartCoroutine(RecoveryRoutine);
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
