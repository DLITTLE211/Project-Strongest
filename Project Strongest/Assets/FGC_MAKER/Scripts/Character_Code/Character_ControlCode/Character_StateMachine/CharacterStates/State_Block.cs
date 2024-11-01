using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using FightingGame_FrameData;
using System;

public class State_Block : BaseState
{
    public State_Block(Character_Base playerBase) : base(playerBase)
    { }
    public override void OnEnter()
    {
        base.OnEnter();
        _base._cHurtBox.ResetExtendedHurtbox();
        DebugMessageHandler.instance.DisplayErrorMessage(1, "Enter Block State");
        if (_base._cStateMachine._CheckBlockButton())
        {
            _base._cBlockHandler.ToggleBlockAnim(true, true);
        }
    }
    public override void OnUpdate()
    {
        base.OnUpdate();
        if (_base._cStateMachine._CheckBlockButton())
        {
            _base._cHurtBox.SetHurboxState(HurtBoxType.BlockHigh);
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

    public override void OnExit()
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
                _base._cBlockHandler.ToggleBlockAnim(true, false);
            }
        }
        base.OnExit();
    }
}
