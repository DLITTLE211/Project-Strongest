using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class CharacterSelect_StageSelect : MonoBehaviour
{
    [SerializeField] private GameObject _mainHolder;
    public GameObject MainHolder { get { return _mainHolder; } }
    [SerializeField] private Image _leftArrow;
    [SerializeField] private Image _rightArrow;
    [SerializeField] private Image _stageImageSlot;
    [SerializeField] private TMP_Text _stageNameText;
    public Stage_StageAsset _stageAsset;
    public bool stageSelectActive;
    List<Stage_StageAsset> totalStages;
    private int _currentStage;
    public bool allowStageSelect;
    public void ActivateStateSeector()
    {
        _mainHolder.SetActive(true);
    }
    public void SetArrowsLitState(List<Stage_StageAsset> _totalStages)
    {
        totalStages = _totalStages;
        if (totalStages.Count > 1)
        {
            _leftArrow.color = new Color(1, 1, 1, 0.75f);
            _rightArrow.color = new Color(1, 1, 1, 0.75f);
        }
        else
        {
            _leftArrow.color = new Color(1, 1, 1, 1);
            _rightArrow.color = new Color(1, 1, 1, 1);
        }
        stageSelectActive = true;
        _currentStage = 0;
        SetActiveStage(totalStages[_currentStage]);
        allowStageSelect = true;
    }
    public void ClearStageSelect()
    {
        allowStageSelect = false;
        _stageAsset = null;
        _mainHolder.SetActive(false);
        stageSelectActive = false;
    }
    public void SetStageSelect(bool state)
    {
        _mainHolder.SetActive(state);
        stageSelectActive = state;
    }
    public void UpdateInfoDown()
    {
        if (_currentStage <= 0)
        {
            _currentStage = totalStages.Count - 1;
        }
        else { _currentStage--; }
        SetActiveStage(totalStages[_currentStage]);
    }
    public void UpdateInfoUp()
    {
        if (_currentStage >= totalStages.Count - 1)
        {
            _currentStage = 0;
        }
        else { _currentStage++; }
        SetActiveStage(totalStages[_currentStage]);
    }
    public void SetActiveStage(Stage_StageAsset chosenStage) 
    {
        _stageAsset = chosenStage;
        _stageImageSlot.sprite = chosenStage.stageImage;
        _stageNameText.text = $"{chosenStage.stageName}"; 
    }
}
