using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class MovelistGenerator : MonoBehaviour
{
    [SerializeField] private Transform _pageTransform;
    [SerializeField] private GameObject movelistPage;
    [SerializeField] private GameObject TextSample;

    public void SetMovelist(Character_MoveList moveList)
    {
        #region Super Attack
        if (moveList.BasicSuperAttacks.Count > 0)
        {
            GameObject superPage = GameObject.Instantiate(movelistPage,_pageTransform);
            List<MoveListAttackInfo> superAttackList = new List<MoveListAttackInfo>();
            superPage.name = "Super Movelist Page";
            for (int i = 0; i < moveList.BasicSuperAttacks.Count; i++)
            {
                superAttackList.Add(moveList.BasicSuperAttacks[i].CreateMoveListData());
            }
            BuildPageData(superPage, superAttackList, "Super Attacks");
        }
        #endregion

        #region Command Grabs Attack
        if (moveList.CommandThrows.Count > 0)
        {
            GameObject cTPage = GameObject.Instantiate(movelistPage, _pageTransform);
            cTPage.name = "Command Movelist Page";
            List<MoveListAttackInfo> commandGrabList = new List<MoveListAttackInfo>();
            for (int i = 0; i < moveList.CommandThrows.Count; i++)
            {
                commandGrabList.Add(moveList.CommandThrows[i].CreateMoveListData());
            }
            BuildPageData(cTPage, commandGrabList, "Command Grabs");
            //cTPage.SetActive(false);
        }
        #endregion
        return;
        /*for (int i = moveList.CommandThrows.Count - 1; i > -1; i--)
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
        }*/
    }
    public void BuildPageData(GameObject CurrentPage, List<MoveListAttackInfo> attackTypeData, string headerMessage) 
    {
        CurrentPage.name = "Super Movelist Page";
        TMP_Text headerText = CurrentPage.GetComponentInChildren<TMP_Text>();
        headerText.SetText(SpriteToTextColorUtility.AppendSpriteName(headerMessage.ToUpper(), Color.white));
        GridLayoutGroup layout = CurrentPage.GetComponentInChildren<GridLayoutGroup>();
        layout.constraintCount = attackTypeData.Count >= 6 ? 2 : 1;
        for (int i = 0; i < attackTypeData.Count; i++)
        {
            MakeAndSetText(attackTypeData[i], layout.transform);
        }
    }
    public void MakeAndSetText(MoveListAttackInfo newAttack, Transform location)
    {
        GameObject curMoveTextAsset = GameObject.Instantiate(TextSample, location);
        Movelist_Display objectDisplay = curMoveTextAsset.GetComponent<Movelist_Display>();
        objectDisplay.SetTextData(newAttack);
    }
}

