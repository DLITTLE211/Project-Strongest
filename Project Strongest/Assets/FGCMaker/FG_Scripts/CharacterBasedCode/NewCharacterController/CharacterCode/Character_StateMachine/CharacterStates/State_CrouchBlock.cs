using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

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
            _cAnim.PlayNextAnimation(cblockHash, _crossFade);
           // _baseAnim.CrossFade(cblockHash, _crossFade, 0, 0); 
            await DeployBlock();
            await WaitToChargeSuperMobility();
        }
    }
    async Task WaitToChargeSuperMobility()
    {
        float OneFrame = 1 / 60f;
        float waitTime = 10 * OneFrame;
        int timeInMS = (int)(waitTime * 1000f);
        await Task.Delay(timeInMS);
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
        while (!_base._cAnimator.canBlock)
        {
            await Task.Yield();
        }
        if (_base._cStateMachine._CheckBlockButton())
        {
            _base._cHurtBox.SetHurboxState(HurtBoxType.BlockLow);
        }
    }
    public override void OnUpdate()
    {
        base.OnUpdate();
        if (_base._cAnimator.canBlock && !_base._cStateMachine._CheckBlockButton())
        {
            _base._cAnimator.canBlock = false;
        }
        if (_base._cAnimator._lastMovementState != lastMovementState.nullified)
        {
           // _base._cAnimator.NullifyMobilityOption();
        }

    }
    public override void OnRecov()
    {
        base.OnRecov();
    }

    public override void OnExit()
    {
        _base._cHurtBox.SetHurboxState();
        base.OnExit();
    }
}
