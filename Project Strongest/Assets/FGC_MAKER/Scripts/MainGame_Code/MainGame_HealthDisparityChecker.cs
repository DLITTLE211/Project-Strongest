using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainGame_HealthDisparityChecker : MonoBehaviour
{
    [SerializeField] private Slider _disparityMeter;
    [SerializeField] private Character_Base leftPlayer, rightPlayer;
    public void UpdateMeterOnDamage() 
    {
        _disparityMeter.minValue = -(leftPlayer._cHealth.health_Main.currentValue);
        _disparityMeter.value = 0;
        _disparityMeter.maxValue = rightPlayer._cHealth.health_Main.currentValue;
    }
}
