using System;
using System.Collections;
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

    void DisableCheckable();
    MoveType GetAttackMoveType();

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
public class AttackInputTypes //: IEqualityComparer<AttackInputTypes>
{
    public Attack_Input specialMoveTypeInput;
    public List<(Attack_BaseInput.MoveInput, Attack_BaseInput.AttackInput)> normalTypeInput;
    public (Attack_BaseInput.MoveInput, Attack_BaseInput.AttackInput) currentAttackInput;
    public AttackInputTypes(Attack_Input _specialMoveTypeInput = null, List<(Attack_BaseInput.MoveInput, Attack_BaseInput.AttackInput)> _normalTypeInput = null) 
    {
        specialMoveTypeInput = _specialMoveTypeInput; 
        normalTypeInput = _normalTypeInput;
    }
    public void AddDirectionalInput(int directionalInput, Character_Face_Direction faceSide) 
    {
        if(faceSide == Character_Face_Direction.FacingLeft) 
        {
            int alteredInput = TransfigureDirectionOnSideSwitch(directionalInput);
            specialMoveTypeInput.attackString += alteredInput.ToString();
            return;
        }
        specialMoveTypeInput.attackString += directionalInput.ToString();
    }
    public void AddAttackInput(int lastDirection, Character_Face_Direction faceSide, Character_ButtonInput attackInput)
    {
        normalTypeInput = new List<(Attack_BaseInput.MoveInput, Attack_BaseInput.AttackInput)>();
        specialMoveTypeInput.attackString += attackInput.Button_Name.ToString();
        specialMoveTypeInput.turnStringToArray();
        if (faceSide == Character_Face_Direction.FacingLeft)
        {
            int alteredInput = TransfigureDirectionOnSideSwitch(lastDirection);
            currentAttackInput.Item1 = (Attack_BaseInput.MoveInput)alteredInput;
        }
        else
        {
            currentAttackInput.Item1 = (Attack_BaseInput.MoveInput)lastDirection;
        }
        char buttonInput = attackInput.Button_Name.ToCharArray()[0];
        currentAttackInput.Item2 = (Attack_BaseInput.AttackInput)buttonInput;
        normalTypeInput.Add(currentAttackInput);
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
        currentAttackInput.Item1 = 0;
        currentAttackInput.Item2 = Attack_BaseInput.AttackInput.A;
    }
    int TransfigureDirectionOnSideSwitch(int move)
    {
        int switchValue = 5;
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

    /*
    public bool Equals(AttackInputTypes x, AttackInputTypes y)
    {
        bool specialInputMatch = x?.specialMoveTypeInput == y?.specialMoveTypeInput;
        bool normalInputMatch = y.CheckMatchingInput(x);
        //ToDo function in attacktype that verifies if normalattackinput at given index match normal input that was recently inputted

        return specialInputMatch || normalInputMatch;
    }
    public int GetHashCode(AttackInputTypes obj)
    {
        int returningValue = obj?.GetHashCode() ?? 0;
        return returningValue;
    }
    */
}
