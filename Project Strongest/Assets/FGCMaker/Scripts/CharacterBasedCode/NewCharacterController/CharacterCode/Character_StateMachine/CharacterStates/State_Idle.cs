using System;
using UnityEngine;
using System.Threading.Tasks;

public class State_Idle : BaseState
{
    public State_Idle(Character_Base playerBase) : base(playerBase){ }
    public override async void OnEnter()
    {
        _base._cAnimator._canRecover = false;
        _base._cAnimator.canBlock = false;
        _base._cHurtBox.SetHitboxSize(HurtBoxSize.Standing);
        await WaitToEndSuperMobility();
        if (_base._cHurtBox.IsGrounded())
        {
            IdleCheck();
        }
        else
        {
            await CheckOnLanding();
        }
        #region Returning to Idle if Subject is Hit
        if (_base._cHurtBox.IsGrounded())
        {
            if (_base._cStateMachine.opponentComboCounter.CurrentHitCount > 0)
            {
                ResetComboInformation();
                IdleCheck();
            }
            else
            {
                await CheckOnLanding(); 
                ResetComboInformation();
                IdleCheck();

            }
        }
        else
        {
            await CheckOnLanding();
            ResetComboInformation();
            IdleCheck();
        }

        #endregion
        try
        {
            if (_base.ReturnMovementInputs() != null)
            {
                _baseForce.SetWalkForce(_base.ReturnMovementInputs());
            }
        }
        catch (ArgumentOutOfRangeException) { return; }
    }
    public async Task CheckOnLanding() 
    {
        while (!_base._cHurtBox.IsGrounded()) 
        {
            await Task.Yield();
        }
    }
    void ResetComboInformation()
    {
        _base._cAnimator.SetShake(false);
        _base._cStateMachine.opponentComboCounter.OnEndCombo();
        _base._cDamageCalculator.ResetScaling();
        _base._cDamageCalculator.ClearDamageText();
    }

    async Task WaitToEndSuperMobility()
    {
        float OneFrame = 1 / 60f;
        float waitTime = 10 * OneFrame;
        int timeInMS = (int)(waitTime * 1000f);
        await Task.Delay(timeInMS);
        _base._cComboDetection.superMobilityOption = false;
    }
    void IdleCheck()
    {
        if (_cAnim.CheckAttackAndMobility() && _base.ReturnMovementInputs().Button_State.directionalInput == 5)
        {
            _cAnim.PlayNextAnimation(groundIdleHash, _crossFade);
        }

        _base._cHurtBox.SetHurboxState(HurtBoxType.NoBlock);
        _base._cHurtBox.SetHitboxSize(HurtBoxSize.Standing);
    }

    public override void OnExit()
    {
        Messenger.Broadcast(Events.ClearLastTime);
        base.OnExit();

        ITransition nextTransition = _base._cStateMachine._playerState.GetTransition();

        if (nextTransition.To == _base._cStateMachine.moveStateRef)
        {
            if (_base.ReturnMovementInputs().Button_State.directionalInput == 4)
            {
                _cAnim.PlayNextAnimation(moveBHash, _crossFade);
            }
            if (_base.ReturnMovementInputs().Button_State.directionalInput == 6)
            {
                _cAnim.PlayNextAnimation(moveFHash, _crossFade);
            }
        }
    }
}
