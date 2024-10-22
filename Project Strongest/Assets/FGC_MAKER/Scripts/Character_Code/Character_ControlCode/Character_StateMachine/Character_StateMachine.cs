using System;
using UnityEngine;
using System.Collections.Generic;

public class Character_StateMachine : MonoBehaviour
{
    public Character_State _playerState;
    [SerializeField] private Character_Base _base;
    [SerializeField] public string curState;
    public Character_ComboCounter opponentComboCounter;
    [HideInInspector] public State_Idle idleStateRef;
    [HideInInspector] public State_Crouch crouchStateRef;
    [HideInInspector] public State_Move moveStateRef;
    [HideInInspector] public State_Dash dashStateRef;
    [HideInInspector] public State_Jump jumpRef;
    [HideInInspector] public State_Block standBlockRef;
    [HideInInspector] public State_CrouchBlock crouchBlockRef;
    [HideInInspector] public State_BlockReact blockReactRef;

    [HideInInspector] public State_Attacking attackingStateRef;
    [HideInInspector] public State_Throw throwState;
    [HideInInspector] public State_Counter counterState;
    [HideInInspector] public State_CustomSuper superState;

    [HideInInspector] public State_Hit hitStateRef;
    // Start is called before the first frame update
    private void Awake()
    {
    }
    public void DefineState() 
    {
        _playerState = new Character_State(_base);
        #region Define States
        var IdleState = new State_Idle(_base);
        idleStateRef = IdleState;
        var MoveState = new State_Move(_base);
        moveStateRef = MoveState;
        var JumpState = new State_Jump(_base);
        jumpRef = JumpState; 
        var AttackState = new State_Attacking(_base);
        attackingStateRef = AttackState;
        var ThrowState = new State_Throw(_base);
        throwState = ThrowState;
        var DashState = new State_Dash(_base);
        dashStateRef = DashState;
        var Hitstate = new State_Hit(_base);
        hitStateRef = Hitstate;
        var CrouchState = new State_Crouch(_base);
        crouchStateRef = CrouchState;
        var S_BlockState = new State_Block(_base);// S = Stand
        standBlockRef = S_BlockState;
        var C_BlockState = new State_CrouchBlock(_base); // C = Crouch 
        crouchBlockRef = C_BlockState;
        var BlockReact = new State_BlockReact(_base); // C = Crouch 
        blockReactRef = BlockReact;
        var CounterState = new State_Counter(_base); // C = Crouch 
        counterState = CounterState;
        var CustomSuperState = new State_CustomSuper(_base); // C = Crouch 
        superState = CustomSuperState;
        #endregion

        #region Define Transitions
        #region At States (Can move from State A -> State B upon Bool Check being Met)
        At(S_BlockState, IdleState, new Predicate(() => At_2Idle()));
        At(BlockReact, IdleState, new Predicate(() => At_2Idle()));
        At(Hitstate, IdleState, new Predicate(() => At_2Idle()));

        At(DashState, MoveState, new Predicate(() => At_2Move()));
        At(CrouchState, MoveState, new Predicate(() => At_2Move()));
        At(IdleState, MoveState, new Predicate(() => At_2Move()));
        At(S_BlockState, MoveState, new Predicate(() => At_2Move()));
        At(C_BlockState, MoveState, new Predicate(() => At_2Move()));
        At(JumpState, MoveState, new Predicate(() => At_Jump2Move()));
        At(AttackState, MoveState, new Predicate(() => At_2Move()));

        At(AttackState, ThrowState, new Predicate(() => At_2Throw()));

        At(AttackState, CustomSuperState, new Predicate(() => ToAttackState() && !At_2Throw() && !At_2Counter() && At_2CustomSuper()));
        At(JumpState, CustomSuperState, new Predicate(() => ToAttackState() && !At_2Throw() && !At_2Counter() && At_2CustomSuper()));

        At(AttackState, JumpState, new Predicate(() => At_2Jump()));
        At(IdleState, JumpState, new Predicate(() => At_2Jump()));
        At(MoveState, JumpState, new Predicate(() => At_2Jump()));
        At(CrouchState, JumpState, new Predicate(() => At_2Jump()));


        At(MoveState, DashState, new Predicate(() => ToDashState()));

        At(IdleState, CrouchState, new Predicate(() => At_2Crouch()));
        At(JumpState, CrouchState, new Predicate(() => At_2Crouch()));
        At(MoveState, CrouchState, new Predicate(() => At_2Crouch()));
        At(C_BlockState, CrouchState, new Predicate(() => At_2Crouch()));
        At(S_BlockState, CrouchState, new Predicate(() => At_2Crouch()));
        At(Hitstate, CrouchState, new Predicate(() => At_2Crouch()));
        At(DashState, CrouchState, new Predicate(() => At_2Crouch()));

        At(AttackState, Hitstate, new Predicate(() => checkAttackValue(lastAttackState.nullified) && At_2Crouch()));
        At(IdleState, Hitstate, new Predicate(() => ToHitState()));
        At(JumpState, Hitstate, new Predicate(() => ToHitState()));
        At(MoveState, Hitstate, new Predicate(() => ToHitState()));
        At(CrouchState, Hitstate, new Predicate(() => ToHitState()));
        At(AttackState, Hitstate, new Predicate(() => ToHitState()));
        At(S_BlockState, C_BlockState, new Predicate(() => At_2CBlock()));

        At(S_BlockState, BlockReact, new Predicate(() => At_2BlockReact()));
        At(C_BlockState, BlockReact, new Predicate(() => At_2BlockReact()));

        At(BlockReact, S_BlockState, new Predicate(() => At_2SBlock()));
        At(BlockReact, C_BlockState, new Predicate(() => At_2CBlock()));

        At(AttackState, CounterState, new Predicate(() => At_2Counter()));
        At(MoveState, CounterState, new Predicate(() => At_2Counter()));
        At(CrouchState, CounterState, new Predicate(() => At_2Counter()));
        At(AttackState, CounterState, new Predicate(() => At_2Counter()));

        #endregion

        #region Any States (Can Move to this state upon Bool Check Being Met)
        Any(AttackState, new Predicate(() => ToAttackState() && !At_2Throw() && !At_2Counter() && !At_2CustomSuper()));
        Any(ThrowState, new Predicate(() => ToAttackState() && At_2Throw() && !At_2Counter() && !At_2CustomSuper()));
        Any(CustomSuperState, new Predicate(() => At_2CustomSuper()));
        Any(S_BlockState, new Predicate(() => At_2SBlock()));
        Any(C_BlockState, new Predicate(() => At_2CBlock()));
        Any(DashState, new Predicate(() => ToDashState()));
        Any(IdleState, new Predicate(() => At_2Idle()));
        Any(Hitstate, new Predicate(() => ToHitState()));
        #endregion


        #endregion

        _playerState.SetState(IdleState);
    }
    #region Referencing PlayerState At/Any Functions
    void At(IState from, IState to, IPredicate condition) =>  _playerState.AddTransition(from,to,condition);
    void Any(IState to, IPredicate condition) => _playerState.AddAnyTransition(to, condition);
    #endregion

    private void Update()
    {
        _playerState.Update();
        curState = _playerState.CurrentStateString;
    }
    public void CallLandingCheck()
    {
        _playerState.SetState(new State_Idle(_base));
        _playerState.Update();
        curState = _playerState.CurrentStateString;
    }
    private void FixedUpdate()
    {
        _playerState.FixedUpdate();
    }
    #region Boolean Checks
    public bool _CheckBlockButton()
    {
        if (_base.ReturnBlockButton() == null) 
        { 
            return false; 
        }
        return _base.ReturnBlockButton().Button_State._state != ButtonStateMachine.InputState.released;
    }
    bool At_2SBlock()
    {
        bool notRecovering = _base._cHitController.ReturnNotRecovering();
        bool _canRecover = _base._cAnimator._canRecover;
        bool _isHit = _base._cAnimator.isHit;
        bool _isBlocking = false;
        bool _currentInput = false;
        bool _isGrounded = _base._cHurtBox.IsGrounded();
        bool _notAttacking = _base._cAnimator.lastAttack == null && checkAttackValue(lastAttackState.nullified);

        if (_base._subState != Character_SubStates.Controlled)
        {
            _isBlocking = false;
            _currentInput = false;
        }
        else
        {
            _currentInput = _base.ReturnMovementInputs().Button_State.directionalInput > 3;
            _isBlocking = _CheckBlockButton();
        }

        return !_isHit && _currentInput && _isBlocking && _isGrounded && !_canRecover && notRecovering && _notAttacking;
    }
    bool At_2BlockReact()
    {
        bool recovering = _base._cHitController.currentHitstun > 0;
        bool attackBlocked = _base._cHitController.CheckIfAttackBlocked();
        IState blockState = _playerState.current.State;
        if (blockState == standBlockRef)
        {
            return attackBlocked && recovering;
        }
        if (blockState == crouchBlockRef)
        {
            return attackBlocked && recovering;
        }
        return false;
    }
    bool At_2CBlock()
    {
        bool notRecovering = _base._cHitController.ReturnNotRecovering();
        bool _canRecover = _base._cAnimator._canRecover;
        bool _isHit = _base._cAnimator.isHit;
        bool _isBlocking;
        bool _currentInput;
        bool _isGrounded = _base._cHurtBox.IsGrounded();
        bool _notAttacking = _base._cAnimator.lastAttack == null && checkAttackValue(lastAttackState.nullified);
        try
        {
            if (_base._subState != Character_SubStates.Controlled)
            {
                _isBlocking = false;
                _currentInput = false;
            }
            else
            {
                _currentInput = _base.ReturnMovementInputs().Button_State.directionalInput <= 3;
                _isBlocking = _CheckBlockButton();
            }
            return !_isHit && _isBlocking && _currentInput && _isGrounded && !_canRecover && notRecovering && _notAttacking;
        }
        catch (ArgumentOutOfRangeException)
        {
            _currentInput = false;
            _isBlocking = false;
            return !_isHit && _isBlocking && _currentInput && _isGrounded && !_canRecover && notRecovering;
        }

    }
    bool At_2Throw()
    {
        bool _attackIsThrow = false;
        List<MoveType> acceptableThrowType = new List<MoveType>();
        if (_base._subState != Character_SubStates.Controlled)
        {
            return false;
        }
        if(_base._cAnimator.lastAttack != null) 
        {
            acceptableThrowType.Add(MoveType.Throw);
            acceptableThrowType.Add(MoveType.CommandGrab);
            _attackIsThrow = acceptableThrowType.Contains(_base._cAnimator.lastAttack._moveType) && _base._cAnimator.lastAttack.hitConnected;
            return _attackIsThrow;
        }
        return false;
    }
    bool At_2Counter()
    {
        bool _attackIsThrow = false;
        if (_base._subState != Character_SubStates.Controlled)
        {
            return false;
        }
        if (_base._cAnimator.lastAttack != null)
        {
            _attackIsThrow = _base._cAnimator.lastAttack._moveType == MoveType.Counter;
            return _attackIsThrow;
        }
        return false;
    }
    bool At_2CustomSuper()
    {
        bool _attackIsSuper = false;
        if (_base._subState != Character_SubStates.Controlled)
        {
            return false;
        }
        if (_base._cAnimator.lastAttack != null)
        {
            _attackIsSuper = _base._cAnimator.lastAttack._moveType == MoveType.Super;
            bool fullCheck = _attackIsSuper ;
            return fullCheck;
        }
        return false;
    }
    bool At_2Crouch()
    {
        bool notRecovering = _base._cHitController.ReturnNotRecovering();
        bool _canRecover = _base._cAnimator._canRecover;
        bool _isHit = _base._cAnimator.isHit;
        bool _currentInput;
        bool _isBlocking;
        bool _isGrounded = _base._cHurtBox.IsGrounded();
        bool _inRekkaOrStance = _base._cAnimator.inStanceState || _base._cAnimator.inRekkaState;
        bool _notAttacking = _base._cAnimator.lastAttack == null && checkAttackValue(lastAttackState.nullified);
        try
        {
            if (_base._subState != Character_SubStates.Controlled)
            {
                _currentInput = true;
                _isBlocking = false;
            }
            else
            {
                _isBlocking = _CheckBlockButton();
                _currentInput = _base.ReturnMovementInputs().Button_State.directionalInput <= 3;
            }
            return !_isHit && !_isBlocking &&_currentInput && _isGrounded && !_canRecover && notRecovering && !_inRekkaOrStance && _notAttacking;
        }
        catch (ArgumentOutOfRangeException)
        {
            _currentInput = false;
            _isBlocking = false;
            return !_isHit && !_isBlocking && _currentInput && _isGrounded && !_canRecover && notRecovering && !_inRekkaOrStance;
        }

    }
    bool At_2Idle()
    {
        bool notRecovering = _base._cHitController.ReturnNotRecovering();
        bool _canRecover = _base._cAnimator._canRecover;
        bool _isHit = _base._cAnimator.isHit;
        bool _currentInput;
        bool _isBlocking;
        bool inputtedDash;
        bool lastAttackValue;
        bool _isGrounded = _base._cHurtBox.IsGrounded();
        bool _canTransitionIdle = _base._cAnimator.canTransitionIdle;
        inputtedDash = CheckLastMovementValue();
        lastAttackValue = checkAttackValue(lastAttackState.nullified);
        try
        {
            if (_base._subState != Character_SubStates.Controlled)
            {
                _currentInput = true;
                _isBlocking = false;
                _canTransitionIdle = true;
            }
            else
            {
                _isBlocking = _CheckBlockButton();
                _currentInput = IdleReturnBool();
            }
            bool fullCheck = !_isHit && !_isBlocking && _currentInput && _isGrounded && !inputtedDash && lastAttackValue && !_canRecover && notRecovering && _canTransitionIdle;
            return fullCheck;
        }
        catch (ArgumentOutOfRangeException)
        {
            _currentInput = false;
            _isBlocking = false;
            return !_isHit && !_isBlocking && _currentInput && _isGrounded && !inputtedDash && lastAttackValue && !_canRecover && notRecovering;
        }
    }
    bool At_2Move() 
    {
        bool notRecovering = _base._cHitController.ReturnNotRecovering();
        bool _canRecover = _base._cAnimator._canRecover;
        bool _isHit = _base._cAnimator.isHit;
        bool _currentInput;
        bool _isBlocking;
        bool _isGrounded = _base._cHurtBox.IsGrounded();
        bool _notAttacking = _base._cAnimator.lastAttack == null && checkAttackValue(lastAttackState.nullified);
        try
        {
            if (_base._subState != Character_SubStates.Controlled)
            {
                _canRecover = false;
                _currentInput = false;
                _isBlocking = false;
            }
            else
            {
                _isBlocking = _CheckBlockButton();
                _currentInput = _base.ReturnMovementInputs().Button_State.directionalInput == 4 || _base.ReturnMovementInputs().Button_State.directionalInput == 6;
            }
            bool canMove = !_isHit && !_isBlocking && _currentInput && _isGrounded && !_canRecover && notRecovering && _notAttacking;
            return canMove;
        }
        catch (ArgumentOutOfRangeException)
        {
            _currentInput = false;
            _isBlocking = false;
            return !_isHit && !_isBlocking && _currentInput && _isGrounded && !_canRecover && notRecovering;
        }
      
    }
    bool At_2Jump()
    {
        bool attackNull = _base._cAnimator.lastAttack == null;
        bool notRecovering = _base._cHitController.ReturnNotRecovering();
        bool _isHit = _base._cAnimator.isHit; 
        bool _currentInput;
        bool _isBlocking;
        try
        {
            if (_base._subState != Character_SubStates.Controlled)
            {
                _currentInput = false;
                _isBlocking = false;
                attackNull = true;
            }
            else
            {
                _isBlocking = _CheckBlockButton();
                _currentInput = _base._cHurtBox.IsGrounded() == true ?_base.ReturnMovementInputs().Button_State.directionalInput >= 7 : true;
            }
            bool fullCheck = !_isHit && !_isBlocking && _currentInput && attackNull && notRecovering;
            return fullCheck;
        }
        catch (ArgumentOutOfRangeException)
        {
            _currentInput = false;
            _isBlocking = false;
            return !_isHit && !_isBlocking && _currentInput&& true  && notRecovering;
        }
    }
    bool At_Jump2Move()
    {
        bool notRecovering = _base._cHitController.ReturnNotRecovering();
        bool _canRecover = _base._cAnimator._canRecover;
        bool _isHit = _base._cAnimator.isHit;
        bool _isGrounded = _base._cHurtBox.IsGrounded();
        bool _isBlocking;
        bool _isLastMovePopulated = checkMovementValue(lastMovementState.nullified);
        bool _currentInput;
        try
        {
            if (_base._subState != Character_SubStates.Controlled)
            {
                _currentInput = true;
                _isBlocking = false;
                _isLastMovePopulated = true;
            }
            else
            {
                _isBlocking = _CheckBlockButton();
                _currentInput = _base.ReturnMovementInputs().Button_State.directionalInput == 4 || _base.ReturnMovementInputs().Button_State.directionalInput == 6;
            }
            return !_isHit && _isGrounded && !_isBlocking && _isLastMovePopulated && _currentInput && !_canRecover && notRecovering;

        }
        catch (ArgumentOutOfRangeException)
        {
            _currentInput = false;
            _isBlocking = false;
            _isLastMovePopulated = true;
            return !_isHit && _isGrounded  && !_isBlocking && _isLastMovePopulated && _currentInput && !_canRecover && notRecovering;
        }
    }
    bool ToDashState()
    {
        bool notRecovering = _base._cHitController.ReturnNotRecovering();
        bool _canRecover = _base._cAnimator._canRecover;
        bool isHit;
        bool inputtedDash;
        bool populatedMove;
        isHit = _base._cAnimator.isHit;
        inputtedDash = CheckLastMovementValue();
        populatedMove = _base._cAnimator._lastMovementState == lastMovementState.populated;
        if (_base._subState != Character_SubStates.Controlled)
        {
            return false;
        }
        else 
        {
            return !isHit && inputtedDash && populatedMove && !_canRecover && notRecovering;
        }

    }
    bool ToAttackState()
    {
        bool notRecovering = _base._cHitController.ReturnNotRecovering();
        bool _canRecover = !_base._cAnimator._canRecover;
        bool isHit;
        bool lastAttackValue;
        bool inBlockState = _playerState.current.State == standBlockRef || _playerState.current.State == crouchBlockRef;
        isHit = _base._cAnimator.isHit;
        lastAttackValue =checkAttackValue(lastAttackState.populated);
        if (_base._subState != Character_SubStates.Controlled)
        {
            return false;
        }
        else 
        {
            bool fullcheck = !isHit && lastAttackValue && _canRecover && notRecovering && !inBlockState;
            return fullcheck;
        }
    }
    bool ToHitState()
    {
        bool notRecovering = !_base._cHitController.CheckIfAttackBlocked();
        return _base._cAnimator.isHit && notRecovering;
    }

    #region Individual Boolean Checks
    bool checkMovementValue(lastMovementState desiredState)
    {
        return _base._cAnimator._lastMovementState == desiredState;
    }

    bool checkAttackValue(lastAttackState desiredState)
    {
        if (_base._subState != Character_SubStates.Controlled)
        {
            return true;
        }
        if (desiredState == lastAttackState.nullified) 
        {
            return _base._cAnimator._lastAttackState == desiredState || _base._aManager.Combo.Count == 0;
        }
        else
        {
            return _base._cAnimator._lastAttackState == desiredState;
        }

    }

    bool CheckLastMovementValue() 
    {
        if(_base._cAnimator.activatedInput == null) 
        {
            return false;
        }
        MovementType _movementType = _base._cAnimator.activatedInput.GetMovementType();
        return _movementType == MovementType.ForwardDash ^ _movementType == MovementType.BackDash;
    }

    bool IdleReturnBool() 
    {
        if (_base._subState != Character_SubStates.Controlled) 
        {
            return(true && _base._cHurtBox.IsGrounded() == true && true); 
        }
        if(_base.ReturnMovementInputs().Button_State.directionalInput >= 7 && _base._cHurtBox.IsGrounded() == true) 
        {
            return true && (checkMovementValue(lastMovementState.nullified));//|| _base._aManager.Combo.Count == 0);
        }
        return (_base.ReturnMovementInputs().Button_State.directionalInput == 5 && _base._cHurtBox.IsGrounded() == true && checkMovementValue(lastMovementState.nullified));
    }
    #endregion
    #endregion
}
