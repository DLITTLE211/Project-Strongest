using System;
using UnityEngine;
using System.Threading.Tasks;

public class State_Idle : BaseState
{
    private bool isAnimatingIdle;
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
                if (_cAnim.lastAttack == null)
                {
                    ResetComboInformation();
                }
                IdleCheck();
            }
            else
            {
                if (_cAnim.lastAttack == null)
                {
                    ResetComboInformation();
                }
                IdleCheck();

            }
        }
        else
        {
            await CheckOnLanding(); 
            if (_cAnim.lastAttack == null)
            {
                ResetComboInformation();
            }
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
    public async Task CheckOutOfThrow()
    {
        while (_base.opponentPlayer._cAttackTimer.ReturnInThrowAnim())
        {
            await Task.Yield();
        }
    }
    public async Task CheckOnLanding() 
    {
        while (!_base._cHurtBox.IsGrounded()) 
        {
            await Task.Yield();
        }
    }
    async void ResetComboInformation()
    {
        _base._cAnimator.SetShake(false);
        await CheckOutOfThrow();
        _base._cStateMachine.opponentComboCounter.OnEndCombo();

        _base._cDamageCalculator.ResetScaling();
        _base._cDamageCalculator.ClearDamageText();
    }

    async Task WaitToEndSuperMobility()
    {
        float OneFrame = 1 / 60f;
        float waitTime = 2 * OneFrame;
        int timeInMS = (int)(waitTime * 1000f);
        await Task.Delay(timeInMS);
        _base._cComboDetection.superMobilityOption = false;
    }

    public override void OnUpdate()
    {
        if (!isAnimatingIdle) 
        {
            IdleCheck();
        }
        base.OnUpdate();
    }
    void IdleCheck()
    {
        try
        {
            if (!isAnimatingIdle)
            {
                if (_cAnim.CheckAttackAndMobility())
                {
                    if (_base._subState == Character_SubStates.Dummy)
                    {
                        isAnimatingIdle = true;
                        _cAnim.PlayNextAnimation(groundIdleHash, 2 * (1 / 60f));
                        _base._aManager.ResetMoveHierarchy();
                    }
                    else
                    {
                        if ((_base.ReturnMovementInputs().Button_State.directionalInput != 6 ^ _base.ReturnMovementInputs().Button_State.directionalInput <= 4))
                        {
                            isAnimatingIdle = true;
                            _cAnim.PlayNextAnimation(groundIdleHash, 2 * (1 / 60f));
                            _base._aManager.ResetMoveHierarchy();
                        }
                    }
                }
                else 
                {
                    _cAnim.ClearLastAttack();
                }
            }
            else
            {
                if (_cAnim.lastAttack != null)
                {
                    _cAnim.ClearLastAttack();
                }
            }
            if (_base._subState != Character_SubStates.Dummy)
            {
                _base._cHurtBox.SetHurboxState(HurtBoxType.NoBlock);
            }
            _base._cHurtBox.SetHitboxSize(HurtBoxSize.Standing);
            ResetComboInformation();
        }
        catch (NullReferenceException) 
        {
            return;
        }
    }

    public override void OnExit()
    {
        Messenger.Broadcast(Events.ClearLastTime);
        base.OnExit();
        isAnimatingIdle = false;
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
