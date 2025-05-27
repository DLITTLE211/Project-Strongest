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
            textObject[i].fontSize = 18;
        }
    }
    public void SetTextLog(List<string> itemInfo, bool numSize) 
    {
        for (int i = 0; i < itemInfo.Count; i++)
        {
            if (i == 0) 
            {
                string stringMessage = itemInfo[0];
                textObject[0].SetText(stringMessage);
            }
            else 
            {
                string stringMessage = itemInfo[i];
                textObject[i].SetText(stringMessage);
            }
        }
    }
}
