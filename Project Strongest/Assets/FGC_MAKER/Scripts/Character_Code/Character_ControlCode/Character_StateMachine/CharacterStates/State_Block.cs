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
    public override async void OnEnter()
    {
        base.OnEnter();
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
            try
            {
                int currentAnimClipName = Animator.StringToHash(_cAnim.myAnim.GetCurrentAnimatorClipInfo(0)[0].clip.name);
                if (currentAnimClipName == sblockHash)
                {
                    _cAnim.PlayNextAnimation(sblockHash, 0);
                }
            }
            catch (IndexOutOfRangeException) { }
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
            _base._cHurtBox.SetHurboxState();
            await DelayStopBlock();
        }
        base.OnExit();
    }
    async Task DelayStopBlock()
    {
        int FrameDelay = (int)((Base_FrameCode.ONE_FRAME * 1000f) * 15);
        await Task.Delay(FrameDelay);
    }
}
