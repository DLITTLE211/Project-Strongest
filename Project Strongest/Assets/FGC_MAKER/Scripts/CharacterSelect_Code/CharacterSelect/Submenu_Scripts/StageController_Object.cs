using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using DG.Tweening;

public class StageController_Object : MonoBehaviour
{
    [SerializeField] private Stage_StageAsset _chosenStage;
    [SerializeField] private RectTransform _objectTransform;
    [SerializeField] private Image _stageImage;
    [SerializeField] private TMP_Text _stageName;
    [SerializeField] private bool ToggleReady;
    public int CurrentStageLocationIndex;

    Tween moveTween;
    public void SetToggleState(bool state) 
    {
        ToggleReady = state; 
    }
    public void UpdateStageData(Stage_StageAsset _currentStage) 
    {
        _chosenStage = _currentStage;
        _stageImage.sprite = _currentStage.stageImage;
        _stageName.text = SpriteToTextColorUtility.AppendSpriteName(_currentStage.stageName.ToUpper(),_stageName.color);
    }
    public Stage_StageAsset ReturnCurrentStage() 
    {
        return _chosenStage;
    }
    public void SetLocationIndex(int index) 
    {
        CurrentStageLocationIndex = index;
    }
    public void Move(Vector3 location,int index ,float speed, out bool moved) 
    {
        if (!ToggleReady) 
        {
            moved = false;
            return;
        }
        if(moveTween != null) 
        {
            moveTween.Complete();
            ToggleReady = true;
            moveTween = null;
        }
       moved = true;
       ToggleReady = false;
       moveTween = _objectTransform.DOLocalMove(location, speed).SetEase(Ease.OutBack).OnComplete(() =>
       {
           SetLocationIndex(index);
           ToggleReady = true;
       });
        moveTween.Play();
    }
    public void MoveInstant(Vector3 addedLocation, Vector3 location, int index, float speed)
    {
        if (!ToggleReady)
        {
            return;
        }
        if (moveTween != null)
        {
            moveTween.Complete();
            moveTween = null;
        }
        moveTween = _objectTransform.DOLocalMove(addedLocation, 0f);
        moveTween.OnComplete(() =>
        {
            ToggleReady = false;
            moveTween = _objectTransform.DOLocalMove(location, speed).SetEase(Ease.OutBack).OnComplete(() =>
            {
                SetLocationIndex(index);
                ToggleReady = true;
            });
            moveTween.Play();
        });
    }
}
