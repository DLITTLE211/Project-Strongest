using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CharacterSelect_SubMenuBase : MonoBehaviour
{
    [SerializeField] protected TMP_Text _topHeaderText;
    [SerializeField] protected TMP_Text _bottomHeaderText;
    public bool allowBase;
    protected virtual void ActivateSubMenu() 
    {

    }
    protected virtual void DeactivateSubMenu()
    {

    }
    public virtual void OnUpdate()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            ActivateSubMenu();
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            DeactivateSubMenu();
        }
    }
    public void SetHeaderText(TMP_Text textAsset,string message)
    {
        string structuredText = $"{message.ToUpper()}";
        textAsset.text = SpriteToTextColorUtility.AppendSpriteName(structuredText, textAsset.color);
    }
    public void SetTopHeaderText(string message) 
    {
        string structuredText = $"{message.ToUpper()}";
        _topHeaderText.text = SpriteToTextColorUtility.AppendSpriteName(structuredText,_topHeaderText.color);
    }
    public void SetBottomHeaderText(string message)
    {
        string structuredText = $"{message.ToUpper()}";
        _bottomHeaderText.text = SpriteToTextColorUtility.AppendSpriteName(structuredText, _bottomHeaderText.color);
    }
}
