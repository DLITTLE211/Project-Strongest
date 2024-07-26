using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Character_SuperMeter : MonoBehaviour
{
    public MainMeterController superMeter;
    public Character_Health controller;
    public List<Amplifiers> curAmplifier;

    public TMP_Text meterTierText;

    public float meterTier;
    public float fullMeterLevel;
    public const float maxMeterLevel = 90;
    public const float meterMaxThreshold = 30;
    // Start is called before the first frame update
    void Start()
    {
        SetStartValue();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            IncreaseMeterValue(10);
        }
        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            IncreaseMeterValue(20);
        }
        if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            IncreaseMeterValue(30);
        }
    }
    void SetStartValue()
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
    bool checkPossibleTier(float calcValue)
    {
        return superMeter.currentValue + calcValue >= meterMaxThreshold;
    }
    bool checkMaxTier(float calcValue)
    {
        return fullMeterLevel + calcValue >= maxMeterLevel;
    }
    void IncreaseMeterValue(float calcValue)
    {
        if (!checkMaxTier(calcValue))
        {
            if (checkPossibleTier(calcValue))
            {
                meterTier += 1;
                SetMeterTierText();
                superMeter.currentValue = 0;
                superMeter.SetCurrentMeterValue(superMeter.currentValue);
                fullMeterLevel += calcValue;
            }
            else
            {
                superMeter.currentValue += calcValue;
                fullMeterLevel += calcValue;
                superMeter.SetCurrentMeterValue(superMeter.currentValue);
            }
        }
        else
        {
            meterTier = 3;
            SetMeterTierText();
            superMeter.currentValue = meterMaxThreshold;
            fullMeterLevel = maxMeterLevel;
            superMeter.SetCurrentMeterValue(superMeter.currentValue);
        }
    }
    #endregion

    #region Subtract Meter Value
    bool checkLowerPossibleTier(float calcValue)
    {
        return superMeter.currentValue - (calcValue * meterMaxThreshold) <= 0;
    }
    bool checkMinTier(float calcValue)
    {
        return fullMeterLevel - (calcValue * meterMaxThreshold) <= 0;
    }
    void DecreaseMeterValue(float calcValue)
    {
        if (!checkMinTier(calcValue))
        {
            if (checkLowerPossibleTier(calcValue))
            {
                meterTier -= 1;
                SetMeterTierText();
                superMeter.currentValue -= (calcValue * meterMaxThreshold);
                superMeter.currentValue += meterMaxThreshold;
                superMeter.SetCurrentMeterValue(superMeter.currentValue);
                fullMeterLevel -= (calcValue * meterMaxThreshold);
            }
            else
            {
                superMeter.currentValue -= (calcValue * meterMaxThreshold);
                fullMeterLevel -= (calcValue * meterMaxThreshold);
                superMeter.SetCurrentMeterValue(superMeter.currentValue);
            }
        }
        else
        {
            meterTier = 0;
            SetMeterTierText();
            superMeter.currentValue = 0;
            fullMeterLevel = 0;
            superMeter.SetCurrentMeterValue(superMeter.currentValue);
        }
    }
    #endregion

    void SetMeterTierText()
    {
        meterTierText.text = $"{meterTier}";
    }
    void AddAmplifier(Amplifiers amp)
    {
        curAmplifier.Add(amp);
    }

    public bool CanTakeFromMeter(float meterRequirement) 
    {
        return meterRequirement > fullMeterLevel;
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
