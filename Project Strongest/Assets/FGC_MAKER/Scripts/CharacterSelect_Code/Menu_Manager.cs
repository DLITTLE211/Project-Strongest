using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;
using UnityEngine.UI;
using TMPro;
using Rewired;
using DG.Tweening;
using UnityEngine.EventSystems;


public class Menu_Manager : MonoBehaviour
{
    [SerializeField] private Menu_StateMachine _menuStateMachine;
    [SerializeField] private CharacterSelect_Setup _characterSelect;
    [SerializeField] private GameObject _mainMenuHolder;
    [SerializeField] private GameObject _titleScreenHolder;
    [SerializeField] private GameObject _mainCamera;
    [SerializeField] private EventSystem _eventSystem; 
    [SerializeField] private Player _mainMenuPlayer;
    private int _mainMenuPlayerID;
    public Character_AvailableID players;
    [SerializeField] private MenuButtonHolder FirstMenuButtonLayer;
    [Space(15)]
    [SerializeField] private Image _backgroundImage,whiteFillerImage;
    [SerializeField] private TMP_Text _titleText,_versionText;
    public static GameModeSet currentMode;
    public OutOfMatchGameState menuGameState;
    Transform _titleTextTransform;
    public static Menu_Manager instance;
    public static bool subsequentLoad;
    [SerializeField] private GameObject exitObject;
    private void Start()
    {
        _mainCamera.SetActive(true);
        Application.targetFrameRate = 60;
        DontDestroyOnLoad(this);
        instance = this;
        SetTitlePage();
    }
    public async void SetTitlePage()
    {
        _menuStateMachine.CallTitleState();
        //ToggleTitleState(true);
        SetPlayerControllers();
        menuGameState = OutOfMatchGameState.Title;
    }
    private void Update()
    {
        if (_mainMenuPlayer != null)
        {
            if (menuGameState == OutOfMatchGameState.Title)
            {
                TitleButtonFunction();
            }
        }
    }
    void TitleButtonFunction() 
    {
        if (_mainMenuPlayer.GetButtonDown("Select"))
        {
            if (exitObject.activeInHierarchy)
            {
                Application.Quit();
            }
            else
            {
                ActivateMainMenuPage();
            }
        }
        if (_mainMenuPlayer.GetButtonDown("Cancel"))
        {
            if (exitObject.activeInHierarchy)
            {
                exitObject.SetActive(false);
            }
            else
            {
                exitObject.SetActive(true);
            }
        }
    }
    public async void ActivateMainMenuPage()
    {
        _menuStateMachine.CallMainMenuState();
        menuGameState = OutOfMatchGameState.Menu;
    }
    public void DelayChosenPage(Callback func) 
    {
        func();
        _mainCamera.SetActive(true);
    }
    void SetActiveButton() 
    {
        _eventSystem.firstSelectedGameObject = FirstMenuButtonLayer.buttonList[0].gameObject;
        FirstMenuButtonLayer.buttonList[0].Select();
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
            players.AddUsedID(players.joystickNames[0]);
            SetCharacterSelectCursorState(0);
        }
    }
    void SetCharacterSelectCursorState(int ID)
    {
        _mainMenuPlayer = ReInput.players.GetPlayer(players.UsedID.Item1[ID]);
        _mainMenuPlayerID = ID;
        _mainMenuPlayer.controllers.AddController(ControllerType.Joystick, players.UsedID.Item1[ID], true);
        _mainMenuPlayer.controllers.maps.LoadMap(ControllerType.Joystick, players.UsedID.Item1[ID],
            $"UI_CanvasController", $"TestPlayer{_mainMenuPlayerID}");
    }
    public void SetButtonHolderImages()
    {
        FirstMenuButtonLayer.SetImageObject(_backgroundImage);
        FirstMenuButtonLayer.EnableButtons();
    }
    #region Training Button Response
    public async void TrainingSelected()
    {
        /*Task[] tasks = new Task[]
        {
            //CloseMainMenuScreen(),
        };
        await Task.WhenAll(tasks);
        await Task.Delay(750);
        ToggleMainMenuState(false);
        await Task.Delay(200);*/
        currentMode = new GameModeSet(GameMode.Training);
        _menuStateMachine.CallCharacterSelectState();
        //_characterSelect.SetUpCharacterSelectScreen(players, currentMode);
    }
    #endregion

    #region Versus Button Response
    public async void VersusSelected()
    {
        Task[] tasks = new Task[]
        {
            CloseMainMenuScreen(),
        };
        await Task.WhenAll(tasks);
        await Task.Delay(750);
        ToggleMainMenuState(false);
        await Task.Delay(200);
        currentMode = new GameModeSet(GameMode.Versus);
        _characterSelect.SetUpCharacterSelectScreen(players, currentMode);
    }
    #endregion

    #region Options Button Response
    public void OptionsSelected()
    {
        //ToggleMainMenuState(false);
    }
    #endregion

    public async void ExitSelected()
    {
        Task[] tasks = new Task[]
        {
            CloseMainMenuScreen(),
        };
        await Task.WhenAll(tasks);
        await Task.Delay(750);
        ToggleMainMenuState(false);
        await Task.Delay(200);
        ToggleTitleState(true);
        menuGameState = OutOfMatchGameState.Title;
    }
    void ToggleTitleState(bool state) 
    {
        _titleScreenHolder.SetActive(state);
    }
    void ToggleMainMenuState(bool state)
    {
        _mainMenuHolder.SetActive(state);
    }
    public async Task CloseMainMenuScreen()
    {
        FirstMenuButtonLayer.DisableButtons();
        FirstMenuButtonLayer.DisableButtons();
        _titleText.DOFade(0, 1.5f);
        _versionText.DOFade(0, 1.5f);
        FirstMenuButtonLayer.SlideHolderOut();
        float moveUpPos = _titleTextTransform.localPosition.y + 225f;
        _titleTextTransform.DOLocalMoveY(moveUpPos, 1.5f);
        _backgroundImage.DOFade(0, 1.5f);
        whiteFillerImage.DOFade(0, 1.5f);
        await Task.Delay(40);
    }
    public async Task OpenMainMenuScreen()
    {
        _titleTextTransform = _titleText.GetComponent<Transform>();
        FirstMenuButtonLayer.SlideHolderIn(SetActiveButton);
        SetButtonHolderImages();
        FirstMenuButtonLayer.SlideHolderIn(SetActiveButton);
        _titleText.DOFade(255f, 1.5f);
        _versionText.DOFade(255f, 1.5f);
        float moveUpPos = _titleTextTransform.localPosition.y - 225f;
        _titleTextTransform.DOLocalMoveY(moveUpPos, 1.5f).SetEase(Ease.InOutBack);
        _backgroundImage.DOFade(1f, 1.5f);
        whiteFillerImage.DOFade(1f, 1.5f);
        await Task.Delay(40);
    }

}
[Serializable]
public class GameModeSet
{
    public GameMode gameMode;

    public List<Callback> startupFunctions;
    public GameModeSet (GameMode _gameMode) 
    {
        gameMode = _gameMode;
    }
    public void DoStartup() 
    {
        for (int i = 0; i < startupFunctions.Count; i++) 
        {
            startupFunctions[i]();
        }
    }
}
[Serializable]
public enum GameMode 
{
    Training,
    Versus,
    Arcade,
    Title,
}
[Serializable]
public enum OutOfMatchGameState
{
    Title,
    Menu,
    Settings,
}
