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
    [SerializeField] private GameObject TextSample_Group;
    public List<GameObject> MovelistPages;
    [SerializeField] private int pageIndex;

    public void SetMovelist(Character_MoveList moveList)
    {
        MovelistPages = new List<GameObject>();
        pageIndex = 0;

        #region Basic Normal Attack
        if (moveList.simpleAttacks.Count > 0)
        {
            GameObject bNPage = GameObject.Instantiate(movelistPage, _pageTransform);
            bNPage.name = "Basic Normal Page";
            List<MoveListAttackInfo> basicNormalList = new List<MoveListAttackInfo>();
            for (int i = 0; i < moveList.simpleAttacks.Count; i++)
            {
                basicNormalList.Add(moveList.simpleAttacks[i].CreateMoveListData());
            }
            MovelistPages.Add(bNPage);
            BuildPageData(bNPage, basicNormalList, "Basic Normals");
        }
        #endregion

        #region Basic Throws
        if (moveList.BasicThrows.Count > 0)
        {
            GameObject throwPage = GameObject.Instantiate(movelistPage, _pageTransform);
            throwPage.name = "Command Normal Page";
            List<MoveListAttackInfo> throwlist = new List<MoveListAttackInfo>();
            for (int i = 0; i < moveList.BasicThrows.Count; i++)
            {
                throwlist.Add(moveList.BasicThrows[i].CreateMoveListData());
            }
            MovelistPages.Add(throwPage);
            BuildPageData(throwPage, throwlist, "Basic Throws");
        }
        #endregion

        #region Command Normal Attack
        if (moveList.commandNormalAttacks.Count > 0)
        {
            GameObject cNPage = GameObject.Instantiate(movelistPage, _pageTransform);
            cNPage.name = "Command Normal Page";
            List<MoveListAttackInfo> cNlist = new List<MoveListAttackInfo>();
            for (int i = 0; i < moveList.commandNormalAttacks.Count; i++)
            {
                cNlist.Add(moveList.commandNormalAttacks[i].CreateMoveListData());
            }
            MovelistPages.Add(cNPage);
            BuildPageData(cNPage, cNlist, "Command Normals");
        }
        #endregion

        #region String Normals Attack
        if (moveList.stringNormalAttacks.Count > 0)
        {
            GameObject sNPage = GameObject.Instantiate(movelistPage, _pageTransform);
            sNPage.name = "String Normal Movelist Page";
            List<MoveListAttackInfo> stringNormalList = new List<MoveListAttackInfo>();
            for (int i = 0; i < moveList.stringNormalAttacks.Count; i++)
            {
                stringNormalList.Clear();
                for (int j = 0; j < moveList.stringNormalAttacks[i]._attackInput._correctInput.Count; j++)
                {
                    Attack_BaseInput curStanceAttack = moveList.stringNormalAttacks[i]._attackInput._correctInput[j];
                    stringNormalList.Add(curStanceAttack.CreateMoveListData(moveList.stringNormalAttacks[i].SpecialAttackName));
                }
                BuildLayeredPageData(sNPage, stringNormalList, "String Normal Attacks");
            }
            MovelistPages.Add(sNPage);
        }
        #endregion

        #region Basic Specials Attack
        if (moveList.special_Simple.Count > 0)
        {
            GameObject sMPage = GameObject.Instantiate(movelistPage, _pageTransform);
            sMPage.name = "Special Move Page";
            List<MoveListAttackInfo> basicSpecialList = new List<MoveListAttackInfo>();
            for (int i = 0; i < moveList.special_Simple.Count; i++)
            {
                basicSpecialList.Add(moveList.special_Simple[i].CreateMoveListData());
            }
            MovelistPages.Add(sMPage);
            BuildPageData(sMPage, basicSpecialList, "Special Moves");
        }
        #endregion

        #region Rekka Attack
        if (moveList.rekkaSpecials.Count > 0)
        {
            GameObject rekkaPage = GameObject.Instantiate(movelistPage, _pageTransform);
            rekkaPage.name = "Rekka Movelist Page";
            List<MoveListAttackInfo> rekkaList = new List<MoveListAttackInfo>();
            for (int i = 0; i < moveList.rekkaSpecials.Count; i++)
            {
                rekkaList.Clear();
                rekkaList.Add(moveList.rekkaSpecials[i].CreateMoveListData());
                for (int j = 0; j < moveList.rekkaSpecials[i].rekkaInput._rekkaPortion.Count; j++)
                {
                    Attack_BaseInput curStanceAttack = moveList.rekkaSpecials[i].rekkaInput._rekkaPortion[j].individualRekkaAttack._correctInput[0];
                    rekkaList.Add(curStanceAttack.CreateMoveListData(moveList.rekkaSpecials[i].RekkaSpecialAttack_Name));
                }
                BuildLayeredPageData(rekkaPage, rekkaList, "Rekka Attacks");
            }
            MovelistPages.Add(rekkaPage);
        }
        #endregion

        #region Stances Attack
        if (moveList.stanceSpecials.Count > 0)
        {
            GameObject stancePage = GameObject.Instantiate(movelistPage, _pageTransform);
            stancePage.name = "Stance Movelist Page";
            List<MoveListAttackInfo> StanceList = new List<MoveListAttackInfo>();
            for (int i = 0; i < moveList.stanceSpecials.Count; i++)
            {
                StanceList.Clear();
                StanceList.Add(moveList.stanceSpecials[i].CreateMoveListData());
                for (int j = 0; j < moveList.stanceSpecials[i].stanceInput.stanceAttack._stanceButtonInput._correctInput.Count; j++)
                {
                    Attack_BaseInput curStanceAttack = moveList.stanceSpecials[i].stanceInput.stanceAttack._stanceButtonInput._correctInput[j];
                    StanceList.Add(curStanceAttack.CreateMoveListData(moveList.stanceSpecials[i].StanceSpecialAttack_Name));
                }
                BuildLayeredPageData(stancePage, StanceList, "Stance Attacks");
            }
            MovelistPages.Add(stancePage);
        }
        #endregion

        #region Counter Grabs Attack
        if (moveList.CounterAttacks.Count > 0)
        {
            GameObject counterPage = GameObject.Instantiate(movelistPage, _pageTransform);
            counterPage.name = "Counter Movelist Page";
            List<MoveListAttackInfo> counterList = new List<MoveListAttackInfo>();
            for (int i = 0; i < moveList.CounterAttacks.Count; i++)
            {
                counterList.Add(moveList.CounterAttacks[i].CreateMoveListData());
            }
            MovelistPages.Add(counterPage);
            BuildPageData(counterPage, counterList, "Counters");
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
            MovelistPages.Add(cTPage);
            BuildPageData(cTPage, commandGrabList, "Command Grabs");
        }
        #endregion

        #region Super Attack
        if (moveList.BasicSuperAttacks.Count > 0)
        {
            GameObject superPage = GameObject.Instantiate(movelistPage, _pageTransform);
            List<MoveListAttackInfo> superAttackList = new List<MoveListAttackInfo>();
            superPage.name = "Super Movelist Page";
            for (int i = 0; i < moveList.BasicSuperAttacks.Count; i++)
            {
                superAttackList.Add(moveList.BasicSuperAttacks[i].CreateMoveListData());
            }
            MovelistPages.Add(superPage);
            BuildPageData(superPage, superAttackList, "Super Attacks");
        }
        #endregion
        SetActivePage();
    }
    public void CyclePageUp() 
    {
        pageIndex++;
        if (pageIndex > MovelistPages.Count-1)
        {
            pageIndex = 0;
        }
        SetActivePage();
    }
    public void CyclePageDown() 
    {
        pageIndex--;
        if (pageIndex < 0) 
        {
            pageIndex = MovelistPages.Count;
        }
        SetActivePage();
    }
    void SetActivePage() 
    {
        for(int i =0; i < MovelistPages.Count; i++) 
        {
            bool activeState = i == pageIndex ? true : false;
            MovelistPages[i].SetActive(activeState);
        }
    }
    #region Layered Data Generation
    public void BuildLayeredPageData(GameObject CurrentPage, List<MoveListAttackInfo> attackTypeData, string headerMessage)
    {
        TMP_Text headerText = CurrentPage.GetComponentInChildren<TMP_Text>();
        headerText.SetText(SpriteToTextColorUtility.AppendSpriteName(headerMessage.ToUpper(), Color.white));
        GridLayoutGroup layout = CurrentPage.GetComponentInChildren<GridLayoutGroup>();
        GameObject curMoveTextAsset = GameObject.Instantiate(TextSample_Group, layout.transform);
        curMoveTextAsset.GetComponentInChildren<TMP_Text>().SetText(attackTypeData[0].SpecialAttackName);
        layout.cellSize = new Vector2(390,180);
        layout.spacing = new Vector2(35, 20);
        layout.padding.top = 40;
        layout.constraintCount = 2;
        curMoveTextAsset.GetComponent<Movelist_Display>().SetTextData(attackTypeData[0]);
        for (int i = 1; i < attackTypeData.Count; i++)
        {
            MakeAndSetLayeredText(attackTypeData[i], curMoveTextAsset.GetComponentInChildren<GridLayoutGroup>().transform);
        }
    }
    public void MakeAndSetLayeredText(MoveListAttackInfo newAttack, Transform location)
    {
        GameObject curMoveTextAsset = GameObject.Instantiate(TextSample, location);
        Movelist_Display objectDisplay = curMoveTextAsset.GetComponent<Movelist_Display>();
        objectDisplay.SetTextData(newAttack);
    }
    #endregion
    #region Base Data Generation
    public void BuildPageData(GameObject CurrentPage, List<MoveListAttackInfo> attackTypeData, string headerMessage) 
    {
        TMP_Text headerText = CurrentPage.GetComponentInChildren<TMP_Text>();
        headerText.SetText(SpriteToTextColorUtility.AppendSpriteName(headerMessage.ToUpper(), Color.white));
        GridLayoutGroup layout = CurrentPage.GetComponentInChildren<GridLayoutGroup>();
        SetGridLayoutSizing(layout, attackTypeData.Count);
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
    #endregion
    public void SetGridLayoutSizing(GridLayoutGroup currentLayout, int count) 
    {
        Vector2 sizing = new Vector2(391, 75);
        if (count > 5) 
        {
            if (count > 10)
            {
                sizing = new Vector2(391,35);
            }
            currentLayout.constraintCount = 2;
        }
        else
        {
            currentLayout.constraintCount = 1;
        }
        currentLayout.cellSize = sizing;
    }
}

