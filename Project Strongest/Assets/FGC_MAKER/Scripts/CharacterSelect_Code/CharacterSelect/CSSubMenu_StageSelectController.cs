using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class CSSubMenu_StageSelectController : CharacterSelect_SubMenuBase
{
    [Header("Stage Select Info")]
    [SerializeField] private List<Stage_StageAsset> _activeStages;
    [SerializeField] private List<StageController_Object> _stageImageObject;
    [SerializeField] private RectTransform _stageImagerHolder;    
    [SerializeField] private List<Vector3> stageImageLocations;
    [SerializeField] private int centerIndex;
    public Stage_StageAsset _chosenStage;

    [Space(15)]
    [Header("Round Select Info")]
    [SerializeField] private RoundSelectObject _roundSelectObject;
    [SerializeField] private int roundCount;
    private GameMode _mode;
    public bool stageSelected;
    public bool roundCountSelected;
    private void OnEnable()
    {
        objectHolder.SetActive(false);
        DeactivateSubMenu();
        GetStageImageLocalLocations();
        SetStartingStageData();
        _roundSelectObject.Deactivate();
    }
    private void GetStageImageLocalLocations() 
    {
        stageImageLocations = new List<Vector3>();
        for (int i = 0; i < _stageImageObject.Count; i++)
        {
            stageImageLocations.Add(_stageImageObject[i].transform.localPosition);
        }
    }
    public void ResetValues()
    {
        stageSelected = false;
        roundCountSelected = false;
    }
    public void Activate()
    {
        objectHolder.SetActive(true);
        ActivateSubMenu();
    }
    public void Deactivate()
    {
        DeactivateSubMenu();
    }
    protected override void ActivateSubMenu()
    {
        _mode = _characterSelect.currentSet.gameMode;
        stageSelected = false;
        roundCountSelected = false;
        _stageImagerHolder.DOLocalMoveX(0, 1.45f).SetEase(Ease.OutBack).OnComplete(() =>
        {
        });
        _topHeaderText.DOFade(1f, 0.15f);
        _bottomHeaderText.DOFade(1f, 0.15f).OnComplete(() =>
        {
            SetHeaderText(_topHeaderText, "Choose Your");
            SetHeaderText(_bottomHeaderText, "Stage");
        });
    }
    public void SetDefaultRoundCount() 
    {
        roundCount = 2;
        SetRoundCountText();
    }
    private void SetStartingStageData()
    {
        for (int i = 0; i < _stageImageObject.Count; i++)
        {
            _stageImageObject[i].UpdateStageData(_activeStages[i]);
            _stageImageObject[i].SetLocationIndex(i);
        }
        centerIndex = 2;
    }
    protected override void DeactivateSubMenu()
    {
        stageSelected = false;
        roundCountSelected = false;
        _stageImagerHolder.DOLocalMoveX(5000, 1.45f);
        _topHeaderText.DOFade(0f, 0.15f);
        _bottomHeaderText.DOFade(0f, 0.15f);
    }

    public void ActivateRoundSelectObject() 
    {
        _roundSelectObject.Activate(SetDefaultRoundCount); 
    }
    public void DeactivateRoundSelectObject()
    {
        _roundSelectObject.Deactivate();
    }
    // Update is called once per frame
    void Update()
    {
        if (allowBase)
        {
            base.OnUpdate();
        }
    }
    #region Toggle Functions
    public void ToggleLeft()
    {
        if (!roundCountSelected)
        {
            if (!stageSelected)
            {
                CycleMenuLeft();
                return;
            }
            IncreaseCount();
        }
    }
    public void ToggleRight()
    {
        if (!roundCountSelected)
        {
            if (!stageSelected)
            {
                CycleMenuRight();
                return;
            }
            DecreaseCount();
        }
    }
    public Stage_StageAsset GetChosenStage()
    {
        if (_chosenStage.stageName == "Random")
        {
            for (int i = 0; i < _activeStages.Count; i++)
            {
                if (_activeStages[i].stageName == "Random")
                {
                    _activeStages.RemoveAt(i);
                    break;
                }
                continue;
            }
            _chosenStage = _activeStages[UnityEngine.Random.Range(0, _activeStages.Count - 1)];
            return _chosenStage;
        }
        return _chosenStage;
    }
    #region Stage Cycle Functions
    private void CycleMenuLeft()
    {
        int locationIndex = 0;
        for (int i = 0; i < stageImageLocations.Count; i++)
        {
            locationIndex = _stageImageObject[i].CurrentStageLocationIndex - 1;
            float speed = 0.85f;
            Vector3 newLoc = Vector3.zero;
            if (locationIndex < 0)
            {
                locationIndex = stageImageLocations.Count - 1;
                newLoc = stageImageLocations[locationIndex];
                Vector3 newIndexPlus = new Vector3(stageImageLocations[locationIndex].x + 700, 0, 0);
                _stageImageObject[i].MoveInstant(newIndexPlus, newLoc, locationIndex, speed);
                continue;
            }
            newLoc = stageImageLocations[locationIndex];
            _stageImageObject[i].Move(newLoc, locationIndex, speed);
        }
        if (centerIndex >= stageImageLocations.Count - 1)
        {
            centerIndex = 0;
        }
        else
        {
            centerIndex++;
        }
        _chosenStage = _stageImageObject[centerIndex].ReturnCurrentStage();
    }
    private void CycleMenuRight()
    {
        float speed = 0.85f;
        int locationIndex = 0;
        for (int i = 0; i < stageImageLocations.Count; i++)
        {
            locationIndex = _stageImageObject[i].CurrentStageLocationIndex + 1;
            Vector3 newLoc = Vector3.zero;
            if (locationIndex >= stageImageLocations.Count)
            {
                locationIndex = 0;
                newLoc = stageImageLocations[locationIndex];
                Vector3 newIndexPlus = new Vector3(stageImageLocations[locationIndex].x - 700, 0, 0);
                _stageImageObject[i].MoveInstant(newIndexPlus, newLoc, locationIndex, speed);
                continue;
            }
            newLoc = stageImageLocations[locationIndex];
            _stageImageObject[i].Move(newLoc, locationIndex, speed);
        }
        if (centerIndex <= 0)
        {
            centerIndex = stageImageLocations.Count - 1;
        }
        else
        {
            centerIndex--;
        }
        _chosenStage = _stageImageObject[centerIndex].ReturnCurrentStage();
    }
    #endregion

    #region Round Count Functions
    public Round_Info GetRoundInfomation()
    {
        return new Round_Info(roundCount);
    }
    private void IncreaseCount() 
    {
        if (roundCount >= 5)
        {
            roundCount = 5;
        }
        else
        {
            roundCount++;
        }
        SetRoundCountText();
    }
    private void DecreaseCount() 
    {
        if (roundCount <= 1)
        {
            roundCount = 1;
        }
        else
        {
            roundCount--;
        }
        SetRoundCountText();
    }
    private void SetRoundCountText() 
    {
        string roundText = roundCount == 1 ? "Round" : "Rounds";
        string fullMessage = $"Best Of {roundCount}  {roundText}";
        _roundSelectObject.SetRoundText(fullMessage);
    }
    #endregion
    
    #endregion
}
