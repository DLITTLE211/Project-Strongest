using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using FightingGame_FrameData;
using System;

public class State_CrouchBlock : BaseState
{
    bool inputIsCrouch;
    public State_CrouchBlock(Character_Base playerBase) : base(playerBase)
    { }
    public override async void OnEnter()
    {
        base.OnEnter();
        _base._cHurtBox.ResetExtendedHurtbox();
        DebugMessageHandler.instance.DisplayErrorMessage(1, "Enter Crouch Block State");
        if (_base._subState == Character_SubStates.Controlled)
        {
            if (_base._cStateMachine._CheckBlockButton() && _base._cComboDetection.lastInput < 3)
            {
                _base._cBlockHandler.ToggleBlockAnim(false, true);
                _base._cComboDetection.superMobilityOption = true;
            }
        }
    }
    public override void OnUpdate()
    {
        base.OnUpdate();
        if (_base._cStateMachine._CheckBlockButton())
        {
            if (_base._cComboDetection.lastInput < 3)
            {
                _base._cHurtBox.SetHurboxState(HurtBoxType.BlockLow);
            }
        }
        else
        {
            _base._cAnimator.canBlock = false;
        }

    }
    public override void OnRecov()
    {
        base.OnRecov();
    }

    public override async void OnExit()
    {
        ITransition nextTransition = _base._cStateMachine._playerState.GetTransition();
        if (nextTransition.To != _base._cStateMachine.blockReactRef)
        {
            if (nextTransition.To != _base._cStateMachine.crouchBlockRef)
            {
                _base._cBlockHandler.KillCurrentRoutine();
                _base._cHurtBox.SetHurboxState();
                _cAnim.SetCanTransitionIdle(true);
            }
            else
            {
                _base._cBlockHandler.ToggleBlockAnim(false, false);
            }
        }
        base.OnExit();
    }
}
