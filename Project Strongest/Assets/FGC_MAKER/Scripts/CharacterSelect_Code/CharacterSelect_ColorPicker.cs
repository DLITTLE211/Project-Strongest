using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class CharacterSelect_ColorPicker : MonoBehaviour
{
    [SerializeField] private TMP_Text _colorText;
    public void UpdateColorChoice(int colorChoice) 
    {
        _colorText.text = $"Color {colorChoice+1}";
    }
    public void ClearText()
    {
        _colorText.text = $"";
    }
}
