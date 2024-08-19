using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Rewired;
using System.Threading.Tasks;
public class Character_Base : MonoBehaviour
{
    #region Character Profile Data
    [Header("__________Character Profile Data__________")]
    public Character_Profile characterProfile;
    [Space(20)]
    #endregion
    #region Script References
    [Header("________CHARACTER SCRIPT REFERENCES_________")]
    public Character_Hitstop _cHitstop;
    public Character_AttackDetection _cADetection;
    public Character_ComboDetection _cComboDetection;
    public Character_Animator _cAnimator;
    public Character_InputDetection _cInput;
    public Character_InputTimer_Attacks _cAttackTimer;
    public Character_InputTimer_Mobility _cMobiltyTimer;
    public ControllerWidgetHandler widget;
    public Character_Force _cForce;
    public Character_StateMachine _cStateMachine;
    public Character_Health _cHealth;
    public Character_DamageCalculator _cDamageCalculator;
    public Character_ComboCounter _cComboCounter;
    public Character_GravityScaler _cGravity;
    public Character_HitStun _cHitstun;
    public Character_HurtboxController _cHurtBox;
    public Character_HitController _cHitController;
    public Character_SuperMeter _cSuperMeter;
    public Attack_Manager _aManager;
    [SerializeField] private Player_SideManager _sideManager;
    [Space(10)]
    public Character_Timer _timer;
    [Space(20)]
    #endregion


    #region Rewired Controls
    [Header("__________REWIRED CONTROLS__________")]
    public int playerID;
    IList<InputAction> _actions;
    Dictionary<int, int> newActions;
    ActionElementMap[] newElements;
    ControllerMap _map;
    public Player player;
    [Space(20)]
    #endregion

    #region General Input Variables
    [Header("________INPUT VARIABLES________")]
    public List<Character_ButtonInput> moveAxes;
    public List<Character_ButtonInput> attackButtons;
    [Space(20)]

    #endregion

    #region Directional Input Detection
    [Header("______DIRECTIONAL INPUT DETECTION________")]
    public Character_MobilityAsset _extraMoveAsset;
    public List<Character_Mobility> _extraMoveControls;
    public HitPointCall activationCall;
    [SerializeField] public float xVal, yVal;
    //[SerializeField, Range(0f, 1f)] public float xYield, yYield;
    public ControllerYield controllerYield;
    [SerializeField] public int numpadValue;
    [SerializeField] public Vector2 numpadVector;
    [Space(20)]
    #endregion

    #region Character Combos
    [Header("_______FACE BUTTON DETECTION_________")]
    //public NewComboList comboList3_0;
    public GameObject comboInstantiatedSpot;
    [SerializeField] private Character_MoveList sourceComboList3_0;
    [SerializeField] private Amplifiers amplifier;
    public Character_MoveList comboList3_0;

    #region Combo Structures (New)
    [SerializeField] private Dictionary<AttackInputTypes, IAttackFunctionality> _characterMoveListAttacks;
    public List<AttackInputTypes> inputVisualiser;
    public Dictionary<AttackInputTypes, IAttackFunctionality> CharacterMoveListAttacks 
    { 
        get 
        { 
            return _characterMoveListAttacks; 
        }
    }
    #endregion


    [Space(20)]
    #endregion

    #region Character Force Variables
    [Header("________FORCE VARIABLES_________")]
    public Rigidbody myRb;
    [SerializeField] private float _jumpForce;
    public float JumpForce { get { return _jumpForce; }}

    [SerializeField] private float _moveForce;
    public float MoveForce { get { return _moveForce; } }
    [SerializeField] private float _dashForce;
    public float DashForce { get { return _dashForce; } }

    [SerializeField] private float _jumpDirForce;
    public float JumpDirForce { get { return _jumpDirForce; }}

    public int movementPC; //movement priority check
    [Space(20)]
    #endregion  
    
    #region Player SubState
    [Header("______PLAYER CONTROLLER SUBSTATES_______")]
    public Character_SubStates _subState;
    [Space(20)]
    #endregion

    #region Side Recognition
    [Header("______SIDE RECOGNITION________")]
    public Player_SideRecognition pSide;
    [Space(20)]
    #endregion

    #region Opponent Reference
    [Header("______OPPONENT CHARACTER________")]
    public Character_Base opponentPlayer;
    [Space(20)]
    #endregion

    private Dictionary<WaitingEnumKey, AwaitCheck> awaitEnums;
    private bool hitWallCheck;
    public bool awaitCondition;

    [SerializeField]
    public int newField;

    #region Initialization Code
    public void Initialize(Character_SubStates setSubState, Amplifiers choseAmplifiers = null, int NewID = -1)
    {
        AddCharacterModel(choseAmplifiers);
        InitButtons(setSubState, NewID);
        _cHitstop.SetCharacterAnimator(playerID, _cAnimator);
        ResetInputLog();
        InitCombos();
        SetAwaitEnums();
        _cComboCounter.SetStartComboCounter();
        _cAnimator.canTransitionIdle = true;
        awaitCondition = true;
    }
    void SetAwaitEnums()
    {
        awaitEnums = new Dictionary<WaitingEnumKey, AwaitCheck>();
        CallbackTest TimerEndDelegate = delegate (bool i)
        {
            return _cAttackTimer.ReturnTimerLessThan(-1 / 60f);
        };

        CallbackTest HitWallDelegate = delegate (bool i) 
        {
            return SetBoolStates(i); 
        };
        AwaitCheck wallCheck = new AwaitCheck(HitWallDelegate);
        awaitEnums.Add(WaitingEnumKey.HitEndWall, wallCheck);

       
        AwaitCheck timerCheck = new AwaitCheck(TimerEndDelegate);
        awaitEnums.Add(WaitingEnumKey.TimerEnd, timerCheck);
    }
    void SetPlayerModelInformation(Character_Animator chosenAnimator,Amplifiers _chosenAmplifier)
    {
        characterProfile.SetCharacterAnimator(chosenAnimator);
        chosenAnimator._base = this;
        _aManager.C_Animator = chosenAnimator;
        if (_chosenAmplifier != null) 
        {
            amplifier = _chosenAmplifier;
        }
        _cHealth.SetHealthInformation(characterProfile);
        _dashForce = characterProfile.dashSpeed;
        _moveForce = characterProfile.MoveVelocity;
        _jumpForce = characterProfile.JumpForce;
        _jumpDirForce = characterProfile.InAirMoveForce;


        _cAnimator = chosenAnimator;
        _cHurtBox.SetCollisionHurtboxStartSize(characterProfile.collisionSizing);
        _cHurtBox.SetTriggerHurtboxStartSize(characterProfile.hurtboxSizing);
        _cComboDetection.SetAnimator(chosenAnimator);
        _cHitstun.SetAnimator(chosenAnimator);
        _cHitController.SetAnimator(chosenAnimator);
        _cAnimator.enabled = true;
        _cAnimator.myAnim.enabled = true;
        _cStateMachine.DefineState();
        _cAttackTimer.ResetTimer();
        _cMobiltyTimer.ResetTimer();
        _cAnimator.ClearLastAttack();
        _cAnimator.NullifyMobilityOption();
        _extraMoveAsset = characterProfile._CharacterMobility;
    }
    void AddCharacterModel(Amplifiers _chosenAmplifier)
    {
        GameObject _chosenCharacter = Instantiate(characterProfile.characterModel, this.gameObject.transform);
        _chosenCharacter.transform.localPosition = new Vector3(0f, -1f, 0f);
        _chosenCharacter.transform.localRotation = Quaternion.identity;
        _chosenCharacter.transform.localScale = Vector3.one;
        _chosenCharacter.SetActive(true);
        Character_Animator _chosneCharacter_Animator = _chosenCharacter.GetComponentInChildren<Character_Animator>();
        pSide.thisPosition.SetModelTransform(_chosenCharacter.transform);
        SetPlayerModelInformation(_chosneCharacter_Animator, _chosenAmplifier);
    }

    void ResetInputLog()
    {
        _timer.inputLogger.ResetAllText();
    }

    void InitCombos()
    {
        _characterMoveListAttacks = new Dictionary<AttackInputTypes, IAttackFunctionality>();
        _extraMoveControls = characterProfile._CharacterMobility.MobilityOptions;
        GetCharacterMoveList();
        inputVisualiser = new List<AttackInputTypes>();
        _cComboDetection.PrimeCombos();

    }
    void GetCharacterMoveList()
    {
        sourceComboList3_0 = characterProfile._characterMoveList;
        Character_MoveList newComboList = Instantiate(sourceComboList3_0, comboInstantiatedSpot.transform);
        comboList3_0 = newComboList;
        comboList3_0.ExtractBaseProperties(this);
    }
    public void CollectCharacterMovelist() 
    {
        comboList3_0.SetupCharacterTotalMoveList(_characterMoveListAttacks, characterProfile.CharacterName, inputVisualiser);
        _cAttackTimer.SetTimerType();
    }
    void InitButtons(Character_SubStates setSubState, int NewID)
    {
        switch (setSubState) 
        {
            case Character_SubStates.Controlled:
                playerID = NewID;
                DesyncVariables();
                HandleButtonInitialization();
                _subState = setSubState;
                break;
            case Character_SubStates.Dummy:
                _subState = setSubState;
                DesyncVariables();
                playerID = -1;
                break;
        }
    }
    void DesyncVariables() 
    {
        if (player != null)
        {
            // player.controllers.RemoveController(ControllerType.Joystick, playerID);
            player.controllers.maps.AddMap(ControllerType.Joystick, playerID, _map);
            //player.controllers.maps.RemoveMap(ControllerType.Joystick, playerID, $"TestPlayer", $"TestPlayer{playerID}");
            player = null;
            newElements = new ActionElementMap[0];
            moveAxes.Clear();
            attackButtons.Clear();
        }
    }
    void HandleButtonInitialization() 
    {
        player = ReInput.players.GetPlayer(playerID);
        player.controllers.AddController(ControllerType.Joystick, playerID,true);
        player.controllers.maps.LoadMap(ControllerType.Joystick, playerID, $"TestPlayer", $"TestPlayer{playerID}");

        _actions = ReInput.mapping.Actions;
        newActions = new Dictionary<int, int>();
        attackButtons = new List<Character_ButtonInput>();
        moveAxes = new List<Character_ButtonInput>();
        try
        {
            //newElements = map.GetElementMaps();
            _map = player.controllers.maps.GetMap(playerID);
            newElements = player.controllers.maps.GetMap(ControllerType.Joystick, playerID, $"TestPlayer", $"TestPlayer{playerID}").GetElementMaps();

            Debug.Log("Controller Map Count: " + player.controllers.maps.GetMaps(ControllerType.Joystick, playerID).Count);
        }
        catch (NullReferenceException) 
        {
            List<ControllerMap> results = new List<ControllerMap>();
            Debug.Log(player.controllers.maps.GetAllMaps(ControllerType.Joystick,results));
            newElements = results[0].GetElementMaps();
        }
        for (int i = 0; i < _actions.Count; i++) 
        {
            newActions.Add(_actions[i].id, i);
        }
        for (int i = 0; i < newElements.Length; i++)
        {
            Character_ButtonInput newButton = new Character_ButtonInput(newElements[i]);
            switch ((newButton.Button_Element.actionDescriptiveName).ToUpper())
            {
                case "VERTICAL":
                    newButton.Button_State.OnDirectional();
                    break;
                case "HORIZONTAL":
                    newButton.Button_State.OnDirectional();
                    break;
                case "FORWARD":
                    newButton.Button_State.OnDirectional();
                    break;
                case "BACKWARD":
                    newButton.Button_State.OnDirectional();
                    break;
                case "UPWARD":
                    newButton.Button_State.OnDirectional();
                    break;
                case "DOWNWARD":
                    newButton.Button_State.OnDirectional();
                    break;
                default:
                    newButton.Button_State.OnReleased();
                    break;
            }
            newButton.SetButton(newElements[i]);
            //buttons.Add(newButton);
            //buttons[i].SetButton(newElements[i]);
            int index = 0;
            if (newActions.TryGetValue(newButton.Button_Element.actionId, out index)) 
            {
                newButton.TryAddButton(_actions[index].name);
                if (newButton.Button_State._state == ButtonStateMachine.InputState.released)
                {
                    attackButtons.Add(newButton);
                }
                else
                {
                    moveAxes.Add(newButton);
                }
            }
        }
    }
    #endregion

    public async void ReceiveCustomCallBack(CustomCallback callback, Callback superIteratorCallback = null) 
    {
        if(callback.customCall == HitPointCall.AwaitSequenceSignifier) 
        {
            await AwaitCustomCall2(callback, superIteratorCallback);
            //StartCoroutine(AwaitCustomCall(callback, superIteratorCallback));
            return;
        }
        CheckCallback(callback);
    }
    void CheckCallback(CustomCallback callback, AwaitClass waitingCheck = null) 
    {
        if (waitingCheck == null)
        {
            if (_cDamageCalculator.customDamageCall.HasFlag(callback.customCall))
            {
                switch (callback.customCall)
                {
                    case HitPointCall.DealCustomDamage:
                        opponentPlayer._cDamageCalculator.TakeDamage(callback.customDamage);
                        break;
                }
            }
            if (_cForce.ForceCall.HasFlag(callback.customCall))
            {
                switch (callback.customCall)
                {
                    case HitPointCall.Force_Right:
                        _cForce.AddLateralForceOnCommand(callback.forceFloat);
                        break;
                    case HitPointCall.Force_Up:
                        _cForce.AddVerticalForceOnCommand(callback.forceFloat);
                        break;
                }
            }
            if (_cAnimator.AttackCall.HasFlag(callback.customCall))
            {
                switch (callback.customCall)
                {
                    case HitPointCall.Force_Right:
                        _cForce.AddLateralForceOnCommand(callback.forceFloat);
                        break;
                    case HitPointCall.Force_Up:
                        _cForce.AddVerticalForceOnCommand(callback.forceFloat);
                        break;
                }
            }
        }
        else 
        {
            if (_cDamageCalculator.customDamageCall.HasFlag(waitingCheck.awaitingCheck))
            {
                switch (waitingCheck.awaitingCheck)
                {
                    case HitPointCall.DealCustomDamage:
                        opponentPlayer._cDamageCalculator.TakeDamage(callback.customDamage);
                        break;
                }
            }
            if (_cForce.ForceCall.HasFlag(waitingCheck.awaitingCheck))
            {
                switch (waitingCheck.awaitingCheck)
                {
                    case HitPointCall.Force_Right:
                        _cForce.AddLateralForceOnCommand(callback.forceFloat);
                        break;
                    case HitPointCall.Force_Up:
                        _cForce.AddVerticalForceOnCommand(callback.forceFloat);
                        break;
                }
            }
            if (_cAnimator.AttackCall.HasFlag(waitingCheck.awaitingCheck))
            {
                switch (waitingCheck.awaitingCheck)
                {
                    case HitPointCall.Force_Right:
                        _cForce.AddLateralForceOnCommand(callback.forceFloat);
                        break;
                    case HitPointCall.Force_Up:
                        _cForce.AddVerticalForceOnCommand(callback.forceFloat);
                        break;
                }
            }
        }
    }
    public void ApplyForceOnCustomCallback(CustomCallback callback, Character_Mobility _mob = null)
    {
        if (activationCall.HasFlag(callback.customCall))
        {
            switch (callback.customCall)
            {
                case HitPointCall.ActivateMobilityAction:
                    _extraMoveAsset.CallMobilityAction(_mob);
                    break;
            }
            switch (callback.customCall)
            {
                case HitPointCall.ClearMobility:
                    _cAnimator.ClearLastActivatedInput();
                    break;
            }
        }
    }
    private void Update()
    {
        _cADetection.CheckButtonPressed();
        _cADetection.CallReturnButton();
        _timer.TimerCountDown();

    }
    bool SetBoolStates(bool check = false) 
    {
        if (!awaitCondition) 
        {
            check = _sideManager.LeftWall.wallIgnore.playerHitEndWall || _sideManager.RightWall.wallIgnore.playerHitEndWall;
            return check;
        }
        return false;
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        _cInput.ReceiveInput();
    }
    public int ReturnInputValue(int value) 
    {
        if (_subState != Character_SubStates.Controlled) 
        { 
            return -1; 
        }

        moveAxes[0].Button_State.directionalInput = value;
        _cComboDetection.CheckPossibleCombos(moveAxes[0]);
        return value; 
    }
    public Character_ButtonInput ReturnMovementInputs()
    {
        if (_subState != Character_SubStates.Controlled) 
        { 
            return null; 
        }
        return moveAxes[0];
    }
    public Character_ButtonInput ReturnBlockButton()
    {
        if (_subState != Character_SubStates.Controlled)
        { 
            return null; 
        }
        return attackButtons[9];
    }
    public Character_ButtonInput ReturnTechButton()
    {
        if (_subState != Character_SubStates.Controlled)
        {
            return null;
        }
        return attackButtons[0];
    }

    public void AwaitCanTransitionIdle(Callback func) 
    {
        StartCoroutine(WaitUntilCanTransitionIdle(func));
    }
    IEnumerator WaitUntilCanTransitionIdle(Callback func)
    {
        while (!_cAnimator.canTransitionIdle) 
        {
            yield return new WaitForSeconds(1f / 60f);
        }
        if (_cAnimator._lastAttackState == Character_Animator.lastAttackState.populated)
        {
            _cAnimator.canTransitionIdle = false;
        }
        else
        {
            func();
        }
    }
    public void AwaitCanTransition_OutAttack(Callback func)
    {
        StartCoroutine(WaitUntilCanTransition(func));
    }
    IEnumerator WaitUntilCanTransition(Callback func)
    {
        while (!_cHurtBox.IsGrounded())
        {
            yield return new WaitForSeconds(1f / 60f);
        }
        IState nextTransition = _cStateMachine._playerState.current.State;

        if (nextTransition == _cStateMachine.idleStateRef)
        {
            _cAnimator.PlayNextAnimation(Animator.StringToHash("Idle"), 0, true);
            func();
        }
        if (nextTransition == _cStateMachine.crouchStateRef)
        {
            _cAnimator.PlayNextAnimation(Animator.StringToHash("Crouch"), 0, true);
            func();
        }
    }
    async Task AwaitCustomCall2(CustomCallback customBoolAwait, Callback superIteratorCallback) 
    {
        if (awaitEnums.ContainsKey(customBoolAwait.awaitEnum.keyRef))
        {
            AwaitCheck stateCheck = awaitEnums[customBoolAwait.awaitEnum.keyRef];
            awaitCondition = false;
            while ((!awaitCondition) && (!stateCheck.testCall(stateCheck.check) && _cAnimator.lastAttack != null))
            {
                CheckCallback(customBoolAwait, customBoolAwait.awaitEnum);
                int singleFrameInInt = (int)(1000 * (1 / 60f));
                await Task.Delay(singleFrameInInt);
            }
            awaitCondition = true;

            if (_cAnimator.lastAttack != null && superIteratorCallback != null)
            {
                superIteratorCallback();
            }
        }
    }
    IEnumerator AwaitCustomCall(CustomCallback customBoolAwait, Callback superIteratorCallback)
    {
        if (awaitEnums.ContainsKey(customBoolAwait.awaitEnum.keyRef))
        {
            AwaitCheck stateCheck = awaitEnums[customBoolAwait.awaitEnum.keyRef];
            awaitCondition = false;
            while (!stateCheck.testCall(stateCheck.check) && _cAnimator.lastAttack != null)
            {
                CheckCallback(customBoolAwait, customBoolAwait.awaitEnum);
                yield return new WaitForSeconds(1 / 60f);
            }
            awaitCondition = true;

            if (_cAnimator.lastAttack != null && superIteratorCallback != null)
            {
                superIteratorCallback();
            }
        }
    }

}
[Serializable]
public class ButtonInput
{
    public InputAction inputAction;
    public ButtonStateMachine buttonState;
    public void SetButton(InputAction action)
    {
        inputAction = action;
        buttonState.OnReleased();
    }
}

[Serializable]
public class ButtonStateMachine
{
    public enum InputState { pressed, held, released, directional };
    public InputState _state;
    public int directionalInput;

    public void OnPressed()
    {
        _state = InputState.pressed;
    }
    public void OnHeld()
    {
        _state = InputState.held;
    }
    public void OnDirectional()
    {
        _state = InputState.directional;
    }
    public void OnReleased()
    {
        _state = InputState.released;
    }
}
[Serializable]
public class ControllerYield
{
    [SerializeField, Range(0f, 1f)] public float positiveXYield, positiveYYield;
    [SerializeField, Range(0f, 1f)] public float negativeXYield, negativeYYield;
}
[Serializable]
public enum WaitingEnumKey
{
    NA,
    HitEndWall,
    TimerEnd,
}
[Serializable]
public class AwaitClass
{
    public WaitingEnumKey keyRef;
    public HitPointCall awaitingCheck;
}
[Serializable]
public class AwaitCheck
{
    public bool check;
    public CallbackTest testCall;

    public AwaitCheck(CallbackTest _testCall) 
    {
        testCall = _testCall;
    }
}
[Serializable]
public delegate bool CallbackTest(bool i = false);