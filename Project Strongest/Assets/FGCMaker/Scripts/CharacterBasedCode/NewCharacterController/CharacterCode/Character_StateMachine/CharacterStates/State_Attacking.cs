using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_Attacking : BaseState
{
    Attack_StanceSpecialMove newStanceAttack = new Attack_StanceSpecialMove();
    Attack_RekkaSpecialMove newRekkaAttack = new Attack_RekkaSpecialMove();
    public State_Attacking(Character_Base playerBase) : base(playerBase)
    { }
    public override void OnEnter()
    {
        switch (_base._cAnimator.lastAttack._moveType)
        {
            case MoveType.Rekka:
                SetRekka();
                _base._cComboDetection.inRekka = true;
                _base._cAttackTimer.SetTimerType(TimerType.InRekka, newRekkaAttack.leewayTime);
                break;
            case MoveType.Stance:
                SetStance();
                _base._cComboDetection.inStance = true;
                _base._cAttackTimer.SetTimerType(TimerType.InStance);
                break;
        }
        DebugMessageHandler.instance.DisplayErrorMessage(1, "Enter IdleState");
    }
    public override void OnRecov()
    {
        base.OnRecov();
    }
    void SetRekka() 
    {
        newRekkaAttack = _base.comboList3_0.GetRekkaAttack(_base._cAnimator.lastAttack);
        newRekkaAttack.SetRekkaStateTrue();
    }
    void SetStance()
    {
        newStanceAttack = _base.comboList3_0.GetStanceAttack(_base._cAnimator.lastAttack);
    }

    public override void OnExit()
    {
        _base._cComboDetection.inRekka = false;
        _base._cComboDetection.inStance = false;
        _base._cAttackTimer.SetTimerType();
        base.OnExit();
    }
}
