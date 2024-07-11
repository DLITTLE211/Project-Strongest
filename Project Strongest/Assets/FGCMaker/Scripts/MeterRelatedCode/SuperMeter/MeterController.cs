using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MeterController : MonoBehaviour
{
    public MainMeterController superMeter;
    public Character_Health controller;
    public List<Amplifiers> curAmplifier;
    public GameManager _gameManager;

    public TMP_Text meterTierText;

    public float meterTier;
    public float fullMeterLevel;
    public const float maxMeterLevel = 30;
    public const float meterMaxThreshold = 10;
    // Start is called before the first frame update
    void Start()
    {
        SetStartValue();
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
        return superMeter.currentValue - calcValue <= 0;
    }
    bool checkMinTier(float calcValue)
    {
        return fullMeterLevel - calcValue <= 0;
    }
    void DecreaseMeterValue(float calcValue)
    {
        if (!checkMinTier(calcValue))
        {
            if (checkLowerPossibleTier(calcValue))
            {
                meterTier -= 1;
                SetMeterTierText();
                superMeter.currentValue -= calcValue;
                superMeter.currentValue += 10;
                superMeter.SetCurrentMeterValue(superMeter.currentValue);
                fullMeterLevel -= calcValue;
            }
            else
            {
                superMeter.currentValue -= calcValue;
                fullMeterLevel -= calcValue;
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

    void AddMeter(float _rawMeterValue) 
    {
        IncreaseMeterValue(_rawMeterValue);
    }
    void TakeMeter(float _rawMeterValue)
    {
        DecreaseMeterValue(_rawMeterValue);
    }
    void Update()
    {
        
    }
    void TestCalculation(KeyCode code)
    {
        switch (code) 
        {
            case KeyCode.Space:
                AddMeter(2);
                break;
            case KeyCode.Keypad5:
                TakeMeter(3);
                break;
        }
    }
}
