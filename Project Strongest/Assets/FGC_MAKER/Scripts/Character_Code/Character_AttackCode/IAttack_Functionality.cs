using System;
using System.Collections;
using System.Collections.Generic;
public interface IAttackFunctionality 
{
    #region Universal Functions
    void PreformAttack(Callback SendAttackOnSucess);
    void SendSuccessfulDamageInfo(Character_Base attacker, Character_Base target, bool blockedAttack, Attack_BaseProperties main, Attack_BaseProperties followUp = null, bool armoredAttack = false);
    void SendCounterHitInfo(Character_Base curBase, Attack_BaseProperties followUp = null);
    void SetComboTimer();
    void SetStarterInformation(Character_Base _base);
    void HandleDamageDealing(Character_Base attacker, Character_Base target, bool blockedAttack, Attack_BaseProperties main, Attack_BaseProperties followUp = null,bool armoredAttack = false);
    MoveType GetAttackMoveType();
    #endregion

    #region Virtual Functions
    virtual int GetCustomAnimLength() { return -1; }
    virtual void DoFollowUpKill(int kill) { }
    virtual void DoFollowUpAttack(Character_ButtonInput buttonInput,int attack, Callback SendAttackOnSucess) { }
    virtual void DoFollowUpAttack(int attack, Callback SendAttackOnSucess) { }
    virtual int GetFollowUpAttackInt() {return -1;}
    virtual void SetFollowUpAttackInt() {}
    virtual void ResetAttackData() { }
    virtual MoveListAttackInfo CreateMoveListData() { return null; }
    #endregion
}

[Serializable]
public class AttackInputTypes
{
    public string AttackName;
    public Attack_Input specialMoveTypeInput;
    public List<string> normalTypeInput;
    public string currentAttackInput;
    public MoveType moveType;
    public AirAttackInfo normalAirAttackInfo;
    public AirAttackInfo keyGroundCheck;
    public AttackInputTypes(
        Attack_Input _specialMoveTypeInput = null, 
        List<string> _normalTypeInput = null,
        MoveType _moveType = MoveType.Normal,
        AirAttackInfo _normalAirAttackInfo = AirAttackInfo.GroundOnly) 
    {
        specialMoveTypeInput = _specialMoveTypeInput; 
        normalTypeInput = _normalTypeInput;
        moveType = _moveType;
        normalAirAttackInfo = _normalAirAttackInfo;
    }
    public void AddDirectionalInput(int directionalInput, Character_Face_Direction faceSide)
    {

        if (faceSide == Character_Face_Direction.FacingLeft)
        {
            int alteredInput = TransfigureDirectionOnSideSwitch(directionalInput);
            specialMoveTypeInput.attackString += alteredInput.ToString();
        }
        else
        {
            specialMoveTypeInput.attackString += directionalInput.ToString();
        }
    }
    public void ClearFirstIndex() 
    {
        if (specialMoveTypeInput.attackString.Length >= 25)
        {
            specialMoveTypeInput.attackString = specialMoveTypeInput.attackString.Remove(0, 1);
        }
    }
    public void AddAttackInput(int lastDirection, Character_Face_Direction faceSide, Character_ButtonInput attackInput, bool groundState)
    {
        currentAttackInput = "";
        normalTypeInput = new List<string>();
        specialMoveTypeInput.attackString += attackInput.Button_Name.ToString();
        specialMoveTypeInput.turnStringToArray();
        if (faceSide == Character_Face_Direction.FacingLeft)
        {
            int alteredInput = TransfigureDirectionOnSideSwitch(lastDirection);
            currentAttackInput += alteredInput;
        }
        else
        {
            currentAttackInput += lastDirection;
        }
        char buttonInput = attackInput.Button_Name.ToCharArray()[0];
        currentAttackInput += buttonInput;
        normalTypeInput.Add(currentAttackInput);
        moveType = MoveType.Key;
        if (groundState)
        {
            keyGroundCheck = AirAttackInfo.GroundOnly;
        }
        else
        {
            keyGroundCheck = AirAttackInfo.AirOnly;
        }
    }

    public bool CheckMatchingInput(AttackInputTypes obj) 
    {
        if (obj != null) 
        {
            return normalTypeInput.Contains(obj.currentAttackInput);
        }
        return false;
    }

    public void ResetComboInfo() 
    {
        specialMoveTypeInput.attackString = "";
        specialMoveTypeInput.attackStringArray = new char[0];
        currentAttackInput = "";
    }
    int TransfigureDirectionOnSideSwitch(int move)
    {
        int switchValue = move;
        switch (move)
        {
            case 9:
                switchValue = 7;
                break;
            case 7:
                switchValue = 9;
                break;
            case 6:
                switchValue = 4;
                break;
            case 3:
                switchValue = 1;
                break;
            case 4:
                switchValue = 6;
                break;
            case 1:
                switchValue = 3;
                break;
        }
        return switchValue;
    }
}
