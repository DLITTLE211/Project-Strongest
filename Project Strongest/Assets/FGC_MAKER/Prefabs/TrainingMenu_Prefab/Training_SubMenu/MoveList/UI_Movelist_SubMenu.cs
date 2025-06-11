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
    [SerializeField] private Transform MovelistTransformObject;
    [SerializeField] private MovelistGenerator _baseGenerator;
    [SerializeField] private MoveListObject P1_MoveList;
    [SerializeField] private MoveListObject P2_MoveList;
    public void SetPlayer1MoveListData(Character_MoveList moveList, string characterName) 
    {
        if (P1_MoveList.dataFilled == false)
        {
            SetMovelistData(P1_MoveList, moveList, characterName);
            /*GameObject playerMovelist = GameObject.Instantiate(_baseGenerator.gameObject, MovelistTransformObject);
            playerMovelist.name = "Player1_MoveList";
            P1_MoveList.moveListInformationTarget = playerMovelist.transform;
            P1_MoveList._moveListObject = playerMovelist.GetComponent<MovelistGenerator>();
            P1_MoveList.MoveListName = characterName;
            _characterMoveListHeader.text = $"Player 1: {P1_MoveList.MoveListName} MOVE LIST";
            P1_MoveList._moveListObject.SetMovelist(moveList);
            P1_MoveList.dataFilled = true;*/
        }
    }
    public void SetMovelistData(MoveListObject _moveListObject, Character_MoveList moveList, string characterName) 
    {
        if (_moveListObject.dataFilled == false)
        {
            GameObject playerMovelist = GameObject.Instantiate(_baseGenerator.gameObject, MovelistTransformObject);
            playerMovelist.name = "Player1_MoveList";
            _moveListObject.moveListInformationTarget = playerMovelist.transform;
            _moveListObject._moveListObject = playerMovelist.GetComponent<MovelistGenerator>();
            _moveListObject.MoveListName = characterName;
            _characterMoveListHeader.text = $"Player 1: {P1_MoveList.MoveListName} MOVE LIST";
            _moveListObject._moveListObject.SetMovelist(moveList);
            _moveListObject.dataFilled = true;
        }
    }
    public void SetPlayer2MoveListData(Character_MoveList moveList, string characterName)
    {
        /*if (P2_MoveList.dataFilled == false)
        {
            P2_MoveList.MoveListName = characterName;
            SetMovelist(moveList, P2_MoveList);
            P2_MoveList.dataFilled = true;
        }*/
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
            for (int j = moveList.stanceSpecials[i].stanceInput.stanceAttack._stanceButtonInput._correctInput.Count - 1; j > -1; j--)
            {
                Attack_BaseInput curStanceAttack = moveList.stanceSpecials[i].stanceInput.stanceAttack._stanceButtonInput._correctInput[j];
                MakeAndSetText(curStanceAttack.CreateMoveListData(), moveListObject.moveListInformationTarget);
            }
        }

        for (int i = moveList.rekkaSpecials.Count - 1; i > -1; i--)
        {
            MakeAndSetText(moveList.rekkaSpecials[i].CreateMoveListData(), moveListObject.moveListInformationTarget);
            for (int j = moveList.rekkaSpecials[i].rekkaInput._rekkaPortion.Count - 1; j > -1; j--)
            {
                Attack_BaseInput curRekkaAttack = moveList.rekkaSpecials[i].rekkaInput._rekkaPortion[j].individualRekkaAttack._correctInput[0];
                MakeAndSetText(curRekkaAttack.CreateMoveListData(), moveListObject.moveListInformationTarget);
            }
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
       /* if (P1_MoveList.moveListInformationTarget.gameObject.activeInHierarchy) 
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
        }*/
        
    }

    public void MakeAndSetText(MoveListAttackInfo newAttack, Transform location)
    {
        GameObject curMoveTextAsset = GameObject.Instantiate(TextSample, location);
        TMP_Text moveTextField = curMoveTextAsset.GetComponentInChildren<TMP_Text>();

        string meterAddendum = newAttack.meterRequirement > 1 ? "bars of meter" : "bar of meter";
        string meterMessage = newAttack.meterRequirement > 0 ? $"<size={16}>(requires {newAttack.meterRequirement} {meterAddendum})" : "";

        string Message = $"<size={18}>{newAttack.AttackInput} <size={25}>{newAttack.AttackName} {meterMessage} ";
        moveTextField.text = Message;
    }
}
[Serializable]
public class MoveListObject 
{
    public Transform moveListInformationTarget;
    public MovelistGenerator _moveListObject;
    public GridLayoutGroup _layoutGroup;
    public TMP_Text HeaderText;
    public string MoveListName;
    public bool dataFilled;
}
[Serializable]
public class MoveListAttackInfo
{
    public string AttackName;
    public string AttackInput;
    public float meterRequirement;
    public MoveListAttackInfo(string _name, string _input, float _m_requirement) 
    {
        AttackName = _name;
        AttackInput = _input;
        meterRequirement = _m_requirement;
    }
}