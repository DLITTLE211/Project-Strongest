using System;
using System.Collections.Generic;
public interface IAttackFunctionality 
{
    //bool IsCorrectInput(Character_ButtonInput movementInput, Character_Base _curBase, int curInput, Character_ButtonInput attackInput = null);
    //bool CheckCombo(Character_ButtonInput Input, Character_Base curBase, Character_ButtonInput attackInput = null);
    void PreformAttack();
    void SendSuccessfulDamageInfo(Character_Base curBase, bool blockedAttack);
    void SendCounterHitInfo(Character_Base curBase);
    void SetComboTimer(Character_InputTimer_Attacks timer);
    void SetStarterInformation();

}

[Serializable]
public class AttackData 
{
    public Character_Base curBase;
    public Attack_BaseProperties mainRekka;
    public Attack_BaseProperties mainStance;
    public RekkaAttack rekkaAttack;
    public StanceAttack stanceAttack;
    public Attack_BaseInput normalAttack;
    public int stanceCurInput;
    public AttackData(Character_Base _curBase, RekkaAttack _rekkaAttack = null, StanceAttack _stanceAttack = null, int _stanceCurInput = -1, Attack_BaseProperties _mainRekka = null, Attack_BaseProperties _mainStance = null, Attack_BaseInput _normalAttack = null) 
    {
        curBase = _curBase;
        rekkaAttack = _rekkaAttack;
        stanceAttack = _stanceAttack;
        stanceCurInput = _stanceCurInput;
        mainRekka = _mainRekka;
        mainStance = _mainStance;
        normalAttack = _normalAttack;
    }
}
[Serializable]
public class AttackInputTypes 
{
    public Attack_Input specialMoveTypeInput;
    public List<Attack_BasicInput> normalTypeInput;
    public MoveType moveType;
    
    public AttackInputTypes(Attack_Input _specialMoveTypeInput = null, List<Attack_BasicInput> _normalTypeInput = null, MoveType _moveType = MoveType.Normal) 
    {
        specialMoveTypeInput = _specialMoveTypeInput; 
        normalTypeInput = _normalTypeInput;
        moveType = _moveType;
    }
}
