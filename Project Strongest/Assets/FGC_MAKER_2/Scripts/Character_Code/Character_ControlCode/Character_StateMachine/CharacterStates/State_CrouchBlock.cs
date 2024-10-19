using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using FightingGame_FrameData;

public class State_CrouchBlock : BaseState
{
    bool inputIsCrouch;
    public State_CrouchBlock(Character_Base playerBase) : base(playerBase)
    { }
    public override async void OnEnter()
    {
        DebugMessageHandler.instance.DisplayErrorMessage(1, "Enter Crouch Block State");
        if (_base._cStateMachine._CheckBlockButton())
        {
            _cAnim.PlayNextAnimation(cblockHash, 0);
            await DeployBlock();
            await WaitToChargeSuperMobility();
        }
    }
    async Task WaitToChargeSuperMobility()
    {
        int FourFrameDelay = (int)((Base_FrameCode.ONE_FRAME * 1000f) * 4);
        await Task.Delay(FourFrameDelay);
        if (_base.ReturnMovementInputs().Button_State.directionalInput <= 3)
        {
            _base._cComboDetection.superMobilityOption = true;
        }
        else
        {
            _base._cAnimator.NullifyMobilityOption(); 
        }
    }
    async Task DeployBlock()
    {
        int FourFrameDelay = (int)((Base_FrameCode.ONE_FRAME * 1000f) * 4);
        await Task.Delay(FourFrameDelay);
        if (_base._cStateMachine._CheckBlockButton())
        {
            _base._cHurtBox.SetHurboxState(HurtBoxType.BlockLow);
        }
    }
    public override void OnUpdate()
    {
        base.OnUpdate();
        if (_base._cStateMachine._CheckBlockButton() && _base.ReturnMovementInputs().Button_State.directionalInput <= 3)
        {
            _base._cHurtBox.SetHurboxState(HurtBoxType.BlockLow);
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
            _base._cHurtBox.SetHurboxState();
        }
        base.OnExit();
    }
}
