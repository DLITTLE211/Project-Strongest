using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Character_StunController : MonoBehaviour
{
    public MainMeterController stunMeter;
    public HealthController healthController;

    public Gradient stunMeterColor;
    public Image stunMeterImage;
    public const float maxStun = 1;
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
        stunMeter.maxValue = maxStun;
        stunMeter.SetCurrentMeterValue(stunMeter.currentValue);
        checkStunGradiet();
    }
    private void Update()
    {
        if (canRecover) 
        {
            checkStunGradiet();
        }
    }
    public void ApplyStun(float stunAmount) 
    {
        StopCoroutine(RecoverStunWaitTime());
        canRecover = false;
        DOTween.Kill(stunMeter.meterSlider);
        stunMeter.currentValue += stunAmount;
        checkStunGradiet();
        stunMeter.SetCurrentMeterValue(stunMeter.currentValue);
        StartCoroutine(RecoverStunWaitTime());
    }
    void checkStunGradiet() 
    {
        stunMeterImage.color = stunMeterColor.Evaluate(stunMeter.currentValue);
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
            checkStunGradiet();
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
        checkStunGradiet();
        canRecover = false;
    }
}
