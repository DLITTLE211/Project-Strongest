using System;

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
    public RekkaAttack rekkaAttack;
    public StanceAttack stanceAttack;
    public AttackData(Character_Base _curBase, RekkaAttack _rekkaAttack = null, StanceAttack _stanceAttack = null) 
    {
        curBase = _curBase;
        rekkaAttack = _rekkaAttack;
        stanceAttack = _stanceAttack;
    }
}
[Serializable]
public class AttackInputTypes 
{
    public Attack_Input specialMoveTypeInput;
    public Attack_BasicInput normalTypeInput;
    public MoveType moveType;
    
    public AttackInputTypes(Attack_Input _specialMoveTypeInput = null, Attack_BasicInput _normalTypeInput = null, MoveType _moveType = MoveType.Normal) 
    {
        specialMoveTypeInput = _specialMoveTypeInput; 
        normalTypeInput = _normalTypeInput;
        moveType = _moveType;
    }
}
