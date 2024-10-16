using System;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.Animations;

public class State_Idle : BaseState
{
    private bool canDoSecondaryIdle;
    private const float startSecondaryIdle = 10f;
    private float timeTillSecondaryIdle;
    private bool inIdle;
    public State_Idle(Character_Base playerBase) : base(playerBase){ }
    public override async void OnEnter()
    {
        canDoSecondaryIdle = false;
        if (_base._subState == Character_SubStates.Controlled)
        {
            Task[] tasks = new Task[]
            {
                WaitToEndSuperMobility(),
                CheckOnLanding(),
            };
            await Task.WhenAll(tasks);
            _base._cAnimator._canRecover = false;
            _base._cAnimator.canBlock = false;
            if (_base._cStateMachine.opponentComboCounter.CurrentHitCount > 0)
            {
                ResetComboInformation();
            }
            PlayerCPUIdleCheck();
            if (_base.ReturnMovementInputs() != null)
            {
                _baseForce.SetWalkForce(_base.ReturnMovementInputs());
            }
        }
        else 
        {
            Task[] tasks = new Task[]
            {
                CheckOnLanding(),
            };
            await Task.WhenAll(tasks);
            if (_base._cStateMachine.opponentComboCounter.CurrentHitCount > 0)
            {
                ResetComboInformation();
            }
            DummyIdleCheck();
        }
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

    public async override void OnUpdate()
    {
        if (!_base.isLockedPause)
        {
            if (canDoSecondaryIdle)
            {
                if (timeTillSecondaryIdle >= 0)
                {
                    timeTillSecondaryIdle -= (1 / 60f);
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
        canDoSecondaryIdle = false;
        _cAnim.PlayNextAnimation(secondaryIdleHash, 2 * (1 / 60f));
        await Task.Delay(100);
        int secondaryAnimDelayTime = (int)(_cAnim.myAnim.GetCurrentAnimatorStateInfo(0).length * 1000);

        while (startTime < secondaryAnimDelayTime) 
        {
            startTime += 16;
            await Task.Yield();
        }
        if (inIdle)
        {
            while (_base.isLockedPause)
            {
                await Task.Yield();
            }
            _cAnim.PlayNextAnimation(groundIdleHash, 2 * (1 / 60f));

        }
        ResetTime();
    }
    void ResetTime() 
    {
        timeTillSecondaryIdle = startSecondaryIdle;
        canDoSecondaryIdle = true;
    }
    void PlayerCPUIdleCheck()
    {
        _base._cHurtBox.SetHurboxState();
        _cAnim.ClearLastAttack();
        if (_base.ReturnMovementInputs().Button_State.directionalInput == 5 && _base._cAnimator.lastAttack == null)
        {
            _cAnim.PlayNextAnimation(groundIdleHash, 2 * (1 / 60f));
            _base._aManager.ResetMoveHierarchy();
        }
        canDoSecondaryIdle = true;
        inIdle = true;
        timeTillSecondaryIdle = startSecondaryIdle;
    }
    
    void DummyIdleCheck()
    {
        _base._cHurtBox.SetHurboxState(HurtBoxType.NoBlock);
        _cAnim.PlayNextAnimation(groundIdleHash, 2 * (1 / 60f));
        canDoSecondaryIdle = true;
        inIdle = true;
        timeTillSecondaryIdle = startSecondaryIdle;
    }
    public override void OnExit()
    {
        Messenger.Broadcast(Events.ClearLastTime);
        base.OnExit();
        inIdle = false;
        canDoSecondaryIdle = false;
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
