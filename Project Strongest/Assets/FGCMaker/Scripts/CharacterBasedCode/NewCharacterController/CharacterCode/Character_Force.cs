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
    private float jumpSpeed;
    private float forwardSpeed;
    [SerializeField] bool isFrozen;
    private bool canToggleKinematic;
    public void Start()
    {
        isFrozen = false;
        canToggleKinematic = true;
        jumpSpeed = ((_base.JumpForce + (0.5f * Time.fixedDeltaTime * -_base._cGravity.ReturnCurrentGravity())) / _myRB.mass);
        forwardSpeed = ((-_base.JumpDirForce + (0.5f * Time.fixedDeltaTime * -_myRB.drag)) / _myRB.mass);
    }
    private void Update()
    {
        _base._cAnimator.myAnim.SetFloat("y", _myRB.velocity.y);
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
        switch (dInput.Button_State.directionalInput)
        {
            case 4:
                _myRB.AddForce(transform.right * (-_base.MoveForce), ForceMode.VelocityChange);
                break;
            case 6:
                _myRB.AddForce(transform.right * (_base.MoveForce), ForceMode.VelocityChange);
                break;
        }
    }
    public void AddForceOnCommand(float value)
    {
        if (_side.thisPosition._directionFacing == Character_Face_Direction.FacingLeft)
        {
            value *= -1;
        }
        _myRB.AddForce(transform.right * value, ForceMode.VelocityChange);
    }

    #region Function Summary
    /// <summary>
    /// Upon Correct MovementInput being done, moves the target character based on movementinput type
    /// </summary>
    /// <returns></returns>
    #endregion
    public void HandleExtraMovement(Character_Mobility _mInput)
    {
        if (_base.movementPC < _mInput.movementPriority)
        {
            _base.movementPC = _mInput.movementPriority;
            switch (_mInput.type)
            {
                case MovementType.BackJump:
                    // Back Jump;
                    yVal = _myRB.velocity.y + EvaluateAndReturnJumpValue();
                    xVal = _myRB.velocity.x + EvaluateAndReturnForwardValue();
                    _myRB.velocity = new Vector3(xVal, yVal);
                    break;
                case MovementType.Jump:
                    // Neutral Jump;
                    _myRB.velocity = new Vector3(_myRB.velocity.x, _myRB.velocity.y + EvaluateAndReturnJumpValue());
                    break;
                case MovementType.ForwardJump:
                    // Forward Jump;
                    yVal = _myRB.velocity.y + EvaluateAndReturnJumpValue();
                    xVal = _myRB.velocity.x + -(EvaluateAndReturnForwardValue());
                    _myRB.velocity = new Vector3(xVal, yVal);
                    break;
                case MovementType.NeutralSuperJump:
                    // Neutral Super Jump;
                    _myRB.velocity = new Vector3(_myRB.velocity.x, _myRB.velocity.y + EvaluateAndReturnJumpValue() + 0.5f);
                    break;
                case MovementType.ForwardSuperJump:
                    // Forward Super Jump;
                    yVal = _myRB.velocity.y + EvaluateAndReturnJumpValue() + 0.5f;
                    xVal = _myRB.velocity.x + EvaluateAndReturnForwardValue() + 7;
                    _myRB.velocity = new Vector3(xVal, yVal);

                    break;
                case MovementType.BackSuperJump:
                    // Back Super Jump;
                    yVal = _myRB.velocity.y + EvaluateAndReturnJumpValue() + 0.5f;
                    xVal = _myRB.velocity.x + -(EvaluateAndReturnForwardValue() + 7);
                    _myRB.velocity = new Vector3(xVal, yVal);
                    break;
                case MovementType.ForwardDash:
                    // Forward Dash;
                    _myRB.AddForce(transform.right * (_base.MoveForce * 20f), ForceMode.VelocityChange);
                    break;
                case MovementType.BackDash:
                    // Back Dash;
                    _myRB.AddForce(transform.right * -(_base.MoveForce * 20f), ForceMode.VelocityChange);
                    break;
            }
            DebugMessageHandler.instance.DisplayErrorMessage(3, $"{_mInput.type} has been performed");
        }
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
            _myRB.AddForce(transform.up * V_KnockDown, ForceMode.VelocityChange);
        }
        else
        {
            _myRB.AddForce(transform.right * (-H_KnockBack), ForceMode.VelocityChange);
            _myRB.AddForce(transform.up * V_KnockDown, ForceMode.VelocityChange);
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
            AddForceOnCommand(4);
        }
    }
}
