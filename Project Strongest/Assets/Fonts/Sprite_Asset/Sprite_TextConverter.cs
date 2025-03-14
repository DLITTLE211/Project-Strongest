using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Sprite_TextConverter : MonoBehaviour
{
    [SerializeField] private string message;
    [SerializeField] private TMP_Text _textObject;
    void Start()
    {
        _textObject.text = SpriteToTextColorUtility.AppendSpriteName(message.ToUpper(), _textObject.color);
    }

}
