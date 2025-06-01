using Rewired;
using System.Collections.Generic;
using System.Collections;
using System.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;


    [SerializeField] private Character_Base leftPlayer,rightPlayer;
    [SerializeField] private EventSystem _eventSystem;
    [SerializeField] private Transform pauseMenuHolder;
    [SerializeField] private GameObject trainingStageMenu, versusStageMenu;
    [SerializeField] private MainGame_SettingsController _settingsController;

    [SerializeField] private MainGame_RoundSystemController _RoundSystemController;

    [SerializeField] private MainGame_UIManager p1UIManager, p2UIManager;
    [SerializeField] private MainGame_Timer _stopWatchController { get; set; }
    [SerializeField] private MainGame_Arena_LoadStage stageLoader;
    public string OnRoundEndStatement { get; private set; }


    private List<ChosenCharacter> playerProfiles;
    private Stage_StageAsset _chosenStage;

    public Character_Hitstop _hitstopController;
    public General_FrameAdvantageCalculator _frameDataCalculator;
    public Character_AvailableID players;
    public GameModeSet _gameModeSet;
    public Player_SideManager sideManager;

    internal Character_Base winningCharacter;

    public MainGame_Timer stopWatchController { get { return _stopWatchController; } }
    public MainGame_SettingsController settingsController { get { return _settingsController; } }
    public MainGame_RoundSystemController RoundSystemController { get { return _RoundSystemController; } }

    public bool awaitedLoadComplete;
    private void Awake()
    {
        awaitedLoadComplete = false;
    }
    public void SetLoadComplete() 
    {
        awaitedLoadComplete = true;
    }
    void Start()
    {
        players = CharacterSelect_LoadArena.curPlayerData;
        players.totalPlayers.Clear();
        players.totalPlayers.Add(leftPlayer);
        players.totalPlayers.Add(rightPlayer);
        sideManager = GetComponent<Player_SideManager>();
        GameObject systemInScene = GameObject.Find("EventSystem");
        _eventSystem = systemInScene.GetComponent<EventSystem>();
        instance = this;
        _stopWatchController = GetComponent<MainGame_Timer>();
        ReInput.ControllerConnectedEvent += SetupPlayers;
        ReInput.ControllerDisconnectedEvent += SetupPlayers;


        if (SceneManager.GetActiveScene().name == "MainGame_MenuScene")
        {
            SceneManager.SetActiveScene(SceneManager.GetSceneByName("MainGame_Arena"));
        }
        if (SceneManager.GetActiveScene().name == "MainGame_Arena")
        {
            LoadStageAsset();
            LoadPlayerAssets();
        }


        _gameModeSet = Menu_Manager.currentMode;
        _gameModeSet.startupFunctions = new List<Callback>();
        if (_gameModeSet.gameMode == GameMode.Training)
        {
            _RoundSystemController.enabled = false;
            _gameModeSet.startupFunctions.Add(() => stopWatchController.SetStartTimerValues(Mathf.Infinity));
            gameObject.AddComponent<MainGame_TrainingSC>();
            GameObject trainingMenu = Instantiate(trainingStageMenu, pauseMenuHolder);
            _settingsController = gameObject.GetComponent<MainGame_TrainingSC>();
            _settingsController._pauseMenu = trainingMenu;
            trainingMenu.transform.localPosition = new Vector3(0, -500f, 0);

        }
        else
        {
            _RoundSystemController.enabled = true;
            gameObject.AddComponent<MainGame_VersusSC>();
            GameObject versusMenu = Instantiate(versusStageMenu, pauseMenuHolder);
            _settingsController = gameObject.GetComponent<MainGame_VersusSC>();
            _settingsController._pauseMenu = versusMenu;
            versusMenu.transform.localPosition = new Vector3(0, -500f, 0);
        }
        _gameModeSet.startupFunctions.Add(() => p1UIManager.SetActiveUI(_gameModeSet.gameMode));
        _gameModeSet.startupFunctions.Add(() => p2UIManager.SetActiveUI(_gameModeSet.gameMode));
        _gameModeSet.DoStartup();
        SetupPlayers();
    }
    public bool CheckWallGreaterPos(ref Vector3 teleportingCharacter) 
    {
        if(teleportingCharacter.x >= _chosenStage.RightWall.position.x)
        {
            teleportingCharacter.x = _chosenStage.RightWall.position.x-0.1f;
            return true;
        }
        if (teleportingCharacter.x <= _chosenStage.LeftWall.position.x)
        {
            teleportingCharacter.x = _chosenStage.RightWall.position.x + 0.1f;
            return true;
        }
        return false;
    }
    public void LoadStageAsset()
    {
        _chosenStage = CharacterSelect_LoadArena.chosenStage;
        stageLoader.LoadStage(_chosenStage);
    }
    public void LoadPlayerAssets() 
    {
        playerProfiles = new List<ChosenCharacter>();
        playerProfiles.Add(CharacterSelect_LoadArena.leftPlayerChosenProfile);
        playerProfiles.Add(CharacterSelect_LoadArena.rightPlayerChosenProfile);
    }

    public void SetupPlayers(ControllerStatusChangedEventArgs args = null)
    {
        if (args != null)
        {
            if (args.controller != null)
            {
                players.SetPlayerCharacter(args.controllerId, Character_SubStates.Controlled);
                playerProfiles[args.controllerId].subState = Character_SubStates.Controlled; 
            }
            else
            {
                players.SetPlayerCharacter(args.controllerId, Character_SubStates.Dummy);
                playerProfiles[args.controllerId].subState = Character_SubStates.Dummy;
            }
        }
        for (int i = 0; i < players.totalPlayers.Count; i++)
        {
            if (!players.totalPlayers[i].gameObject.activeInHierarchy)
            {
                players.totalPlayers[i].gameObject.SetActive(true);
                continue;
            }
            else { continue; }
        }
        int controllerNameLength = ReInput.controllers.GetJoystickNames().Length;
        if (controllerNameLength <= 0)
        {
            for (int i = 0; i < players.totalPlayers.Count; i++)
            {
                players.totalPlayers[i].Initialize(Character_SubStates.Dummy,i,0,null,-1);
                _settingsController.SetPlayerData(players.totalPlayers[i]);
            }
        }
        else
        {
            for (int i = 0; i < players.totalPlayers.Count; i++) 
            {
                ChosenCharacter CurChosenCharacter = playerProfiles[i];
                if (CurChosenCharacter.ChosenPlayerSide != -1 && CurChosenCharacter.subState == Character_SubStates.Controlled)
                {
                    players.totalPlayers[CurChosenCharacter.ChosenPlayerSide].characterProfile = CurChosenCharacter.chosenCharacter;
                    Character_Base curCharacter = players.totalPlayers[CurChosenCharacter.ChosenPlayerSide];
                    curCharacter.Initialize(Character_SubStates.Controlled, CurChosenCharacter.ChosenPlayerSide, CurChosenCharacter.ColorChoice, CurChosenCharacter.chosenAmplifier, players.characterIdentification.IDs[i]);
                }
                else 
                {
                    if (playerProfiles[0].ChosenPlayerSide == 0)
                    {
                        playerProfiles[i].ChosenPlayerSide = 1;
                    }
                    else if (playerProfiles[0].ChosenPlayerSide == 1)
                    {
                        playerProfiles[i].ChosenPlayerSide = 0;
                    }
                    players.totalPlayers[playerProfiles[i].ChosenPlayerSide].characterProfile = playerProfiles[i].chosenCharacter;
                    players.totalPlayers[playerProfiles[i].ChosenPlayerSide].Initialize(Character_SubStates.Dummy,i, CurChosenCharacter.ColorChoice, null, -1);
                }
                _settingsController.SetPlayerData(players.totalPlayers[i]);
            }
        }
        _settingsController.SetEventSystem(_eventSystem);
        if (_gameModeSet.gameMode == GameMode.Training)
        {
            _settingsController.SetTeleportPositions();
        }
        if (_RoundSystemController.enabled) 
        {
            _RoundSystemController.Initialize();
        }
    }
    public void CallPlayerDeath(Character_Base _winningCharacter) 
    {
        if (winningCharacter == null)
        {
            winningCharacter = _winningCharacter;
            _RoundSystemController.StateMachine.CallResultState();
        }
    }
    public void SetOnRoundEndStatement(string message) 
    {
        OnRoundEndStatement = message;
    }
    public Character_Base CallPlayerDeathOnTimerEnd()
    {
        float leftPlayerHealth = leftPlayer._cHealth.ReturnHealthDisparity();
        float rightPlayerHealth = rightPlayer._cHealth.ReturnHealthDisparity();
        if (leftPlayerHealth == rightPlayerHealth) 
        {
            _RoundSystemController.AwardTieWin();
        }
        else 
        {
            winningCharacter = leftPlayerHealth < rightPlayerHealth ? leftPlayer : rightPlayer;
            return winningCharacter;
        }
        return null;
    }
   /* public void DesyncPlayers(ControllerStatusChangedEventArgs args)
    {
        players.RemovePlayer(args.controllerId);
        if (players.totalPlayers[0].playerID == args.controllerId) 
        {
            players.totalPlayers[0].Initialize(Character_SubStates.Dummy, 0, 0, null, -1);
        }
        else 
        {
            players.totalPlayers[1].Initialize(Character_SubStates.Dummy, 1, 0, null, -1);
        }
    }*/

    public void UnloadFightingArena()
    {
        if (SceneManager.GetActiveScene().name == "MainGame_Arena")
        {
            Scene menuScene = SceneManager.GetSceneByName("MainGame_MenuScene");
            SceneManager.SetActiveScene(menuScene);
            SceneManager.UnloadSceneAsync("MainGame_Arena");
        }

    }
    public void PauseGame() 
    {
        _settingsController.TogglePauseMenu();
    }
    public void TeleportPosition()
    {
        _settingsController.SetPlayersPosition();
    }
    public void SetupEndScreen(Callback<EventSystem> func) 
    {
        func(_eventSystem);
    }
    private void OnApplicationQuit()
    {
        ReInput.ControllerConnectedEvent -= SetupPlayers;
        ReInput.ControllerDisconnectedEvent -= SetupPlayers;
    }
}
[Serializable]
public class PlayerCharacter_Controller
{
    public int ID;
    public string ControllerName;
    public Character_Base controllerPlayer;
    public Character_SubStates subState;
    public PlayerCharacter_Controller(int _newID, string _controllerName, Character_Base player = null, Character_SubStates _subState = Character_SubStates.Dummy) 
    { 
        ID = _newID;
        ControllerName = _controllerName;
        controllerPlayer = player;
        subState = _subState;
    }
    public void ClearPlayerData() 
    {
        ID = -1;
        ControllerName = "";
        controllerPlayer = null;
        subState = Character_SubStates.Dummy;
    }
    public void SetPlayer_Active(int _newID, string _controllerName)
    {
        ID = _newID;
        ControllerName = _controllerName;
        subState = Character_SubStates.Controlled;
    }
    public void SetPlayer_State(Character_SubStates newState)
    {
        subState = newState;
    }
    public void SetPlayer_Controlled(Character_Base player) 
    {
        controllerPlayer = player;
        subState = Character_SubStates.Controlled;
    }
    public void SetPlayer_CPU(Character_Base player) 
    {
        controllerPlayer = player;
        subState = Character_SubStates.CPU;
    }
}
[Serializable]
public class Identification 
{
    public List<int> IDs;
    public List<string> controllerNames;
    public Identification() 
    {
        IDs= new List<int>();
        controllerNames= new List<string>();
    }
    public void AddEntry(int newUsedID, string newUsedControllerName) 
    {
        IDs.Add(newUsedID);
        controllerNames.Add(newUsedControllerName);
    }
    public void RemoveEntry(int removedID) 
    {
        int indexOFID = IDs.IndexOf(removedID);
        IDs.RemoveAt(indexOFID);
        controllerNames.RemoveAt(indexOFID);
    }
    public int ReturnUseableIds() 
    {
        if (IDs.Count == 0)
        {
            return 0;
        }
        else 
        {
            if(IDs.Count >= 2) 
            {
                return IDs.Count;
            }
            else 
            {
                if (IDs.Contains(0)) 
                {
                    return 1;
                }
                if (IDs.Contains(1))
                {
                    return 0;
                }
                return IDs.Count;
            }
        }
    }
}
[Serializable]
public class Character_AvailableID 
{
    public PlayerCharacter_Controller Player1;
    public PlayerCharacter_Controller Player2;
    public Identification characterIdentification;
    public List<Character_Base> totalPlayers;
    public void InitializePlayerControllers()
    {
        characterIdentification = new Identification();
        Player1 = new PlayerCharacter_Controller(-1, "");
        Player2 = new PlayerCharacter_Controller(-1, "");
    }
    public void AddNewPlayer(int ID, string controllerName) 
    {
        if(Player1.ID == -1) 
        {
            Player1.SetPlayer_Active(ID, controllerName);
        }
        else if (Player2.ID == -1) 
        {
            Player2.SetPlayer_Active(ID, controllerName);
        }
        characterIdentification.AddEntry(ID, controllerName);
    }
    public void RemovePlayer(int ID)
    {
        if(Player1.ID == ID) 
        {
            Player1.ClearPlayerData();
        }
        else if (Player2.ID == ID) 
        {
            Player2.ClearPlayerData();
        }
        characterIdentification.RemoveEntry(ID);
    }
    public void SetPlayerCharacter(int ID,Character_SubStates newState)
    {
        if (Player1.ID == ID)
        {
            Player1.SetPlayer_State(newState);
        }
        else if (Player2.ID == ID)
        {
            Player2.SetPlayer_State(newState);
        }
    }
}

[System.Serializable]
public enum Character_SubStates
{ 
    Controlled, 
    CPU, 
    Dummy
}


