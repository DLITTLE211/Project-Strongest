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
    [SerializeField] private GameObject _roundSettingsObject;
    [SerializeField] private TMP_Text _roundSettingsText;
    [Range(1,5),SerializeField] private int winningRoundCount;
    public Stage_StageAsset _stageAsset;
    public bool stageSelectActive;
    List<Stage_StageAsset> totalStages;
    private int _currentStage;
    public bool allowStageSelect;
    public void ActivateStateSeector(GameMode _curGameMode)
    {
        _mainHolder.SetActive(true);
        if (_curGameMode == GameMode.Training)
        {
            _roundSettingsObject.SetActive(false);
        }
        else if (_curGameMode != GameMode.Title) 
        {
            _roundSettingsObject.SetActive(true);
            winningRoundCount = 2;
            SetRoundCount();
        }
    }
    public void SetRoundCount() 
    {
        string roundText = winningRoundCount == 1 ? "Round" : "Rounds";
        _roundSettingsText.text = $"Best Of {winningRoundCount} {roundText}";
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
    public void UpdateRoundCountDown()
    {
        if (winningRoundCount < 1)
        {
            winningRoundCount = 1;
        }
        else { winningRoundCount--; }
        SetRoundCount();
    }
    public void UpdateRoundCountUp()
    {
        if (winningRoundCount > 5)
        {
            winningRoundCount = 5;
        }
        else { winningRoundCount++; }
        SetRoundCount();
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
