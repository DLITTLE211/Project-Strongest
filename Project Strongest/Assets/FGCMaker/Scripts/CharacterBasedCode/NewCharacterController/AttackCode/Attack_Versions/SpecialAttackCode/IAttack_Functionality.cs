using System;
using System.Collections;
using System.Collections.Generic;
public interface IAttackFunctionality 
{
    //bool IsCorrectInput(Character_ButtonInput movementInput, Character_Base _curBase, int curInput, Character_ButtonInput attackInput = null);
    void PreformAttack();
    void SendSuccessfulDamageInfo(Character_Base curBase, bool blockedAttack);
    void SendCounterHitInfo(Character_Base curBase);
    void SetComboTimer(Character_InputTimer_Attacks timer);
    void SetStarterInformation();
    MoveType GetAttackMoveType();

    virtual void DoFollowUpAttack(int attack) { }
    virtual int GetFollowUpAttackInt() {return -1;}
    virtual void SetFollowUpAttackInt() {}
    virtual void ResetAttackData() { }

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
    public List<string> normalTypeInput;
    public string currentAttackInput;
    public MoveType moveType;
    public AirAttackInfo normalAirAttackInfo;
    public AirAttackInfo keyGroundCheck;
    public int hash;
    public AttackInputTypes(Attack_Input _specialMoveTypeInput = null, List<string> _normalTypeInput = null, MoveType _moveType = MoveType.Normal, AirAttackInfo _normalAirAttackInfo = AirAttackInfo.GroundOnly) 
    {
        specialMoveTypeInput = _specialMoveTypeInput; 
        normalTypeInput = _normalTypeInput;
        moveType = _moveType;
        normalAirAttackInfo = _normalAirAttackInfo;
    }
    public void AddDirectionalInput(int directionalInput, Character_Face_Direction faceSide)
    {
        if (specialMoveTypeInput.attackString.Length > 8)
        {
            string attackSubString = specialMoveTypeInput.attackString.Substring(1);
            specialMoveTypeInput.attackString = attackSubString;
        }
        if (faceSide == Character_Face_Direction.FacingLeft) 
        {
            int alteredInput = TransfigureDirectionOnSideSwitch(directionalInput);
            specialMoveTypeInput.attackString += alteredInput.ToString();
            return;
        }
        specialMoveTypeInput.attackString += directionalInput.ToString();
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
