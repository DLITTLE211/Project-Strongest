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
    [SerializeField] private ChooseSide_Object player1;
    [SerializeField] private ChooseSide_Object player2;
    [SerializeField] private CharacterSelect_ChosenSideController sideController;
    [Space(15)]

    [Header("____Character Cursor Information____")]
    [SerializeField] private List<Character_Profile> _activeProfiles;
    [SerializeField] private List<Amplifiers> _activeAmplifiers;
    [SerializeField] private List<GameObject> activeCharacterSelectButtons;
    [SerializeField] private CharacterSelect_Page _leftPlayerPage,_rightPlayerPage;
    [SerializeField] private CharacterSelect_Cursor _leftPlayer, _rightPlayer;
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
    // Start is called before the first frame update
    void Start()
    {
        SetupPlayerPage(_leftPlayerPage);
        SetupPlayerPage(_rightPlayerPage);
        player1.sideIterator = 1;
        player2.sideIterator = 1;
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
    
    public async void SetUpCharacterSelectScreen(Character_AvailableID _characterSelectplayers, GameModeSet set)
    {
        SetListeners();
        SideSelectionObject.SetActive(false);
        mainObjectHolder.SetActive(true);
        CharacterSelectObject.SetActive(false);
        players = _characterSelectplayers;
        currentSet = set;
        if (set.gameMode == GameMode.Versus)
        {
            advidoryMessage.gameObject.SetActive(false);
            SideSelectionObject.SetActive(true);
            if (players.UsedID.Item1.Count == 0)
            {
                advidoryMessage.gameObject.SetActive(true);
                player1.SetImageCPU();
                player2.SetImageCPU();
                CheckPlayerCount();
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
    private void Update()
    {
        CursorController(_leftPlayer);
        CursorController(_rightPlayer);
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
            _selectButtonInfo.GetComponent<CharacterSelect_Button>().GetLeftCursor(_leftPlayer.cursorObject.transform);
            _selectButtonInfo.GetComponent<CharacterSelect_Button>().GetRightCursor(_rightPlayer.cursorObject.transform);
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
            players.InitAvailableIDs();
            players.AddToJoystickNames(ReInput.controllers.GetJoystickNames());
            if (ReInput.controllers.GetJoystickNames().Length == 1)
            {
                players.AddUsedID(players.joystickNames[0]);
                if (_leftPlayer.curPlayer == null)
                {
                    SetCharacterSelectCursorState(_leftPlayer, 0);
                }
                else
                {
                    _leftPlayer.UnlockCharacterChoice();
                    _leftPlayer.isConnected = true;
                    _leftPlayerPage.ClearInfo();
                }
            }
            else
            {
                players.InitAvailableIDs();
                for (int i = 0; i < ReInput.controllers.GetJoystickNames().Length; i++)
                {
                    players.AddUsedID(players.joystickNames[i]);
                    if (i == 0)
                    {
                        if (_leftPlayer.curPlayer == null)
                        {
                            SetCharacterSelectCursorState(_leftPlayer, i);
                        }
                        else
                        {
                            _leftPlayer.UnlockCharacterChoice();
                            _leftPlayer.isConnected = true;
                            _leftPlayerPage.ClearInfo();
                        }
                    }
                    if (i == 1)
                    {
                        if (_rightPlayer.curPlayer == null)
                        {
                            SetCharacterSelectCursorState(_rightPlayer, i);
                        }
                        else
                        {
                            _rightPlayer.UnlockCharacterChoice();
                            _rightPlayer.isConnected = true;
                            _rightPlayerPage.ClearInfo();
                        }
                    }
                    else { continue; }
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
        if (cursorHighlight.ID == 0)
        {
            _leftPlayerPage.UpdateInfo(hoveredProfile);
        }
        if (cursorHighlight.ID == 1)
        {
            _rightPlayerPage.UpdateInfo(hoveredProfile);
        }
    }
    public void ClearCharacterSelectInformation(int curHighlightedPlayerID)
    {
        if (curHighlightedPlayerID == 0)
        {
            _leftPlayerPage.ClearInfo();
        }
        if (curHighlightedPlayerID == 1)
        {
            _rightPlayerPage.ClearInfo();
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
        _leftPlayer.DesyncController();
        _rightPlayer.DesyncController();
        await Task.Delay(400);
    }
    public async Task TogglePlayerInfo(float value) 
    {
        _leftPlayerPage.SetPlayerInfo(value);
        _rightPlayerPage.SetPlayerInfo(value);
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
        if (cursor == _leftPlayer)
        {
            if (cursor.cursorPage.lockedIn == true)
            {
                if (_rightPlayer.cursorObject.activeInHierarchy && !_rightPlayer.cursorPage.lockedIn && !_rightPlayer.canChooseStage)
                {
                    _leftPlayer.canChooseStage = true;
                }
                else if (_rightPlayer.cursorObject.activeInHierarchy && _rightPlayer.cursorPage.lockedIn && !_rightPlayer.canChooseStage)
                {
                    _rightPlayer.canChooseStage = true;
                    ActivateStageSelector();
                }
                else if (_rightPlayer.cursorObject.activeInHierarchy && _rightPlayer.cursorPage.lockedIn && _rightPlayer.canChooseStage)
                {
                    ActivateStageSelector();
                }
                else
                {
                    _leftPlayer.canChooseStage = true;
                    ActivateStageSelector();
                }
            }
        }
        else if (cursor == _rightPlayer) 
        {
            if (cursor.cursorPage.lockedIn == true)
            {
                if (_leftPlayer.cursorObject.activeInHierarchy && !_leftPlayer.cursorPage.lockedIn && !_leftPlayer.canChooseStage)
                {
                    _rightPlayer.canChooseStage = true;
                }
                else if (_leftPlayer.cursorObject.activeInHierarchy && _leftPlayer.cursorPage.lockedIn && !_leftPlayer.canChooseStage)
                {
                    _leftPlayer.canChooseStage = true;
                    ActivateStageSelector();
                }
                else if (_leftPlayer.cursorObject.activeInHierarchy && _leftPlayer.cursorPage.lockedIn && _leftPlayer.canChooseStage)
                {
                    ActivateStageSelector();
                }
                else
                {
                    _rightPlayer.canChooseStage = true;
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
                if (currentController.profile == null)
                {
                    Messenger.Broadcast<CharacterSelect_Cursor>(Events.TryApplyCharacter, currentController);
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
                StartCoroutine(currentController.cursorPage.DelayResetBool());
                currentController.cursorPage.characterAmplify.UpdateInfoUp();
            }
            if (currentController.curPlayer.GetButtonDown("Shift_Left"))
            {
                StartCoroutine(currentController.cursorPage.DelayResetBool());
                currentController.cursorPage.characterAmplify.UpdateInfoDown();
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
                        sideController.UpdateControllerSide(currentController.ID == 0 ? player1 : player2, (int)currentController.xVal); 
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
                                _stageSelecter.UpdateInfoUp();
                                StartCoroutine(DelayResetStageBool());
                            }
                        }
                        else if (currentController.xVal == -1)
                        {
                            if (!stageSelectCooldown)
                            {
                                _stageSelecter.UpdateInfoDown();
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
    IEnumerator DelayResetStageBool() 
    {
        stageSelectCooldown = true;
        yield return new WaitForSeconds(1f);
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
        _stageSelecter.SetArrowsLitState(_activeStages);
    }
    #region Return Character Select Information
    public ChosenCharacter GetLeftPlayerProfile() 
    {
        if (_leftPlayer.cursorPage.chosenCharacter != null)
        {
            ChosenCharacter leftPlayerCharacter = new ChosenCharacter(_leftPlayer.cursorPage.chosenCharacter, _leftPlayer.cursorPage.chosenAmplifier);
            return leftPlayerCharacter;
        }
        return RandomizeChoice();
    }
    public ChosenCharacter GetRightPlayerProfile()
    {
        if(_rightPlayer.cursorPage.chosenCharacter != null)
        {
            ChosenCharacter rightPlayerCharacter = new ChosenCharacter(_rightPlayer.cursorPage.chosenCharacter, _rightPlayer.cursorPage.chosenAmplifier);
            return rightPlayerCharacter;
        }
        return RandomizeChoice();
    }
    public ChosenCharacter RandomizeChoice()
    {
        int randomProfile = UnityEngine.Random.Range(0, _activeProfiles.Count - 1);
        int randomAmplifier = UnityEngine.Random.Range(0, _activeAmplifiers.Count - 1);
        ChosenCharacter _randomizedCharacter = new ChosenCharacter(_activeProfiles[randomProfile], _activeAmplifiers[randomAmplifier]);
        return _randomizedCharacter;
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
    public ChosenCharacter(Character_Profile _chosenCharacter, Amplifiers _chosenAmplifier) 
    {
        chosenCharacter = _chosenCharacter;
        chosenAmplifier = _chosenAmplifier;
    }
}
[Serializable]
public class ChooseSide_Object
{
    public int sideIterator;
    public GameObject _object;
    public Image _coloredControllerImage;
    public TMP_Text objectText;
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