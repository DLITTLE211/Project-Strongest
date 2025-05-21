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
        _base._aFrameDataMeter.ResetMeterData();

        _base._cComboDetection.superMobilityOption = false;
        if (_base._cHurtBox.IsGrounded())
        {
            if (_base._cForce.xSpeed > 0)
            {
                _base.myRb.velocity = Vector3.zero;
                _base.myRb.drag = 100000;
            }
        }
        _base._cHitController.ClearRecoveryRoutine(false);
        base.OnEnter();
        Attack_BaseProperties hitboxProperty = _base._cHitboxManager.GetActiveHitBox().hitboxProperties;
        switch (hitboxProperty._moveType)
        {
            case MoveType.Rekka:
                SetRekka();
                _base._cComboDetection.inRekka = true;
                break;
            case MoveType.Stance:
                SetStance();
                _base._cComboDetection.inStance = true;
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
        _base._cHurtBox.SetHurboxState();
        _base._cComboDetection.inRekka = false;
        _base._cComboDetection.inStance = false;
        //_base._cAttackTimer.SetTimerType(TimerType.Normal);

        ITransition nextTransition = _base._cStateMachine._playerState.GetTransition();

        if (nextTransition.To == _base._cStateMachine.idleStateRef)
        {
            _base.AwaitCanTransitionIdle(() => base.OnExit());
            return;
        }
        if (nextTransition.To == _base._cStateMachine.crouchStateRef)
        {
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
