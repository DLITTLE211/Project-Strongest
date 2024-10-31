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
        _colorText.text = $"Color {lastInt + 1}";
    }
    public void LockInColorChoice()
    {
        _colorText.text = $"Color {lastInt + 1} Selected";
    }
    public void ClearText()
    {
        _colorText.text = $"";
    }
}
