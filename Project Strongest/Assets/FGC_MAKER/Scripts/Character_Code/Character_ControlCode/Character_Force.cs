using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using FightingGame_FrameData;

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
    [SerializeField] bool isFrozen;
    [SerializeField] private Rigidbody _myRB => _base.myRb;
    [SerializeField] private Player_SideRecognition _side => _base.pSide;

    private float jumpSpeed;
    private float forwardSpeed;
    private bool canToggleKinematic;
    public float xSpeed;
    public bool beingPushed, stillnessCheck;
    bool sendingForce;
    List<IState> acceptableStates = new List<IState>();
    Dictionary<MovementType, Vector2> movementTypeVectors = new Dictionary<MovementType, Vector2>();
    public void Start()
    {
    }
    public void Force_InitFunctions()
    {
        isFrozen = false;
        sendingForce = false;
        canToggleKinematic = true;
        jumpSpeed = ((_base.JumpForce + (0.5f * Time.fixedDeltaTime * -_base._cGravity.ReturnCurrentGravity())) / _myRB.mass);
        forwardSpeed = ((-_base.JumpDirForce + (0.5f * Time.fixedDeltaTime * -_myRB.drag)) / _myRB.mass);

        #region Movement Dictionary Setup
        float neutralSuperJumpYVal = (_myRB.velocity.y + EvaluateAndReturnJumpValue()) + (_myRB.velocity.y + EvaluateAndReturnJumpValue() / 8.75f);
        float superJumpYVal = (_myRB.velocity.y + EvaluateAndReturnJumpValue()) - (_myRB.velocity.y + EvaluateAndReturnJumpValue()/8f);
        float superJumpXVal = Mathf.Abs((_myRB.velocity.x + EvaluateAndReturnForwardValue()) + (_myRB.velocity.x + EvaluateAndReturnForwardValue() / 1.15f));


        movementTypeVectors.Add(MovementType.NeutralSuperJump,new Vector2(0, neutralSuperJumpYVal));
        movementTypeVectors.Add(MovementType.Jump, new Vector2(0, _myRB.velocity.y + EvaluateAndReturnJumpValue()));
        movementTypeVectors.Add(MovementType.ForwardSuperJump, new Vector2(superJumpXVal, superJumpYVal));
        movementTypeVectors.Add(MovementType.BackSuperJump, new Vector2(-superJumpXVal, superJumpYVal));
        movementTypeVectors.Add(MovementType.ForwardJump, new Vector2(_myRB.velocity.x + (-EvaluateAndReturnForwardValue()), _myRB.velocity.y + EvaluateAndReturnJumpValue()));
        movementTypeVectors.Add(MovementType.BackJump, new Vector2(_myRB.velocity.x + EvaluateAndReturnForwardValue(), _myRB.velocity.y + EvaluateAndReturnJumpValue())); 
        movementTypeVectors.Add(MovementType.BackDash, new Vector2());
        movementTypeVectors.Add(MovementType.ForwardDash, new Vector2());
        #endregion
    }
    public void AddAcceptableStates() 
    {
        acceptableStates.Add(_base._cStateMachine.jumpRef);
        acceptableStates.Add(_base._cStateMachine.moveStateRef);
        acceptableStates.Add(_base._cStateMachine.dashStateRef);
        acceptableStates.Add(_base._cStateMachine.crouchStateRef);
        acceptableStates.Add(_base._cStateMachine.idleStateRef);
        acceptableStates.Add(_base._cStateMachine.attackingStateRef);
        acceptableStates.Add(_base._cStateMachine.superState);
    }
    public bool CanSendForce()
    {
        return !sendingForce;
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
    public void ForceStillPlayer() 
    {
        if (!beingPushed)
        {
            if (checkAllowPush(_base._cStateMachine._playerState.current.State))
            {
                _myRB.drag = 100000;
                return;
            }
        }
        _myRB.drag = 1;
    }
    bool checkAllowPush(IState curState) 
    {
        List<IState> acceptableHoldStates = new List<IState>();
        acceptableHoldStates.Add(_base._cStateMachine.dashStateRef);
        acceptableHoldStates.Add(_base._cStateMachine.secondIdleState);
        acceptableHoldStates.Add(_base._cStateMachine.idleStateRef);
        bool attackAwaitCondition = true;
        if (_base.opponentPlayer._cAnimator.CheckAttackState())
        {
            if (_base.opponentPlayer._cAnimator.lastAttack._moveType == MoveType.Super) 
            {
                attackAwaitCondition = _base.opponentPlayer.awaitCondition;
            }
        }
        stillnessCheck = !_base._cAnimator.CheckAttackState() && acceptableHoldStates.Contains(curState) 
            && _base._cHurtBox.IsGrounded() && _base._cAnimator.activatedInput == null && _base.ReturnMovementInputs() != null 
            && (_base.ReturnMovementInputs().Button_State.directionalInput < 4 || _base.ReturnMovementInputs().Button_State.directionalInput == 5) && attackAwaitCondition;
        return stillnessCheck;
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
        if (_base._cAnimator._lastAttackState == lastAttackState.nullified)
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
    public float GetXSpeed() 
    {
        return _myRB.velocity.x;
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
    #region Callback Functions
    public void AddLateralForceOnCommand(CustomCallback callback = null)
    {
        if (_side.thisPosition._directionFacing == Character_Face_Direction.FacingLeft)
        {
            float val = callback.forceFloat * -1;
            _myRB.AddForce(transform.right * val, ForceMode.VelocityChange);
        }
        else
        {
            _myRB.AddForce(transform.right * callback.forceFloat, ForceMode.VelocityChange);
        }
        if (beingPushed)
        {
            beingPushed = false;
        }
    }
    public void AddVerticalForceOnCommand(CustomCallback callback = null)
    {
        _myRB.AddForce(transform.up * callback.forceFloat, ForceMode.VelocityChange);
    }
    public void TeleportOnCommand(CustomCallback callback = null)
    {
        Vector3 curPos = new Vector3(_myRB.transform.position.x, _myRB.transform.position.y, _myRB.transform.position.z);
        Vector3 newPos = Vector3.zero;
        if (_side.thisPosition._directionFacing == Character_Face_Direction.FacingLeft)
        {
            newPos = new Vector3(_myRB.transform.position.x + -callback.forceFloat, _myRB.transform.position.y, _myRB.transform.position.z);
        }
        else if (_side.thisPosition._directionFacing == Character_Face_Direction.FacingRight)
        {
            newPos = new Vector3(_myRB.transform.position.x + callback.forceFloat, _myRB.transform.position.y, _myRB.transform.position.z);
        }

        if (!GameManager.instance.CheckWallGreaterPos(ref newPos))
        {
            float opponentX = _base.opponentPlayer._cForce._myRB.transform.localPosition.x;
            float bias;
            if (GetBiasForTeleportPos(newPos.x, opponentX, out bias))
            {
                if (bias != 0)
                {
                    if (_side.thisPosition._directionFacing == Character_Face_Direction.FacingLeft)
                    {
                        newPos = new Vector3(_myRB.transform.position.x + bias, _myRB.transform.position.y, _myRB.transform.position.z);
                    }
                    else if (_side.thisPosition._directionFacing == Character_Face_Direction.FacingRight)
                    {
                        newPos = new Vector3(_myRB.transform.position.x - bias, _myRB.transform.position.y, _myRB.transform.position.z);
                    }
                }
            }
        }
        _myRB.transform.position = Vector3.Slerp(curPos, newPos, 1f);
    }
    #endregion
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
    public void TeleportOnCommand(float value)
    {
        Vector3 curPos = new Vector3(_myRB.transform.position.x, _myRB.transform.position.y, _myRB.transform.position.z);
        Vector3 newPos = Vector3.zero;
        if (_side.thisPosition._directionFacing == Character_Face_Direction.FacingLeft)
        {
            newPos = new Vector3(_myRB.transform.position.x + -value, _myRB.transform.position.y, _myRB.transform.position.z);
        }
        else if (_side.thisPosition._directionFacing == Character_Face_Direction.FacingRight) 
        {
            newPos = new Vector3(_myRB.transform.position.x + value, _myRB.transform.position.y, _myRB.transform.position.z);
        }

        if (!GameManager.instance.CheckWallGreaterPos(ref newPos)) 
        {
            float opponentX = _base.opponentPlayer._cForce._myRB.transform.localPosition.x;
            float bias;
            if (GetBiasForTeleportPos(newPos.x, opponentX,out bias))
            {
                if (bias != 0)
                {
                    if (_side.thisPosition._directionFacing == Character_Face_Direction.FacingLeft)
                    {
                        newPos = new Vector3(_myRB.transform.position.x + bias, _myRB.transform.position.y, _myRB.transform.position.z);
                    }
                    else if (_side.thisPosition._directionFacing == Character_Face_Direction.FacingRight)
                    {
                        newPos = new Vector3(_myRB.transform.position.x - bias, _myRB.transform.position.y, _myRB.transform.position.z);
                    }
                }
            }
        }
        _myRB.transform.position = Vector3.Slerp(curPos, newPos, 1f);
    }

    bool GetBiasForTeleportPos(float teleportPoint, float opponentPos, out float bias)
    {
        float minDistance = 0f;
        bias = 0f;
        if (_side.thisPosition._directionFacing == Character_Face_Direction.FacingLeft)
        {
            if (teleportPoint > opponentPos)
            {
                return true;
            }
            else
            {
                minDistance = _side.thisPosition._directionFacing == Character_Face_Direction.FacingLeft ? Mathf.Abs(teleportPoint) + Mathf.Abs(opponentPos) : Mathf.Abs(teleportPoint) - Mathf.Abs(opponentPos);
                if (minDistance > 0.5f)
                {
                    bias = _side.thisPosition._directionFacing == Character_Face_Direction.FacingLeft ? bias = opponentPos - 0.5f : bias = opponentPos + 0.5f;
                    return true;
                }
                else
                {
                    bias = _side.thisPosition._directionFacing == Character_Face_Direction.FacingLeft ? bias = 0.5f : bias = -0.5f;
                    return true;
                }
            }
        }
        if (_side.thisPosition._directionFacing == Character_Face_Direction.FacingRight)
        {
            if (teleportPoint < opponentPos)
            {
                return true;
            }
            else
            {
                minDistance =  Mathf.Abs(teleportPoint) + Mathf.Abs(opponentPos) ;
                if (minDistance > 0.5f)
                {
                    bias = _side.thisPosition._directionFacing == Character_Face_Direction.FacingLeft ? bias = opponentPos + 0.5f : bias = opponentPos - 0.5f;
                    return true;
                }
                else
                {
                    bias = _side.thisPosition._directionFacing == Character_Face_Direction.FacingLeft ? bias = 0.5f : bias = -0.5f;
                    return true;
                }
            }
        }
        return false;
    }

    #region Function Summary
    /// <summary>
    /// Upon Correct MovementInput being done, moves the target character based on movementinput type
    /// </summary>
    /// <returns></returns>
    #endregion
    void DoForceOnDelay(Character_MobilityOption _mInput)
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
        _base._aManager.ClearAttacks();
        Vector2 moveVectors = new Vector2();
        if (movementTypeVectors.TryGetValue(_mInput.GetMovementType(), out moveVectors))
        {
            if ( _mInput.GetMovementType() == MovementType.ForwardDash)
            {
                _myRB.constraints = RigidbodyConstraints.FreezeAll;
                StartCoroutine(OnDelayDash(forwardMult * _base.DashForce));
                return;
            }
            if (_mInput.GetMovementType() == MovementType.BackDash)
            {
                _myRB.constraints = RigidbodyConstraints.FreezeAll;
                StartCoroutine(OnDelayDash(forwardMult * -_base.DashForce));
                return;
            }
            xVal = moveVectors.x;
            yVal = moveVectors.y;
            _myRB.velocity = new Vector3(forwardMult * xVal, yVal);
            if (_mInput._requiresCharge) 
            {
                _base._cComboDetection.superMobilityOption = false;
            }
            sendingForce = false;
        }
    }
    public void HandleExtraMovement(Character_MobilityOption _mInput)
    {
        if (_base._cHurtBox.IsGrounded())
        {
            if (_mInput.movementPriority == 2)
            {
                bool attackDashCancellable = true;
                if (_base._cAnimator.CheckAttackState()) 
                {
                    if (!_base._cAnimator.lastAttack.dashCancelable) 
                    {
                        attackDashCancellable = false;
                    }
                    else { attackDashCancellable = true; }
                }
                List<IState> validDashStates = new List<IState>() 
                {
                    _base._cStateMachine.dashStateRef,
                    _base._cStateMachine.attackingStateRef
                };
                bool inDashAvailableState = validDashStates.Contains(_base._cStateMachine._playerState.current.State);
                if (!inDashAvailableState || !attackDashCancellable)
                {
                    acceptableStates.Clear();
                    sendingForce = false;
                    return;
                }
                _base._cAnimator.PlayNextAnimation(_mInput._animInformation._animHash, 2 * Base_FrameCode.ONE_FRAME, true);
            }
            else
            {
                bool attackJumpCancellable = true;
                if (_base._cAnimator.CheckAttackState())
                {
                    if (!_base._cAnimator.lastAttack.JumpCancelable)
                    {
                        attackJumpCancellable = false;
                    }
                    else { attackJumpCancellable = true; }
                }
                acceptableStates.Clear();
                AddAcceptableStates();
                if (!acceptableStates.Contains(_base._cStateMachine._playerState.current.State) || !attackJumpCancellable)
                {
                    acceptableStates.Clear();
                    sendingForce = false;
                    return;
                }
                _base._cAnimator.PlayNextAnimation(_mInput._animInformation._animHash, 2 * Base_FrameCode.ONE_FRAME, true);
            }
            _base.movementPC = _mInput.movementPriority;
            _base._cAnimator.PlayNextAnimation(_mInput._animInformation._animHash, 2 * Base_FrameCode.ONE_FRAME, true);
            if (sendingForce == false)
            {
               DoForceOnDelay(_mInput);
                DebugMessageHandler.instance.DisplayErrorMessage(3, $"{_mInput.GetMovementType()} has been performed");
            }
        }
    }
    IEnumerator OnDelayDash(float speed)
    {
        yield return new WaitForSeconds(Base_FrameCode.ONE_FRAME);
        _myRB.constraints = (RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ);
        _myRB.AddForce(transform.right * speed, ForceMode.VelocityChange);
        sendingForce = false;
    }
    #region Function Summary
    /// <summary>
    /// Knocks Away Targeted Opponent with provided Attacks Knockback Values
    /// </summary>
    /// <param name="property"></param>
    #endregion
    public void SendKnockBackOnHit(Attack_BaseProperties property,bool blockedAttack)
    {
        _base.UnlockPosWithoutStorage();
        float H_KnockBack = 0;
        float V_KnockDown = 0;
        if (blockedAttack) 
        {
            H_KnockBack = property.LateralKB_Data.Block_Value;
            V_KnockDown = property.VerticalKB_Data.Block_Value;
            if (property.VerticalKB_Data.Block_VKB_Level.ToString().Contains("_KD"))
            {
                V_KnockDown = -property.VerticalKB_Data.Block_Value;
            }
        }
        else 
        {
            H_KnockBack = property.LateralKB_Data.Hit_Value;
            if (property.VerticalKB_Data.Hit_VKB_Level.ToString().Contains("_KD"))
            {
                V_KnockDown = -property.VerticalKB_Data.Hit_Value;
            }
            V_KnockDown = property.VerticalKB_Data.Hit_Value - _base._cDamageCalculator.GetCurrentScaling();
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
                    _myRB.AddForce(transform.up * -V_KnockDown, ForceMode.VelocityChange);
                }
                else 
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
    public void SendKnockBackOnHit(CustomDamageField property)
    {
        float H_KnockBack = 0;
        float V_KnockDown = 0;

        string V_KnockbackType = property.verticalKBP.Hit_VKB_Level.ToString();

        H_KnockBack = property.lateralKBP.Hit_Value;
        V_KnockDown = V_KnockbackType.Contains("_KD") ? -property.verticalKBP.Hit_Value : property.verticalKBP.Hit_Value;

        if (_side.thisPosition._directionFacing == Character_Face_Direction.FacingLeft)
        {
            _myRB.AddForce(transform.right * H_KnockBack, ForceMode.VelocityChange);
            _myRB.AddForce(transform.up * V_KnockDown, ForceMode.VelocityChange);
        }
        else
        {
            _myRB.AddForce(transform.right * (-H_KnockBack), ForceMode.VelocityChange);
            _myRB.AddForce(transform.up * V_KnockDown, ForceMode.VelocityChange);
        }
    }

    public float EvaluateAndReturnJumpValue() 
    {
        jumpSpeed = ((_base.JumpForce + (0.5f * Time.fixedDeltaTime * -10f)) / _myRB.mass);
        return jumpSpeed;
    }
    public float EvaluateAndReturnForwardValue()
    {
        forwardSpeed = ((-_base.JumpDirForce + (0.5f * Time.fixedDeltaTime * -_myRB.drag)) / _myRB.mass);
        return forwardSpeed;
    }

    public void DoGroundBounce(Attack_BaseProperties groundBouncingAttack) 
    {
        float groundBounceValue = groundBouncingAttack.VerticalKB_Data.Hit_Value - (groundBouncingAttack.VerticalKB_Data.Hit_Value * 0.45f);
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
        float heightLaunch = Mathf.Abs(wallBouncingAttack.VerticalKB_Data.Hit_Value + (wallBouncingAttack.VerticalKB_Data.Hit_Value * 1.25f));

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
