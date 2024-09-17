using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UI_SubMenuBase : MonoBehaviour
{
    [Header("Objects for Menu Visual Controls")]
    public List<Image> menuImageList;
    public List<UI_ToggleableElement> toggleableElements;
    public MenuType _menuType;
    public void EnableMenu(Callback<object> func) 
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
        for(int i = 0; i < menuImageList.Count; i++) 
        {
            menuImageList[i].DOFade(value, 0.75f);
        }
        for (int i = 0; i < toggleableElements.Count; i++)
        {
            if (toggleableElements[i]._toggleStyle == ToggledElement.ButtonToggle)
            {
                toggleableElements[i]._leftButton.targetGraphic.DOFade(value, 0.75f);
                toggleableElements[i]._rightButton.targetGraphic.DOFade(value, 0.75f);
            }
            if(toggleableElements[i]._toggleStyle == ToggledElement.SliderToggle) 
            {
                toggleableElements[i]._elementSlider.targetGraphic.DOFade(value, 0.75f).OnComplete(() => 
                {
                    if (func != null)
                    {
                        func();
                    }
                });
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

