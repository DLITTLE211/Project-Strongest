using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Character_MoveList : MonoBehaviour
{
    [Header("_____Basic Supers_____")]
    [SerializeField] protected internal List<Attack_AdvancedSpecialMove> BasicSuperAttacks;
    [Header("_____Command Grabs_____")]
    [SerializeField] protected internal List<Attack_AdvancedSpecialMove> CommandThrows;
    [Header("_____Counters_____")]
    [SerializeField] protected internal List<Attack_AdvancedSpecialMove> CounterAttacks;
    [Header("_____Stance Special Moves_____")]
    [SerializeField] protected internal List<Attack_StanceSpecialMove> stanceSpecials;
    [Space(15)]
    [Header("_____Rekka Special Moves_____")]
    [SerializeField] protected internal List<Attack_RekkaSpecialMove> rekkaSpecials;
    [Space(15)]
    [Header("_____Basic Special Moves_____")]
    [SerializeField] protected internal List<Attack_BasicSpecialMove> special_Simple;
    [Space(15)]
    [Header("_____Simple Attacks_____")]
    [SerializeField] protected internal List<Attack_NonSpecialAttack> simpleAttacks;
    [Space(15)]
    [Header("_____Basic Throws_____")]
    [SerializeField] protected internal List<Attack_ThrowBase> BasicThrows;
    [Header("Complete Movelist Properties")]
    [Space(15)]
    [SerializeField] private Dictionary<Attack_BaseProperties, Attack_NonSpecialAttack> simpleAttackProperties;
    [SerializeField] private List<Attack_BaseProperties> basicSpecialProperties;
    [SerializeField] private List<Attack_BaseProperties> rekkaBaseProperties;
    [SerializeField] private List<Attack_BaseProperties> rekkaSubAttackProperties;
    [SerializeField] private List<Attack_BaseProperties> stanceMainProperties;
    [SerializeField] private List<Attack_BaseProperties> stanceAttackProperties;
    [SerializeField] private List<Attack_BaseProperties> stanceKillProperties;
    [SerializeField] private List<Attack_BaseProperties> counterProperties;
    [SerializeField] private List<Attack_BaseProperties> commandThrowProperties;
    [SerializeField] private List<Attack_BaseProperties> basicSuperAttackProperties;
    [Header("PathData")]
    [Space(15)]
    [SerializeField] protected internal Path_Data currentPathData;

    public void UpdatePathData(Path_Data _pathData) 
    {
        currentPathData = _pathData;
    }
    public void ExtractBaseProperties(Character_Base baseCharacterInfo)
    {
        #region Simple Attacks Storage
        simpleAttackProperties = new Dictionary<Attack_BaseProperties, Attack_NonSpecialAttack>();
        GetNormalAttacks(baseCharacterInfo);
        for (int i = 0; i < simpleAttacks.Count; i++)
        {
            for (int j = 0; j < simpleAttacks[i]._attackInput._correctInput.Count; j++)
            {
                Attack_BaseProperties property = simpleAttacks[i]._attackInput._correctInput[j].property;
                string attackName = property._attackName;
                simpleAttackProperties.Add(property, simpleAttacks[i]);
            }
        }
        #endregion

        #region Throw Attack Storage
        basicSpecialProperties = new List<Attack_BaseProperties>();
        GetThrows(baseCharacterInfo);
        for (int i = 0; i < BasicThrows.Count; i++)
        {
            for (int j = 0; j < BasicThrows[i]._attackInput._correctInput.Count; j++)
            {
                basicSpecialProperties.Add(BasicThrows[i]._attackInput._correctInput[j].property);
            }
        }
        #endregion

        #region Simple Special Moves Storage
        GetSimpleSpecials(baseCharacterInfo);
        basicSpecialProperties = new List<Attack_BaseProperties>();
        for (int i = 0; i < special_Simple.Count; i++)
        {
            basicSpecialProperties.Add(special_Simple[i].property);
        }
        #endregion

        #region Rekka Move Storage
        GetRekkaSpecials(baseCharacterInfo);
        rekkaBaseProperties = new List<Attack_BaseProperties>();
        for (int i = 0; i < rekkaSpecials.Count; i++)
        {
            rekkaBaseProperties.Add(rekkaSpecials[i].rekkaInput.mainAttackProperty);
        }
        #region Rekka SubAttack Storage
        rekkaSubAttackProperties = new List<Attack_BaseProperties>();
        foreach (Attack_RekkaSpecialMove rekka in rekkaSpecials)
        {
            for (int j = 0; j < rekka.rekkaInput._rekkaPortion.Count; j++)
            {
                for (int k = 0; k < rekka.rekkaInput._rekkaPortion[j].individualRekkaAttack._correctInput.Count; k++)
                {
                    rekkaSubAttackProperties.Add(rekka.rekkaInput._rekkaPortion[j].individualRekkaAttack._correctInput[k].property);
                }
            }
        }
        #endregion
        #endregion

        #region Stance Special Storage
        GetStanceAttacks(baseCharacterInfo);
        stanceMainProperties = new List<Attack_BaseProperties>();
        stanceAttackProperties = new List<Attack_BaseProperties>();
        stanceKillProperties = new List<Attack_BaseProperties>();
        for (int i = 0; i < stanceSpecials.Count; i++)
        {
            stanceMainProperties.Add(stanceSpecials[i].stanceStartProperty);
            for (int j = 0; j < stanceSpecials[i].stanceInput.stanceAttack._stanceButtonInput._correctInput.Count; j++)
            {
                stanceAttackProperties.Add(stanceSpecials[i].stanceInput.stanceAttack._stanceButtonInput._correctInput[0].property);
            }
            for (int j = 0; j < stanceSpecials[i].stanceInput.stanceKill._stanceButtonInput._correctInput.Count; j++)
            {
                stanceKillProperties.Add(stanceSpecials[i].stanceInput.stanceKill._stanceButtonInput._correctInput[0].property);
            }
        }
        #endregion

        #region Counter Property Storage
        GetCounterSpecials(baseCharacterInfo);
        counterProperties = new List<Attack_BaseProperties>();
        for (int i = 0; i < CounterAttacks.Count; i++)
        {
            counterProperties.Add(CounterAttacks[i].property);
        }
        #endregion

        #region Command Throw Property Storage
        GetCommandThrowSpecials(baseCharacterInfo);
        commandThrowProperties = new List<Attack_BaseProperties>();
        for (int i = 0; i < CommandThrows.Count; i++)
        {
            commandThrowProperties.Add(CommandThrows[i].property);
        }
        #endregion

        #region Base Super Property Storage
        GetCustomSuperSpecials(baseCharacterInfo);
        basicSuperAttackProperties = new List<Attack_BaseProperties>();
        for (int i = 0; i < BasicSuperAttacks.Count; i++)
        {
            basicSuperAttackProperties.Add(BasicSuperAttacks[i].property);
        }
        #endregion
        
    }

    public void GetNormalAttacks(Character_Base baseCharacterInfo)
    {
        for (int i = 0; i < simpleAttacks.Count; i++)
        {
            simpleAttacks[i].SetComboTimer(baseCharacterInfo._cAttackTimer);
            simpleAttacks[i].SetStarterInformation();
            for (int j = 0; j < simpleAttacks[i]._attackInput._correctInput.Count; j++)
            {
                simpleAttacks[i]._attackInput._correctInput[j].SetInnerAttackAnimations(baseCharacterInfo._cAnimator);
            }
        }
    }
    public void GetThrows(Character_Base baseCharacterInfo)
    {
        for (int i = 0; i < BasicThrows.Count; i++)
        {
            BasicThrows[i].SetComboTimer(baseCharacterInfo._cAttackTimer);
            BasicThrows[i].SetStarterInformation();
            for (int j = 0; j < BasicThrows[i]._attackInput._correctInput.Count; j++)
            {
                BasicThrows[i]._attackInput._correctInput[j].SetInnerAttackAnimations(baseCharacterInfo._cAnimator);
            }
        }
    }
    public void GetSimpleSpecials(Character_Base baseCharacterInfo)
    {
        for (int i = 0; i < special_Simple.Count; i++)
        {
            special_Simple[i].SetComboTimer(baseCharacterInfo._cAttackTimer);
            special_Simple[i].TurnInputsToString();
            special_Simple[i].property.SetAttackAnims(baseCharacterInfo._cAnimator);
        }
    }
    public void GetRekkaSpecials(Character_Base baseCharacterInfo)
    {
        for (int i = 0; i < rekkaSpecials.Count; i++)
        {
            rekkaSpecials[i].SetComboTimer(baseCharacterInfo._cAttackTimer);
            rekkaSpecials[i].TurnInputsToString();
            rekkaSpecials[i].SetAttackAnims(baseCharacterInfo._cAnimator); 
            for (int j = 0; j < rekkaSpecials[i].rekkaInput._rekkaPortion.Count; j++)
            {
                rekkaSpecials[i].rekkaInput._rekkaPortion[j].individualRekkaAttack._correctInput[0].SetInnerAttackAnimations(baseCharacterInfo._cAnimator);
            }
            rekkaSpecials[i].rekkaInput.SetRekkaProperties();
        }
    }
    public void GetStanceAttacks(Character_Base baseCharacterInfo)
    {
        for (int i = 0; i < stanceSpecials.Count; i++)
        {
            stanceSpecials[i].SetComboTimer(baseCharacterInfo._cAttackTimer);
            stanceSpecials[i].TurnInputsToString();
            stanceSpecials[i].SetAttackAnims(baseCharacterInfo._cAnimator);
        }
    }
    public void GetCounterSpecials(Character_Base baseCharacterInfo)
    {
        for (int i = 0; i < CounterAttacks.Count; i++)
        {
            CounterAttacks[i].SetComboTimer(baseCharacterInfo._cAttackTimer);
            CounterAttacks[i].TurnInputsToString();
            CounterAttacks[i].property.SetAttackAnims(baseCharacterInfo._cAnimator);
            for(int j = 0; j < CounterAttacks[i]._customAnimation.Count; j++) 
            {
                CounterAttacks[i]._customAnimation[j].SetAttackAnim(baseCharacterInfo._cAnimator);
            }
        }
    }
    public void GetCommandThrowSpecials(Character_Base baseCharacterInfo)
    {
        for (int i = 0; i < CommandThrows.Count; i++)
        {
            CommandThrows[i].SetComboTimer(baseCharacterInfo._cAttackTimer);
            CommandThrows[i].TurnInputsToString();
            CommandThrows[i].property.SetAttackAnims(baseCharacterInfo._cAnimator);
            for (int j = 0; j < CommandThrows[i]._customAnimation.Count; j++)
            {
                CommandThrows[i]._customAnimation[j].SetAttackAnim(baseCharacterInfo._cAnimator);
            }
        }
    }
    public void GetCustomSuperSpecials(Character_Base baseCharacterInfo)
    {
        for (int i = 0; i < BasicSuperAttacks.Count; i++)
        {
            BasicSuperAttacks[i].SetComboTimer(baseCharacterInfo._cAttackTimer);
            BasicSuperAttacks[i].TurnInputsToString();
            BasicSuperAttacks[i].property.SetAttackAnims(baseCharacterInfo._cAnimator);
            for (int j = 0; j < BasicSuperAttacks[i]._customAnimation.Count; j++)
            {
                BasicSuperAttacks[i]._customAnimation[j].SetAttackAnim(baseCharacterInfo._cAnimator);
            }
        }
    }
    public void CheckAndApply(Attack_BaseProperties attack, Character_Base target, Character_Base attacker, bool blockedAttack)
    {
        if (!blockedAttack)
        {
            switch (attack._moveType)
            {
                case MoveType.Normal:
                    if (simpleAttackProperties.ContainsKey(attack))
                    {
                        Attack_NonSpecialAttack currentNormal = simpleAttackProperties[attack];
                        attacker._cComboCounter.OnHit_CountUp();
                        currentNormal.SendCounterHitInfo(attack, target);
                        currentNormal.SendSuccessfulDamageInfo(attack, target);
                        return;
                    }
                    break;
                case MoveType.String_Normal:
                    if (simpleAttackProperties.ContainsKey(attack))
                    {
                        Attack_NonSpecialAttack currentNormal = simpleAttackProperties[attack];
                        attacker._cComboCounter.OnHit_CountUp();
                        currentNormal.SendCounterHitInfo(attack, target);
                        currentNormal.SendSuccessfulDamageInfo(attack, target);
                        return;
                    }
                    break;
                case MoveType.Command_Normal:
                    if (simpleAttackProperties.ContainsKey(attack))
                    {
                        Attack_NonSpecialAttack currentNormal = simpleAttackProperties[attack];
                        attacker._cComboCounter.OnHit_CountUp();
                        currentNormal.SendCounterHitInfo(attack, target);
                        currentNormal.SendSuccessfulDamageInfo(attack, target);
                        return;
                    }
                    break;
                case MoveType.Throw:
                    for (int i = 0; i < BasicThrows.Count; i++)
                    {
                        try
                        {
                            if (BasicThrows[i]._attackInput._correctInput[0].property.AttackAnims.animName == attack.AttackAnims.animName)
                            {
                                BasicThrows[i].SendCounterHitInfo(currentPathData, target);
                                BasicThrows[i].SendSuccessfulDamageInfo(currentPathData, target);
                                BasicThrows[i].HandleThrowAnimAttackInfo();
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
                case MoveType.Counter:
                    for (int i = 0; i < CounterAttacks.Count; i++)
                    {
                        try
                        {
                            if (CounterAttacks[i].property.AttackAnims.animName == attack.AttackAnims.animName)
                            {
                                 CounterAttacks[i].SendCounterHitInfo(target);
                                CounterAttacks[i].SendSuccessfulDamageInfo(target, false);
                                CounterAttacks[i].HandleCounterAnimAttackInfo();
                                return;
                            }
                            else { continue; }
                        }
                        catch (Exception)
                        { continue; }
                    }
                    break;
                case MoveType.CommandGrab:
                    for (int i = 0; i < CommandThrows.Count; i++)
                    {
                        try
                        {
                            if (CommandThrows[i].property.AttackAnims.animName == attack.AttackAnims.animName)
                            {
                                CommandThrows[i].SendCounterHitInfo(target);
                                CommandThrows[i].SendSuccessfulDamageInfo(target, false);
                                CommandThrows[i].HandleCounterAnimAttackInfo();
                                return;
                            }
                            else { continue; }
                        }
                        catch (Exception)
                        { continue; }
                    }
                    break;
                case MoveType.Super:
                    for (int i = 0; i < BasicSuperAttacks.Count; i++)
                    {
                        try
                        {
                            if (BasicSuperAttacks[i].property.AttackAnims.animName == attack.AttackAnims.animName)
                            {
                                BasicSuperAttacks[i].SendCounterHitInfo(target);
                                BasicSuperAttacks[i].SendSuccessfulDamageInfo(target, false);
                                BasicSuperAttacks[i].HandleSuperMultipleAnimAttackInfo();

                                return;
                            }
                            else { continue; }
                        }
                        catch (Exception)
                        { continue; }
                    }
                    break;
            }
        }
        else
        {
            switch (attack._moveType)
            {
                case MoveType.Normal:

                    if (simpleAttackProperties.ContainsKey(attack))
                    {
                        Attack_NonSpecialAttack currentNormal = simpleAttackProperties[attack];
                        currentNormal.SendSuccessfulDamageInfo(attack, target, blockedAttack);
                        return;
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
