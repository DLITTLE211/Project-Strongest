using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MovelistGenerator : MonoBehaviour
{
    [SerializeField] private Transform _pageTransform;
    [SerializeField] private GameObject movelistPage;
    [SerializeField] private GameObject TextSample;

    public void SetMovelist(Character_MoveList moveList, MoveListObject moveListObject)
    {
        GameObject SuperPageTransform = GameObject.Instantiate(movelistPage, _pageTransform);
        GridLayoutGroup superGrid = SuperPageTransform.GetComponentInChildren<GridLayoutGroup>();
        TMP_Text tMP_Text = SuperPageTransform.GetComponentInChildren<TMP_Text>();
        if (moveList.BasicSuperAttacks.Count > 0)
        {
            tMP_Text.SetText($"{moveList.BasicSuperAttacks[0].GetAttackMoveType()}");
            for (int i = moveList.BasicSuperAttacks.Count - 1; i > -1; i--)
            {
                MakeAndSetText(moveList.BasicSuperAttacks[i].CreateMoveListData(), superGrid.transform);
            }
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
    public void MakeAndSetText(MoveListAttackInfo newAttack, Transform location)
    {
        GameObject curMoveTextAsset = GameObject.Instantiate(TextSample, location);
        Movelist_Display objectDisplay = curMoveTextAsset.GetComponent<Movelist_Display>();
        objectDisplay.SetTextData(newAttack);
    }
}
