using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Movelist_Display : MonoBehaviour
{
    [SerializeField] private TMP_Text NameTextObject;
    [SerializeField] private TMP_Text InputTextObject;
    public void SetTextData(MoveListAttackInfo newAttack)
    {
        string meterAddendum = newAttack.meterRequirement > 1 ? "bars of meter" : "bar of meter";
        string meterMessage = newAttack.meterRequirement > 0 ? $"(requires {newAttack.meterRequirement} {meterAddendum})" : "";

        NameTextObject.SetText(SpriteToTextColorUtility.AppendSpriteName($"{newAttack.AttackName.ToUpper()} {meterMessage.ToUpper()}", Color.white));
        InputTextObject.SetText(SpriteToTextColorUtility.AppendSpriteName($"{newAttack.AttackInput}", Color.white));
    }
}
