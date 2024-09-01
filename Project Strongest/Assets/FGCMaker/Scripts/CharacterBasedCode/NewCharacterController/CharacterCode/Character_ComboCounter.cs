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
    public int CurrentHitCount { get {return _currentComboCount; } set { _currentComboCount = value; } }

    [SerializeField] private TMP_Text _counterText,_QualityText;
    [SerializeField] private Color32 _counterColor, _QualityColor;
    [Range(0,255f)] public float alphaLevel;
    [SerializeField] Transform comboHolder;
    Tween fadeTextTween;
    [SerializeField] private Dictionary<int, Callback> _comboProficiencyDictionary;
    bool canFade;
    Sequence fadeTextOut;
    IEnumerator fadeTextRoutine;
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
        _comboProficiencyDictionary = new Dictionary<int, Callback>();

        _comboProficiencyDictionary.Add(5, () => UpdateQualityText("Basic"));
        _comboProficiencyDictionary.Add(10, () => UpdateQualityText("Advanced"));
        _comboProficiencyDictionary.Add(15, () => UpdateQualityText("Amazing"));
        _comboProficiencyDictionary.Add(20, () => UpdateQualityText("Superior"));
        _comboProficiencyDictionary.Add(25, () => UpdateQualityText("Fantastic"));
        _comboProficiencyDictionary.Add(30, () => UpdateQualityText("Unbelievable"));
        _comboProficiencyDictionary.Add(35, () => UpdateQualityText("Unreal"));
        _comboProficiencyDictionary.Add(45, () => UpdateQualityText("Maximum"));
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
        canFade = false;
        UpdateText();

        DOTween.Complete(comboHolder);
        comboHolder.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        comboHolder.DOScale(new Vector3(1f, 1f, 1f), 0.65f);
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
        canFade = true;
        ResetComboCounter();
        fadeTextOut = DOTween.Sequence();
        fadeTextOut.Append(_counterText.DOFade(0, .75f));
        _QualityText.DOFade(0, .45f);
        fadeTextOut.OnComplete(() =>
        {
            canFade = false;
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
        for (int i = _comboProficiencyDictionary.Keys.Count -1; i >= 0; i--) 
        {
            int keyRef = _comboProficiencyDictionary.Keys.ElementAt(i);
            if (_currentComboCount >= keyRef) 
            {
                _comboProficiencyDictionary[keyRef]();
            }
            continue;
        }
        yield return new WaitForSeconds(1.75f);
        FadeOutText();
        canFade = true;
        #endregion
    }
}