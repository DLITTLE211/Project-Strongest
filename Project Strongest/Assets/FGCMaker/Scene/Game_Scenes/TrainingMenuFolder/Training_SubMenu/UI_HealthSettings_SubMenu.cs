using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UI_HealthSettings_SubMenu : UI_SubMenuBase
{
    [Space(20)]
    public UI_ToggleableElement P1HealthSlider;
    public UI_ToggleableElement P1StunSlider;
    [Space(20)]
    public UI_ToggleableElement P2HealthSlider;
    public UI_ToggleableElement P2StunSlider;

    public float ReturnP1HealthValue() 
    {
        return P1HealthSlider._elementSlider.value;
    }
    public float ReturnP1StunValue()
    {
        return P1StunSlider._elementSlider.value;
    }
    public float ReturnP2HealthValue()
    {
        return P2HealthSlider._elementSlider.value;
    }
    public float ReturnP2StunValue()
    {
        return P2StunSlider._elementSlider.value;
    }
}
