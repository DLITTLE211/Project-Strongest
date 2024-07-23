using System.Threading.Tasks;
using UnityEngine;

public class State_Crouch : BaseState
{
    public State_Crouch(Character_Base playerBase) : base(playerBase)
    {}
    public override async void OnEnter()
    {
        DebugMessageHandler.instance.DisplayErrorMessage(1, "Enter CrouchState");

        _base._cHurtBox.SetHitboxSize(HurtBoxSize.Crouching);
        _base._cHurtBox.SetHurboxState(HurtBoxType.NoBlock);
        await WaitToChargeSuperMobility();
    }
    async Task WaitToChargeSuperMobility() 
    {
        float OneFrame = 1 / 60f;
        float waitTime = 2 * OneFrame;
        int timeInMS = (int)(waitTime * 1000f);
        await Task.Delay(timeInMS);
        if (_base.ReturnMovementInputs().Button_State.directionalInput <= 3)
        {
            _base._cComboDetection.superMobilityOption = true;
        }
        float TenwaitTime = 10 * OneFrame;
        int TentimeInMS = (int)(TenwaitTime * 1000f);
        await Task.Delay(TentimeInMS);
        if (_base.ReturnMovementInputs().Button_State.directionalInput <= 3)
        {
            _cAnim.PlayNextAnimation(0, 0, false, I2CString);
        }
    }
    public override void OnUpdate()
    {
        if (_base._cAnimator._lastMovementState != Character_Animator.lastMovementState.nullified)
        {
           // _base._cAnimator.NullifyMobilityOption();
        }
        base.OnUpdate();
    }
    public override void OnRecov()
    {
        base.OnRecov();
    }

    public override void OnExit()
    {
        _base._cHurtBox.SetHitboxSize(HurtBoxSize.Standing);
        base.OnExit();
        ITransition nextTransition = _base._cStateMachine._playerState.GetTransition();

        if (nextTransition.To == _base._cStateMachine.idleStateRef ^ nextTransition.To == _base._cStateMachine.moveStateRef) 
        {
            _cAnim.PlayNextAnimation(C2IHash, 0);
        }
    }
}
