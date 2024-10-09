using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class UI_MeterSetting_SubMenu : UI_SubMenuBase
{
    public int P1_meterIterator;
    public int P1_AmplifyIterator;
    public MeterRecoveryType P1_RecoveryType;
    [Space(20)]
    public TMP_Text P1_ToggleText;
    public TMP_Text P1_AmplifyText;
    public UI_ToggleableElement P1MeterSlider;
    public UI_ToggleableElement P1AmplifySlider;
    [Space(20)]
    public int P2_meterIterator;
    public int P2_AmplifyIterator;
    public TMP_Text P2_ToggleText;
    public TMP_Text P2_AmplifyText;
    public MeterRecoveryType P2_RecoveryType;
    public UI_ToggleableElement P2MeterSlider;
    public UI_ToggleableElement P2AmplifySlider;
    int amplifyCount;
    private void Start()
    {
        P1_meterIterator = 0;
        P2_meterIterator = 0;
        P1_AmplifyIterator = 0;
        P2_AmplifyIterator = 0;
        SetToggleText(P1_ToggleText, P1_meterIterator);
        SetToggleText(P2_ToggleText, P2_meterIterator);
        SetAmplifyToggleText(P1_AmplifyText, P1_AmplifyIterator);
        SetAmplifyToggleText(P2_AmplifyText, P2_AmplifyIterator);
        meterRecoveryLength = Enum.GetNames(typeof(MeterRecoveryType)).Length;
        amplifyCount = Enum.GetNames(typeof(StatusEffect.Effect_Amplify)).Length;
    }
    #region Meter Type Updates
    public void UpdateP1Data(int Direction) 
    {
        if(Direction == 1) 
        {
            ToggleP1_MeterTypeUp(P1_ToggleText);
        }
        if (Direction == -1)
        {
            ToggleP1_MeterTypeDown(P1_ToggleText);
        }
    }
    public void UpdateP2Data(int Direction)
    {
        if (Direction == 1)
        {
            ToggleP2_MeterTypeUp(P2_ToggleText);
        }
        if (Direction == -1)
        {
            ToggleP2_MeterTypeDown(P2_ToggleText);
        }
    }
    public void ToggleP1_MeterTypeUp(TMP_Text textObject)
    {
        P1_meterIterator++;
        if (P1_meterIterator > meterRecoveryLength)
        {
            P1_meterIterator = 0;
        }
        SetToggleText(textObject, P1_meterIterator);
    }
    public void ToggleP1_MeterTypeDown(TMP_Text textObject)
    {
        P1_meterIterator--;
        if (P1_meterIterator < 0)
        {
            P1_meterIterator = meterRecoveryLength - 1;
        }
        SetToggleText(textObject, P1_meterIterator);
    }
    public void ToggleP2_MeterTypeUp(TMP_Text textObject)
    {
        P2_meterIterator++;
        if (P2_meterIterator > meterRecoveryLength)
        {
            P2_meterIterator = 0;
        }
        SetToggleText(textObject, P2_meterIterator);
    }
    public void ToggleP2_MeterTypeDown(TMP_Text textObject)
    {
        P2_meterIterator--;
        if (P2_meterIterator < 0)
        {
            P2_meterIterator = meterRecoveryLength -1;
        }
        SetToggleText(textObject, P2_meterIterator);
    }
    #endregion


    #region Amplify Type Updates
    public void UpdateAmplifyP1Data(int Direction)
    {
        if (Direction == 1)
        {
            ToggleP1_AmplifyTypeUp(P1_AmplifyText);
        }
        if (Direction == -1)
        {
            ToggleP1_AmplifyTypeDown(P1_AmplifyText);
        }
    }
    public void UpdateAmplifyP2Data(int Direction)
    {
        if (Direction == 1)
        {
            ToggleP2_AmplifyTypeUp(P2_AmplifyText);
        }
        if (Direction == -1)
        {
            ToggleP2_AmplifyTypeDown(P2_AmplifyText);
        }
    }
    public void ToggleP1_AmplifyTypeUp(TMP_Text textObject)
    {
        P1_AmplifyIterator++;
        if (P1_AmplifyIterator > amplifyCount)
        {
            P1_AmplifyIterator = 0;
        }
        SetAmplifyToggleText(textObject, P1_AmplifyIterator);
    }
    public void ToggleP1_AmplifyTypeDown(TMP_Text textObject)
    {
        P1_AmplifyIterator--;
        if (P1_AmplifyIterator < 0)
        {
            P1_AmplifyIterator = amplifyCount - 1;
        }
        SetAmplifyToggleText(textObject, P1_AmplifyIterator);
    }
    public void ToggleP2_AmplifyTypeUp(TMP_Text textObject)
    {
        P2_AmplifyIterator++;
        if (P2_AmplifyIterator > amplifyCount)
        {
            P2_AmplifyIterator = 0;
        }
        SetAmplifyToggleText(textObject, P2_AmplifyIterator);
    }
    public void ToggleP2_AmplifyTypeDown(TMP_Text textObject)
    {
        P2_AmplifyIterator--;
        if (P2_AmplifyIterator < 0)
        {
            P2_AmplifyIterator = amplifyCount - 1;
        }
        SetAmplifyToggleText(textObject, P2_AmplifyIterator);
    }
    #endregion

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
    void SetToggleText(TMP_Text textObject,int iterator) 
    {
        MeterRecoveryType currentType = (MeterRecoveryType)iterator;
        textObject.text = currentType.ToString();
    }
    void SetAmplifyToggleText(TMP_Text textObject, int iterator)
    {
        StatusEffect.Effect_Amplify currentType = (StatusEffect.Effect_Amplify)iterator;
        textObject.text = currentType.ToString();
    }
}
