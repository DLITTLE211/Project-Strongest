using System.Threading.Tasks;
using UnityEngine;

public class State_Crouch : BaseState
{
    private bool inCrouch;
    private bool canDoSecondaryCrouch;
    private const float startSecondaryCrouch = 10f;
    private float timeTillSecondaryCrouch;
    public State_Crouch(Character_Base playerBase) : base(playerBase)
    {}
    public override async void OnEnter()
    {
        canDoSecondaryCrouch = false;
        DebugMessageHandler.instance.DisplayErrorMessage(1, "Enter CrouchState");
        _base._cHurtBox.SetHitboxSize(HurtBoxSize.Crouching);
        _base._cHurtBox.SetHurboxState(HurtBoxType.NoBlock);
        await WaitToChargeSuperMobility();
        inCrouch = true;
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
            if (_cAnim.RoutineActive())
            {
                _cAnim.PlayNextAnimation(crouchHash, 2 * (1 / 60f));
                ResetTime();
            }
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
    public async override void OnUpdate()
    {
        if (!_base.isLockedPause)
        {
            if (canDoSecondaryCrouch)
            {
                if (timeTillSecondaryCrouch >= 0)
                {
                    timeTillSecondaryCrouch -= (1 / 60f);
                }
                else
                {
                    await PlaySecondaryAnim();
                }
            }
        }
        base.OnUpdate();
    }
    async Task PlaySecondaryAnim()
    {
        float startTime = 0;
        canDoSecondaryCrouch = false;
        _cAnim.PlayNextAnimation(secondaryCrouchHash, 2 * (1 / 60f));
        await Task.Delay(100);
        int secondaryAnimDelayTime = (int)(_cAnim.myAnim.GetCurrentAnimatorStateInfo(0).length * 1000);
        while (startTime < secondaryAnimDelayTime)
        {
            startTime += 16;
            await Task.Yield();
        }
        if (inCrouch)
        {
            _cAnim.PlayNextAnimation(crouchHash, 2 * (1 / 60f));
        }
        ResetTime();
    }
    void ResetTime()
    {
        timeTillSecondaryCrouch = startSecondaryCrouch;
        canDoSecondaryCrouch = true;
    }
    public override void OnRecov()
    {
        base.OnRecov();
    }

    public override void OnExit()
    {
        inCrouch = false;
        _base._cHurtBox.SetHitboxSize(HurtBoxSize.Standing);
        canDoSecondaryCrouch = false;
        base.OnExit();
    }
}
