using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using UnityEngine;

public class State_Attacking : BaseState
{
    Attack_StanceSpecialMove newStanceAttack = new Attack_StanceSpecialMove();
    Attack_RekkaSpecialMove newRekkaAttack = new Attack_RekkaSpecialMove();
    public State_Attacking(Character_Base playerBase) : base(playerBase)
    { }
    public override void OnEnter()
    {
        Attack_BaseProperties hitboxProperty = _base._cHitboxManager.GetActiveHitBox().hitboxProperties;
        switch (hitboxProperty._moveType)
        {
            case MoveType.Rekka:
                SetRekka();
                _base._cComboDetection.inRekka = true;
               // _base._cAttackTimer.SetTimerType(TimerType.InRekka, newRekkaAttack.leewayTime);
                break;
            case MoveType.Stance:
                SetStance();
                _base._cComboDetection.inStance = true;
               // _base._cAttackTimer.SetTimerType(TimerType.InStance,newStanceAttack.stanceHeldTime * (1 / 60f));
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
        _cAnim.SetRekkaBool(true);
        newRekkaAttack = _base.comboList3_0.GetRekkaAttack(_base._cAnimator.lastAttack);
        newRekkaAttack.SetRekkaStateTrue();
    }
    void SetStance()
    {
        _cAnim.SetStanceBool(true);
        newStanceAttack = _base.comboList3_0.GetStanceAttack(_base._cAnimator.lastAttack);
    }

    public override void OnExit()
    {
        _base._cComboDetection.inRekka = false;
        _base._cComboDetection.inStance = false;
        _base._cAttackTimer.SetTimerType(TimerType.Normal);

        ITransition nextTransition = _base._cStateMachine._playerState.GetTransition();

        if (nextTransition.To == _base._cStateMachine.idleStateRef)
        {
            /* Debug.Log("Back To Idle");
             if (_base._cAnimator.lastAttack != null)
             {
                 _base._cAnimator.ClearLastAttack();
                 _cAnim.PlayNextAnimation(groundIdleHash, 0, false);
             }*/
            _base.AwaitCanTransitionIdle(() => base.OnExit());
            return;
        }
        if (nextTransition.To == _base._cStateMachine.crouchStateRef)
        {
            /*Debug.Log("Back To Crouch"); 
            if (_base._cAnimator.lastAttack != null)
            {
                _base._cAnimator.ClearLastAttack();
                _cAnim.PlayNextAnimation(I2CHash, 0, false);
            }*/
            _base.AwaitCanTransitionIdle(() => base.OnExit());
            return;
        }

        if (nextTransition.To == _base._cStateMachine.jumpRef)
        {
            Debug.Log("Back To Jump");
            _base.AwaitCanTransition_OutAttack(() => base.OnExit());

            return;
        }
        base.OnExit();
    }
}
