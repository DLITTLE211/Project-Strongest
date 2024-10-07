using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Character_MoveList : MonoBehaviour
{
    [Header("_____Basic Supers_____")]
    [SerializeField] protected internal List<Attack_AdvancedSpecialMove> BasicSuperAttacks;
    [Space(15)]
    [Header("_____Command Grabs_____")]
    [SerializeField] protected internal List<Attack_AdvancedSpecialMove> CommandThrows;
    [Space(15)]
    [Header("_____Counters_____")]
    [SerializeField] protected internal List<Attack_AdvancedSpecialMove> CounterAttacks;
    [Space(15)]
    [Header("_____Stance Special Moves_____")]
    [SerializeField] protected internal List<Attack_StanceSpecialMove> stanceSpecials;
    [Space(15)]
    [Space(15)]
    [Header("_____Rekka Special Moves_____")]
    [SerializeField] protected internal List<Attack_RekkaSpecialMove> rekkaSpecials;
    [Space(15)]
    [Space(15)]
    [Header("_____Basic Special Moves_____")]
    [SerializeField] protected internal List<Attack_BasicSpecialMove> special_Simple;
    [Space(15)]
    [Space(15)]
    [Header("_____Simple Attacks_____")]
    [SerializeField] protected internal List<Attack_NonSpecialAttack> stringNormalAttacks;
    [Space(15)]
    [Header("_____Simple Attacks_____")]
    [SerializeField] protected internal List<Attack_NonSpecialAttack> commandNormalAttacks;
    [Space(15)]
    [Header("_____Simple Attacks_____")]
    [SerializeField] protected internal List<Attack_NonSpecialAttack> simpleAttacks;
    [Space(15)]
    [Header("_____Basic Throws_____")]
    [SerializeField] protected internal List<Attack_ThrowBase> BasicThrows;

    [Header("KeyValue Property")]
    [Space(15)]
    [SerializeField] private KeyValuePair<AttackInputTypes, IAttackFunctionality> currentAttack;
    [SerializeField] private Attack_BaseProperties followUp;

    public void SetCurrentAttack(KeyValuePair<AttackInputTypes, IAttackFunctionality> newAttack)
    {
        Debug.LogError("Re-Assigned Attack Value in setcurrentattack");
        currentAttack = newAttack;
    }
    public IAttackFunctionality ReturnCurrentAttack() 
    {
        return currentAttack.Value;
    }
    public MoveType GetCurrentActiveMoveType() 
    {
        if(currentAttack.Value != null) 
        {
            return currentAttack.Value.GetAttackMoveType();
        }
        return (MoveType)0;
    }
    public void SetFollowAttack(Attack_BaseProperties _followUp)
    {
        followUp = _followUp;
    }
    public void ClearFollowUpAttack() 
    {
        SetFollowAttack(null);
    }
    public void ClearAttackData(IState state, IState attacking)
    {
        if(state == attacking) 
        {
            return;
        }
        currentAttack = new KeyValuePair<AttackInputTypes, IAttackFunctionality>(null, null);
        followUp = null;
    }

    public void ExtractBaseProperties(Character_Base baseCharacterInfo)
    {
        #region Movelist Setup
        GetNormalAttacks(baseCharacterInfo);

        GetCommandAttacks(baseCharacterInfo);

        GetStringAttacks(baseCharacterInfo);

        GetThrows(baseCharacterInfo);

        GetSimpleSpecials(baseCharacterInfo);

        GetRekkaSpecials(baseCharacterInfo);

        GetStanceAttacks(baseCharacterInfo);

        GetCounterSpecials(baseCharacterInfo);

        GetCommandThrowSpecials(baseCharacterInfo);

        GetCustomSuperSpecials(baseCharacterInfo);
        #endregion
        
    }
    public int GetCurrentSuperCustomAnimLength() 
    {
        return currentAttack.Value.GetCustomAnimLength();
    }

    #region Extract AttackType Functions
    public void GetNormalAttacks(Character_Base baseCharacterInfo)
    {
        for (int i = 0; i < simpleAttacks.Count; i++)
        {
            simpleAttacks[i].SetStarterInformation(baseCharacterInfo);
            for (int j = 0; j < simpleAttacks[i]._attackInput._correctInput.Count; j++)
            {
                simpleAttacks[i]._attackInput._correctInput[j].SetInnerAttackAnimations(baseCharacterInfo._cAnimator);
            }
        }
    }
    public void GetCommandAttacks(Character_Base baseCharacterInfo)
    {
        for (int i = 0; i < commandNormalAttacks.Count; i++)
        {
            commandNormalAttacks[i].SetStarterInformation(baseCharacterInfo);
            for (int j = 0; j < simpleAttacks[i]._attackInput._correctInput.Count; j++)
            {
                commandNormalAttacks[i]._attackInput._correctInput[j].SetInnerAttackAnimations(baseCharacterInfo._cAnimator);
            }
        }
    }
    public void GetStringAttacks(Character_Base baseCharacterInfo)
    {
        for (int i = 0; i < stringNormalAttacks.Count; i++)
        {
            stringNormalAttacks[i].SetStarterInformation(baseCharacterInfo);
            for (int j = 0; j < stringNormalAttacks[i]._attackInput._correctInput.Count; j++)
            {
                stringNormalAttacks[i]._attackInput._correctInput[j].SetInnerAttackAnimations(baseCharacterInfo._cAnimator);
            }
        }
    }
    public void GetThrows(Character_Base baseCharacterInfo)
    {
        for (int i = 0; i < BasicThrows.Count; i++)
        {
            BasicThrows[i].SetStarterInformation(baseCharacterInfo);
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
            special_Simple[i].SetStarterInformation(baseCharacterInfo);
            special_Simple[i].property.SetAttackAnims(baseCharacterInfo._cAnimator);
        }
    }
    public void GetRekkaSpecials(Character_Base baseCharacterInfo)
    {
        for (int i = 0; i < rekkaSpecials.Count; i++)
        {
            rekkaSpecials[i].SetStarterInformation(baseCharacterInfo);
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
            stanceSpecials[i].SetStarterInformation(baseCharacterInfo);
        }
    }
    public void GetCounterSpecials(Character_Base baseCharacterInfo)
    {
        for (int i = 0; i < CounterAttacks.Count; i++)
        {
            CounterAttacks[i].SetStarterInformation(baseCharacterInfo);
            CounterAttacks[i].property.SetAttackAnims(baseCharacterInfo._cAnimator);
            for (int j = 0; j < CounterAttacks[i]._customAnimation.Count; j++)
            {
                CounterAttacks[i]._customAnimation[j].SetAttackAnim(baseCharacterInfo._cAnimator);
            }
        }
    }
    public void GetCommandThrowSpecials(Character_Base baseCharacterInfo)
    {
        for (int i = 0; i < CommandThrows.Count; i++)
        {
            CommandThrows[i].SetStarterInformation(baseCharacterInfo);
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
            BasicSuperAttacks[i].SetStarterInformation(baseCharacterInfo);
            BasicSuperAttacks[i].property.SetAttackAnims(baseCharacterInfo._cAnimator);
            for (int j = 0; j < BasicSuperAttacks[i]._customAnimation.Count; j++)
            {
                BasicSuperAttacks[i]._customAnimation[j].SetAttackAnim(baseCharacterInfo._cAnimator);
            }
        }
    }
    #endregion

    public void SetupCharacterTotalMoveList(Dictionary<AttackInputTypes, IAttackFunctionality> totalAttackDictionary, string characterName, List<AttackInputTypes> _types) 
    {
        string errorMessage = "";
        try
        {
            #region Super Registration
            errorMessage = "Start Super Addition";
            for (int i = 0; i < BasicSuperAttacks.Count; i++)
            {
                for (int j = 0; j < BasicSuperAttacks[i].attackInput.Count; j++)
                {
                    AttackInputTypes superAttackType = new AttackInputTypes(BasicSuperAttacks[i].attackInput[j], null, BasicSuperAttacks[i].property._moveType);
                    totalAttackDictionary.Add(superAttackType, BasicSuperAttacks[i]);
                    _types.Add(superAttackType);
                }
            }
            #endregion

            #region CommandThrow Registration
            errorMessage = "Start Command Throw Addition";
            for (int i = 0; i < CommandThrows.Count; i++)
            {
                for (int j = 0; j < CommandThrows[i].attackInput.Count; j++)
                {
                    AttackInputTypes commandThrowType = new AttackInputTypes(CommandThrows[i].attackInput[j], null, CommandThrows[i].property._moveType);
                    totalAttackDictionary.Add(commandThrowType, CommandThrows[i]);
                    _types.Add(commandThrowType);
                }
            }
            #endregion

            #region Counter Registration
            errorMessage = "Start Counter Addition";
            for (int i = 0; i < CounterAttacks.Count; i++)
            {
                for (int j = 0; j < CounterAttacks[i].attackInput.Count; j++)
                {
                    AttackInputTypes counterInputType = new AttackInputTypes(CounterAttacks[i].attackInput[j], null, CounterAttacks[i].property._moveType);
                    totalAttackDictionary.Add(counterInputType, CounterAttacks[i]);
                    _types.Add(counterInputType);
                }
            }

            #endregion

            #region Stance Registration
            errorMessage = "Start Stance Attack Addition";
            for (int i = 0; i < stanceSpecials.Count; i++)
            {
                List<string> totalSubInputs = new List<string>();
                List<Attack_BaseInput> stanceAttacks = stanceSpecials[i].stanceInput.stanceAttack._stanceButtonInput._correctInput;
                List<Attack_BaseInput> stanceKills = stanceSpecials[i].stanceInput.stanceKill._stanceButtonInput._correctInput;
                for (int j = 0; j < stanceAttacks.Count; j++)
                {
                    totalSubInputs.Add(stanceAttacks[j]._correctSequence);
                }
                for (int j = 0; j < stanceKills.Count; j++)
                {
                    totalSubInputs.Add(stanceKills[j]._correctSequence);
                }
                for (int j = 0; j < stanceSpecials[i].stanceInput._stanceInput.Count; j++)
                {
                    AttackInputTypes stanceInputType = new AttackInputTypes(stanceSpecials[i].stanceInput._stanceInput[j], totalSubInputs, stanceSpecials[i].stanceStartProperty._moveType);
                    totalAttackDictionary.Add(stanceInputType, stanceSpecials[i]);
                    _types.Add(stanceInputType);
                }
            }
            #endregion

            #region Rekka Registration
            errorMessage = "Start Rekka Attack Addition";
            for (int i = 0; i < rekkaSpecials.Count; i++)
            {
                List<string> totalSubInputs = new List<string>();
                for (int j = 0; j < rekkaSpecials[i].rekkaInput._rekkaPortion.Count; j++) 
                {
                    for (int k = 0; k < rekkaSpecials[i].rekkaInput._rekkaPortion[j].individualRekkaAttack._correctInput.Count; k++)
                    {
                        totalSubInputs.Add(rekkaSpecials[i].rekkaInput._rekkaPortion[j].individualRekkaAttack._correctInput[k]._correctSequence);
                    }
                }
                for (int j = 0; j < rekkaSpecials[i].rekkaInput.mainAttackInput.Count; j++)
                {
                    AttackInputTypes rekkaInputType = new AttackInputTypes(rekkaSpecials[i].rekkaInput.mainAttackInput[j], totalSubInputs, rekkaSpecials[i].rekkaInput.mainAttackProperty._moveType);
                    totalAttackDictionary.Add(rekkaInputType, rekkaSpecials[i]);
                    _types.Add(rekkaInputType);
                }
            }
            #endregion

            #region Basic Special Registration
            errorMessage = "Start Basic Attack Addition";
            for (int i = 0; i < special_Simple.Count; i++)
            {
                for (int j = 0; j < special_Simple[i].attackInput.Count; j++) 
                {
                    AttackInputTypes superInputType = new AttackInputTypes(special_Simple[i].attackInput[j], null, special_Simple[i].property._moveType);
                    totalAttackDictionary.Add(superInputType, special_Simple[i]);
                    _types.Add(superInputType);
                }
            }
            #endregion

            #region String Normals Registration
            errorMessage = "Start String Normal Attack Addition";
            for (int i = 0; i < stringNormalAttacks.Count; i++)
            {
                List<string> totalSubInputs = new List<string>();
                for (int j = 0; j < stringNormalAttacks[i]._attackInput._correctInput.Count; j++)
                {
                    totalSubInputs.Add(stringNormalAttacks[i]._attackInput._correctInput[j]._correctSequence);
                }
                AttackInputTypes superInputType = new AttackInputTypes(null, totalSubInputs, stringNormalAttacks[i]._attackInput._correctInput[0].property._moveType, stringNormalAttacks[i]._attackInput._correctInput[0].property._airInfo);
                totalAttackDictionary.Add(superInputType, stringNormalAttacks[i]);
                _types.Add(superInputType);
            }
            #endregion

            #region Command Normals Registration
            errorMessage = "Start Command Normal Attack Addition";
            for (int i = 0; i < commandNormalAttacks.Count; i++)
            {
                List<string> totalSubInputs = new List<string>();
                for (int j = 0; j < commandNormalAttacks[i]._attackInput._correctInput.Count; j++)
                {
                    totalSubInputs.Add(commandNormalAttacks[i]._attackInput._correctInput[j]._correctSequence);
                }
                AttackInputTypes superInputType = new AttackInputTypes(null, totalSubInputs, commandNormalAttacks[i]._attackInput._correctInput[0].property._moveType, commandNormalAttacks[i]._attackInput._correctInput[0].property._airInfo);
                totalAttackDictionary.Add(superInputType, commandNormalAttacks[i]);
                _types.Add(superInputType);
            }
            #endregion

            #region Normals Registration
            errorMessage = "Start Normal Attack Addition";
            for (int i = 0; i < simpleAttacks.Count; i++)
            {
                List<string> totalSubInputs = new List<string>(); 
                for (int j = 0; j < simpleAttacks[i]._attackInput._correctInput.Count; j++)
                {
                    totalSubInputs.Add(simpleAttacks[i]._attackInput._correctInput[j]._correctSequence);
                }
                AttackInputTypes superInputType = new AttackInputTypes(null, totalSubInputs, simpleAttacks[i]._attackInput._correctInput[0].property._moveType, simpleAttacks[i]._attackInput._correctInput[0].property._airInfo);
                totalAttackDictionary.Add(superInputType, simpleAttacks[i]);
                _types.Add(superInputType);
            }
            #endregion

            #region Throw Registration
            errorMessage = "Start Throws Addition";
            for (int i = 0; i < BasicThrows.Count; i++)
            {
                List<string> totalSubInputs = new List<string>();
                for (int j = 0; j < BasicThrows[i]._attackInput._correctInput.Count; j++)
                {
                    totalSubInputs.Add(BasicThrows[i]._attackInput._correctInput[j]._correctSequence);
                }
                AttackInputTypes superInputType = new AttackInputTypes(null, totalSubInputs, BasicThrows[i]._attackInput._correctInput[0].property._moveType, BasicThrows[i]._attackInput._correctInput[0].property._airInfo);
                totalAttackDictionary.Add(superInputType, BasicThrows[i]);
                _types.Add(superInputType);
            }
            #endregion

            ////////////////////////////////////////////////////////////
            Debug.Log($"All Attacks Added to Dictionary. Have a \"Fight\"-Tastic Day, {characterName}!! ");
            Debug.Log($"Dictionary TotalCount is... {totalAttackDictionary.Count}.");
        }
        catch (Exception) 
        {
            Debug.LogError($"Failed to Add All Attacks to Dictionary. Stopping at \"{errorMessage}\". ");
            Debug.LogError($"Dictionary TotalCount is... {totalAttackDictionary.Count}.");
            Debug.Break();
        }
    }


    public void NewCheckAndApply(Character_Base target, Character_Base attacker, bool blockedAttack, Attack_BaseProperties attack, bool armoredHit = false) 
    {
        if (currentAttack.Value != null)
        {
            if (followUp != null)
            {
                if (blockedAttack)
                {
                    currentAttack.Value.HandleDamageDealing(attacker, target, blockedAttack, attack, followUp);
                    //Call interface function for dealing damage w/out block;
                }
                else
                {
                    //attacker._cComboCounter.OnHit_CountUp();
                    currentAttack.Value.HandleDamageDealing(attacker, target, blockedAttack, attack, followUp, armoredHit);
                    //Call interface function for dealing damage w/ block;
                }
            }
            else
            {
                if (blockedAttack)
                {
                    currentAttack.Value.HandleDamageDealing(attacker, target, blockedAttack, attack);
                    //Call interface function for dealing damage w/out block;
                }
                else
                {
                    //attacker._cComboCounter.OnHit_CountUp();
                    currentAttack.Value.HandleDamageDealing(attacker, target, blockedAttack, attack, null, armoredHit);
                    //Call interface function for dealing damage w/ block;
                }
            }
        }
        else 
        {
            //Debug.Break();
            Debug.LogError("Current Attack Value is Null");
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
    }/*
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
    }*/
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
    /*public (Attack_RekkaSpecialMove, RekkaAttack) GetInnerRekkaAttack(Attack_BaseProperties attack)
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
    }*/
    public Attack_RekkaSpecialMove GetRekkaRouteAttack(Attack_BaseProperties attack)
    {
        for (int i = 0; i < rekkaSpecials.Count; i++)
        {
            if (rekkaSpecials[i].rekkaInput._rekkaProperties.Contains(attack)) 
            {
                return rekkaSpecials[i];
            }
            
            /*for (int j = 0; j < rekkaSpecials[i].rekkaInput._rekkaPortion.Count; j++)
            {
                if (rekkaSpecials[i].rekkaInput._rekkaPortion[j].individualRekkaAttack._correctInput[0].property == attack)
                {
                    return rekkaSpecials[i];
                }
            }*/
        }
        return null;
    }
    #endregion
}
