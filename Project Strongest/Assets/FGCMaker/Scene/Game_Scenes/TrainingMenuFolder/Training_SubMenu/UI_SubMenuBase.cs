using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public abstract class UI_SubMenuBase : MonoBehaviour
{
    public List<Image> menuImageList;
    public List<UI_ToggleableElement> toggleableElements;

    public void EnableMenu() 
    {
        FadeElements(1);
    }
    public void DisableMenu() 
    {
        FadeElements(0);
    }
    private void FadeElements(float value) 
    {
        for(int i = 0; i < menuImageList.Count; i++) 
        {
            menuImageList[i].DOFade(value, 0.75f);
        }
       /* for (int i = 0; i < toggleableElements.Count; i++)
        {
            toggleableElements[i].menuButton.targetGraphic.DOFade(value, 0.75f);
        }*/
    }
}

