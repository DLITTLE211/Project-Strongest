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
        base.OnEnter();
        _base._cHurtBox.ResetExtendedHurtbox();
        _base.allowSecondIdleAnim = false;
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
        float waitTime = 20 * OneFrame;
        int timeInMS = (int)(waitTime * 1000f);
        await Task.Delay(timeInMS);
        _base._cComboDetection.superMobilityOption = false;
    }

    public override void OnUpdate()
    {
        if (!_base.isLockedPause)
        {
            if (canDoSecondaryIdle)
            {
                if (timeTillSecondaryIdle <= -(1 / 60f))
                {
                    CallSecondaryIdleAnim();
                }
                else
                {
                    timeTillSecondaryIdle -= (1 / 60f);
                }
            }
        }
        base.OnUpdate();
    }
    public void CallSecondaryIdleAnim() 
    {
        canDoSecondaryIdle = false;
        _base.TriggerSecondaryIdleAnim();
        ResetTime();
    }
    void ResetTime() 
    {
        timeTillSecondaryIdle = startSecondaryIdle;
        canDoSecondaryIdle = true;
    }
    async void PlayerCPUIdleCheck()
    {
        _base._cHurtBox.SetHurboxState();
        _cAnim.ClearLastAttack();
        while (!CanTransitionToIdle())
        {
            await DelayFrame();
        }
        _cAnim.PlayNextAnimation(groundIdleHash, 2 * (1 / 60f));
        _base._aManager.ResetMoveHierarchy();
        canDoSecondaryIdle = true;
        timeTillSecondaryIdle = startSecondaryIdle;
    }
    async Task DelayFrame() 
    {
        await Task.Yield();
    }
    public bool CanTransitionToIdle() 
    {
        bool neutralInput = _base.ReturnMovementInputs().Button_State.directionalInput == 5;
        bool notPressingButtons = true;
        for(int i = 0; i < _base.attackButtons.Count; i++) 
        {
            if(_base.attackButtons[i].Button_State._state != ButtonStateMachine.InputState.released) 
            {
                notPressingButtons = false;
                break;
            }
            if (_base.blockButton.Button_State._state != ButtonStateMachine.InputState.released)
            {
                notPressingButtons = false;
                break;
            }
            continue;
        }
        bool lastAttackNull = _base._cAnimator.lastAttack == null;
        bool lastMobilityNull = _base._cAnimator.activatedInput == null;
        bool canTransitionIdle = _cAnim.canTransitionIdle == true;
        return neutralInput && notPressingButtons && lastAttackNull && lastMobilityNull && canTransitionIdle;
    }
    
    void DummyIdleCheck()
    {
        _base._cHurtBox.SetHurboxState(HurtBoxType.NoBlock);
        _cAnim.PlayNextAnimation(groundIdleHash, 2 * (1 / 60f));
        canDoSecondaryIdle = true;
        timeTillSecondaryIdle = startSecondaryIdle;
    }
    public override void OnExit()
    {
        base.OnExit();
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
