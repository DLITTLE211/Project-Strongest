using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;
using System;
using DG.Tweening;
using UnityEngine.UI;
using System.Threading.Tasks;

public class CharacterSelect_Setup : MonoBehaviour
{
    [Header("____CharacterSelect Assets____")]
    [SerializeField] private GameObject characterSelectButtonPrefab;
    [SerializeField] private GameObject characterSelectHolder;
    [SerializeField] private GameObject characterSelect_Header;
    [SerializeField] private List<GameObject> characterSelect_Assets;
    [SerializeField] private Image characterSelectBackgroundImage;
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

    [SerializeField] private bool stageSelectCooldown;
    // Start is called before the first frame update
    void Start()
    {
        SetupPlayerPage(_leftPlayerPage);
        SetupPlayerPage(_rightPlayerPage);

        Messenger.AddListener<Character_Profile, CharacterSelect_Cursor>(Events.DisplayCharacterInfo, DisplayCharacterSelectInformation);
        Messenger.AddListener<int>(Events.ClearCharacterInfo, ClearCharacterSelectInformation);
        Messenger.AddListener<Character_Profile, CharacterSelect_Cursor>(Events.LockinCharacterChoice, LockinCharacterChoice);

        AddCharacterSelectButtons();
    }
    void SetupPlayerPage(CharacterSelect_Page playerPage) 
    {
        playerPage.characterAmplify.GetListOfAmplifiers(_activeAmplifiers);
        playerPage.SetPlayerInfo();
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
            _selectButtonInfo.GetComponentInChildren<CharacterSelect_Button>().characterProfile = _activeProfiles[i];
            _selectButtonInfo.GetComponentInChildren<CharacterSelect_Button>().GetLeftCursor(_leftPlayer.cursorObject.transform);
            _selectButtonInfo.GetComponentInChildren<CharacterSelect_Button>().GetRightCursor(_rightPlayer.cursorObject.transform);
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
        }
        SetPlayerControllers();
    }

    void SetPlayerControllers() 
    {
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
                SetCharacterSelectCursorState(_leftPlayer, 0);
            }
            else
            {
                players.InitAvailableIDs();
                for (int i = 0; i < ReInput.controllers.GetJoystickNames().Length; i++)
                {
                    players.AddUsedID(players.joystickNames[i]);
                    if (i == 0)
                    {
                        SetCharacterSelectCursorState(_leftPlayer, i);
                    }
                    if (i == 1)
                    {
                        SetCharacterSelectCursorState(_rightPlayer, i);
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
        player.cursorObject.SetActive(true);
        player.cursorText.text = $"{players.UsedID.Item1[ID] + 1}";
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
    public async Task ClearCharacterSelectInfo() 
    {
        for (int i = 0; i < activeCharacterSelectButtons.Count; i++)
        {
            activeCharacterSelectButtons[i].GetComponentInChildren<Button>().interactable = false;
            activeCharacterSelectButtons[i].GetComponentInChildren<Button>().image.DOFade(0f, 1.5f);
            activeCharacterSelectButtons[i].SetActive(false);
        }
        characterSelectBackgroundImage.gameObject.SetActive(false);
        characterSelect_Header.SetActive(false);
        characterSelectHolder.SetActive(false);
        await Task.Delay(400);
    }
    public async Task ClearLeftPlayerInfo()
    {
        _leftPlayerPage.ClearPlayerInfo();
        await Task.Delay(400);
    }
    public async Task ClearRightPlayerInfo()
    {
        _rightPlayerPage.ClearPlayerInfo();
        for (int i = 0; i < characterSelect_Assets.Count; i++)
        {
            characterSelect_Assets[i].SetActive(false);
        }
        await Task.Delay(400);
    }
    public async Task ClearStageSelect() 
    {
        _stageSelecter.CloseStageSelect();
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

    #region CursorController
    void CursorController(CharacterSelect_Cursor currentController) 
    {
        if (currentController.isConnected)
        {;
            if (currentController.curPlayer.GetButtonDown("C"))
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
            if (currentController.curPlayer.GetButtonDown("D"))
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
            if (currentController.curPlayer.GetButtonDown(19))
            {
                //if (!currentController.cursorPage.amplifySelectCooldown)
                //{
                    StartCoroutine(currentController.cursorPage.DelayResetBool());
                    currentController.cursorPage.characterAmplify.UpdateInfoUp();
                //}
            }
            if(currentController.curPlayer.GetButtonDown(21))
            {
                //if (!currentController.cursorPage.amplifySelectCooldown)
               // {
                    StartCoroutine(currentController.cursorPage.DelayResetBool());
                    currentController.cursorPage.characterAmplify.UpdateInfoDown();
               // }
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