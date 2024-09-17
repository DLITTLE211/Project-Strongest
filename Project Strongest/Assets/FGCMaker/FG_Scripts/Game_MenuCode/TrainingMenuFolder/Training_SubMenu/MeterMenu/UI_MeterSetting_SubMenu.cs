using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_MeterSetting_SubMenu : UI_SubMenuBase
{
    [Space(20)]
    public UI_ToggleableElement P1MeterSlider;
    public UI_ToggleableElement P1AmplifySlider;
    [Space(20)]
    public UI_ToggleableElement P2MeterSlider;
    public UI_ToggleableElement P2AmplifySlider;

    public float ReturnP1MeterValue()
    {
        return P1MeterSlider._elementSlider.value;
    }
    public float ReturnP1AmplifyValue()
    {
        return P1AmplifySlider._elementSlider.value;
    }
    public float ReturnP2MeterValue()
    {
        return P2MeterSlider._elementSlider.value;
    }
    public float ReturnP2AmplifyValue()
    {
        return P2AmplifySlider._elementSlider.value;
    }
}
