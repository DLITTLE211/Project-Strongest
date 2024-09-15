using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using DG.Tweening;
using UnityEngine;

public class Character_ComboCounter : MonoBehaviour
{
    [SerializeField] private int _currentComboCount;
    [SerializeField] private TMP_Text _counterText, _QualityText;
    [SerializeField] Transform comboHolder;
    [SerializeField] private List<ComboProficiencyLevel> _comboProficiencyList;
    Tween fadeTextTween;
    Sequence fadeTextOut;
    IEnumerator fadeTextRoutine;
    public int CurrentHitCount
    {
        get { return _currentComboCount; }
        set { _currentComboCount = value; }
    }
    public int ReturnCurrentComboCount()
    {
        return _currentComboCount;
    }
    private void Start()
    {
        SetComboProficiencyDictionary();
    }
    void SetComboProficiencyDictionary() 
    {
        _comboProficiencyList = new List<ComboProficiencyLevel>();
        #region Define Combo Levels
        ComboProficiencyLevel basicLevel = new ComboProficiencyLevel(3, "Basic");
        ComboProficiencyLevel advancedLevel = new ComboProficiencyLevel(6, "Advanced");
        ComboProficiencyLevel amazingLevel = new ComboProficiencyLevel(9, "Amazing");
        ComboProficiencyLevel superiorLevel = new ComboProficiencyLevel(12, "Superior");
        ComboProficiencyLevel fantasticLevel = new ComboProficiencyLevel(15, "Fantastic");
        ComboProficiencyLevel unbelievableLevel = new ComboProficiencyLevel(20, "Unbelievable");
        ComboProficiencyLevel unrealLevel = new ComboProficiencyLevel(25, "Unreal");
        ComboProficiencyLevel maximumLevel = new ComboProficiencyLevel(30, "Maximum");
        #endregion

        #region Add Levels to List
        _comboProficiencyList.Add(basicLevel);
        _comboProficiencyList.Add(advancedLevel);
        _comboProficiencyList.Add(amazingLevel);
        _comboProficiencyList.Add(superiorLevel);
        _comboProficiencyList.Add(fantasticLevel);
        _comboProficiencyList.Add(unbelievableLevel);
        _comboProficiencyList.Add(unrealLevel);
        _comboProficiencyList.Add(maximumLevel);
        #endregion

        fadeTextOut = null;
        fadeTextRoutine = null;
    }

    public void SetStartComboCounter()
    {
        UpdateQualityText();
        UpdateText();
    }
    public void ResetComboCounter()
    {
        CurrentHitCount = 0; 
    }
    public void OnHit_CountUp() 
    {
        fadeTextTween.Kill();
        fadeTextTween = null;
        CurrentHitCount++;
        UpdateText();

        DOTween.Complete(comboHolder);
        comboHolder.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        comboHolder.DOScale(new Vector3(1f, 1f, 1f), 0.65f);
    }

    #region UpdateComboCounter
    void UpdateText() 
    {
        StopFadeRoutine();
        string hitCount = CurrentHitCount <= 1 ? "HIT" : "HITS";
        if (CurrentHitCount == 0)
        {
            _counterText.text = "";
        }
        else
        {
            if (_counterText.color.a != 255)

            {
                _counterText.DOFade(1, 0.05f).OnStart(() =>
                {
                    _counterText.text = $"{CurrentHitCount} {hitCount}";
                });
            }
            else
            {
                _counterText.text = $"{CurrentHitCount} {hitCount}";
            }
        }
    }
    void UpdateQualityText(string qualityType = "")
    {
        if (qualityType == "")
        {
            _QualityText.text = qualityType;
            if (fadeTextOut != null)
            {
                fadeTextOut = null;
            }
            return;
        }
        else
        {
            if (_counterText.color.a != 255)
            {
                _QualityText.DOFade(1, 0.05f).OnStart(() => { _QualityText.text = $"{qualityType} Combo!!"; });
            }

            else 
            { 
                _QualityText.text = $"{qualityType} Combo!!"; 
            }
        }
    }
    #endregion
    
    void FadeOutText()
    {
        ResetComboCounter();
        fadeTextOut = DOTween.Sequence();
        fadeTextOut.Append(_counterText.DOFade(0, .75f));
        _QualityText.DOFade(0, .45f);
        fadeTextOut.OnComplete(() =>
        {
            SetStartComboCounter();
            fadeTextRoutine = null;
        });
    }
    public void StopFadeRoutine() 
    {
        if (fadeTextOut != null) 
        {
            fadeTextOut.Kill();
            fadeTextOut = null;
        }
        if (fadeTextRoutine != null)
        {
            StopCoroutine(fadeTextRoutine);
            fadeTextRoutine = null;
        }
    }

    public void OnEndCombo()
    {
        if (fadeTextRoutine != null)
        {
            fadeTextRoutine = null;
        }
        fadeTextRoutine = OnEndComboCount();
        StartCoroutine(fadeTextRoutine);
    }
    IEnumerator OnEndComboCount() 
    {
        #region Check ComboQualityOnQuickEnd
        for (int i = _comboProficiencyList.Count -1; i >= 0; i--) 
        {
            if (_currentComboCount >= _comboProficiencyList[i].comboLevel) 
            {
                UpdateQualityText(_comboProficiencyList[i].comboLevelSubText);
                break;
            }
            continue;
        }
        yield return new WaitForSeconds(0.75f);
        FadeOutText();
        #endregion
    }
}

[Serializable]
public class ComboProficiencyLevel 
{
    public int comboLevel;
    public string comboLevelSubText;

    public ComboProficiencyLevel(int _level, string _text) 
    {
        comboLevel = _level;
        comboLevelSubText = _text;
    }
}