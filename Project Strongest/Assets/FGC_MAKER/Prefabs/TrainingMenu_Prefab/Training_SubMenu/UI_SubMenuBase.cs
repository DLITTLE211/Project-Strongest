using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class UI_SubMenuBase : MonoBehaviour
{
    [Header("Objects for Menu Visual Controls")]
    public List<Image> menuImageList;
    public List<UI_ToggleableElement> toggleableElements;
    public MenuType _menuType;

    internal int meterRecoveryLength;
    private void Start()
    {
    }
    
    public void EnableMenu(Callback<GameObject> func) 
    {
        FadeElements(1);
        func(toggleableElements[0].GetActiveObject()[0]);
    }
    public void DisableMenu(Callback func) 
    {
        FadeElements(0,func);
    }
    private void FadeElements(float value, Callback func = null) 
    {
        ActivateMenuItems(value, func);
        for (int i = 0; i < menuImageList.Count; i++) 
        {
            menuImageList[i].DOFade(value, 0.75f);
        }
    }
    private void ActivateMenuItems(float value,Callback func = null) 
    {
        for (int i = 0; i < toggleableElements.Count; i++)
        {
            if (value == 0)
            {
                toggleableElements[i].gameObject.SetActive(false);
                if (func != null)
                {
                    func();
                }
            }
            else
            {
                toggleableElements[i].gameObject.SetActive(true);
            }
        }
    }
}

public enum MenuType 
{
    HealthToggle =0,
    MeterToggle = 1,
    DummyController =2,
    InfoDisplay = 3,
    MoveListDisplay = 4,

}

