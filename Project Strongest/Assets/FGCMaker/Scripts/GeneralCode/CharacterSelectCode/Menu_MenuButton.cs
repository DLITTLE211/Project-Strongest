using System.Collections;
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
        float moveToPos = NavigationButton.transform.localPosition.x + 50f;
        NavigationButton.DOMoveX(moveToPos, 0.35f);
        NavigationButton.DOScale(1.1f, 0.35f);
        backgroundImage.DOColor(backgroundHighligtedColor, 0.35f);
    }
    public void Lower()
    {
        NavigationButton.DOLocalMoveX(startingPos.x, 0.35f);
        NavigationButton.DOScale(1, 0.35f);
    }

}
[CustomEditor(typeof(Menu_MenuButton))]
public class UIButtonEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
    }
}
[Serializable]
public class MenuButtonHolder
{
    public GameObject buttonHolder;
    public List<Menu_MenuButton> buttonList;
    public void SetImageObject(Image _image) 
    {
        for (int i = 0; i < buttonList.Count; i++) 
        {
            buttonList[i].backgroundImage = _image;
        }
    }
}