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
    public void setFirstItem(List<string> itemInfo) 
    {
        textObject[0].text = itemInfo[0];
    }

    public void setNextItemInList(List<string> itemInfo) 
    {
        for (int i = itemInfo.Count-1; i > 0; i--) 
        {
            textObject[i].text = textObject[i-1].text;
        }
        textObject[0].text = itemInfo[0];
    }
}
