using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class InputLogger
{
    public List<TMP_Text> textObject;

    public void ResetAllText() 
    {
        for (int i = 0; i < textObject.Count; i++) 
        {
            textObject[i].text = "";
        }
    }
    public void setFirstItem(List<string> itemInfo, bool numSize)
    {
        string stringMessage = SpriteToTextColorUtility.AppendSpriteName($"{itemInfo[0]}", Color.white);
        textObject[0].SetText(stringMessage);
        if (numSize) 
        {
            textObject[0].fontSize = 17;
        }
        else 
        {
            textObject[0].fontSize = 22;
        }
    }

    public void setNextItemInList(List<string> itemInfo, bool numSize)
    {
        string stringMessage = "";
        for (int i = itemInfo.Count-1; i > 0; i--) 
        {
            stringMessage = textObject[i - 1].text;
            textObject[i].SetText(stringMessage);
            textObject[i].fontSize = textObject[i - 1].fontSize;
        }
        stringMessage = SpriteToTextColorUtility.AppendSpriteName($"{itemInfo[0]}", Color.white);
        textObject[0].SetText(stringMessage);
        if (numSize)
        {
            textObject[0].fontSize = 17;
        }
        else
        {
            textObject[0].fontSize = 22;
        }
    }
}
