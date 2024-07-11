using Rewired;
using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Attack_NonSpecial_Base
{
    public string SpecialAttackName;
    public Attack_BasicInput _attackInput;
    public abstract bool ContinueCombo(Character_ButtonInput i, Character_ButtonInput j,Character_Base curBase);
    public abstract void CheckButtonInfo(InputAction buttonInfo);
    public abstract void ResetCombo();
    public abstract void ResetMoveCombo();

}
[System.Serializable]
public class Attack_BasicInput
{
    public List<Attack_BaseInput> _correctInput;
    public void ActivateAttackInfo()
    {
        for (int i = 0; i < _correctInput.Count; i++)
        {
            _correctInput[i].SetAttackInfo(_correctInput[i]._correctSequence);
        }
    }
}
[System.Serializable]
public class Attack_BaseInput
{
    public enum MoveInput
    {
        One = 1,
        Two = 2,
        Three = 3,
        Four = 4,
        Five = 5,
        Six = 6,
        Seven = 7,
        Eight = 8,
        Nine = 9,
        Zero = 0,
    }
    public enum AttackInput
    {
        A = 'A',
        B = 'B',
        C = 'C',
        D = 'D',
        E = 'E',
        F = 'F',
        G = 'G',
    }
    public string _correctSequence;
    public int correctAttack;
    public char correctInput;
    public MoveInput moveInput;
    public AttackInput attackInput;
    public (MoveInput, AttackInput) verifyAttackInput;
    public Attack_BaseProperties property;
    public ButtonStateMachine attackInputState;
    public void SetInnerAttackAnimations(Character_Animator animController)
    {
        property.attackHashes.Clear();
        property.AttackAnims.SetAttackAnim(animController);
        property.attackHashes.Add(Animator.StringToHash(property.AttackAnims.animName));

    }

    public void SetAttackInfo(string NewAttackString)
    {
        correctInput = (NewAttackString.ToCharArray()[1]);
        correctAttack = Int32.Parse(NewAttackString.Split(correctInput)[0]);
        verifyAttackInput = ((Attack_BaseInput.MoveInput)correctAttack, (Attack_BaseInput.AttackInput)correctInput);
        moveInput = verifyAttackInput.Item1;
        attackInput = verifyAttackInput.Item2;
    }
}

