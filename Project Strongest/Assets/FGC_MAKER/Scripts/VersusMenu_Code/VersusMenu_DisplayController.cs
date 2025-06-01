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

    private void Start()
    {
        DontDestroyOnLoad(this);
        VersusImage.DOFade(0, 0);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T)) 
        {
            CloseAndDisplayPlayerData();
        }
    }
    public int index;
    public void CloseAndDisplayPlayerData()
    {
        VersusImage.DOFade(0, 0);

        UpperBorderObject.BorderObject.DOLocalMoveY(UpperBorderObject._endYPosition, 1.15f).SetEase(Ease.InBack).OnComplete(() => 
        {
            _leftPlayerDisplay.DisplayChosenPlayerData();
        });
        BottomBorderObject.BorderObject.DOLocalMoveY(BottomBorderObject._endYPosition, 1.15f).SetEase(Ease.InBack).OnComplete(() =>
        {
            _rightPlayerDisplay.DisplayChosenPlayerData(DisplayVersusImage);
        });
    }
    public void DisplayVersusImage() 
    {
        VersusImage.transform.DOScale(1.35f, 0);
        VersusImage.transform.DOScale(1f, 0.35f);
        VersusImage.DOFade(1, 0.25f);
    }
    public void OpenDisplay() 
    {
        UpperBorderObject.BorderObject.DOLocalMoveY(UpperBorderObject._startYPosition, 0f);
        BottomBorderObject.BorderObject.DOLocalMoveY(BottomBorderObject._startYPosition, 0f);
        _leftPlayerDisplay.OpenVersusSide();
        _rightPlayerDisplay.OpenVersusSide();
    }
}

[Serializable]
public class BorderObject_Transform 
{
    public Transform BorderObject;
    public float _startYPosition;
    public float _endYPosition;
}