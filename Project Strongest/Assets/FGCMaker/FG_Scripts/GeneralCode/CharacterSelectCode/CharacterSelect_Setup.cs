using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;
using System;
using System.Linq;
using DG.Tweening;
using UnityEngine.UI;
using System.Threading.Tasks;
using TMPro;

public class CharacterSelect_Setup : MonoBehaviour
{
    [Header("____CharacterSelect Assets____")]
    [SerializeField] private GameObject SideSelectionObject;
    [SerializeField] private GameObject mainObjectHolder;
    [SerializeField] private GameObject CharacterSelectObject;
    [SerializeField] private GameObject characterSelectButtonPrefab;
    [SerializeField] private GameObject characterSelectHolder;
    [SerializeField] private GameObject characterSelect_Header;
    [SerializeField] private List<GameObject> characterSelect_Assets;
    [SerializeField] private Image characterSelectBackgroundImage;
    [Space(15)]

    [Header("____Character Side Information____")]
    [SerializeField] private TMP_Text advidoryMessage;
    [SerializeField] private CharacterSelect_ChosenSideController sideController;
    [Space(15)]

    [Header("____Character Cursor Information____")]
    [SerializeField] private List<Character_Profile> _activeProfiles;
    [SerializeField] private List<Amplifiers> _activeAmplifiers;
    [SerializeField] private List<GameObject> activeCharacterSelectButtons;

    [SerializeField] private CharacterSelect_Page _player1_PlayerPage, _player2_PlayerPage;
    [SerializeField] private CharacterSelect_Cursor _player1_Cursor, _player2_Cursor;
    [SerializeField] private ChooseSide_Object player1;
    [SerializeField] private ChooseSide_Object player2;
    [Space(15)]


    [Header("____Stage Select Information____")]
    [SerializeField] private CharacterSelect_StageSelect _stageSelecter;
    [SerializeField] private List<Stage_StageAsset> _activeStages;
    [SerializeField] private Stage_StageAsset _chosenStage;
    [SerializeField] private CharacterSelect_LoadArena _arenaLoader;

    [Header("____Rewired Players____")]
    public Character_AvailableID players;
    public Transform upBound,downBound,leftBound,rightBound;
    GameModeSet currentSet;
    [SerializeField] private bool stageSelectCooldown;
    Sequence DisplayMessageSequence;
    // Start is called before the first frame update
    void Start()
    {
        SetupPlayerPage(_player1_PlayerPage);
        SetupPlayerPage(_player2_PlayerPage);
        player1.sideIterator = 1;
        player2.sideIterator = 1;
    }
    private void Update()
    {
        CursorController(_player1_Cursor);
        CursorController(_player2_Cursor);
    }
    public void SetListeners() 
    {
        Messenger.AddListener<Character_Profile, CharacterSelect_Cursor>(Events.DisplayCharacterInfo, DisplayCharacterSelectInformation);
        Messenger.AddListener<int>(Events.ClearCharacterInfo, ClearCharacterSelectInformation);
        Messenger.AddListener<Character_Profile, CharacterSelect_Cursor>(Events.LockinCharacterChoice, LockinCharacterChoice);
        ReInput.ControllerConnectedEvent += AddControllerCounter;
        ReInput.ControllerDisconnectedEvent += SubtractControllerCounter;
    }
    void SetupPlayerPage(CharacterSelect_Page playerPage) 
    {
        playerPage.characterAmplify.GetListOfAmplifiers(_activeAmplifiers);
        playerPage.SetPlayerInfo(255f);

    }
    public void SubtractControllerCounter(ControllerStatusChangedEventArgs args = null)
    {
        players.SubtractFromJoystickNames(ReInput.controllers.GetJoystickNames());
        CheckPlayerCount(args);
    }
    public void AddControllerCounter(ControllerStatusChangedEventArgs args = null)
    {
        List<string> controllerNames = new List<string>();
        controllerNames = ReInput.controllers.GetJoystickNames().ToList();
        for (int i = 0; i < controllerNames.Count; i++) 
        {
            if (players.UsedID.Item2.Contains(controllerNames[i])) 
            {
                continue;
            }
            players.AddUsedID(controllerNames[i]);
        }
        CheckPlayerCount(args);
    }
    public void CheckPlayerCount(ControllerStatusChangedEventArgs args = null) 
    {
        if (players.UsedID.Item1.Count == 0)
        {
            advidoryMessage.text = "Please Plug in a controller to continue";
            advidoryMessage.gameObject.SetActive(true);
            player1.SetImageCPU();
            player2.SetImageCPU();
        }
        advidoryMessage.gameObject.SetActive(false);
        if (players.UsedID.Item1.Count == 1)
        {
            players.SubtractFromJoystickNames(ReInput.controllers.GetJoystickNames());
            player1.SetImageP1();
            player2.SetImageCPU();
        }
        else if (players.UsedID.Item1.Count == 2)
        {
            player1.SetImageP1();
            player2.SetImageP2();
        }
    }
    async void SetPlayerInformation_OnCharacterSelect()
    {
        CharacterSelectObject.SetActive(true);
        Task[] tasks = new Task[]
        {
            ToggleStageSelectState(true),
            ToggleCharacterSelectInfo(true,255f),
            TogglePlayerInfo(255f),
        };
        await Task.WhenAll(tasks);
        for (int i = 0; i < characterSelect_Assets.Count; i++)
        {
            if (i == 1)
            {
                characterSelect_Assets[i].SetActive(false);
                continue;
            }
            characterSelect_Assets[i].SetActive(true);
        }
        if (player1.sideIterator == 1) 
        {
            Debug.Log("Dummy Player_Null 1");
        }
        else
        {
            _player1_Cursor.gameObject.SetActive(true);
            if (player1.sideIterator == 0)
            {
                _player1_Cursor.ChosenPlayerSide = 0;
                _player1_Cursor.cursorPage = _player1_PlayerPage;
                _player1_Cursor.cursorText.text = $"{_player1_Cursor.ID + 1}_L";
            }
            if (player1.sideIterator == 2)
            {
                _player1_Cursor.ChosenPlayerSide = 1;
                _player1_Cursor.cursorPage = _player2_PlayerPage;
                _player1_Cursor.cursorText.text = $"{_player1_Cursor.ID + 1}_R";
            }
        }
        if (player2.sideIterator == 1)
        {
            if (player1.sideIterator == 0)
            {
                _player2_Cursor.cursorPage = _player2_PlayerPage;
            }
            if (player1.sideIterator == 2)
            {
                _player2_Cursor.cursorPage = _player1_PlayerPage;
            }
        }
        else
        {
            _player2_Cursor.gameObject.SetActive(true);
            if (player2.sideIterator == 0)
            {
                _player2_Cursor.ChosenPlayerSide = 0;
                _player2_Cursor.cursorPage = _player1_PlayerPage;
                _player2_Cursor.cursorText.text = $"{_player1_Cursor.ID + 1}_L";

            }
            if (player2.sideIterator == 2)
            {
                _player2_Cursor.ChosenPlayerSide = 1;
                _player2_Cursor.cursorPage = _player2_PlayerPage;
                _player2_Cursor.cursorText.text = $"{_player1_Cursor.ID + 1}_R";
            }
        }

    }
    public async void SetUpCharacterSelectScreen(Character_AvailableID _characterSelectplayers, GameModeSet set)
    {
        SetListeners();
        SideSelectionObject.SetActive(false);
        mainObjectHolder.SetActive(true);
        CharacterSelectObject.SetActive(false);
        players = _characterSelectplayers;
        currentSet = set;
        _stageSelecter.ResetValues();
        if (set.gameMode == GameMode.Versus)
        {
            _player1_Cursor.UnlockCharacterChoice();
            _player1_PlayerPage.ClearInfo();
            _player2_Cursor.UnlockCharacterChoice();
            player1.InitSideIterator();
            player2.InitSideIterator();
            advidoryMessage.gameObject.SetActive(false);
            SideSelectionObject.SetActive(true);
            AddControllerCounter();
            CheckPlayerCount();
            if (players.UsedID.Item1.Count == 0)
            {
                advidoryMessage.gameObject.SetActive(true);
                player1.SetImageCPU();
                player2.SetImageCPU();
            }
            if (players.UsedID.Item1.Count == 1)
            {
                player1.SetImageP1();
                player2.SetImageCPU();
            }
            else
            {
                player1.SetImageP1();
                player2.SetImageP2();
            }
        }
        if (set.gameMode == GameMode.Training)
        {
            CharacterSelectObject.SetActive(true);
            Task[] tasks = new Task[]
            {
            ToggleStageSelectState(true),
            ToggleCharacterSelectInfo(true,255f),
            TogglePlayerInfo(255f),
            };
            await Task.WhenAll(tasks);
            for (int i = 0; i < characterSelect_Assets.Count; i++)
            {
                if (i == 1)
                {
                    characterSelect_Assets[i].SetActive(false);
                    continue;
                }
                characterSelect_Assets[i].SetActive(true);
            }
        }
        if (activeCharacterSelectButtons != null)
        {
            if (activeCharacterSelectButtons.Count > 0)
            {
                for (int i = 0; i < activeCharacterSelectButtons.Count; i++)
                {
                    Destroy(activeCharacterSelectButtons[i]);
                }
                activeCharacterSelectButtons = new List<GameObject>();
            }
        }
        AddCharacterSelectButtons();
    }
    public async void ActivateMenuAssets() 
    {
        Task[] tasks = new Task[]
        {
            ToggleStageSelectState(true),
            ToggleCharacterSelectInfo(true,255f),
            TogglePlayerInfo(255f),
        };
        await Task.WhenAll(tasks);
    }
    public void AddCharacterSelectButtons()
    {
        for (int i = 0; i < _activeProfiles.Count; i++)
        {
            GameObject selectButton = Instantiate(characterSelectButtonPrefab, characterSelectHolder.transform);
            selectButton.gameObject.transform.localPosition = new Vector3(1, 1, 1);
            selectButton.gameObject.transform.localRotation = Quaternion.identity;
            selectButton.gameObject.transform.localScale = Vector3.one;
            selectButton.GetComponentInChildren<Button>().image.sprite = _activeProfiles[i].CharacterProfileImage;
            selectButton.GetComponentInChildren<Button>().interactable = true;
            GameObject _selectButtonInfo = selectButton;
            _selectButtonInfo.GetComponent<CharacterSelect_Button>().characterProfile = _activeProfiles[i];
            _selectButtonInfo.GetComponent<CharacterSelect_Button>().GetLeftCursor(_player1_Cursor.cursorObject.transform);
            _selectButtonInfo.GetComponent<CharacterSelect_Button>().GetRightCursor(_player2_Cursor.cursorObject.transform);
            activeCharacterSelectButtons.Add(_selectButtonInfo);
        }
        StartCoroutine(CascadeScaleSelectButtons());
    }
    IEnumerator CascadeScaleSelectButtons()
    {
        for (int i = 0; i < activeCharacterSelectButtons.Count; i++)
        {
            yield return new WaitForSeconds(0.05f);
            Vector3 selectButtonFirstSize = new Vector3(1.15f, 1.15f, 1.15f);
            activeCharacterSelectButtons[i].transform.DOScale(selectButtonFirstSize, 0.15f);
            yield return new WaitForSeconds(0.025f);
            activeCharacterSelectButtons[i].transform.DOScale(Vector3.one, 0.15f);
            activeCharacterSelectButtons[i].GetComponent<CharacterSelect_Button>().SetPosition();
        }
        SetPlayerControllers();
    }

    void SetPlayerControllers()
    {
        _stageSelecter.ClearStageSelect();
        if (ReInput.controllers.GetJoystickNames().Length <= 0)
        {
            return;
        }
        else
        {
            for (int i = 0; i < players.UsedID.Item1.Count; i++)
            {
                if (i == 0)
                {
                    if (_player1_Cursor.curPlayer == null)
                    {
                        SetCharacterSelectCursorState(_player1_Cursor, 0);
                    }
                    else
                    {
                        _player1_Cursor.UnlockCharacterChoice();
                        _player1_Cursor.isConnected = true;
                        _player1_PlayerPage.ClearInfo();
                    }
                }
                else if (i == 1) 
                {
                    if (_player2_Cursor.curPlayer == null)
                    {
                        SetCharacterSelectCursorState(_player2_Cursor, i);
                    }
                    else
                    {
                        _player2_Cursor.UnlockCharacterChoice();
                        _player2_Cursor.isConnected = true;
                        _player2_PlayerPage.ClearInfo();
                    }
                }
            }
        }
    }
    void SetCharacterSelectCursorState(CharacterSelect_Cursor player, int ID) 
    {
        player.curPlayer = ReInput.players.GetPlayer(players.UsedID.Item1[ID]);
        player.ID = ID;
        player.curPlayer.controllers.AddController(ControllerType.Joystick, players.UsedID.Item1[ID], true);
        player.curPlayer.controllers.maps.LoadMap(ControllerType.Joystick, players.UsedID.Item1[ID], $"UI_CanvasController", $"TestPlayer{players.UsedID.Item1[ID]}");
        if (currentSet.gameMode == GameMode.Training)
        {
            player.cursorObject.SetActive(true);
            player.cursorText.text = $"{players.UsedID.Item1[ID] + 1}";

        }
        player.isConnected = true;
    }
    public void DisplayCharacterSelectInformation(Character_Profile hoveredProfile, CharacterSelect_Cursor cursorHighlight)
    {
        cursorHighlight.cursorPage.UpdateInfo(hoveredProfile);
    }
    public void ClearCharacterSelectInformation(int curHighlightedPlayerID)
    {
        if (curHighlightedPlayerID == 0)
        {
            _player1_PlayerPage.ClearInfo();
        }
        if (curHighlightedPlayerID == 1)
        {
            _player2_PlayerPage.ClearInfo();
        }
    }
    #region Deactivate Character Select
    public async Task ToggleCharacterSelectInfo(bool state, float fadeValue) 
    {
        for (int i = 0; i < activeCharacterSelectButtons.Count; i++)
        {
            activeCharacterSelectButtons[i].GetComponentInChildren<Button>().interactable = state;
            activeCharacterSelectButtons[i].GetComponentInChildren<Button>().image.DOFade(fadeValue, 1.5f);
            activeCharacterSelectButtons[i].SetActive(state);
        }
        characterSelectBackgroundImage.gameObject.SetActive(state);
        characterSelect_Header.SetActive(state);
        characterSelectHolder.SetActive(state);
        await Task.Delay(400);
    }
    public async Task DisableCharacterCursors() 
    {
        _player1_Cursor.DesyncController();
        _player2_Cursor.DesyncController();
        await Task.Delay(400);
    }
    public async Task TogglePlayerInfo(float value) 
    {
        _player1_PlayerPage.SetPlayerInfo(value);
        _player2_PlayerPage.SetPlayerInfo(value);
        if (value == 0) 
        {
            for (int i = 0; i < characterSelect_Assets.Count; i++)
            {
                characterSelect_Assets[i].SetActive(false);
            }
            await Task.Delay(200);
            mainObjectHolder.SetActive(false);
            await Task.Delay(200);
            return;
        }
        await Task.Delay(400);
    }
    public async Task ToggleStageSelectState(bool state) 
    {
        _stageSelecter.SetStageSelect(state);
        await Task.Delay(400);
    }
    #endregion

    void CheckIfBothPlayersLockedIn(CharacterSelect_Cursor cursor)
    {

        if (cursor == _player1_Cursor)
        {
            if (cursor.cursorPage.lockedIn == true)
            {
                if (_player2_Cursor.cursorObject.activeInHierarchy && !_player2_Cursor.cursorPage.lockedIn && !_player2_Cursor.canChooseStage)
                {
                    _player1_Cursor.canChooseStage = true;
                }
                else if (_player2_Cursor.cursorObject.activeInHierarchy && _player2_Cursor.cursorPage.lockedIn && !_player2_Cursor.canChooseStage)
                {
                    _player2_Cursor.canChooseStage = true;
                    ActivateStageSelector();
                }
                else if (_player2_Cursor.cursorObject.activeInHierarchy && _player2_Cursor.cursorPage.lockedIn && _player2_Cursor.canChooseStage)
                {
                    ActivateStageSelector();
                }
                else
                {
                    _player1_Cursor.canChooseStage = true;
                    ActivateStageSelector();
                }
            }
        }
        else if (cursor == _player2_Cursor) 
        {
            if (cursor.cursorPage.lockedIn == true)
            {
                if (_player1_Cursor.cursorObject.activeInHierarchy && !_player1_Cursor.cursorPage.lockedIn && !_player1_Cursor.canChooseStage)
                {
                    _player2_Cursor.canChooseStage = true;
                }
                else if (_player1_Cursor.cursorObject.activeInHierarchy && _player1_Cursor.cursorPage.lockedIn && !_player1_Cursor.canChooseStage)
                {
                    _player1_Cursor.canChooseStage = true;
                    ActivateStageSelector();
                }
                else if (_player1_Cursor.cursorObject.activeInHierarchy && _player1_Cursor.cursorPage.lockedIn && _player1_Cursor.canChooseStage)
                {
                    ActivateStageSelector();
                }
                else
                {
                    _player2_Cursor.canChooseStage = true;
                    ActivateStageSelector();
                }
            }
        }
    }
    public void ClearListeners() 
    {
        ReInput.ControllerConnectedEvent -= AddControllerCounter;
        ReInput.ControllerDisconnectedEvent -= SubtractControllerCounter;
        Messenger.RemoveListener<Character_Profile, CharacterSelect_Cursor>(Events.DisplayCharacterInfo, DisplayCharacterSelectInformation);
        Messenger.RemoveListener<int>(Events.ClearCharacterInfo, ClearCharacterSelectInformation);
        Messenger.RemoveListener<Character_Profile, CharacterSelect_Cursor>(Events.LockinCharacterChoice, LockinCharacterChoice);
    }
    #region CursorController
    void CursorController(CharacterSelect_Cursor currentController) 
    {
        if (currentController.isConnected)
        {
            if (currentController.curPlayer.GetButtonDown(17))
            {
                if (SideSelectionObject.activeInHierarchy)
                {
                    sideController.CloseChooseSideMenu(SetPlayerInformation_OnCharacterSelect);
                }
                else
                {
                    if (currentController.profile == null)
                    {
                        Messenger.Broadcast<CharacterSelect_Cursor>(Events.TryApplyCharacter, currentController);
                    }
                    else
                    {
                        if (_stageSelecter.allowRoundSelect)
                        {
                            _stageSelecter.ActivateStageSelectObject();
                        }
                        else
                        {
                            if (currentController.canChooseStage && _stageSelecter.allowStageSelect)
                            {
                                _chosenStage = _stageSelecter._stageAsset;
                                _arenaLoader.OnCharactersAndStageSelected();
                            }
                        }
                    }
                }
            }
            if (currentController.curPlayer.GetButton(18))
            {
                if (currentController.profile != null)
                {
                    currentController.UnlockCharacterChoice();
                }
                else
                {
                    if (_stageSelecter.MainHolder.activeInHierarchy)
                    {
                        _stageSelecter.ClearStageSelect();
                        currentController.UnlockCharacterChoice();
                    }
                }
            }
            if (currentController.curPlayer.GetButtonDown("Shift_Right"))
            {
                if (!_stageSelecter.allowRoundSelect && !_stageSelecter.allowStageSelect)
                {
                    if (!SideSelectionObject.activeInHierarchy)
                    {
                        StartCoroutine(currentController.cursorPage.DelayResetBool());
                        currentController.cursorPage.characterAmplify.UpdateInfoUp();
                    }
                }
            }
            if (currentController.curPlayer.GetButtonDown("Shift_Left"))
            {
                if (!_stageSelecter.allowRoundSelect && !_stageSelecter.allowStageSelect)
                {
                    if (!SideSelectionObject.activeInHierarchy)
                    {
                        StartCoroutine(currentController.cursorPage.DelayResetBool());
                        currentController.cursorPage.characterAmplify.UpdateInfoDown();
                    }
                }
            }
            if (currentController.profile == null)
            {
                currentController.xVal = currentController.curPlayer.GetAxisRaw("Horizontal");
                currentController.yVal = currentController.curPlayer.GetAxisRaw("Vertical");
                currentController.xVal = (currentController.xVal >= currentController.xYield) ? 1 : ((currentController.xVal <= -currentController.xYield) ? -1 : 0);
                currentController.yVal = (currentController.yVal >= currentController.yYield) ? 1 : ((currentController.yVal <= -currentController.yYield) ? -1 : 0);


                if (currentController.xVal == 0 && currentController.yVal == 0)
                {
                    currentController.cursorObject.GetComponent<Rigidbody2D>().drag = 10000f;
                }
                else
                {
                    if (SideSelectionObject.activeInHierarchy)
                    {
                        ChooseSide_Object curObject = currentController.ID == 0 ? player1 : player2;
                        sideController.UpdateControllerSide(curObject, (int)currentController.xVal, DisplayObjectlapMessage); 
                    }
                    else
                    {
                        float xVal = currentController.xVal * 3;
                        float yVal = currentController.yVal * 3;
                        currentController.cursorObject.GetComponent<Rigidbody2D>().drag = 0;
                        if (HitHeightBound(currentController.cursorObject.transform))
                        {
                            xVal = 0;
                        }
                        if (HitWidthBound(currentController.cursorObject.transform))
                        {
                            yVal = 0;
                        }
                        currentController.cursorObject.transform.Translate(new Vector3(xVal, yVal, 0));
                    }
                }
            }
            else
            {
                if (currentController.cursorPage.lockedIn)
                {
                    if (currentController.canChooseStage)
                    {
                        currentController.xVal = currentController.curPlayer.GetAxis("Horizontal");
                        currentController.xVal = (currentController.xVal >= currentController.xYield) ? 1 : ((currentController.xVal <= -currentController.xYield) ? -1 : 0);
                        if (currentController.xVal == 1)
                        {
                            if (!stageSelectCooldown)
                            {
                                _stageSelecter.ToggleUp();
                                    StartCoroutine(DelayResetStageBool());
                            }
                        }
                        else if (currentController.xVal == -1)
                        {
                            if (!stageSelectCooldown)
                            {
                                _stageSelecter.ToggleDown();
                                    StartCoroutine(DelayResetStageBool());
                            }
                        }
                        else
                        {
                            StopCoroutine(DelayResetStageBool());
                            stageSelectCooldown = false;
                        }
                    }
                }
            }
        }
    }
    public void DisplayObjectlapMessage()
    {
        advidoryMessage.gameObject.SetActive(true);
        if (DisplayMessageSequence != null) 
        {
            DOTween.Kill(DisplayMessageSequence);
            DisplayMessageSequence = null;
        }
        string message = "Players cannot choose same side";
        advidoryMessage.text = $"<size=95>{message}";
        DisplayMessageSequence = DOTween.Sequence();
        DisplayMessageSequence.Append(advidoryMessage.DOFade(1f, 0f));
        DisplayMessageSequence.Append(advidoryMessage.DOFade(0.99f, 2f));
        DisplayMessageSequence.Append(advidoryMessage.DOFade(0f, 1.35f));
        DisplayMessageSequence.OnComplete(() =>
        {
            advidoryMessage.gameObject.SetActive(false);
        });
    }
    IEnumerator DelayResetStageBool() 
    {
        stageSelectCooldown = true;
        yield return new WaitForSeconds(0.65f);
        stageSelectCooldown = false;
    }
    bool HitHeightBound(Transform cursorTransform) 
    {
        Bounds heightBounds = new Bounds(cursorTransform.localPosition, Vector3.zero);
        heightBounds.SetMinMax(downBound.localPosition, upBound.localPosition);
        float yPos = cursorTransform.localPosition.y;
        if (yPos > heightBounds.max.y - 1f)
        {
            cursorTransform.GetComponent<Rigidbody2D>().drag = 10000f;
            cursorTransform.localPosition = new Vector3(cursorTransform.localPosition.x, heightBounds.max.y - 10f, 0);
            return true;
        }
        if (yPos < heightBounds.min.y + 1f)
        {
            cursorTransform.GetComponent<Rigidbody2D>().drag = 10000f;
            cursorTransform.localPosition = new Vector3(cursorTransform.localPosition.x, heightBounds.min.y + 10f, 0); ;
            return true;
        }
        return false;
    }
    bool HitWidthBound(Transform cursorTransform)
    {
        Bounds widthBounds = new Bounds(cursorTransform.localPosition, Vector3.zero);
        widthBounds.SetMinMax(leftBound.localPosition, rightBound.localPosition);
        float xPos = cursorTransform.localPosition.x;
        if (xPos > widthBounds.max.x - 1)
        {
            cursorTransform.GetComponent<Rigidbody2D>().drag = 10000f;

            cursorTransform.localPosition = new Vector3(widthBounds.max.x - 10f, cursorTransform.localPosition.y, 0);
            return true;
        }
        if (xPos < widthBounds.min.x + 1)
        {
            cursorTransform.GetComponent<Rigidbody2D>().drag = 10000f;
            cursorTransform.localPosition = new Vector3(widthBounds.min.x + 10f, cursorTransform.localPosition.y, 0);
            return true;
        }
        return false;
    }

    #endregion

    void LockinCharacterChoice(Character_Profile chosenProfile, CharacterSelect_Cursor cursor)
    {
        cursor.LockinCharacterChoice(chosenProfile);
        CheckIfBothPlayersLockedIn(cursor);
    }

    void ActivateStageSelector()
    {
        characterSelect_Assets[1].SetActive(true);
        _stageSelecter.ActivateRoundSelector(currentSet.gameMode);
        _stageSelecter.SetArrowsLitState(_activeStages);
    }
    #region Return Character Select Information
    public ChosenCharacter GetLeftPlayerProfile() 
    {
        if (_player1_Cursor.cursorPage.chosenCharacter != null)
        {
            ChosenCharacter leftPlayerCharacter = new ChosenCharacter(_player1_Cursor.cursorPage.chosenCharacter, _player1_Cursor.cursorPage.chosenAmplifier, _player1_Cursor.ChosenPlayerSide,Character_SubStates.Controlled);
            return leftPlayerCharacter;
        }
        return RandomizeChoice(player1, _player1_Cursor);
    }
    public ChosenCharacter GetRightPlayerProfile()
    {
        if(_player2_Cursor.cursorPage.chosenCharacter != null)
        {
            ChosenCharacter rightPlayerCharacter = new ChosenCharacter(_player2_Cursor.cursorPage.chosenCharacter, _player2_Cursor.cursorPage.chosenAmplifier, _player2_Cursor.ChosenPlayerSide, Character_SubStates.Controlled);
            return rightPlayerCharacter;
        }
        return RandomizeChoice(player2, _player2_Cursor);
    }
    public Round_Info GetRoundInfomation()
    {
        return new Round_Info(_stageSelecter.winningRoundCount);
    }
    public ChosenCharacter RandomizeChoice(ChooseSide_Object chosenSide, CharacterSelect_Cursor cursorObject)
    {
        int randomProfile = UnityEngine.Random.Range(0, _activeProfiles.Count - 1);
        int randomAmplifier = UnityEngine.Random.Range(0, _activeAmplifiers.Count - 1);
        if (chosenSide.sideIterator == 1)
        {
            ChosenCharacter _randomizedCharacter = new ChosenCharacter(_activeProfiles[randomProfile], _activeAmplifiers[randomAmplifier], -1);
            return _randomizedCharacter;
        }
        else 
        {
            ChosenCharacter _randomizedCharacter = new ChosenCharacter(_activeProfiles[randomProfile], _activeAmplifiers[randomAmplifier], cursorObject.ChosenPlayerSide, Character_SubStates.Controlled);
            return _randomizedCharacter;
        }
    }
    public Stage_StageAsset GetChosenStage()
    {
        if (_chosenStage.stageName == "Random")
        {
            for (int i = 0; i < _activeStages.Count; i++)
            {
                if (_activeStages[i].stageName == "Random")
                {
                    _activeStages.RemoveAt(i);
                    break; 
                }
                continue;
            }
            _chosenStage = _activeStages[UnityEngine.Random.Range(0, _activeStages.Count - 1)];
            return _chosenStage;
        }
        return _chosenStage;
    }
    #endregion
}
[Serializable]
public class ChosenCharacter 
{
    public Character_Profile chosenCharacter;
    public Amplifiers chosenAmplifier;
    public int ChosenPlayerSide;
    public Character_SubStates subState;
    public ChosenCharacter(Character_Profile _chosenCharacter, Amplifiers _chosenAmplifier,int _chosenSide, Character_SubStates _subState = Character_SubStates.Dummy) 
    {
        chosenCharacter = _chosenCharacter;
        chosenAmplifier = _chosenAmplifier;
        ChosenPlayerSide = _chosenSide;
        subState = _subState;
    }
}
[Serializable]
public class ChooseSide_Object
{
    public int sideIterator;
    public GameObject _object;
    public Image _coloredControllerImage;
    public TMP_Text objectText;

    public void InitSideIterator() 
    {
        sideIterator = 1;
    }
    public void SetImageCPU(bool setText = true) 
    {
        _coloredControllerImage.DOColor(Color.gray, 0.45f).OnComplete(() =>
        {
            if (setText)
            {
                objectText.text = "CPU";
            }
        });
    }
    public void SetImageP1(bool setText = true)
    {
        _coloredControllerImage.DOColor(Color.red, 0.45f).OnComplete(() =>
        {
            if (setText)
            {
                objectText.text = "P1";
            }
        });
    }
    public void SetImageP2(bool setText = true)
    {
        _coloredControllerImage.DOColor(Color.blue, 0.45f).OnComplete(() =>
        {
            if (setText)
            {
                objectText.text = "P2";
            }
        });
    }
}