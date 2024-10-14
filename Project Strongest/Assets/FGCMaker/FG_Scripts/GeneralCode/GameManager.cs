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
    [SerializeField] private Character_Base leftPlayer,rightPlayer;
    [SerializeField] private EventSystem _eventSystem;
    [SerializeField] private Transform pauseMenuHolder;
    [SerializeField] private GameObject trainingStageMenu, versusStageMenu;
    [SerializeField] private MainGame_SettingsController _settingsController;
    public MainGame_SettingsController settingsController { get { return _settingsController; } }

    [SerializeField] private MainGame_RoundSystemController _RoundSystemController;
    public MainGame_RoundSystemController RoundSystemController { get { return _RoundSystemController; } }


    [SerializeField] private MainGame_UIManager p1UIManager, p2UIManager;
    [SerializeField] private MainGame_Timer _stopWatchController;
    public MainGame_Timer stopWatchController { get { return _stopWatchController; } }
    [SerializeField] private MainGame_Arena_LoadStage stageLoader;
    private List<ChosenCharacter> playerProfiles;
    private Stage_StageAsset _chosenStage;
    public Character_AvailableID players;
    public GameModeSet _gameModeSet;
    internal Character_Base winningCharacter;
    public static GameManager instance;
    public Player_SideManager sideManager;
    void Start()
    {
        players = CharacterSelect_LoadArena.curPlayerData;
        players.totalPlayers.Add(leftPlayer);
        players.totalPlayers.Add(rightPlayer);
        sideManager = GetComponent<Player_SideManager>();
        GameObject systemInScene = GameObject.Find("EventSystem");
        _eventSystem = systemInScene.GetComponent<EventSystem>();
        instance = this;
        _stopWatchController = GetComponent<MainGame_Timer>();
        ReInput.ControllerConnectedEvent += SetupPlayers;
        ReInput.ControllerDisconnectedEvent += DesyncPlayers;
        SetTargetFrameRate();
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
    public void SetTargetFrameRate(int frameRate = 60) 
    {
        Application.targetFrameRate = frameRate;
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
        for (int i = 0; i < players.totalPlayers.Count; i++)
        {
            if (!players.totalPlayers[i].gameObject.activeInHierarchy)
            {
                players.totalPlayers[i].gameObject.SetActive(true);
                continue;
            }
            else { continue; }
        }
        if (ReInput.controllers.GetJoystickNames().Length <= 0)
        {
            for (int i = 0; i < players.totalPlayers.Count; i++)
            {
                players.totalPlayers[i].Initialize(Character_SubStates.Dummy,i,null,-1);
                _settingsController.SetPlayerData(players.totalPlayers[i]);
            }
        }
        else
        {
            players.AddToJoystickNames(ReInput.controllers.GetJoystickNames());
            for (int i = 0; i < players.totalPlayers.Count; i++) 
            {
                ChosenCharacter CurChosenCharacter = playerProfiles[i];
                if (CurChosenCharacter.ChosenPlayerSide != -1 && CurChosenCharacter.subState == Character_SubStates.Controlled)
                {
                    players.totalPlayers[CurChosenCharacter.ChosenPlayerSide].characterProfile = CurChosenCharacter.chosenCharacter;
                    Character_Base curCharacter = players.totalPlayers[CurChosenCharacter.ChosenPlayerSide];
                    curCharacter.Initialize(Character_SubStates.Controlled, CurChosenCharacter.ChosenPlayerSide, CurChosenCharacter.chosenAmplifier, players.UsedID.Item1[i]);
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
                    players.totalPlayers[playerProfiles[i].ChosenPlayerSide].Initialize(Character_SubStates.Dummy,i, null, -1);
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
    public Character_Base CallPlayerDeathOnTimerEnd()
    {
        if (players.totalPlayers[0]._cHealth.health_Main.currentValue > players.totalPlayers[1]._cHealth.health_Main.currentValue)
        {
            winningCharacter = players.totalPlayers[0];
            return winningCharacter;
        }
        if (players.totalPlayers[1]._cHealth.health_Main.currentValue > players.totalPlayers[0]._cHealth.health_Main.currentValue)
        {
            winningCharacter = players.totalPlayers[1];
            return winningCharacter;
        }
        if (players.totalPlayers[0]._cHealth.health_Main.currentValue == players.totalPlayers[1]._cHealth.health_Main.currentValue)
        {
            _RoundSystemController.AwardTieWin();
        }
        return null;
    }
    public void DesyncPlayers(ControllerStatusChangedEventArgs args)
    {
        players.SubtractFromJoystickNames(ReInput.controllers.GetJoystickNames());
        for(int i = 0; i < players.totalPlayers.Count; i++) 
        {
            if (!players.UsedID.Item1.Contains(players.totalPlayers[i].playerID)) 
            {
                players.totalPlayers[i].Initialize(Character_SubStates.Dummy,i, null, -1);
            }
        }
    }

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
[System.Serializable]
public class Character_AvailableID 
{
    public List<int> availableIds;
    public (List<int>, List<string>) UsedID;
    public List<Character_Base> totalPlayers;
    public List<string> joystickNames,currentNames;

    [SerializeField] List<int> usedIntID;
    [SerializeField] List<string> usedStrings;
    public void InitAvailableIDs() 
    {
        availableIds = new List<int>();
        UsedID.Item1 = new List<int>();
        UsedID.Item2 = new List<string>();
        availableIds.Add(0);
        availableIds.Add(1);
    }
    public void AddToJoystickNames(string[] names) 
    {
        for (int i = 0; i < names.Length; i++)
        {
            if (joystickNames.Contains(names[i])) 
            {
                continue;
            }
            joystickNames.Add(names[i]);
        }
    }
    public void SubtractFromJoystickNames(string[] names)
    {
        currentNames = new List<string>();
        if (names.Length == 0)
        {
            availableIds.Add(UsedID.Item1[0]);
            UsedID.Item2.RemoveAt(0);
            UsedID.Item1.RemoveAt(0);
            joystickNames.RemoveAt(0);

            usedIntID.Clear();
            usedStrings.Clear();
            if(availableIds[0] > availableIds[1]) 
            {
                availableIds.Clear();
                availableIds.Add(0);
                availableIds.Add(1);
            }
        }
        else
        {
            for (int i = 0; i < names.Length; i++)
            {
                currentNames.Add(names[i]);
            }
            for (int i = 0; i < joystickNames.Count; i++)
            {
                if (!currentNames.Contains(joystickNames[i]))
                {
                    availableIds.Add(UsedID.Item1[i]);
                    UsedID.Item2.RemoveAt(i);
                    UsedID.Item1.RemoveAt(i);
                    joystickNames.RemoveAt(i);
                }
            }
        }

        usedIntID = UsedID.Item1;
        usedStrings = UsedID.Item2;
    }
    public void AddUsedID(string addectJoystick)
    {
        UsedID.Item1.Add(availableIds[0]);
        UsedID.Item2.Add(addectJoystick);
        foreach (int id in UsedID.Item1) 
        {
            availableIds.Remove(id);
        }
        usedIntID = UsedID.Item1;
        usedStrings = UsedID.Item2;
    }
}

[System.Serializable]
public enum Character_SubStates
{ 
    Controlled, 
    CPU, 
    Dummy
}


