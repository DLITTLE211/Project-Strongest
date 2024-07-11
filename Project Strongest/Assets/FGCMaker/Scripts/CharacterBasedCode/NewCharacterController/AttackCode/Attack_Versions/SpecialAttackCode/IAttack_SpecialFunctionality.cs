public interface IAttack_SpecialFuctionality 
{
    #region Function Summary
    /// <summary>
    /// Checks if Button input received is the same with Current Attack information
    /// </summary>
    /// <returns></returns>
    #endregion
    bool IsCorrectInput(Character_ButtonInput testInput, Character_Base _curBase, int curInput, Character_ButtonInput attackInput = null);

    #region Function Summary
    /// <summary>
    /// Checks combo information against inputs provided
    /// </summary>
    /// <returns></returns>
    #endregion
    bool CheckCombo(Character_ButtonInput Input, Character_Base curBase, Character_ButtonInput attackInput = null);

    #region Function Summary
    /// <summary>
    /// Upon Successful attack, sends information to character animator to do action
    /// </summary>
    /// <returns></returns>
    #endregion
    void PreformAttack(Character_Base curBase);

    #region Function Summary
    /// <summary>
    /// On Hit, will send damage numbers to hit target
    /// </summary>
    /// <returns></returns>
    #endregion
    void SendSuccessfulDamageInfo(Character_Base curBase, bool blockedAttack);

    #region Function Summary
    /// <summary>
    /// On Hit, will send counter hit damage properties to 
    /// </summary>
    /// <returns></returns>
    #endregion
    void SendCounterHitInfo(Character_Base curBase);

    #region Function Summary
    /// <summary>
    /// Sets attack timer information to this attack timer information
    /// </summary>
    /// <returns></returns>
    #endregion
    void SetComboTimer(Character_InputTimer_Attacks timer);
}
public interface IAttack_RekkaFuctionality
{
    #region Function Summary
    /// <summary>
    /// Checks if Button input received is the same with Current Attack information
    /// </summary>
    /// <returns></returns>
    #endregion
    bool IsCorrectInput(Character_ButtonInput testInput, Character_Base _curBase, int curInput, Character_ButtonInput attackInput = null);

    #region Function Summary
    /// <summary>
    /// Checks combo information against inputs provided
    /// </summary>
    /// <returns></returns>
    #endregion
    bool CheckCombo(Character_ButtonInput Input, Character_Base curBase, Character_ButtonInput attackInput = null);

    #region Function Summary
    /// <summary>
    /// Upon Successful attack, sends information to character animator to do action
    /// </summary>
    /// <returns></returns>
    #endregion
    void PreformAttack(Character_Base curBase, RekkaAttack _rekka);

    #region Function Summary
    /// <summary>
    /// On Hit, will send damage numbers to hit target
    /// </summary>
    /// <returns></returns>
    #endregion
    void SendSuccessfulDamageInfo(Character_Base curBase, bool blockedAttack, RekkaAttack rekka = null);

    #region Function Summary
    /// <summary>
    /// On Hit, will send counter hit damage properties to 
    /// </summary>
    /// <returns></returns>
    #endregion
    void SendCounterHitInfo(Character_Base curBase, RekkaAttack rekka = null);

    #region Function Summary
    /// <summary>
    /// Sets attack timer information to this attack timer information
    /// </summary>
    /// <returns></returns>
    #endregion
    void SetComboTimer(Character_InputTimer_Attacks timer);
}
public interface IAttack_StanceFuctionality
{
    #region Function Summary
    /// <summary>
    /// Checks if Button input received is the same with Current Attack information
    /// </summary>
    /// <returns></returns>
    #endregion
    bool IsCorrectInput(Character_ButtonInput testInput, Character_Base _curBase, int curInput, Character_ButtonInput attackInput = null);

    #region Function Summary
    /// <summary>
    /// Checks combo information against inputs provided
    /// </summary>
    /// <returns></returns>
    #endregion
    bool CheckCombo(Character_ButtonInput Input, Character_Base curBase, Character_ButtonInput attackInput = null);

    #region Function Summary
    /// <summary>
    /// Upon Successful attack, sends information to character animator to do action
    /// </summary>
    /// <returns></returns>
    #endregion
    void PreformAttack(Character_Base curBase,StanceAttack action = null);

    #region Function Summary
    /// <summary>
    /// On Hit, will send damage numbers to hit target
    /// </summary>
    /// <returns></returns>
    #endregion
    void SendSuccessfulDamageInfo(Character_Base curBase, bool blockedAttack, StanceAttack _stanceMove = null);

    #region Function Summary
    /// <summary>
    /// On Hit, will send counter hit damage properties to 
    /// </summary>
    /// <returns></returns>
    #endregion
    void SendCounterHitInfo(Character_Base curBase,StanceAttack _stanceMove = null);

    #region Function Summary
    /// <summary>
    /// Sets attack timer information to this attack timer information
    /// </summary>
    /// <returns></returns>
    #endregion
    void SetComboTimer(Character_InputTimer_Attacks timer);
}