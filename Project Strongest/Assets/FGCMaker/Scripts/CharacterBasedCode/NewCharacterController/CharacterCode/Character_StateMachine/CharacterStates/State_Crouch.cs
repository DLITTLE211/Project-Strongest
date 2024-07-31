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
        await ActivateSuperMobility(OneFrame);
        float TenwaitTime = 2 * OneFrame;
        int TentimeInMS = (int)(TenwaitTime * 1000f);
        await Task.Delay(TentimeInMS);
        if (_base.ReturnMovementInputs().Button_State.directionalInput <= 3)
        {
            _cAnim.PlayNextAnimation(crouchHash, 2 * (1 / 60f), false);
        }
    }
    async Task ActivateSuperMobility(float OneFrame) 
    {
        float waitTime = 2 * OneFrame;
        int timeInMS = (int)(waitTime * 1000f);
        await Task.Delay(timeInMS);
        if (_base.ReturnMovementInputs().Button_State.directionalInput <= 3)
        {
            _base._cComboDetection.superMobilityOption = true;
        }
    }
    public override void OnUpdate()
    {
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
    }
}
