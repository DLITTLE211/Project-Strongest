using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using FightingGame_FrameData;


public class State_Block : BaseState
{
    public State_Block(Character_Base playerBase) : base(playerBase)
    { }
    public override async void OnEnter()
    {
        DebugMessageHandler.instance.DisplayErrorMessage(1, "Enter Block State");
        if (_base._cStateMachine._CheckBlockButton())
        {
            _cAnim.PlayNextAnimation(sblockHash, 0);
            await DeployBlock();
        }
    }
    async Task DeployBlock() 
    {
        int FourFrameDelay = (int)((Base_FrameCode.ONE_FRAME * 1000f) * 4);
        await Task.Delay(FourFrameDelay);
        if (_base._cStateMachine._CheckBlockButton())
        {
            _base._cHurtBox.SetHurboxState(HurtBoxType.BlockHigh);
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
            _base._cHurtBox.SetHurboxState();
        }
        base.OnExit();
    }
}
