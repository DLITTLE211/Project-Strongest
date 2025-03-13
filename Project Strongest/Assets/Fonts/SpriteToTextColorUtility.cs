using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SpriteToTextColorUtility
{
    public static string AppendSpriteName(string number, UnityEngine.Color color)
    {
        string HexColor = UnityEngine.ColorUtility.ToHtmlStringRGBA(color);
        string formated = "";

        for (int i = 0; i < number.Length; i++)
        {
            formated += $"<sprite name=\"{number[i]}\"" + " color=#" + HexColor + ">";
        }

        return formated;
    }
}
