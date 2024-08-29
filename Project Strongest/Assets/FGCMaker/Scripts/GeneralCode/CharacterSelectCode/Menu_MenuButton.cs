using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEditor;

[Serializable]
public class Menu_MenuButton : Button, ISelectHandler
{
    public GameObject NavigationButton;
    public override void OnSelect(BaseEventData eventData)
    {
        Raise();
    }
    public void Raise()
    {
        NavigationButton.transform.DOScale(1.15f, 0.75f);
    }

}
[CustomEditor(typeof(Menu_MenuButton))]
public class UIButtonEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Menu_MenuButton t = (Menu_MenuButton)target;
        base.OnInspectorGUI();
    }
}
[Serializable]
public class MenuButtonHolder
{
    public GameObject buttonHolder;
    public List<Menu_MenuButton> buttonList;
}