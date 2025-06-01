using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class VersusMenu_DisplayController : MonoBehaviour
{
    [SerializeField] private BorderObject_Transform UpperBorderObject;
    [SerializeField] private BorderObject_Transform BottomBorderObject;

    [SerializeField] private VersusMenu_DisplayObject _leftPlayerDisplay;
    [SerializeField] private VersusMenu_DisplayObject _rightPlayerDisplay;

    [SerializeField] private Image VersusImage;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T)) 
        {
            CloseAndDisplayPlayerData();
        }
    }
    public void CloseAndDisplayPlayerData() 
    {
        VersusImage.DOFade(0, 0);
        UpperBorderObject.BorderObject.DOLocalMoveY(UpperBorderObject._endYPosition, 0.15f).SetEase(Ease.InOutElastic).OnComplete(() => 
        {
            _leftPlayerDisplay.DisplayChosenPlayerData();
        });
        BottomBorderObject.BorderObject.DOLocalMoveY(BottomBorderObject._endYPosition, 0.15f).SetEase(Ease.InOutElastic).OnComplete(() =>
        {
            _rightPlayerDisplay.DisplayChosenPlayerData();
        });
    }
    public void DisplayVersusImage() 
    {
        VersusImage.transform.DOScale(1.35f, 0);
        VersusImage.transform.DOScale(1f, 0.35f);
        VersusImage.DOFade(1, 0.25f);
    }
}

[Serializable]
public class BorderObject_Transform 
{
    public Transform BorderObject;
    public float _endYPosition;
}