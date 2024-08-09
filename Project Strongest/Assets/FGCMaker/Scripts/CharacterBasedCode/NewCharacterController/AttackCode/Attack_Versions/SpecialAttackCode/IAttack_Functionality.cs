public interface IAttackFunctionality 
{
    //bool IsCorrectInput(Character_ButtonInput movementInput, Character_Base _curBase, int curInput, Character_ButtonInput attackInput = null);
    //bool CheckCombo(Character_ButtonInput Input, Character_Base curBase, Character_ButtonInput attackInput = null);
    void PreformAttack(AttackData attackData);
    void SendSuccessfulDamageInfo(Character_Base curBase, bool blockedAttack);
    void SendCounterHitInfo(Character_Base curBase);
    void SetComboTimer(Character_InputTimer_Attacks timer);
    void SetStarterInformation();

}

public class AttackData 
{
    public Character_Base curBase;
    public RekkaAttack rekkaAttack;
    public StanceAttack stanceAttack;
    public AttackData(Character_Base _curBase, RekkaAttack _rekkaAttack, StanceAttack _stanceAttack) 
    {
        curBase = _curBase;
        rekkaAttack = _rekkaAttack;
        stanceAttack = _stanceAttack;
    }
}

public class AttackInputTypes 
{
    public Attack_Input specialMoveTypeInput;
    public Attack_BasicInput normalTypeInput;
    public AttackInputTypes(Attack_Input _specialMoveTypeInput = null, Attack_BasicInput _normalTypeInput = null) 
    {
        specialMoveTypeInput = _specialMoveTypeInput; 
        normalTypeInput = _normalTypeInput;
    }
}
