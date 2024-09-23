using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;
using TMPro;

public class UI_HealthSettings_SubMenu : UI_SubMenuBase
{
    public int P1_healthIterator;
    public HealthRecoveryType P1_RecoveryType;
    [Space(20)]
    public TMP_Text P1_ToggleText;
    public UI_ToggleableElement P1HealthSlider;
    public UI_ToggleableElement P1StunSlider;
    [Space(20)]
    public int P2_healthIterator;
    public TMP_Text P2_ToggleText;
    public HealthRecoveryType P2_RecoveryType;
    public UI_ToggleableElement P2HealthSlider;
    public UI_ToggleableElement P2StunSlider;
    int healthRecoveryLength;
    private void Start()
    {
        P1_healthIterator = 0;
        P2_healthIterator = 0;
        SetP1ToggleText();
        SetP2ToggleText();

        healthRecoveryLength = Enum.GetNames(typeof(HealthRecoveryType)).Length; 
    }
    public void ToggleP1_HealthTypeUp()
    {
        P1_healthIterator++;
        if (P1_healthIterator > healthRecoveryLength-1) 
        {
            P1_healthIterator = 0;
        }
        SetP1ToggleText();
    }
    public void ToggleP1_HealthTypeDown()
    {
        P1_healthIterator--;
        if (P1_healthIterator < 0)
        {
            P1_healthIterator = healthRecoveryLength-1;
        }
        SetP1ToggleText();
    }
    public void ToggleP2_HealthTypeUp()
    {
        P2_healthIterator++;
        if (P2_healthIterator > healthRecoveryLength-1)
        {
            P2_healthIterator = 0;
        }
        SetP2ToggleText();
    }
    public void ToggleP2_HealthTypeDown()
    {
        P2_healthIterator--;
        if (P2_healthIterator < 0)
        {
            P2_healthIterator = healthRecoveryLength-1;
        }
        SetP2ToggleText();
    }
    public void SetP1ToggleText() 
    {
        P1_ToggleText.text = ((HealthRecoveryType)(P1_healthIterator)).ToString();
    }
    public void SetP2ToggleText()
    {
        P2_ToggleText.text = ((HealthRecoveryType)(P2_healthIterator)).ToString();
    }
    public HealthRecoveryType ReturnP1RecoveryType() 
    {
        return P1_RecoveryType;
    }
    public HealthRecoveryType ReturnP2RecoveryType()
    {
        return P2_RecoveryType;
    }
    public float ReturnP1HealthValue() 
    {
        return P1HealthSlider._elementSlider.value;
    }
    public float ReturnP1StunValue()
    {
        return P1StunSlider._elementSlider.value;
    }
    public float ReturnP2HealthValue()
    {
        return P2HealthSlider._elementSlider.value;
    }
    public float ReturnP2StunValue()
    {
        return P2StunSlider._elementSlider.value;
    }
}
public enum HealthRecoveryType 
{
    Instant,
    Standard,
}