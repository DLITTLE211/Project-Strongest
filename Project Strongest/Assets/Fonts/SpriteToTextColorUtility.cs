using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SpriteToTextColorUtility
{
    public static string AppendSpriteName(string message, UnityEngine.Color color)
    {
        string HexColor = UnityEngine.ColorUtility.ToHtmlStringRGBA(color);
        string formated = "";

        for (int i = 0; i < message.Length; i++)
        {
            if (message[i].ToString() == " ") 
            {
                formated += " ";
                continue;
            }
            formated += $"<sprite name=\"{message[i]}\"" + " color=#" + HexColor + ">";
        }

        return formated;
    }
}
