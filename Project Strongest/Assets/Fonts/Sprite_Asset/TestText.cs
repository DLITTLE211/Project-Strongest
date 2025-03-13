using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TestText : MonoBehaviour
{
    public TMP_Text text1;
    public TMP_Text text2;
    public string testString1,testString2;
    // Update is called once per frame
    private void Start()
    {
        setStringAsset(text1, testString1);
        setStringAsset(text2, testString2);
    }
    void setStringAsset(TMP_Text text, string Message) 
    {
        string newMessage = "";
        char[] characters = Message.ToCharArray();
        for(int i = 0; i < characters.Length; i++)
        {
            for (int j = 0; j < text.spriteAsset.spriteCharacterTable.Count; j++)
            {
                string letter = text.spriteAsset.spriteCharacterTable[j].name;
                if (characters[i].ToString() == letter)
                {
                    newMessage += $"<sprite name=\"{ letter}\">";
                    break;
                }
            }
        }
        text.text = newMessage;
    }
}
