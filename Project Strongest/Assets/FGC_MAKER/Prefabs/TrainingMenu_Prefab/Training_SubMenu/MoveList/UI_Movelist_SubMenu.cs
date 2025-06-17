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
    private MoveListObject activeMoveList;
    public void SetPlayer1MoveListData(Character_MoveList moveList, string characterName, int playerID) 
    {
        if (P1_MoveList.dataFilled == false)
        {
            SetMovelistData(P1_MoveList, moveList, characterName, playerID);
            _characterMoveListHeader.text = P1_MoveList.headerMessage;
            activeMoveList = P1_MoveList;
        }
    }
    public void SetMovelistData(MoveListObject _moveListObject, Character_MoveList moveList, string characterName, int playerID) 
    {
        if (_moveListObject.dataFilled == false)
        {
            GameObject playerMovelist = GameObject.Instantiate(_baseGenerator.gameObject, MovelistTransformObject);
            playerMovelist.name = $"Player{playerID+1}_MoveList";
            _moveListObject.moveListInformationTarget = playerMovelist.transform;
            _moveListObject._moveListObject = playerMovelist.GetComponent<MovelistGenerator>();
            _moveListObject.MoveListName = characterName;
            _moveListObject.headerMessage = $"Player {playerID+1}: {_moveListObject.MoveListName} MOVE LIST";
            _characterMoveListHeader.text = _moveListObject.headerMessage;
            _moveListObject._moveListObject.SetMovelist(moveList);
            _moveListObject.dataFilled = true;
        }
    }
    public void SetPlayer2MoveListData(Character_MoveList moveList, string characterName, int playerID)
    {
        if (P2_MoveList.dataFilled == false)
        {
            SetMovelistData(P2_MoveList, moveList, characterName, playerID);
            P2_MoveList.moveListInformationTarget.gameObject.SetActive(false);
        }
    }
    public void CycleMovelist()
    {
        activeMoveList.moveListInformationTarget.gameObject.SetActive(false);
        bool isFirstPlayerData = activeMoveList.moveListInformationTarget.gameObject == P1_MoveList.moveListInformationTarget.gameObject;
        activeMoveList = isFirstPlayerData ? P2_MoveList : P1_MoveList;
        activeMoveList.moveListInformationTarget.gameObject.SetActive(true);
        _characterMoveListHeader.text = activeMoveList.headerMessage;

    }
}
[Serializable]
public class MoveListObject 
{
    public Transform moveListInformationTarget;
    public MovelistGenerator _moveListObject;
    public GridLayoutGroup _layoutGroup;
    public string headerMessage;
    public TMP_Text HeaderText;
    public string MoveListName;
    public bool dataFilled;
}
[Serializable]
public class MoveListAttackInfo
{
    public string SpecialAttackName;
    public string AttackName;
    public string AttackInput;
    public float meterRequirement;
    public MoveListAttackInfo(string _name, string _input, float _m_requirement, string _specialAttackName = "") 
    {
        SpecialAttackName = _specialAttackName;
        AttackName = _name;
        AttackInput = _input;
        meterRequirement = _m_requirement;
    }
}