public interface IAttack_BasicFunctionality
{
    bool IsCorrectInput(Character_ButtonInput move, Character_ButtonInput attack, Character_Base curBase);
    bool CheckCombo(Character_ButtonInput move, Character_ButtonInput attack, Character_Base curBase);

    void PreformAttack(int curInput, int CurrentAttack,Character_Base curBase);
    void SendSuccessfulDamageInfo(Path_Data _data, Character_Base target,bool blockedAttack);
    void SendCounterHitInfo(Path_Data _data, Character_Base target);
    void SetStarterInformation();
    void UpdateLastDirectional(Character_ButtonInput newDirection);
    void SetComboTimer(Character_InputTimer_Attacks timer);
}
