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
        _disparityMeter.minValue = -(1 * (leftPlayer._cHealth.ReturnMeterDiscrepency()));
        _disparityMeter.value = 0;
        _disparityMeter.maxValue = 1 * (rightPlayer._cHealth.ReturnMeterDiscrepency());
    }
}
