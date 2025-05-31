using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class State_Move : BaseState
{
    bool onBack, onForward;
    OnMoveUpdate _onMoveUpdate;
    delegate void OnMoveUpdate();
    public State_Move(Character_Base playerBase) : base(playerBase)
    {
    }
    public override async void OnEnter()
    {
        base.OnEnter();
        _base._cHurtBox.SetHurboxState();
        _base._cHurtBox.ResetExtendedHurtbox();
        _base._cHitboxManager.DisableAllHitboxes();
        DebugMessageHandler.instance.DisplayErrorMessage(1, "Enter MoveState");


        Character_ButtonInput _baseDirectionalInput = _base.ReturnMovementInputs();
        if (_base.pSide.thisPosition._directionFacing != Character_Face_Direction.Neither)
        {
            _onMoveUpdate = _base.pSide.thisPosition._directionFacing == Character_Face_Direction.FacingRight ?
                () => CheckRightFaceState(_baseDirectionalInput) :
                () => CheckLeftFaceState(_baseDirectionalInput);

            _onMoveUpdate();
            _baseForce.SetWalkForce(_baseDirectionalInput);
        }

        float fourFrameWaitTime = 4 * (1 / 60f);
        int fourtimeInMS = (int)(fourFrameWaitTime * 1000f);
        await Task.Delay(fourtimeInMS);
        _baseForce.SetWalkForce(_base.ReturnMovementInputs());

    }
    public override void OnUpdate()
    {
        Character_ButtonInput _baseDirectionalInput = _base.ReturnMovementInputs();
        if (_base.pSide.thisPosition._directionFacing != Character_Face_Direction.Neither)
        {
            _onMoveUpdate = _base.pSide.thisPosition._directionFacing == Character_Face_Direction.FacingRight ? 
                () => CheckRightFaceState(_baseDirectionalInput) :
                () => CheckLeftFaceState(_baseDirectionalInput);

            _onMoveUpdate();
            _baseForce.SetWalkForce(_baseDirectionalInput);
            base.OnUpdate();
        }
    }
    void CheckRightFaceState(Character_ButtonInput moveInput)
    {
        if (moveInput != null)
        {
            if (moveInput.Button_State.directionalInput == 4)
            {
                if (!onBack)
                {
                    HandleBackwardAnimation();
                }
            }
            if (moveInput.Button_State.directionalInput == 6)
            {
                if (!onForward)
                {
                    HandleForwardAnimation();
                }
            }
        }
    }
    void CheckLeftFaceState(Character_ButtonInput moveInput)
    {
        if (moveInput != null)
        {
            if (moveInput.Button_State.directionalInput == 4)
            {
                if (!onForward)
                {
                    HandleForwardAnimation();
                }
            }
            if (moveInput.Button_State.directionalInput == 6)
            {
                if (!onBack)
                {
                    HandleBackwardAnimation();
                }
            }
        }
    }
    void HandleForwardAnimation() 
    {
        onForward = true;
        onBack = false;
        _cAnim.PlayNextAnimation(moveFHash, 1 * (1 / 60f));
    }
    void HandleBackwardAnimation()
    {
        onBack = true;
        onForward = false;
        _cAnim.PlayNextAnimation(moveBHash, 1 * (1 / 60f));
    }
    public override void OnRecov()
    {
        base.OnRecov();
    }

    public override void OnExit()
    {
        onForward = false;
        onBack = false;

        ITransition nextTransition = _base._cStateMachine._playerState.GetTransition();
        if (nextTransition.To == _base._cStateMachine.idleStateRef)
        {
            if (_base._subState == Character_SubStates.Controlled)
            {
                if (_base.ReturnMovementInputs().Button_State.directionalInput == 5)
                {
                    _cAnim.PlayNextAnimation(groundIdleHash, 2 * (1 / 60f));
                }
            }
            else 
            {
                _cAnim.PlayNextAnimation(groundIdleHash, 2 * (1 / 60f));
            }
        }
        base.OnExit();
    }
}
