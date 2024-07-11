using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;


public class State_Block : BaseState
{
    bool inputIsCrouch;
    public State_Block(Character_Base playerBase) : base(playerBase)
    { }
    public override async void OnEnter()
    {
        DebugMessageHandler.instance.DisplayErrorMessage(1, "Enter Block State");
        if (_base._cStateMachine._CheckBlockButton())
        {
            _cAnim.PlayNextAnimation(sblockHash, _crossFade);
           // _baseAnim.CrossFade(sblockHash, _crossFade, 0, 0);
            await DeployBlock();
        }
    }
    async Task DeployBlock() 
    {
        while (!_base._cAnimator.canBlock) 
        {
            await Task.Yield();
        }
        if (_base._cStateMachine._CheckBlockButton())
        {
            _base._cHurtBox.SetHurboxState(HurtBoxType.BlockHigh);
        }
    }
    public override void OnUpdate()
    {
        if (!_base._cStateMachine._CheckBlockButton())
        {
            if (_base._cAnimator.canBlock)
            {
                _base._cAnimator.canBlock = false;
            }
            base.OnUpdate();
        }
    }
    public override void OnRecov()
    {
        base.OnRecov();
    }

    public override void OnExit()
    {
        if (!_base._cHitController.ReturnCrouchBlock() && !_base._cHitController.ReturnStandBlock())
        {
            _base._cHurtBox.SetHurboxState(HurtBoxType.NoBlock);
        }
        if (_base._cHitController.bigHitRecovering) 
        {
            _base._cHurtBox.SetHurboxState(HurtBoxType.NoBlock);
        }
        base.OnExit();
    }
}
