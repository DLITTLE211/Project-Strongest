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
    [Range(1,5)] public int winningRoundCount;
    public Stage_StageAsset _stageAsset;
    public bool stageSelectActive;
    List<Stage_StageAsset> totalStages;
    private int _currentStage;
    public bool allowStageSelect;
    public bool allowRoundSelect;
    GameMode _mode;
    public void ActivateRoundSelector(GameMode _curGameMode)
    {
        _mode = _curGameMode;
        if (_mode == GameMode.Training)
        {
            allowRoundSelect = false;
            _roundSettingsObject.SetActive(false);
            _mainHolder.SetActive(true);
        }
        else if (_mode == GameMode.Versus)
        {
            allowRoundSelect = true;
            _mainHolder.SetActive(false);
            _roundSettingsObject.SetActive(true);
            winningRoundCount = 2;
            SetRoundCount();
        }
    }
    public void ActivateStageSelectObject()
    {
        allowRoundSelect = false;
        _roundSettingsObject.SetActive(false);
        _mainHolder.SetActive(true);
        allowStageSelect = true;
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
        if (_mode == GameMode.Versus)
        {
            allowRoundSelect = true;
        }
        else 
        {
            allowStageSelect = true;
        }
    }
    public void ClearStageSelect()
    {
        allowRoundSelect = false;
        _stageAsset = null;
        _roundSettingsObject.SetActive(false);
        _mainHolder.SetActive(false);
        stageSelectActive = false;
    }
    public void SetStageSelect(bool state)
    {
        _mainHolder.SetActive(state);
        stageSelectActive = state;
    }
    public void ToggleUp() 
    {
        if (allowRoundSelect) 
        {
            UpdateRoundCountUp();
        }
        if (allowStageSelect) 
        {
            UpdateInfoUp();
        }
    }
    public void ToggleDown()
    {
        if (allowRoundSelect)
        {
            UpdateRoundCountDown();
        }
        if (allowStageSelect)
        {
            UpdateInfoDown();
        }
    }

    #region Update Info Functions
    void UpdateRoundCountDown()
    {
        if (winningRoundCount <= 1)
        {
            winningRoundCount = 1;
        }
        else 
        { 
            winningRoundCount--; 
        }
        SetRoundCount();
    }

    void UpdateRoundCountUp()
    {
        if (winningRoundCount >= 5)
        {
            winningRoundCount = 5;
        }
        else 
        { 
            winningRoundCount++; 
        }
        SetRoundCount();
    }
    void UpdateInfoDown()
    {
        if (_currentStage <= 0)
        {
            _currentStage = totalStages.Count - 1;
        }
        else { _currentStage--; }
        SetActiveStage(totalStages[_currentStage]);
    }
    void UpdateInfoUp()
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
    #endregion
}
