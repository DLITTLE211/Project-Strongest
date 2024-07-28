using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Character_Force : MonoBehaviour
{
    /*
     * Basic jump
     * Up force 350
     * forwardJforce 125
     * Drag 1
     * 
     * Forward super jump
     * up force 375
     * forward jforce 175-185
     * drag 1
     */
    #region Accidentally created teleporting DO NOT DELETE
    /*Vector3 curPos = new Vector3(myRB.transform.position.x, myRB.transform.position.y, myRB.transform.position.z);
    Vector3 newPos = new Vector3(myRB.transform.position.x + 2, myRB.transform.position.y, myRB.transform.position.z);
    myRB.transform.position = Vector3.Slerp(curPos, newPos, 1f);*/
    #endregion
    [SerializeField] private InGameCameraController camController;
    [SerializeField] private Character_Base _base;
    [SerializeField] private float xVal, yVal;
    [SerializeField] private Rigidbody _myRB => _base.myRb;
    [SerializeField] private Player_SideRecognition _side => _base.pSide;
    [SerializeField] private HitPointCall forceHitCall;
    [SerializeField] private HitPointCall teleportCall;
    public HitPointCall ForceCall {get { return forceHitCall; }}
    public HitPointCall TeleportCall { get { return teleportCall; }}
    private float jumpSpeed;
    private float forwardSpeed;
    [SerializeField] bool isFrozen;
    private bool canToggleKinematic;
    public float xSpeed;
    bool sendingForce;
    public bool beingPushed;
    public void Start()
    {
        isFrozen = false; 
        sendingForce = false;
         canToggleKinematic = true;
        jumpSpeed = ((_base.JumpForce + (0.5f * Time.fixedDeltaTime * -_base._cGravity.ReturnCurrentGravity())) / _myRB.mass);
        forwardSpeed = ((-_base.JumpDirForce + (0.5f * Time.fixedDeltaTime * -_myRB.drag)) / _myRB.mass);

        Messenger.AddListener<CustomCallback>(Events.CustomCallback, ApplyForceOnCustomCallback);
    }
    public bool CanSendForce()
    {
        return !sendingForce;
    }
    void ApplyForceOnCustomCallback(CustomCallback callback) 
    {
        if (forceHitCall.HasFlag(callback.customCall)) 
        {
            switch (callback.customCall) 
            {
                case HitPointCall.Force_Up:
                    AddVerticalForceOnCommand(callback.forceFloat);
                    break;
                case HitPointCall.Force_Right:
                    AddLateralForceOnCommand(callback.forceFloat);
                    break;
            }
        }
        if (teleportCall.HasFlag(callback.customCall))
        {
            if (callback.customCall == HitPointCall.TeleportForward) 
            {
                TeleportOnCommand(callback.forceFloat);
            }
            if (callback.customCall == HitPointCall.TeleportBackward)
            {
                TeleportOnCommand(-callback.forceFloat);
            }
        }
    }
    private void Update()
    {
        if (_base._cAnimator != null)
        {
            _base._cAnimator.myAnim.SetFloat("Y_Float", _myRB.velocity.y);
        }
        xSpeed = _myRB.velocity.x;
        ForceStillPlayer();
    }
    void ForceStillPlayer() 
    {
        if (!beingPushed)
        {
            if (_base._cHurtBox.IsGrounded() && _base._cStateMachine._playerState.current.State == _base._cStateMachine.idleStateRef)
            {
                if (_base._cAnimator.activatedInput == null)
                {
                    if (_base.ReturnMovementInputs() != null)
                    {
                        if (_base.ReturnMovementInputs().Button_State.directionalInput == 5)
                        {
                            _myRB.drag = 100000;
                            return;
                        }
                    }
                }
            }
            if (_base._cHurtBox.IsGrounded() && _base._cStateMachine._playerState.current.State == _base._cStateMachine.dashStateRef)
            {
                if (_base._cAnimator.activatedInput == null)
                {
                    if (_base.ReturnMovementInputs() != null)
                    {
                        if (_base.ReturnMovementInputs().Button_State.directionalInput < 4)
                        {
                            _myRB.drag = 100000;
                            return;
                        }
                    }
                }
            }
        }
        _myRB.drag = 1;
    }
    public void HandleForceFreeze(bool state)
    {
        if (state)
        {
            if (!isFrozen)
            {
                isFrozen = true;
            }
        }
        else
        {
            if (isFrozen)
            {
                isFrozen = false;
            }
        }
        if (canToggleKinematic)
        {
            _myRB.isKinematic = isFrozen;
        }
    }
    public void CallUnlockKinematic()
    {
        canToggleKinematic = true;
        _myRB.isKinematic = isFrozen;
    }
    public void CallLockKinematic()
    {
        _myRB.isKinematic = isFrozen;
        canToggleKinematic = false;
    }
    #region Function Summary
    /// <summary>
    /// Resets movement's Priority Checker after movement timer is 0
    /// </summary>
    /// <returns></returns>
    #endregion
    public void ResetPriority()
    {
        _base.movementPC = 0;
    }
    #region Function Summary
    /// <summary>
    /// Moves characters based on Character's Movement values
    /// </summary>
    /// <returns></returns>
    #endregion
    public void SetWalkForce(Character_ButtonInput dInput)
    {
        if (_base._cAnimator._lastAttackState == Character_Animator.lastAttackState.nullified)
        {
            switch (dInput.Button_State.directionalInput)
            {
                case 4:
                    _myRB.drag = 1;
                    _myRB.velocity = new Vector3(-Mathf.RoundToInt(_base.MoveForce), _myRB.velocity.y, 0f);
                    break;
                case 6:
                    _myRB.drag = 1;
                    _myRB.velocity = new Vector3(Mathf.RoundToInt(_base.MoveForce), _myRB.velocity.y, 0f);
                    break;
            }
        }
    }
    public void AddVerticalForceOnCommand(float value)
    {
        _myRB.AddForce(transform.up * value, ForceMode.VelocityChange);
    }
    public void AddLateralForceOnCommand(float value, bool forceDirection = false)
    {
        if (!forceDirection)
        {
            if (_side.thisPosition._directionFacing == Character_Face_Direction.FacingLeft)
            {
                value *= -1;
            }
        }
        _myRB.AddForce(transform.right * value, ForceMode.VelocityChange);
        if (beingPushed)
        {
            beingPushed = false;
        }
    }
    public void InstantForceAway(float value, bool forceDirection = false)
    {
        if (!forceDirection)
        {
            if (_side.thisPosition._directionFacing == Character_Face_Direction.FacingLeft)
            {
                value *= -1;
            }
        }
        Vector3 curPos = new Vector3(_myRB.transform.position.x, _myRB.transform.position.y, _myRB.transform.position.z);
        Vector3 newPos = new Vector3(_myRB.transform.position.x + value, _myRB.transform.position.y, _myRB.transform.position.z);
        _myRB.transform.position = Vector3.Slerp(curPos, newPos, 1f);
        if (beingPushed)
        {
            beingPushed = false;
        }
    }
    void TeleportOnCommand(float value)
    {
        Vector3 curPos = new Vector3(_myRB.transform.position.x, _myRB.transform.position.y, _myRB.transform.position.z);
        Vector3 newPos;
        if (_side.thisPosition._directionFacing == Character_Face_Direction.FacingLeft)
        {
            value *= -1;
            newPos = new Vector3(-(_myRB.transform.position.x + 2), _myRB.transform.position.y, _myRB.transform.position.z);
            _myRB.transform.position = Vector3.Slerp(curPos, newPos, 1f);
            return;
        }
        newPos = new Vector3(_myRB.transform.position.x + 2, _myRB.transform.position.y, _myRB.transform.position.z);
        _myRB.transform.position = Vector3.Slerp(curPos, newPos, 1f);
    }

    #region Function Summary
    /// <summary>
    /// Upon Correct MovementInput being done, moves the target character based on movementinput type
    /// </summary>
    /// <returns></returns>
    #endregion
    IEnumerator DoForceOnDelay(Character_Mobility _mInput)
    {
        int forwardMult = 0;
        if (_base.pSide.thisPosition._directionFacing == Character_Face_Direction.FacingRight)
        {
            forwardMult = 1;
        }
        else
        {
            forwardMult = -1;
        }
        sendingForce = true;
        yield return new WaitForSeconds(2 / 60f);

        switch (_mInput.type)
        {
            case MovementType.BackJump:
                // Back Jump;
                yVal = _myRB.velocity.y + EvaluateAndReturnJumpValue();
                xVal = _myRB.velocity.x + EvaluateAndReturnForwardValue();
                _myRB.velocity = new Vector3(forwardMult * xVal, yVal);
                break;
            case MovementType.Jump:
                // Neutral Jump;
                _myRB.velocity = new Vector3(forwardMult * _myRB.velocity.x, _myRB.velocity.y + EvaluateAndReturnJumpValue());
                break;
            case MovementType.ForwardJump:
                // Forward Jump;
                yVal = _myRB.velocity.y + EvaluateAndReturnJumpValue();
                xVal = _myRB.velocity.x + -(EvaluateAndReturnForwardValue());
                _myRB.velocity = new Vector3(forwardMult * xVal, yVal);
                break;
            case MovementType.NeutralSuperJump:
                // Neutral Super Jump;
                _myRB.velocity = new Vector3(forwardMult * _myRB.velocity.x, _myRB.velocity.y + EvaluateAndReturnJumpValue() + 0.5f);
                break;
            case MovementType.ForwardSuperJump:
                // Forward Super Jump;
                yVal = _myRB.velocity.y + EvaluateAndReturnJumpValue() + 0.5f;
                xVal = _myRB.velocity.x + EvaluateAndReturnForwardValue() + 7;
                _myRB.velocity = new Vector3(forwardMult * xVal, yVal);

                break;
            case MovementType.BackSuperJump:
                // Back Super Jump;
                yVal = _myRB.velocity.y + EvaluateAndReturnJumpValue() + 0.5f;
                xVal = _myRB.velocity.x + -(EvaluateAndReturnForwardValue() + 7);
                _myRB.velocity = new Vector3(forwardMult * xVal, yVal);
                break;
            case MovementType.ForwardDash:
                // Forward Dash;
                StartCoroutine(OnDelayDash(forwardMult * _base.DashForce * 2f));
                break;
            case MovementType.BackDash:
                // Back Dash;
                StartCoroutine(OnDelayDash(forwardMult * -_base.DashForce * 2f));
                break;
        }
        yield return new WaitForSeconds(2 / 60f);
        sendingForce = false;
    }
    public void HandleExtraMovement(Character_Mobility _mInput)
    {
        List<IState> acceptableStates = new List<IState>();
        if (_base._cHurtBox.IsGrounded())
        {
            if (_mInput.movementPriority == 2)
            {
                acceptableStates.Add(_base._cStateMachine.dashStateRef);
                if (!acceptableStates.Contains(_base._cStateMachine._playerState.current.State))
                {
                    acceptableStates.Clear();
                    sendingForce = false;
                    return;
                }
            }
            else
            {
                acceptableStates.Add(_base._cStateMachine.jumpRef);
                acceptableStates.Add(_base._cStateMachine.crouchStateRef);
                acceptableStates.Add(_base._cStateMachine.idleStateRef);
                if (!acceptableStates.Contains(_base._cStateMachine._playerState.current.State))
                {
                    acceptableStates.Clear();
                    sendingForce = false;
                    return;
                }
            }
            _base.movementPC = _mInput.movementPriority;
            if (sendingForce == false)
            {
                StartCoroutine(DoForceOnDelay(_mInput));
                DebugMessageHandler.instance.DisplayErrorMessage(3, $"{_mInput.type} has been performed");
            }
        }
    }
    IEnumerator OnDelayDash(float speed)
    {
        yield return new WaitForSeconds(2 / 60f);
        _myRB.velocity = new Vector3(Mathf.RoundToInt(speed), _myRB.velocity.y);
    }
    #region Function Summary
    /// <summary>
    /// Knocks Away Targeted Opponent with provided Attacks Knockback Values
    /// </summary>
    /// <param name="property"></param>
    #endregion
    public void SendKnockBackOnHit(Attack_BaseProperties property)
    {
        int H_KnockBack = property.lateralKBP.Value;
        int V_KnockDown = property.verticalKBP.Value;
        if (property.verticalKBP.verticalKBP.ToString().Contains("_KD")) 
        {
            V_KnockDown = -property.verticalKBP.Value;
        }

        if (_side.thisPosition._directionFacing == Character_Face_Direction.FacingLeft)
        {
            _myRB.AddForce(transform.right * H_KnockBack, ForceMode.VelocityChange);
            if (property._airInfo == AirAttackInfo.GroundOnly)
            {
                _myRB.AddForce(transform.up * V_KnockDown, ForceMode.VelocityChange);
            }
            else if(property._airInfo == AirAttackInfo.AirOnly)
            {
                if (_base._cHurtBox.IsGrounded() == false) 
                {
                    _myRB.AddForce(transform.up * V_KnockDown, ForceMode.VelocityChange);
                }
            }
        }
        else
        {
            _myRB.AddForce(transform.right * (-H_KnockBack), ForceMode.VelocityChange); 
            if (property._airInfo == AirAttackInfo.GroundOnly)
            {
                _myRB.AddForce(transform.up * V_KnockDown, ForceMode.VelocityChange);
            }
            else if (property._airInfo == AirAttackInfo.AirOnly)
            {
                if (_base._cHurtBox.IsGrounded() == false)
                {
                    _myRB.AddForce(transform.up * V_KnockDown, ForceMode.VelocityChange);
                }
            }
        }
    }

    float EvaluateAndReturnJumpValue() 
    {
        jumpSpeed = ((_base.JumpForce + (0.5f * Time.fixedDeltaTime * -_base._cGravity.ReturnCurrentGravity())) / _myRB.mass);
        return jumpSpeed;
    }
    float EvaluateAndReturnForwardValue()
    {
        forwardSpeed = ((-_base.JumpDirForce + (0.5f * Time.fixedDeltaTime * -_myRB.drag)) / _myRB.mass);
        return forwardSpeed;
    }

    public void DoGroundBounce(Attack_BaseProperties groundBouncingAttack) 
    {
        float groundBounceValue = groundBouncingAttack.verticalKBP.Value - (groundBouncingAttack.verticalKBP.Value * 0.45f);
        _myRB.AddForce(transform.up * groundBounceValue);
    }
    public IEnumerator DoWallLaunch()
    {
        yield return new WaitForSeconds(1 / 60f);
        _myRB.AddForce(transform.up * 9, ForceMode.VelocityChange);
        DoWallBounce();
    }
    void DoWallBounce()
    {
        if (_side.thisPosition._directionFacing == Character_Face_Direction.FacingRight)
        {
            _myRB.AddForce(transform.right * 3, ForceMode.VelocityChange);
        }
        else
        {
            _myRB.AddForce(transform.right * (-3), ForceMode.VelocityChange);
        }
    }
    
    public void DoWallStick(Attack_BaseProperties wallBouncingAttack)
    {
        float heightLaunch = Mathf.Abs(wallBouncingAttack.verticalKBP.Value + (wallBouncingAttack.verticalKBP.Value * 1.25f));

        if (_side.thisPosition._directionFacing == Character_Face_Direction.FacingRight)
        {
            _myRB.AddForce(transform.up * heightLaunch, ForceMode.VelocityChange);
        }
        else
        {
            _myRB.AddForce(transform.up * heightLaunch, ForceMode.VelocityChange);
        }
    }

    public void RecoverWithDirection(float direction) 
    {
        if (_base.ReturnMovementInputs().Button_State.directionalInput == 4 
            ^_base.ReturnMovementInputs().Button_State.directionalInput == 4)
        {
            AddLateralForceOnCommand(4);
        }
    }
}
