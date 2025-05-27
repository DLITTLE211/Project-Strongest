using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class CharacterSelect_ColorPicker : MonoBehaviour
{
    [SerializeField] private TMP_Text _colorText;
    int lastInt;
    public void UpdateColorChoice(int colorChoice) 
    {
        lastInt = colorChoice;
        string message = $"Color {lastInt + 1}";
        _colorText.text = SpriteToTextColorUtility.AppendSpriteName(message.ToUpper(), _colorText.color);
    }
    public void LockInColorChoice()
    {
        string message = $"Color {lastInt + 1} Selected";
        _colorText.text = SpriteToTextColorUtility.AppendSpriteName(message.ToUpper(), _colorText.color);
    }
    public void ClearText()
    {
        _colorText.text = $"";
    }
}
