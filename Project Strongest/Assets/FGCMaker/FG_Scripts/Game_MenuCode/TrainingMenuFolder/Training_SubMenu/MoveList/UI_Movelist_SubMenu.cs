using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;
using TMPro;

public class UI_Movelist_SubMenu : UI_SubMenuBase
{
    [SerializeField] private TMP_Text _characterMoveListHeader;
    [SerializeField] private GameObject TextSample;
    [SerializeField] private MoveListObject P1_MoveList;
    [SerializeField] private MoveListObject P2_MoveList;
    public void SetPlayer1MoveListData(Character_MoveList moveList, string characterName) 
    {
        if (P1_MoveList.dataFilled == false)
        {
            P1_MoveList.MoveListName = characterName;
            SetMovelist(moveList, P1_MoveList);
            P1_MoveList.dataFilled = true;
            _characterMoveListHeader.text = $"Player 1: {P1_MoveList.MoveListName} MOVE LIST";
        }
    }
    public void SetPlayer2MoveListData(Character_MoveList moveList, string characterName)
    {
        if (P2_MoveList.dataFilled == false)
        {
            P2_MoveList.MoveListName = characterName;
            SetMovelist(moveList, P2_MoveList);
            P2_MoveList.dataFilled = true;
        }
        CycleMovelist();
    }
    public void SetMovelist(Character_MoveList moveList, MoveListObject moveListObject)
    {
        for(int i = moveList.BasicSuperAttacks.Count-1; i > -1; i--) 
        {
            MakeAndSetText(moveList.BasicSuperAttacks[i].CreateMoveListData(), moveListObject.moveListInformationTarget);
        }

        for (int i = moveList.CommandThrows.Count - 1; i > -1; i--)
        {
            MakeAndSetText(moveList.CommandThrows[i].CreateMoveListData(), moveListObject.moveListInformationTarget);
        }

        for (int i = moveList.CounterAttacks.Count - 1; i > -1; i--)
        {
            MakeAndSetText(moveList.CounterAttacks[i].CreateMoveListData(), moveListObject.moveListInformationTarget);
        }

        for (int i = moveList.stanceSpecials.Count - 1; i > -1; i--)
        {
            MakeAndSetText(moveList.stanceSpecials[i].CreateMoveListData(), moveListObject.moveListInformationTarget);
        }

        for (int i = moveList.rekkaSpecials.Count - 1; i > -1; i--)
        {
            MakeAndSetText(moveList.rekkaSpecials[i].CreateMoveListData(), moveListObject.moveListInformationTarget);
        }

        for (int i = moveList.special_Simple.Count - 1; i > -1; i--)
        {
            MakeAndSetText(moveList.special_Simple[i].CreateMoveListData(), moveListObject.moveListInformationTarget);
        }
        for (int i = moveList.commandNormalAttacks.Count - 1; i > -1; i--)
        {
            MakeAndSetText(moveList.commandNormalAttacks[i].CreateMoveListData(), moveListObject.moveListInformationTarget);
        }
        for (int i = moveList.BasicThrows.Count - 1; i > -1; i--)
        {
            MakeAndSetText(moveList.BasicThrows[i].CreateMoveListData(), moveListObject.moveListInformationTarget);
        }
    }
    public void CycleMovelist() 
    {
        if (P1_MoveList.moveListInformationTarget.gameObject.activeInHierarchy) 
        {
            P1_MoveList.moveListInformationTarget.gameObject.SetActive(false);
            P2_MoveList.moveListInformationTarget.gameObject.SetActive(true);
            _characterMoveListHeader.text = $"Player 2: {P2_MoveList.MoveListName} MOVE LIST";
        }
        else if (P2_MoveList.moveListInformationTarget.gameObject.activeInHierarchy)
        {
            P2_MoveList.moveListInformationTarget.gameObject.SetActive(false);
            P1_MoveList.moveListInformationTarget.gameObject.SetActive(true);
            _characterMoveListHeader.text = $"Player 1: {P1_MoveList.MoveListName} MOVE LIST";
        }
        else 
        {
            P2_MoveList.moveListInformationTarget.gameObject.SetActive(false);
            P1_MoveList.moveListInformationTarget.gameObject.SetActive(true);
            _characterMoveListHeader.text = $"Player 1: {P1_MoveList.MoveListName} MOVE LIST";
        }
        
    }

    public void MakeAndSetText(MoveListAttackInfo newAttack, Transform location)
    {
        GameObject curMoveTextAsset = GameObject.Instantiate(TextSample, location);
        TMP_Text moveTextField = curMoveTextAsset.GetComponentInChildren<TMP_Text>();

        string meterAddendum = newAttack.meterRequirement > 1 ? "bars of meter" : "bar of meter";
        string meterMessage = newAttack.meterRequirement > 0 ? $"<size={18}>(requires {newAttack.meterRequirement} {meterAddendum})" : "";

        string Message = $"{newAttack.AttackName} {meterMessage} \n <size={18}>{newAttack.AttackInput}";
        moveTextField.text = Message;
    }
}
[Serializable]
public class MoveListObject 
{
    public GameObject moveListObject;
    public Transform moveListInformationTarget;
    public string MoveListName;
    public bool dataFilled;
}
[Serializable]
public class MoveListAttackInfo
{
    public string AttackName;
    public string AttackInput;
    public int meterRequirement;
    public MoveListAttackInfo(string _name, string _input, int _m_requirement) 
    {
        AttackName = _name;
        AttackInput = _input;
        meterRequirement = _m_requirement;
    }
}