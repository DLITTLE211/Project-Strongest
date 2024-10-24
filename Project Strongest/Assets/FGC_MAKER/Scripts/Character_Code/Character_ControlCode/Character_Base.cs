using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Rewired;
using System.Linq;
using System.Threading.Tasks;
public class Character_Base : MonoBehaviour
{
    public bool allowSecondIdleAnim;
    #region Character Profile Data
    [Header("__________Character Profile Data__________")]
    public Character_Profile characterProfile;
    [Space(20)]
    #endregion

    #region Script References
    [Header("________CHARACTER SCRIPT REFERENCES_________")]
    public Character_Hitstop _cHitstop;
    public Character_AttackDetection _cADetection;
    public Character_AfflictionManager _afflictionManager;
    public Character_AmplifyController _amplifyController;
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
    public Character_SystemButtons _cSystemButtons;
    [SerializeField] private Player_SideManager _sideManager;
    public Character_HitboxManager _cHitboxManager;
    public AttackHandler_FrameDataMeter _aFrameDataMeter;
    [Space(10)]
    public Character_Timer _timer;
    [Space(20)]
    #endregion

    #region Rewired Controls
    [Header("__________REWIRED CONTROLS__________")]
    IList<InputAction> _actions;
    public int playerID;
    Dictionary<int, int> newActions;
    ActionElementMap[] newElements;
    ControllerMap _map;
    public Player player;
    public int _side;
    bool activated;
    public bool ACTIVATED { get { return activated; } }
    [Space(20)]
    #endregion

    #region General Input Variables
    [Header("________INPUT VARIABLES________")]
    public List<Character_ButtonInput> moveAxes;
    public List<Character_ButtonInput> attackButtons;
    public Character_ButtonInput blockButton;
    [Space(20)]

    #endregion

    #region Directional Input Detection
    [Header("______DIRECTIONAL INPUT DETECTION________")]
    public Character_MobilityOptions _mobilitySource;
    public Character_MobilityOptions character_MobilityOptions;
    public HitPointCall activationCall;
    [SerializeField] public float xVal, yVal;
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
    public Dictionary<AttackInputTypes, IAttackFunctionality> ChosenCharacterMoveList { get { return _characterMoveListAttacks; } }
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

    private float storedXVelocity, storedYVelocity;
    internal bool isLockedPause;

    private Dictionary<WaitingEnumKey, AwaitCheck> awaitEnums;
    private Dictionary<HitPointCall, Callback<CustomCallback>> mainCallbackDictionary = new Dictionary<HitPointCall, Callback<CustomCallback>>();
    public bool callSource;
    public bool awaitCondition;


    #region Initialization Code
    public void Initialize(Character_SubStates setSubState, int hitboxSideDetection, Amplifiers choseAmplifiers = null, int NewID = -1)
    {
        _aFrameDataMeter.gameObject.SetActive(false);
           activated = false;
        _side = hitboxSideDetection;
        AddCharacterModel(choseAmplifiers);
        InitButtons(setSubState, NewID);
        _cHitboxManager.SetupHitboxes(hitboxSideDetection + 1);
        //_cHitstop.SetCharacterAnimator(playerID, _cAnimator);
        ResetInputLog();
        InitCombos();
        SetAwaitEnums();

        _amplifyController.SetChosenAmplifier(choseAmplifiers);
        _cComboCounter.SetStartComboCounter();
        _cAnimator.canTransitionIdle = true;
        awaitCondition = true;
        if (GameManager.instance._gameModeSet.gameMode == GameMode.Training)
        {
            Activate();
        }
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

        _cMobiltyTimer.SetStartingValues();
        _cAnimator = chosenAnimator;
        _cHurtBox.SetCollisionHurtboxStartSize(characterProfile.collisionSizing);
        _cHurtBox.SetTriggerHurtboxStartSize(characterProfile.hurtboxSizing);
        _cComboDetection.SetAnimator(chosenAnimator);
        _cHitstun.SetAnimator(chosenAnimator);
        _cAnimator.enabled = true;
        _cDamageCalculator.SetAllowDeathCheck();
        _cHurtBox.SetMarkers(_cAnimator.ReturnMarkers());
        _cHitController.SetHitReactions(chosenAnimator);
        _cAnimator.myAnim.enabled = true;
        _cStateMachine.DefineState();
        _cAttackTimer.ResetTimer();
        _cMobiltyTimer.ResetTimer();
        _cAnimator.ClearLastAttack();
        _cAnimator.NullifyMobilityOption();
        SetMainCustomCallbackDictionary();
    }
    void AddCharacterModel(Amplifiers _chosenAmplifier)
    {
        GameObject _chosenCharacter = Instantiate(characterProfile.characterModel, this.gameObject.transform);
        _chosenCharacter.transform.localPosition = new Vector3(0f, -1f, 0f);
        _chosenCharacter.transform.localRotation = Quaternion.identity;
        _chosenCharacter.transform.localScale = Vector3.one;
        _chosenCharacter.SetActive(true);
        Character_Animator _chosenCharacter_Animator = _chosenCharacter.GetComponentInChildren<Character_Animator>();
        pSide.thisPosition.SetModelTransform(_chosenCharacter.transform);
        SetPlayerModelInformation(_chosenCharacter_Animator, _chosenAmplifier);
    }

    void ResetInputLog()
    {
        _timer.inputLogger.ResetAllText();
    }

    void InitCombos()
    {
        _characterMoveListAttacks = new Dictionary<AttackInputTypes, IAttackFunctionality>();
        _mobilitySource = characterProfile._NewCharacterMobility;
        Character_MobilityOptions newMobilityOptions = Instantiate(_mobilitySource, comboInstantiatedSpot.transform);
        character_MobilityOptions = newMobilityOptions;
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
        comboList3_0.SetupCharacterTotalMoveList(_characterMoveListAttacks, characterProfile.CharacterName, inputVisualiser,this);
        _cAttackTimer.SetTimerType();
    }
    void InitButtons(Character_SubStates setSubState, int NewID)
    {
        switch (setSubState) 
        {
            case Character_SubStates.Controlled:
                playerID = NewID;
                _aFrameDataMeter.gameObject.SetActive(true);
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
                    if (newButton.Button_Element.actionDescriptiveName.ToLower().Contains("button")) 
                    {
                        _cSystemButtons.AddSystemButton(newButton);
                        break;
                    }
                    break;
            }
            newButton.SetButton(newElements[i]);
            int index = 0;
            if (newActions.TryGetValue(newButton.Button_Element.actionId, out index)) 
            {
                newButton.TryAddButton(_actions[index].name);
                if (newButton.Button_State._state == ButtonStateMachine.InputState.released)
                {
                    if (!_cSystemButtons._systemButtons.Contains(newButton))
                    {
                        if (newButton.Button_Name == "G")
                        {
                            blockButton = newButton;
                        }
                        else
                        {
                            attackButtons.Add(newButton);
                        }
                    }
                }
                else
                {
                    moveAxes.Add(newButton);
                }
            }
        }
    }

    public void SetMainCustomCallbackDictionary() 
    {
        mainCallbackDictionary.Add(HitPointCall.ShootProjectile,null);

        mainCallbackDictionary.Add(HitPointCall.Force_Right, _cForce.AddLateralForceOnCommand);
        mainCallbackDictionary.Add(HitPointCall.Force_Up, _cForce.AddVerticalForceOnCommand);
        mainCallbackDictionary.Add(HitPointCall.TeleportForward, _cForce.TeleportOnCommand);
        mainCallbackDictionary.Add(HitPointCall.TeleportBackward, _cForce.TeleportOnCommand);
        mainCallbackDictionary.Add(HitPointCall.LockPos, LockPos);
        mainCallbackDictionary.Add(HitPointCall.UnlockPos, UnlockPos);

        //mainCallbackDictionary.Add(HitPointCall.KillStance, null);

        mainCallbackDictionary.Add(HitPointCall.ToggleArmor, null);
        mainCallbackDictionary.Add(HitPointCall.ToggleInvincible, null);
        mainCallbackDictionary.Add(HitPointCall.ToggleAntiAir, null);

        //mainCallbackDictionary.Add(HitPointCall.ActivateMobilityAction, null);
        //mainCallbackDictionary.Add(HitPointCall.ClearMobility, null);

        mainCallbackDictionary.Add(HitPointCall.UnFreeze, _cAnimator.SetSelfUnfreeze);
        mainCallbackDictionary.Add(HitPointCall.ToggleFreeze_Self, _cAnimator.SetSelfFreeze);
        mainCallbackDictionary.Add(HitPointCall.ToggleFreeze_Other, _cAnimator.SetOpponentFreeze);
        mainCallbackDictionary.Add(HitPointCall.ToggleFreeze_Both, _cAnimator.FreezeBoth);

        mainCallbackDictionary.Add(HitPointCall.PanPosOnTarget, null);
        mainCallbackDictionary.Add(HitPointCall.PanRotateOnTarget, null);
        mainCallbackDictionary.Add(HitPointCall.PanZoomOnTarget, null);

        mainCallbackDictionary.Add(HitPointCall.DealCustomDamage, opponentPlayer._cDamageCalculator.TakeCustomDamage);
        mainCallbackDictionary.Add(HitPointCall.ForceSideSwitch, _sideManager.ForceSideSwitch);
    }
    #endregion

    public async void InitialReset()
    {
        await ResetPlayerOnTeleport();
        _cDamageCalculator.SetVictoryHitState(false);
        _cSuperMeter.SetStartValue();
        _cHealth.SetStartingHealthValues();
        _cHealth.stunController.SetStartStunValues();
        Deactivate();
        _cAttackTimer.ClearAttackLanded();
        if (!awaitCondition) 
        {
            awaitCondition = true;
        }
        _cAnimator.PlayNextAnimation(Animator.StringToHash("Idle"), 0);
    }

    public async void ReceiveCustomCallBack(CustomCallback callback, Callback superIteratorCallback = null) 
    {
        if(callback.customCall == HitPointCall.AwaitSequenceSignifier) 
        {
            await AwaitCustomCall(callback, superIteratorCallback);
            return;
        }
        CheckCallback(callback);
    }
    void CheckCallback(CustomCallback callback, AwaitClass waitingCheck = null) 
    {
        if (waitingCheck == null)
        {
            Callback<CustomCallback> dictionaryFunc = null;
            if (mainCallbackDictionary.TryGetValue(callback.customCall , out dictionaryFunc)) 
            {
                callSource = true;
                dictionaryFunc(callback);
                callSource = false;
                return;
            }
        }
        else 
        {
            Callback<CustomCallback> dictionaryFunc = null;
            if (mainCallbackDictionary.TryGetValue(waitingCheck.awaitingCheck, out dictionaryFunc))
            {
                callSource = true;
                dictionaryFunc(callback);
                callSource = false;
                return;
            }
        }
    }
    public void ApplyForceOnCustomCallback(CustomCallback callback, Character_MobilityOption _mob = null)
    {
        if (activationCall.HasFlag(callback.customCall))
        {
            if (callback.customCall == HitPointCall.ActivateMobilityAction)
            {
                character_MobilityOptions.CallMobilityAction(_mob);
            }
            else if (callback.customCall == HitPointCall.ClearMobility) 
            {
                _cAnimator.ClearLastActivatedInput();
            }
        }
    }
    public void Activate() 
    {
        activated = true;
    }
    public void Deactivate()
    {
        activated = false;
    }
    private void Update()
    {
        if (!ReturnIfPaused() && activated)
        {
            _cADetection.CheckButtonPressed(attackButtons);
            _cADetection.CallReturnButton();
            _timer.TimerCountDown();
        }

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
        if (!ReturnIfPaused() && activated)
        {
            _cInput.ReceiveInput();
        }
    }
    public int ReturnInputValue(int value)
    {
        if (ReturnIfPaused())
        {
            return -1;
        }
        if (_subState != Character_SubStates.Controlled)
        {
            return -1;
        }
        if (!activated)
        {
            return 5;
        }

        moveAxes[0].Button_State.directionalInput = value;
        _cComboDetection.CheckPossibleCombos(moveAxes[0]);
        return value;
    }
    public bool ReturnIfPaused() 
    {
        return isLockedPause;
    }
    public Character_ButtonInput ReturnMovementInputs()
    {
        if (_subState != Character_SubStates.Controlled) 
        { 
            return null;
        }
        if (ReturnIfPaused())
        {
            moveAxes[0].Button_State.directionalInput = 5;
        }
        if (!activated)
        {
            moveAxes[0].Button_State.directionalInput = 5;
        }
        return moveAxes[0];
    }
    public Character_ButtonInput ReturnBlockButton()
    {
        if (ReturnIfPaused())
        {
            return null;
        }
        if (_subState != Character_SubStates.Controlled)
        { 
            return null; 
        }
        if (!activated)
        {
            return null;
        }
        return blockButton;
    }
    public Character_ButtonInput ReturnTechButton()
    {
        if (ReturnIfPaused())
        {
            return null;
        }
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
        if (_cAnimator._lastAttackState == lastAttackState.populated)
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
    public void TriggerSecondaryIdleAnim() 
    {
        allowSecondIdleAnim = true;
    }
    public void CallWaitAnimFinish(float time)
    {
        StartCoroutine(AwaitAnimFinish(time));
    }
    IEnumerator AwaitAnimFinish(float time) 
    {
        yield return new WaitForSeconds(time);
        allowSecondIdleAnim = false;
    }
    async Task AwaitCustomCall(CustomCallback customBoolAwait, Callback superIteratorCallback) 
    {
        if (awaitEnums.ContainsKey(customBoolAwait.awaitEnum.keyRef))
        {
            AwaitCheck stateCheck = awaitEnums[customBoolAwait.awaitEnum.keyRef];
            awaitCondition = false;
            while (!isLockedPause && (!awaitCondition) && (!stateCheck.testCall(stateCheck.check) && _cAnimator.lastAttack != null))
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
    public async Task ResetPlayerOnTeleport() 
    {
        awaitCondition = true;
        _cComboCounter.OnEndCombo();
        _cComboDetection.ResetCombos();
        _aManager.ResetMoveHierarchy();
        _cAttackTimer.SetTimerType();
        _aManager.ClearAttacks();
        _cAnimator.CountUpNegativeFrames(0);
        _cAnimator.lastAttack = null;
        _cAnimator.canTransitionIdle = true;
        _cAnimator._lastAttackState = lastAttackState.nullified;
        await Task.Delay(50);
    }
    public void LockPos(CustomCallback callback = null)
    {
        storedXVelocity = myRb.velocity.x;
        storedYVelocity = myRb.velocity.y;
        myRb.drag = 100000;
        myRb.velocity = new Vector3(0f, 0f, 0f);
        myRb.useGravity = false;
        myRb.isKinematic = true;
        myRb.constraints = RigidbodyConstraints.FreezeAll;
    }
    public void UnlockPos(CustomCallback callback = null)
    {
        myRb.isKinematic = false;
        myRb.drag = 1;
        myRb.constraints = (RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ);
        myRb.velocity = new Vector3(storedXVelocity, storedYVelocity, 0f);
        storedXVelocity = 0f;
        storedYVelocity = 0f;
        myRb.useGravity = true;
    }
    public void LockPlayerInPause()
    {
        LockPos();
        isLockedPause = true;
        myRb.constraints = RigidbodyConstraints.FreezeAll;
        _cHitstun.HandleAnimatorFreeze(true, 0f);
        _cHitboxManager.DisableCurrentHitbox();
    }
    public float factor;
    public void UnlockPlayerInPause()
    {
        UnlockPos();
        isLockedPause = false;
        _cHitstun.HandleAnimatorFreeze(false);
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