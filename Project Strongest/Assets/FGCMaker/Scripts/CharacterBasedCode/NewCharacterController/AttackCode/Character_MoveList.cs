using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Character_MoveList : MonoBehaviour
{
    [SerializeField] protected internal List<Attack_StanceSpecialMove> stanceSpecials;
    [SerializeField] protected internal List<Attack_RekkaSpecialMove> rekkaSpecials;
    [SerializeField] protected internal List<Attack_BasicSpecialMove> special_Simple;
    [SerializeField] protected internal List<Attack_NonSpecialAttack> simpleAttacks;
    [SerializeField] protected internal Path_Data currentPathData;

    public void UpdatePathData(Path_Data _pathData) 
    {
        currentPathData = _pathData;
    }
    public void CheckAndApply(Attack_BaseProperties attack, Character_Base target, Character_Base attacker, bool blockedAttack)
    {
        if (!blockedAttack)
        {
            switch (attack._moveType)
            {
                case MoveType.Normal:
                    for (int i = 0; i < simpleAttacks.Count; i++)
                    {
                        try
                        {
                            if (simpleAttacks[i]._attackInput._correctInput[currentPathData._curInputPath].property.AttackAnims.animName == attack.AttackAnims.animName)
                            {
                                attacker._cComboCounter.OnHit_CountUp();
                                simpleAttacks[i].SendCounterHitInfo(currentPathData, target);
                                simpleAttacks[i].SendSuccessfulDamageInfo(currentPathData, target);
                                //lastProperty = SimpleAttacks[i]._attackInput._correctInput[currentPathData._curInputPath].property;
                                return;
                            }
                            else { continue; }
                        }
                        catch (ArgumentOutOfRangeException) 
                        { continue; }
                    }
                    break;
                case MoveType.BasicSpeical:

                    for (int i = 0; i < special_Simple.Count; i++)
                    {
                        if (special_Simple[i].property.AttackAnims.animName == attack.AttackAnims.animName)
                        {
                            attacker._cComboCounter.OnHit_CountUp();
                            special_Simple[i].SendSuccessfulDamageInfo(target, false);
                            special_Simple[i].SendCounterHitInfo(target);
                            //lastProperty = Special_Simple[i].property;
                            return;
                        }
                        else 
                        { continue; }
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
                        stance_SubAttacks.Item1.SendSuccessfulDamageInfo(target, false, stance_SubAttacks.Item2);

                    }
                    break;
            }
        }
        else
        {
            switch (attack._moveType)
            {
                case MoveType.Normal:
                    for (int i = 0; i < simpleAttacks.Count; i++)
                    {
                        try
                        {
                            if (simpleAttacks[i]._attackInput._correctInput[currentPathData._curInputPath].property.AttackAnims.animName == attack.AttackAnims.animName)
                            {
                                //Switch to new function of  SendChipDamageInfo();
                                simpleAttacks[i].SendSuccessfulDamageInfo(currentPathData, target, blockedAttack);
                                return;
                            }
                            else
                            {continue;}
                        }
                        catch (ArgumentOutOfRangeException) 
                        { continue; }
                    }
                    break;
                case MoveType.BasicSpeical:

                    for (int i = 0; i < special_Simple.Count; i++)
                    {
                        if (special_Simple[i].property.AttackAnims.animName == attack.AttackAnims.animName)
                        {
                            //Switch to new function of  SendChipDamageInfo();
                            special_Simple[i].SendSuccessfulDamageInfo(target, blockedAttack);
                            return;
                        }
                        else 
                        { continue; }
                    }
                    break;

                case MoveType.Rekka:
                    Attack_RekkaSpecialMove correctRekkaMove = GetRekkaAttack(attack);
                    if (correctRekkaMove != null)
                    {
                        if (correctRekkaMove.rekkaInput.mainAttackProperty.cancelProperty.cancelTo == Cancel_State.Rekka_Input_Start)
                        {
                            //Switch to new function of  SendChipDamageInfo();
                            correctRekkaMove.SendSuccessfulDamageInfo(target, blockedAttack);
                        }
                    }
                    else
                    {
                        (Attack_RekkaSpecialMove, RekkaAttack) mainRekkaAttack = GetInnerRekkaAttack(attack);
                        if (mainRekkaAttack.Item1.inRekkaState == true)
                        {
                            //Switch to new function of  SendChipDamageInfo();
                            mainRekkaAttack.Item1.SendSuccessfulDamageInfo(target, blockedAttack, mainRekkaAttack.Item2);
                        }
                    }
                    break;

                case MoveType.Stance:
                    Attack_StanceSpecialMove correctStanceMove = GetStanceAttack(attack);
                    if (correctStanceMove != null)
                    {
                        //Switch to new function of  SendChipDamageInfo();
                        GetStanceAttack(attack).SendSuccessfulDamageInfo(target, blockedAttack);
                    }
                    else
                    {
                        (Attack_StanceSpecialMove, StanceAttack) stance_SubAttacks = GetInnerStanceAttack(attack);
                        //Switch to new function of  SendChipDamageInfo();
                        stance_SubAttacks.Item1.SendSuccessfulDamageInfo(target, blockedAttack, stance_SubAttacks.Item2);

                    }
                    break;
            }
        }
    }

    #region Stance Verification Code
    public Attack_StanceSpecialMove GetStanceAttack(Attack_BaseProperties attack)
    {
        for (int i = 0; i < stanceSpecials.Count; i++)
        {
            if (stanceSpecials[i].stanceStartProperty == attack)
            {
                return stanceSpecials[i];
            }
        }
        return null;
    }
    public (Attack_StanceSpecialMove, StanceAttack) GetInnerStanceAttack(Attack_BaseProperties attack)
    {
        for (int i = 0; i < stanceSpecials.Count; i++)
        {
            if (stanceSpecials[i].stanceInput.stanceAttack._stanceButtonInput._correctInput[0].property == attack)
            {
                return (stanceSpecials[i], stanceSpecials[i].stanceInput.stanceAttack);
            }
            if (stanceSpecials[i].stanceInput.stanceKill._stanceButtonInput._correctInput[0].property == attack)
            {
                return (stanceSpecials[i], stanceSpecials[i].stanceInput.stanceKill);
            }
        }
        return (null, null);
    }
    #endregion

    #region Rekka Verification Code
    public Attack_RekkaSpecialMove GetRekkaAttack(Attack_BaseProperties attack)
    {
        for (int i = 0; i < rekkaSpecials.Count; i++)
        {
            if (rekkaSpecials[i].rekkaInput.mainAttackProperty == attack)
            {
                return rekkaSpecials[i];
            }
        }
        return null;
    }
    public (Attack_RekkaSpecialMove, RekkaAttack) GetInnerRekkaAttack(Attack_BaseProperties attack)
    {
        for (int i = 0; i < rekkaSpecials.Count; i++)
        {
            for (int j = 0; j < rekkaSpecials[i].rekkaInput._rekkaPortion.Count; j++)
            {
                if (rekkaSpecials[i].rekkaInput._rekkaPortion[j].individualRekkaAttack._correctInput[0].property == attack)
                {
                    return (rekkaSpecials[i], rekkaSpecials[i].rekkaInput._rekkaPortion[j]);
                }
            }
        }
        return (null, null);
    }
    public Attack_RekkaSpecialMove GetRekkaRouteAttack(Attack_BaseProperties attack)
    {
        for (int i = 0; i < rekkaSpecials.Count; i++)
        {
            for (int j = 0; j < rekkaSpecials[i].rekkaInput._rekkaPortion.Count; j++)
            {
                if (rekkaSpecials[i].rekkaInput._rekkaPortion[j].individualRekkaAttack._correctInput[0].property == attack)
                {
                    return rekkaSpecials[i];
                }
            }
        }
        return null;
    }
    #endregion
}
