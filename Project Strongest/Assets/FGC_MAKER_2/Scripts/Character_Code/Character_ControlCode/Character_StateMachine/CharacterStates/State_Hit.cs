using System;
using System.Threading.Tasks;

public class State_Hit : BaseState
{
    public State_Hit(Character_Base playerBase) : base(playerBase)
    { }
    public override void OnEnter()
    {
        DebugMessageHandler.instance.DisplayErrorMessage(1, "Enter HitState");
    }
    public override async void OnUpdate()
    {
        try
        {

            ButtonStateMachine techButton = _base.ReturnTechButton().Button_State;
            if (_base._cAnimator._canRecover && techButton._state == ButtonStateMachine.InputState.held)
            {
                CheckRecovDirection();
                if (_base.ReturnMovementInputs().Button_State.directionalInput > 3)
                {
                    //await _base._cHitController.RecoverAfterHit();
                }
                else
                {
                    await DelayWakeUp();
                }
            }
        }
        catch (NullReferenceException) 
        {
            return;
        }
        base.OnUpdate();
    }
    void CheckRecovDirection()
    {
        _base._cForce.RecoverWithDirection(_base.ReturnMovementInputs().Button_State.directionalInput);
    }
    async Task DelayWakeUp() 
    {
        float delayWakeUpTime = (15 * (1 / 60f)) * 1000f;
        await Task.Delay((int)delayWakeUpTime);
        if (_base.ReturnMovementInputs().Button_State.directionalInput <= 3)
        {
            //await _base._cHitController.RecoverAfterHit();
        }
    }
    public override void OnRecov()
    {
        base.OnRecov();
    }

    public override void OnExit()
    {
        base.OnExit();
    }
}
