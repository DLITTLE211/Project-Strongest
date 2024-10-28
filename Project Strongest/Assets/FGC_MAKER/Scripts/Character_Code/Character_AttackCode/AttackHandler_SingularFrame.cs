using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AttackHandler_SingularFrame : MonoBehaviour
{
    [SerializeField] private List<Color> sf_Colors = new List<Color>()
    {
        Color.white,
        Color.blue,
        Color.green,
        Color.red,
        Color.black,
    };
    [SerializeField] private FrameType _curFrameType;
    [SerializeField] private Image sf_FrameImage;
    [SerializeField] private TMP_Text _frameText;
    public void InitFrame() 
    {
        _curFrameType = FrameType.Reset;
        _frameText.text = "";
        SetColor();
    }
    public void SetFrame_FrameType(FrameType newFrameType,int endingFrame = -1) 
    {
        _curFrameType = newFrameType;
        SetColor();
        if (endingFrame != -1) 
        {
            SetText(endingFrame);
        }
    }
    public void SetColor() 
    {
        int frameColorIndex = (int)_curFrameType;
        sf_FrameImage.color = sf_Colors[frameColorIndex];
    }
    public void SetText(int endingFrame)
    {
        _frameText.text = $"{endingFrame}";
    }
}
public enum FrameType 
{
    Init = 0,
    Startup = 1,
    Active = 2,
    Recovery = 3,
    Reset = 4,
}