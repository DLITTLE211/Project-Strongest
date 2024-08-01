using System;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using DG.Tweening;
using UnityEngine;

public class Character_ComboCounter : MonoBehaviour
{
    [SerializeField] private int _currentComboCount;
    public int CurrentHitCount { get {return _currentComboCount; } set { _currentComboCount = value; } }

    [SerializeField] private TMP_Text _counterText,_QualityText;
    [SerializeField] private Color32 _counterColor, _QualityColor;
    [Range(0,255f)] public float alphaLevel;
    [SerializeField] Transform comboHolder;
    bool canFade;
    public void SetStartComboCounter()
    {
        SetStartColors();
        CurrentHitCount = 0;
        UpdateQualityText();
        UpdateText();
    }
    void SetStartColors()
    {
        alphaLevel = 255f;
    }
    public void ResetComboCounter() 
    {
        SetStartComboCounter();
    }
    public void OnHit_CountUp() 
    {
        CurrentHitCount++;
        canFade = false;
        SetStartColors();
        UpdateText();

        DOTween.Complete(comboHolder);
        comboHolder.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        comboHolder.DOScale(new Vector3(1f, 1f, 1f), 0.65f);
    }
    public int ReturnCurrentComboCount() 
    {
        return _currentComboCount;
    }

    #region UpdateComboCounter
    void UpdateText() 
    {
        string hitCount = CurrentHitCount <= 1 ? "HIT" : "HITS";
        if (CurrentHitCount == 0)
        {
            _counterText.text = "";
        }
        else
        {
            if (_counterText.color.a != 255)

            { _counterText.DOFade(1, 0.05f).OnStart(() => { _counterText.text = $"{CurrentHitCount} {hitCount}"; });}
           
            else 
            {_counterText.text = $"{CurrentHitCount} {hitCount}"; }
        }
        
    }
    void UpdateQualityText(string qualityType = "")
    {
        if (CurrentHitCount == 0)
        {
            _QualityText.text = "";
        }
        else
        {
            if (_counterText.color.a != 255)
            {
                _QualityText.DOFade(1, 0.05f).OnStart(() => { _QualityText.text = $"{qualityType} Combo!!"; });
            }

            else 
            { _QualityText.text = $"{qualityType} Combo!!"; }
        }
    }
    #endregion

    private void Update()
    {
        SetColorNumbers();
        if (canFade)
        {
            FadeToClear();
        }

    }
    void SetColorNumbers() 
    {
        _counterText.color = _counterColor;
        _QualityText.color = _QualityColor;
        _counterColor.a = (byte)alphaLevel;
        _QualityColor.a = (byte)alphaLevel;
    }
    void FadeToClear() 
    {
        if (alphaLevel <= 0)
        {
            alphaLevel = 0;
            canFade = false;
            ResetComboCounter();
            UpdateQualityText();
        }
        else 
        {
            alphaLevel -= 3.75f;
        }
    }

    public void OnEndCombo()
    {
        OnEndComboCount();
    }
    void OnEndComboCount() 
    {
        #region Check ComboQualityOnQuickEnd
        if (CurrentHitCount >= 45)
        {
            UpdateQualityText("Maximum");
        }
        else if (CurrentHitCount >= 30)
        {
            UpdateQualityText("Unbelievable");
        }
        else if (CurrentHitCount >= 25)
        {
            UpdateQualityText("Fantastic");
        }
        else if (CurrentHitCount >= 20)
        {
            UpdateQualityText("Superior");
        }
        else if (CurrentHitCount >= 15)
        {
            UpdateQualityText("Amazing");
        }
        else if (CurrentHitCount >= 10)
        {
            UpdateQualityText("Advanced");
        }
        else if (CurrentHitCount >= 5)
        {
            UpdateQualityText("Basic");
        }
        #endregion
        canFade = true;
    }
}
