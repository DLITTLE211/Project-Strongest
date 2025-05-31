using System.Threading.Tasks;
using UnityEngine;

public class State_Crouch : BaseState
{
    private bool inCrouch;
    private const float startSecondaryCrouch = 10f;
    public State_Crouch(Character_Base playerBase) : base(playerBase)
    {}
    public override async void OnEnter()
    {
        base.OnEnter();

        _base._cHurtBox.SetHurboxState();
        _base._cHurtBox.ResetExtendedHurtbox();
        _base._cHitboxManager.DisableAllHitboxes();

        DebugMessageHandler.instance.DisplayErrorMessage(1, "Enter CrouchState");
        _base._cHurtBox.SetHurboxState(HurtBoxType.NoBlock); 
        _base._cComboDetection.superMobilityOption = true;
        if (_base._subState == Character_SubStates.Controlled)
        {
            await WaitToChargeSuperMobility();
        }
        inCrouch = true;
    }
    async Task WaitToChargeSuperMobility()
    {
        float OneFrame = 1 / 60f;
        float TenwaitTime = 2 * OneFrame;
        int TentimeInMS = (int)(TenwaitTime * 1000f);
        await Task.Delay(TentimeInMS);
        if (_base.ReturnMovementInputs().Button_State.directionalInput <= 3)
        {
            if (_cAnim.RoutineActive())
            {
                PlayCrouchIdle();
            }
        }
    }
    void PlayCrouchIdle() 
    {
        _cAnim.PlayNextAnimation(crouchHash, 2 * (1 / 60f));
    }
    public async override void OnUpdate()
    {
        if (!_base.isLockedPause)
        {
            _base._cSubStateController?.PlaySecondaryAnimation(PlayCrouchIdle);
        }
        if (!_base._cComboDetection.superMobilityOption) 
        {
            _base._cComboDetection.superMobilityOption = true;
        }
        base.OnUpdate();
    }
    public override void OnRecov()
    {
        base.OnRecov();
    }

    public override void OnExit()
    {
        inCrouch = false;
        base.OnExit();
    }
}
