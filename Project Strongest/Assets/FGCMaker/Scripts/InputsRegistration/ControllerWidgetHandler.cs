using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class ControllerWidgetHandler : MonoBehaviour
{
    public Transform knobPos;
    public List<Image> buttonList;
    public List<Vector2> axisKnobLocations;
    private int prevInt;
    [SerializeField] private TMP_Text numText;

    private void Start()
    {
        prevInt = 5;
    }
    // Start is called before the first frame update
    public void GetAxisLocation(int aKlocation) 
    {
        if (prevInt == aKlocation) 
        {
            return;
        }
        prevInt = aKlocation;
        numText.text = aKlocation.ToString();
        SetKnobPos(axisKnobLocations[prevInt - 1]);
    }
    public void SetKnobPos(Vector2 newPos) 
    {
        if (knobPos.position == new Vector3(newPos.x, newPos.y, 0)) 
        {
            return;
        }

        knobPos.DOLocalMove(newPos, 0.075f);
    }
    public void ValidateButtonHoldInput(Character_ButtonInput button)
    {
        switch (button.Button_Name)
        {
            case "A":
                SetButtonHold(0);
                break;
            case "B":
                SetButtonHold(1);
                break;
            case "C":
                SetButtonHold(2);
                break;
            case "D":
                SetButtonHold(3);
                break;
            case "E":
                SetButtonHold(4);
                break;
            case "F":
                SetButtonHold(5);
                break;
            case "G":
                SetButtonHold(6);
                break;
            case "H":
                SetButtonHold(7);
                break;
        }
    }
    public void ValidateButtonReleaseInput(Character_ButtonInput button)
    {
        switch (button.Button_Name)
        {
            case "A":
                ReleasedButton(0);
                break;
            case "B":
                ReleasedButton(1);
                break;
            case "C":
                ReleasedButton(2);
                break;
            case "D":
                ReleasedButton(3);
                break;
            case "E":
                ReleasedButton(4);
                break;
            case "F":
                ReleasedButton(5);
                break;
            case "G":
                ReleasedButton(6);
                break;
            case "H":
                ReleasedButton(7);
                break;
        }
    }

    public void SetButtonHold(int i) 
    {
        buttonList[i].color = Color.gray;
    }

    public void ReleasedButton(int i) 
    {
        buttonList[i].color = Color.white;
    }
}
