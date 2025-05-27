using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AttackHandler_SingularFrame : MonoBehaviour
{
    [SerializeField] private List<Color> sf_Colors = new List<Color>();
    [SerializeField] private FrameType _curFrameType;
    [SerializeField] private Image sf_FrameImage;
    [SerializeField] private Image sf_FrameImage_Outline;
    [SerializeField] private TMP_Text _frameText;
    public void InitColors() 
    {
        Color32 purpleStun = new Color32((byte)254f, (byte)66f, (byte)213f, (byte)255f);
        Color32 darkPurpleStun = new Color32((byte)204f, (byte)36f, (byte)153f, (byte)205f);
        sf_Colors = new List<Color>(){Color.black,  Color.cyan, Color.green, Color.red, purpleStun, darkPurpleStun, Color.grey, Color.yellow,};
    }
    public void InitFrame()
    {
        DisablePreviousFrameHighlight();
        _curFrameType = FrameType.Reset;
        _frameText.text = "";
        SetColor();
    }
    public void SetFrame_FrameType(FrameType newFrameType, int endingFrame = -1)
    {
        sf_FrameImage_Outline.enabled = true;
        _curFrameType = newFrameType;
        SetColor();
        if (endingFrame != -1)
        {
            SetText(endingFrame);
        }
    }
    public void DisablePreviousFrameHighlight() 
    {
        sf_FrameImage_Outline.enabled = false;
    }
    public void SetColor()
    {
        int frameColorIndex = (int)_curFrameType;
        Color curFrameColor = sf_Colors[frameColorIndex];
        sf_FrameImage.color = curFrameColor;
    }
    public void SetText(int endingFrame)
    {
        _frameText.text = $"{endingFrame}";
    }
}
public enum FrameType
{
    Reset = 0,
    Startup = 1,
    Active = 2,
    Recovery = 3,
    Stun = 4,
    StunNext = 5,
    Grounded = 6,
    HitStop = 7,
}