using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class UI_Movelist_SubMenu : UI_SubMenuBase
{
    [SerializeField] private Transform _instantiateTarget;
    [SerializeField] private GameObject TextSample;
    public void SetMovelist(Character_Base _curBase)
    {
        for(int i = 0; i < _curBase.ChosenCharacterMoveList.Count; i++) 
        {
            MakeAndSetText(_curBase,i);
        }
    }
    
    public void MakeAndSetText(Character_Base _curBase, int i) 
    {
        AttackInputTypes curAttack = _curBase.ChosenCharacterMoveList.ElementAt(i).Key;
        string attackName = curAttack.AttackName;
        string specialMoveInput = curAttack.specialMoveTypeInput.attackString;
        GameObject curMoveTextAsset = GameObject.Instantiate(TextSample, _instantiateTarget);
        TMP_Text moveTextField = curMoveTextAsset.GetComponent<TMP_Text>();
        moveTextField.text = $"{attackName} \n <size = 25>{specialMoveInput}";
    }
}
