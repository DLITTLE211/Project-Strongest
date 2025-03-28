using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class CSSubMenu_StageSelectController : CharacterSelect_SubMenuBase
{
    [SerializeField] private List<Stage_StageAsset> _activeStages;
    [SerializeField] private Stage_StageAsset _chosenStage;
    [SerializeField] private List<StageController_Object> _stageImageObject;
    [SerializeField] private RectTransform _stageImagerHolder;    
    [SerializeField] private List<Vector3> stageImageLocations;
    [SerializeField] private int centerIndex;
    private void OnEnable()
    {
        DeactivateSubMenu();
        GetStageImageLocalLocations();
        SetStartingStageData();
    }
    private void GetStageImageLocalLocations() 
    {
        stageImageLocations = new List<Vector3>();
        for (int i = 0; i < _stageImageObject.Count; i++)
        {
            stageImageLocations.Add(_stageImageObject[i].transform.localPosition);
        }
    }
    protected override void ActivateSubMenu()
    {
        _stageImagerHolder.DOLocalMoveX(0, 1.45f).SetEase(Ease.OutBack).OnComplete(() => 
        {
            for (int i = 0; i < _stageImageObject.Count; i++)
            {
                _stageImageObject[i].SetToggleState(true);
            }
        });
    }
    protected override void DeactivateSubMenu()
    {
        for (int i = 0; i < stageImageLocations.Count; i++)
        {
            _stageImageObject[i].SetToggleState(false);
        }
        _stageImagerHolder.DOLocalMoveX(5000, 1.45f);
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            CycleMenuRight();
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            CycleMenuLeft();
        }
        if (allowBase)
        {
            base.OnUpdate();
        }
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
    public void CycleMenuLeft() 
    {
        bool moved = false;
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
                Vector3 newIndexPlus = new Vector3(stageImageLocations[locationIndex].x +700,0,0);
                _stageImageObject[i].MoveInstant(newIndexPlus, newLoc, locationIndex, speed);
                continue;
            }
             newLoc = stageImageLocations[locationIndex];
            _stageImageObject[i].Move(newLoc, locationIndex, speed,out moved);
        }
        if (moved)
        {
            if (centerIndex >= stageImageLocations.Count-1)
            {
                centerIndex = 0;
            }
            else
            {
                centerIndex++;
            }
        }
        _chosenStage = _stageImageObject[centerIndex].ReturnCurrentStage();
    }
    public void CycleMenuRight()
    {
        bool moved = false;
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
                _stageImageObject[i].MoveInstant(newIndexPlus, newLoc, locationIndex,speed);
                continue;
            }
             newLoc = stageImageLocations[locationIndex];
            _stageImageObject[i].Move(newLoc, locationIndex, speed, out moved);
        }
        if (moved)
        {
            if (centerIndex <= 0)
            {
                centerIndex = stageImageLocations.Count - 1;
            }
            else
            {
                centerIndex--;
            }
        }

        _chosenStage = _stageImageObject[centerIndex].ReturnCurrentStage();
    }
}
