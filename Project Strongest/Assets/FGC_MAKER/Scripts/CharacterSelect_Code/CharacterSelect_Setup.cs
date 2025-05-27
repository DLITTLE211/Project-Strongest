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
    public Menu_StateMachine _menuStateMachine;
    [Header("____CharacterSelect Assets____")]
    [SerializeField] private GameObject SideSelectionObject;
    [SerializeField] private GameObject mainObjectHolder;
    [SerializeField] private GameObject CharacterSelectObject;
    [SerializeField] private List<GameObject> characterSelect_Assets;
    [Space(15)]

    [Header("____Character Side Information____")]
    [SerializeField] private TMP_Text advisoryMessage;
    [SerializeField] private CharacterSelect_ChosenSideController sideController;
    [Space(15)]

    [Header("____Character Cursor Information____")]
    public List<CharacterSelect_Cursor> _playerCursors;
    [SerializeField] private CharacterSelect_Page topPage, bottomPage;
    [SerializeField] private CharacterSelect_Cursor _player1_Cursor, _player2_Cursor;
    public ChooseSide_Object player1;
    public ChooseSide_Object player2;
    [Space(15)]
    [Header("____Stage Select Information____")]
    [SerializeField] private CharacterSelect_LoadArena _arenaLoader;
    [Header("____Rewired Players____")]
    public Character_AvailableID players;
    public GameModeSet currentSet;
    Sequence DisplayMessageSequence;
    // Start is called before the first frame update
    void Start()
    {
        SetupPlayerPage(topPage);
        SetupPlayerPage(bottomPage);
        player1.sideIterator = 1;
        player2.sideIterator = 1;
    }
    
    public void SetListeners() 
    {
        ReInput.ControllerConnectedEvent += AddControllerCounter;
        ReInput.ControllerDisconnectedEvent += SubtractControllerCounter;
    }
    void SetupPlayerPage(CharacterSelect_Page playerPage) 
    {
        playerPage.SetPlayerInfo(255f);
    }
    public void SubtractControllerCounter(ControllerStatusChangedEventArgs args = null)
    {
        players.RemovePlayer(args.controllerId);
        CheckPlayerCount();
    }
    public void AddControllerCounter(ControllerStatusChangedEventArgs args = null)
    {
        List<string> controllerNames = new List<string>();
        controllerNames = ReInput.controllers.GetJoystickNames().ToList();
        
        for (int i = 0; i < controllerNames.Count; i++) 
        {
            if (players.characterIdentification.controllerNames.Contains(controllerNames[i]))
            {
                continue;
            }
            int useableId = players.characterIdentification.ReturnUseableIds();
            players.AddNewPlayer(useableId, controllerNames[i]);
        }
        CheckPlayerCount();
    }
    public void CheckPlayerCount() 
    {
        int playerCount = players.characterIdentification.IDs.Count;
        if (playerCount == 0)
        {
            advisoryMessage.text = "Please Plug in a controller to continue";
            advisoryMessage.gameObject.SetActive(true);
            player1.SetImageCPU();
            player2.SetImageCPU();
            return;
        }
        advisoryMessage.gameObject.SetActive(false);
        if (playerCount == 1)
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
    async void SetPlayerInformation_OnCharacterSelect()
    {
        CharacterSelectObject.SetActive(true);
        Task[] tasks = new Task[]
        {
            TogglePlayerInfo(255f),
        };
        await Task.WhenAll(tasks);
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
                _player1_Cursor.cursorPage = topPage;
            }
            if (player1.sideIterator == 2)
            {
                _player1_Cursor.ChosenPlayerSide = 1;
                _player1_Cursor.cursorPage = bottomPage;
            }
        }
        if (player2.sideIterator == 1)
        {
            _player2_Cursor.cursorPage = player1.sideIterator == 0 ? bottomPage : topPage;
        }
        else
        {
            _player2_Cursor.gameObject.SetActive(true);
            if (player2.sideIterator == 0)
            {
                _player2_Cursor.ChosenPlayerSide = 0;
                _player2_Cursor.cursorPage = topPage;

            }
            if (player2.sideIterator == 2)
            {
                _player2_Cursor.ChosenPlayerSide = 1;
                _player2_Cursor.cursorPage = bottomPage;
            }
        }

    }
    public void SetUpCharacterSelectScreen(Character_AvailableID _characterSelectplayers, GameModeSet set)
    {
        players = _characterSelectplayers;
        currentSet = set;
    }
    public async void CallCharacterSelectObject()
    {
        await OpenCharacterSelectObject();
    }
    public async Task OpenCharacterSelectObject() 
    {
        if (CharacterSelectObject.activeInHierarchy) 
        {
            return;
        }
        CharacterSelectObject.SetActive(true);
        Task[] tasks = new Task[]
        {
            //ToggleCharacterSelectInfo(true,255f),
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
    public void CheckControllerState()
    {
        int playerCount = players.characterIdentification.IDs.Count;
        if (playerCount == 0)
        {
            advisoryMessage.gameObject.SetActive(true);
            player1.SetImageCPU();
            player2.SetImageCPU();
        }
        if (playerCount == 1)
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

   
    public async Task DisableCharacterCursors() 
    {
        _player1_Cursor.DesyncController();
        _player2_Cursor.DesyncController();
        await Task.Delay(400);
    }
    public async Task TogglePlayerInfo(float value) 
    {
        topPage.SetPlayerInfo(value);
        bottomPage.SetPlayerInfo(value);
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

    public void CheckIfBothPlayersLockedIn(CharacterSelect_Cursor cursor)
    {
        if (cursor == _player1_Cursor)
        {
            if (!_player2_Cursor.cursorPage.lockedIn)
            {
                _player1_Cursor.canChooseStage = true;
            }
            else
            {
                _player2_Cursor.canChooseStage = true;
            }
            MoveToColorSelectState(_player2_Cursor);
            return;
        }
        if (cursor == _player2_Cursor)
        {
            if (!_player1_Cursor.cursorPage.lockedIn)
            {
                _player2_Cursor.canChooseStage = true;
            }
            else
            {
                _player1_Cursor.canChooseStage = true;
            }
            MoveToColorSelectState(_player1_Cursor);
        }
    }
    public void MoveToColorSelectState(CharacterSelect_Cursor oppositeCursor) 
    {
        if (oppositeCursor.isConnected)
        {
            if (oppositeCursor.cursorPage.lockedIn)
            {
                _menuStateMachine.CallColorSelectState();
            }
        }
        else
        {
            _menuStateMachine.CallColorSelectState();
        }
    }
    bool CheckPlayersReady(CharacterSelect_Cursor oppositeCursor)
    {
        if (oppositeCursor.isConnected)
        {
            if (oppositeCursor.colorChosen)
            {
                return true;
            }
            return false;
        }
        return true;
    }
    public void CheckAmplifiersFilled() 
    {
        bool allActive = true;
        for(int i = 0; i < _playerCursors.Count; i++) 
        {
            if(_playerCursors[i].isConnected && _playerCursors[i].cursorPage.chosenAmplifier == null) 
            {
                allActive = false;
                break;
            }
        }
        if (allActive) 
        {
            _menuStateMachine.CallCharacterSelectState();
        }
    }
    public void CheckGameModeSet() 
    {
        if (CheckPlayersReady(_player1_Cursor) && CheckPlayersReady(_player2_Cursor))
        {
            _menuStateMachine.CallStageSelectState();
        }
    }
    public void ClearListeners() 
    {
        ReInput.ControllerConnectedEvent -= AddControllerCounter;
        ReInput.ControllerDisconnectedEvent -= SubtractControllerCounter;
    }
    public void CallStageSelected() 
    {
        _arenaLoader.OnCharactersAndStageSelected();
    }
    public void ResetCharacterSide() 
    {
        sideController.SetStartParent(player1);
        sideController.SetStartParent(player2);
    }
    #region CursorController
    public void CursorController(CharacterSelect_Cursor currentController) 
    {
        if (currentController.isConnected)
        {
            if (currentController.curPlayer.GetButtonDown(17))
            {
                if (SideSelectionObject.activeInHierarchy)
                {
                    _menuStateMachine.CallAmplifierSelectState();
                    sideController.CloseChooseSideMenu(SetPlayerInformation_OnCharacterSelect);
                }
                else
                {
                    _menuStateMachine.GetCurrentState().Select(currentController);
                }
            }
            if (currentController.curPlayer.GetButton(18))
            {
                _menuStateMachine.GetCurrentState().Cancel(currentController);
            }
            currentController.xVal = currentController.curPlayer.GetAxisRaw("Horizontal");
            currentController.yVal = currentController.curPlayer.GetAxisRaw("Vertical");
            currentController.xVal = (currentController.xVal >= currentController.xYield) ? 1 : ((currentController.xVal <= -currentController.xYield) ? -1 : 0);
            currentController.yVal = (currentController.yVal >= currentController.yYield) ? 1 : ((currentController.yVal <= -currentController.yYield) ? -1 : 0);
            if (SideSelectionObject.activeInHierarchy)
            {
                ChooseSide_Object curObject = currentController.ID == 0 ? player1 : player2;
                if ((int)currentController.xVal != 0)
                {
                    sideController.UpdateControllerSide(curObject, (int)currentController.xVal, DisplayObjectlapMessage);
                }
            }
            else
            {
                if (currentController.yVal == 1)
                {
                    _menuStateMachine.GetCurrentState().CycleUp(currentController);
                    return;
                }
                if (currentController.yVal == -1)
                {
                    _menuStateMachine.GetCurrentState().CycleDown(currentController);
                    return;
                }
                if (currentController.xVal == 1)
                {
                    _menuStateMachine.GetCurrentState().CycleRight(currentController);
                    return;
                }
                if (currentController.xVal == -1)
                {
                    _menuStateMachine.GetCurrentState().CycleLeft(currentController);
                    return;
                }
            }

        }
    }
    #endregion
    public void DisplayObjectlapMessage()
    {
        advisoryMessage.gameObject.SetActive(true);
        if (DisplayMessageSequence != null) 
        {
            DOTween.Kill(DisplayMessageSequence);
            DisplayMessageSequence = null;
        }
        string message = "Players cannot choose same side";
        advisoryMessage.text = $"<size=95>{message}";
        DisplayMessageSequence = DOTween.Sequence();
        DisplayMessageSequence.Append(advisoryMessage.DOFade(1f, 0f));
        DisplayMessageSequence.Append(advisoryMessage.DOFade(0.99f, 2f));
        DisplayMessageSequence.Append(advisoryMessage.DOFade(0f, 1.35f));
        DisplayMessageSequence.OnComplete(() =>
        {
            advisoryMessage.gameObject.SetActive(false);
        });
    }
}
[Serializable]
public class ChosenCharacter 
{
    public Character_Profile chosenCharacter;
    public Amplifiers chosenAmplifier;
    public int ChosenPlayerSide;
    public int ColorChoice;
    public Character_SubStates subState;
    public ChosenCharacter(Character_Profile _chosenCharacter, Amplifiers _chosenAmplifier,int _chosenSide,int _colorChoice ,Character_SubStates _subState = Character_SubStates.Dummy) 
    {
        chosenCharacter = _chosenCharacter;
        chosenAmplifier = _chosenAmplifier;
        ChosenPlayerSide = _chosenSide;
        ColorChoice = _colorChoice;
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
        SetImageCPU(false);
    }
    public void SetImageCPU(bool setText = true) 
    {
        _coloredControllerImage.DOColor(Color.gray, 0.45f).OnComplete(() =>
        {
            if (setText)
            {
                objectText.text = SpriteToTextColorUtility.AppendSpriteName("CPU",objectText.color);
            }
        });
    }
    public void SetImageP1(bool setText = true)
    {
        _coloredControllerImage.DOColor(Color.red, 0.45f).OnComplete(() =>
        {
            if (setText)
            {
                objectText.text = SpriteToTextColorUtility.AppendSpriteName("P1", objectText.color);
            }
        });
    }
    public void SetImageP2(bool setText = true)
    {
        _coloredControllerImage.DOColor(Color.blue, 0.45f).OnComplete(() =>
        {
            if (setText)
            {
                objectText.text = SpriteToTextColorUtility.AppendSpriteName("P2", objectText.color);
            }
        });
    }
}