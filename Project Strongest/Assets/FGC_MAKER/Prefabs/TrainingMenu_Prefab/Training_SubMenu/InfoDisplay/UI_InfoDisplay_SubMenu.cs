using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;
using TMPro;

public class UI_InfoDisplay_SubMenu : UI_SubMenuBase
{
    // Start is called before the first frame update
    public UI_DisplaySet p1_booleanState;
    public UI_DisplaySet p2_booleanState;
    private void Start()
    {
        if(p1_booleanState == null) 
        {
            p1_booleanState = new UI_DisplaySet();
        }
        if (p2_booleanState == null)
        {
            p2_booleanState = new UI_DisplaySet();
        }
    }
    public void P1_ToggleButtonState(int level) 
    {
        p1_booleanState.displayToggle[level].SetState(!p1_booleanState.displayToggle[level].currentState);
    }
    public void P2_ToggleButtonState(int level)
    {
        p2_booleanState.displayToggle[level].SetState(!p2_booleanState.displayToggle[level].currentState);
    }
}
[Serializable]
public class UI_DisplaySet 
{
    public List<UI_DisplayToggle> displayToggle;
    public List<bool> trainingUi_BooleanState;

    public UI_DisplaySet()
    {
        trainingUi_BooleanState = new List<bool>();
        for (int i = 0; i < displayToggle.Count; i++) 
        {
            displayToggle[i].currentState = false;
            string headerMessage = SpriteToTextColorUtility.AppendSpriteName($"{displayToggle[i].headerText.text.ToUpper()}", displayToggle[i].headerText.color);
            displayToggle[i].headerText.SetText(headerMessage);
            displayToggle[i].SetState(false);
            trainingUi_BooleanState.Add(false);
        }
    }
    public List<bool> ReturnBooleanStatesForObject() 
    {
        for(int i = 0; i < displayToggle.Count; i++) 
        {
            trainingUi_BooleanState[i] = displayToggle[i].currentState;
        }
        return trainingUi_BooleanState;
    } 
}
[Serializable]
public class UI_DisplayToggle 
{
    public string ToggleName;
    public Button leftButton;
    public Button rightButton;
    public TMP_Text headerText;
    public TMP_Text boolStateText;
    public bool currentState;
    public void SetState(bool state) 
    {
        currentState = state;
        string message = state ? SpriteToTextColorUtility.AppendSpriteName("ON".ToUpper(), Color.white) : SpriteToTextColorUtility.AppendSpriteName("OFF".ToUpper(), Color.white);
        boolStateText.SetText(message);
    }
}