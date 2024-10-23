using TMPro;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEditor;

[Serializable]
public class Menu_MenuButton : Button
{
    public Image buttonBackgroundImage, buttonImage;
    public TMP_Text buttonNameText;
    public Color32 backgroundHighligtedColor;
    public Transform NavigationButton;
    private Vector3 startingPos;
    public Image backgroundImage;
    public override void OnSelect(BaseEventData eventData)
    {
        startingPos = NavigationButton.localPosition;
        Raise();
    }
    public override void OnDeselect(BaseEventData eventData)
    {
        Lower();
    }
    public void Raise()
    {
        float moveToPos = NavigationButton.transform.localPosition.x + 95f;
        NavigationButton.DOMoveX(moveToPos, 0.35f);
        NavigationButton.DOScale(1.1f, 0.35f);
        backgroundImage.DOColor(backgroundHighligtedColor, 0.35f);
    }
    public void Lower()
    {
        NavigationButton.DOLocalMoveX(startingPos.x, 0.35f);
        NavigationButton.DOScale(1, 0.35f);
    }
    public void Fade(float valuePoint, float _time, bool _interactable) 
    {
        interactable = _interactable;
        buttonBackgroundImage.DOFade(valuePoint, _time);
        buttonImage.DOFade(valuePoint, _time);
        buttonNameText.DOFade(valuePoint, _time);
    }
}
#if UNITY_EDITOR
[CustomEditor(typeof(Menu_MenuButton))]
public class UIButtonEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
    }
}
#endif
[Serializable]
public class MenuButtonHolder
{
    public Transform buttonHolder;
    public List<Menu_MenuButton> buttonList;
    public void SetImageObject(Image _image) 
    {
        for (int i = 0; i < buttonList.Count; i++) 
        {
            if (buttonList[i].backgroundImage == null)
            {
                buttonList[i].backgroundImage = _image;
            }
            continue;
        }
    }
    public void SlideHolderOut() 
    {
        float slidePos = buttonHolder.localPosition.x - 1750f;
        buttonHolder.DOLocalMoveX(slidePos, 1.5f);
    }
    public void SlideHolderIn(Callback SetFirstActiveButton)
    {
        DisableButtons(0f);
        float slidePos = buttonHolder.localPosition.x + 1750f;
        buttonHolder.DOLocalMoveX(slidePos, 1.5f).SetEase(Ease.InOutBack).OnComplete(() =>
        {
            EnableButtons();
            SetFirstActiveButton();
        });
    }
    public void DisableButtons(float time = 1.5f)
    {
        for (int i = 0; i < buttonList.Count; i++)
        {
            buttonList[i].Fade(0, time, false);
        }
    }
    public void EnableButtons(float time = 1.5f)
    {
        for (int i = 0; i < buttonList.Count; i++)
        {
            buttonList[i].Fade(255f, time,true);
        }
        
    }
}