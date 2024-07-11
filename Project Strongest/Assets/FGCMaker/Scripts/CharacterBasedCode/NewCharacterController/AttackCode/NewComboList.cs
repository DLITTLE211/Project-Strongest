using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "New Combo List", menuName = "Combo List 2.0")]
public class NewComboList : ScriptableObject
{
    public List<Attack_StanceSpecialMove> StanceSpecials;
    public List<Attack_RekkaSpecialMove> RekkaSpecials;
    public List<Attack_BasicSpecialMove> Special_Simple;
    public List<Attack_NonSpecialAttack> SimpleAttacks;
    public Path_Data currentPathData;
    public void SetCombos()
    {
        SimpleAttacks[0]._attackInput.ActivateAttackInfo();
    }
    public void UpdatePathData(Path_Data newPathData) 
    {
        currentPathData = newPathData;
    }
    public void CheckAndApply(Attack_BaseProperties attack, Character_Base target, Character_Base attacker, bool blockedAttack)
    {
        if (!blockedAttack)
        {
            #region Hit Character Attack Check
            switch (attack._moveType)
            {
                case MoveType.Normal:
                    for (int i = 0; i < SimpleAttacks.Count; i++)
                    {
                        try
                        {
                            if (SimpleAttacks[i]._attackInput._correctInput[currentPathData._curInputPath].property.AttackAnims.animName == attack.AttackAnims.animName)
                            {
                                attacker._cComboCounter.OnHit_CountUp();
                                SimpleAttacks[i].SendCounterHitInfo(currentPathData, target);
                                SimpleAttacks[i].SendSuccessfulDamageInfo(currentPathData, target);
                                //lastProperty = SimpleAttacks[i]._attackInput._correctInput[currentPathData._curInputPath].property;
                                return;
                            }
                            else { continue; }
                        }
                        catch (ArgumentOutOfRangeException) { continue; }
                    }
                    break;
                case MoveType.BasicSpeical:

                    for (int i = 0; i < Special_Simple.Count; i++)
                    {
                        if (Special_Simple[i].property.AttackAnims.animName == attack.AttackAnims.animName)
                        {
                            attacker._cComboCounter.OnHit_CountUp();
                            Special_Simple[i].SendSuccessfulDamageInfo(target, false);
                            Special_Simple[i].SendCounterHitInfo(target);
                            //lastProperty = Special_Simple[i].property;
                            return;
                        }
                        else { continue; }
                    }
                    break;

                case MoveType.Rekka:
                    Attack_RekkaSpecialMove correctRekkaMove = GetRekkaAttack(attack);
                    if (correctRekkaMove != null)
                    {
                        if (correctRekkaMove.rekkaInput.mainAttackProperty.cancelProperty.cancelTo == Cancel_State.Rekka_Input_Start)
                        {
                            attacker._cComboCounter.OnHit_CountUp();
                            correctRekkaMove.SendSuccessfulDamageInfo(target, false);
                        }
                    }
                    else
                    {
                        (Attack_RekkaSpecialMove, RekkaAttack) mainRekkaAttack = GetInnerRekkaAttack(attack);
                        if (mainRekkaAttack.Item1.inRekkaState == true)
                        {
                            attacker._cComboCounter.OnHit_CountUp();
                            mainRekkaAttack.Item1.SendSuccessfulDamageInfo(target, false, mainRekkaAttack.Item2);
                        }
                    }
                    break;

                case MoveType.Stance:
                    Attack_StanceSpecialMove correctStanceMove = GetStanceAttack(attack);
                    if (correctStanceMove != null)
                    {
                        attacker._cComboCounter.OnHit_CountUp();
                        GetStanceAttack(attack).SendSuccessfulDamageInfo(target, false);
                    }
                    else
                    {
                        (Attack_StanceSpecialMove, StanceAttack) stance_SubAttacks = GetInnerStanceAttack(attack);
                        attacker._cComboCounter.OnHit_CountUp();
                        stance_SubAttacks.Item1.SendSuccessfulDamageInfo(target, false,stance_SubAttacks.Item2);

                    }
                    break;
            }
            #endregion
        }
        else 
        {
            #region Blocked Attack Check Region
            switch (attack._moveType)
            {
                case MoveType.Normal:
                    for (int i = 0; i < SimpleAttacks.Count; i++)
                    {
                        try
                        {
                            if (SimpleAttacks[i]._attackInput._correctInput[currentPathData._curInputPath].property.AttackAnims.animName == attack.AttackAnims.animName)
                            {
                                //Switch to new function of  SendChipDamageInfo();
                                SimpleAttacks[i].SendSuccessfulDamageInfo(currentPathData, target);
                                return;
                            }
                            else 
                            { 
                                continue; 
                            }
                        }
                        catch (ArgumentOutOfRangeException) { continue; }
                    }
                    break;
                case MoveType.BasicSpeical:

                    for (int i = 0; i < Special_Simple.Count; i++)
                    {
                        if (Special_Simple[i].property.AttackAnims.animName == attack.AttackAnims.animName)
                        {
                            //Switch to new function of  SendChipDamageInfo();
                            Special_Simple[i].SendSuccessfulDamageInfo(target, true);
                            return;
                        }
                        else { continue; }
                    }
                    break;

                case MoveType.Rekka:
                    Attack_RekkaSpecialMove correctRekkaMove = GetRekkaAttack(attack);
                    if (correctRekkaMove != null)
                    {
                        if (correctRekkaMove.rekkaInput.mainAttackProperty.cancelProperty.cancelTo == Cancel_State.Rekka_Input_Start)
                        {
                            //Switch to new function of  SendChipDamageInfo();
                            correctRekkaMove.SendSuccessfulDamageInfo(target,true);
                        }
                    }
                    else
                    {
                        (Attack_RekkaSpecialMove, RekkaAttack) mainRekkaAttack = GetInnerRekkaAttack(attack);
                        if (mainRekkaAttack.Item1.inRekkaState == true)
                        {
                            //Switch to new function of  SendChipDamageInfo();
                            mainRekkaAttack.Item1.SendSuccessfulDamageInfo(target, true,mainRekkaAttack.Item2);
                        }
                    }
                    break;

                case MoveType.Stance:
                    Attack_StanceSpecialMove correctStanceMove = GetStanceAttack(attack);
                    if (correctStanceMove != null)
                    {
                        //Switch to new function of  SendChipDamageInfo();
                        GetStanceAttack(attack).SendSuccessfulDamageInfo(target,true);
                    }
                    else
                    {
                        (Attack_StanceSpecialMove, StanceAttack) stance_SubAttacks = GetInnerStanceAttack(attack);
                        //Switch to new function of  SendChipDamageInfo();
                        stance_SubAttacks.Item1.SendSuccessfulDamageInfo(target,true, stance_SubAttacks.Item2);

                    }
                    break;
            }
            #endregion
        }
    }

    #region Stance Verification Code
    public Attack_StanceSpecialMove GetStanceAttack(Attack_BaseProperties attack)
    {
        for (int i = 0; i < StanceSpecials.Count; i++) 
        {
            if (StanceSpecials[i].stanceStartProperty == attack) 
            {
                return StanceSpecials[i];
            }
        }
        return null;
    }
    public (Attack_StanceSpecialMove, StanceAttack) GetInnerStanceAttack(Attack_BaseProperties attack)
    {
        for (int i = 0; i < StanceSpecials.Count; i++)
        {
            if (StanceSpecials[i].stanceInput.stanceAttack._stanceButtonInput._correctInput[0].property == attack)
            {
                return (StanceSpecials[i], StanceSpecials[i].stanceInput.stanceAttack);
            }
            if (StanceSpecials[i].stanceInput.stanceKill._stanceButtonInput._correctInput[0].property == attack)
            {
                return (StanceSpecials[i], StanceSpecials[i].stanceInput.stanceKill);
            }
        }
        return (null,null);
    }
    #endregion

    #region Rekka Verification Code
    public Attack_RekkaSpecialMove GetRekkaAttack(Attack_BaseProperties attack)
    {
        for (int i = 0; i < RekkaSpecials.Count; i++)
        {
            if (RekkaSpecials[i].rekkaInput.mainAttackProperty == attack)
            {
                return RekkaSpecials[i];
            }
        }
        return null;
    }
    public (Attack_RekkaSpecialMove, RekkaAttack) GetInnerRekkaAttack(Attack_BaseProperties attack)
    {
        for (int i = 0; i < RekkaSpecials.Count; i++)
        {
            for (int j = 0; j < RekkaSpecials[i].rekkaInput._rekkaPortion.Count; j++)
            {
                if (RekkaSpecials[i].rekkaInput._rekkaPortion[j].individualRekkaAttack._correctInput[0].property == attack)
                {
                    return (RekkaSpecials[i], RekkaSpecials[i].rekkaInput._rekkaPortion[j]);
                }
            }
        }
        return (null,null);
    }
    public Attack_RekkaSpecialMove GetRekkaRouteAttack(Attack_BaseProperties attack)
    {
        for (int i = 0; i < RekkaSpecials.Count; i++)
        {
            for (int j = 0; j < RekkaSpecials[i].rekkaInput._rekkaPortion.Count; j++)
            {
                if (RekkaSpecials[i].rekkaInput._rekkaPortion[j].individualRekkaAttack._correctInput[0].property == attack)
                {
                    return RekkaSpecials[i];
                }
            }
        }
        return null;
    }
    #endregion
}
