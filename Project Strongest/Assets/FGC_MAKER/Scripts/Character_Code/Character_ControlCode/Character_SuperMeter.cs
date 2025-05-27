using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Character_SuperMeter : MonoBehaviour
{
    public MainMeterController superMeter;
    public Character_Health controller;

    public TMP_Text meterTierText;

    public float meterTier;
    public float fullMeterLevel;
    public const float maxMeterLevel = 120;
    public const float meterMaxThreshold = 30;
    public float meterDebuffPercentage;
    // Start is called before the first frame update
    void Start()
    {
        SetStartValue();
        ResetMeterDebuffPercent();
    }
    public void SetMeterDebuffPercent(float percentValue) 
    {
        meterDebuffPercentage = percentValue;
    }
    public void ResetMeterDebuffPercent() 
    {
        meterDebuffPercentage = 1;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            IncreaseMeterValue(30);
        }
        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            IncreaseMeterValue(60);
        }
        if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            IncreaseMeterValue(90);
        }
        if (Input.GetKeyDown(KeyCode.Keypad4))
        {
            IncreaseMeterValue(120);
        }
    }
    public void SetStartValue()
    {
        meterTier = 0;
        SetMeterTierText();
        superMeter.startValue = 0;
        fullMeterLevel = superMeter.startValue;
        superMeter.currentValue = superMeter.startValue;
        superMeter.meterSlider.value = superMeter.startValue;
        superMeter.meterSlider.maxValue = meterMaxThreshold;
    }
    #region Adding To Meter Value
    void IncreaseMeterValue(float calcValue)
    {
        if(meterDebuffPercentage >= 0) 
        {
            calcValue = Mathf.Abs(calcValue * meterDebuffPercentage);
        }
        fullMeterLevel += calcValue;
        if (fullMeterLevel >= 120)
        {
            fullMeterLevel = 120;
        }
        SetMeterValue();
    }
    #endregion

    #region Subtract Meter Value
    void DecreaseMeterValue(float calcValue)
    {
        float subtractedValue = (calcValue * meterMaxThreshold);
        fullMeterLevel -= subtractedValue;
        SetMeterValue();
    }
    void SetMeterValue() 
    {
        meterTier = 0;
        superMeter.currentValue = 0;
        for (int i = 0; i <= fullMeterLevel; i++)
        {
            superMeter.currentValue += 1;
            if (superMeter.currentValue >= 30)
            {
                meterTier += 1;
                if (meterTier < 4)
                {
                    superMeter.currentValue = 0;
                }
                else 
                {
                    meterTier = 4;
                }
            }
        }
        SetMeterTierText();
        superMeter.SetCurrentMeterValue(superMeter.currentValue);
    }
    public void DecreaseMeterAmount(int amount)
    {
        if ((superMeter.currentValue - amount <= 0) 
            && fullMeterLevel > 0
            && meterTier > 0)
        {
            meterTier -= 1;
            SetMeterTierText();
            superMeter.currentValue += meterMaxThreshold;
        }
        superMeter.currentValue -= amount;
        fullMeterLevel -= amount;
        superMeter.SetCurrentMeterValue(superMeter.currentValue);
    }
    #endregion

    void SetMeterTierText()
    {
        string meterTierValue = SpriteToTextColorUtility.AppendSpriteName($"{meterTier}", Color.white);
        meterTierText.text = meterTierValue;
    }

    public bool CanTakeFromMeter(float meterRequirement) 
    {
        float _requiredMeter = meterRequirement * 30;
        return fullMeterLevel >= _requiredMeter;
    }
    public void AddMeter(float _scaledMeterGain)
    {
        IncreaseMeterValue(_scaledMeterGain);
    }
    public void TakeMeter(float _rawMeterValue)
    {
        DecreaseMeterValue(_rawMeterValue);
    }
}
