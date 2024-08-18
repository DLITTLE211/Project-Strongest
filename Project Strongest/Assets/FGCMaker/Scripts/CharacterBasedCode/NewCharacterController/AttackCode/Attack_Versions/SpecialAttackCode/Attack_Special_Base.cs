using System.Collections.Generic;
using System;
[Serializable]
public enum MoveType
{
    Normal,
    Throw,
    Command_Normal,
    String_Normal,
    BasicSpeical,
    Rekka,
    Stance,
    Counter,
    CommandGrab,
    Super,
    Key,

    /*
     * Basic special move: Completed. Search for input, upon completion 
     * Do attack matching attack string
     * 
     * Rekka: Similar to basic special except for key features
     * There will be a rekka point system. This will be used for keeping track of
     * how many attacks can be performed when in rekka state.
     * Attacks can be done in any order but will count down from point system until
     * reaching 0.
     * 
     * Stance: Also similar to basic special move except for one key feature.
     * There is no attack when doing the input.
     * There will be both a held and released check for the attack button 
     * on each given attack of this type
     * When released, attack information will be sent and done similar to a regular
     * attack
     */
}
#region Basic SpecialMove Code
public abstract class Attack_Special_Base 
{
    public string BasicSpecialAttack_Name;

    public Attack_BaseProperties property;
    public List<Attack_Input> attackInput;
    public ButtonStateMachine attackInputState;
}
#endregion

#region Rekka SpecialMove Code
public abstract class Attack_Special_Rekka
{
    public string RekkaSpecialAttack_Name;
    #region Function Summary
    /// <summary>
    /// Reset combo upon timer ending
    /// </summary>
    /// <param name="input"></param>
    /// <param name="curBase"></param>
    /// <returns></returns>
    #endregion
    public abstract void ResetCombo();

    public RekkaInput rekkaInput;
}
[Serializable]
public class RekkaInput
{
    public Attack_Input mainAttackInput;
    public Attack_BaseProperties mainAttackProperty;
    public ButtonStateMachine mainAttackInputState;
    public List<RekkaAttack> _rekkaPortion;
    public List<Attack_BaseProperties> _rekkaProperties;
    public void SetRekkaProperties() 
    {
        for (int i = 0; i < _rekkaPortion.Count; i++) 
        {
            _rekkaProperties.Add(_rekkaPortion[i].individualRekkaAttack._correctInput[0].property);
        }
    }
}
[Serializable]
public class RekkaAttack
{
    public Attack_BasicInput individualRekkaAttack;
}
#endregion

#region Stance SpecialMove Code
[Serializable]
public abstract class Attack_Special_Stance
{
    public string StanceSpecialAttack_Name;

    #region Function Summary
    /// <summary>
    /// Reset combo upon timer ending
    /// </summary>
    /// <param name="input"></param>
    /// <param name="curBase"></param>
    /// <returns></returns>
    #endregion
    public abstract void ResetCombo();

    public Attack_BaseProperties stanceStartProperty;
    public StanceInput stanceInput;
}
[Serializable]
public class StanceInput
{
    public Attack_Input _stanceInput;
    public ButtonStateMachine _stanceInputState;
    public StanceAttack stanceAttack;
    public StanceAttack stanceKill;
}
[Serializable]
public class StanceAttack
{
    public Attack_BasicInput _stanceButtonInput;
}
#endregion