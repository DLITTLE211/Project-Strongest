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
    [SerializeField] private EventSystem _eventSystem;
    [SerializeField] private Transform pauseMenuHolder;
    [SerializeField] private GameObject trainingStageMenu, versusStageMenu;
    [SerializeField] private MainGame_SettingsController _settingsController;
    [SerializeField] private MainGame_UIManager p1UIManager, p2UIManager;
    [SerializeField] private MainGame_Timer stopWatchController;
    [SerializeField] private MainGame_Arena_LoadStage stageLoader;
    private List<ChosenCharacter> playerProfiles;
    private Stage_StageAsset _chosenStage;
    public Character_AvailableID players;
    GameModeSet _gameModeSet;
    public static GameManager instance;
    void Start()
    {
        GameObject systemInScene = GameObject.Find("EventSystem");
        _eventSystem = systemInScene.GetComponent<EventSystem>();
        instance = this;
        stopWatchController = GetComponent<MainGame_Timer>();
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
            _gameModeSet.startupFunctions.Add(() => stopWatchController.SetStartTimerValues());
            gameObject.AddComponent<MainGame_TrainingSC>();
            GameObject trainingMenu = Instantiate(trainingStageMenu, pauseMenuHolder);
            _settingsController = gameObject.GetComponent<MainGame_TrainingSC>();
            _settingsController._pauseMenu = trainingMenu;
            trainingMenu.transform.localPosition = new Vector3(0, -500f, 0);

        }
        else
        {
            _gameModeSet.startupFunctions.Add(() => stopWatchController.SetStartTimerValues(99));
            gameObject.AddComponent<MainGame_SettingsController>();
            GameObject versusMenu = Instantiate(versusStageMenu, pauseMenuHolder);
            _settingsController = gameObject.GetComponent<MainGame_SettingsController>();
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
        if (args == null)
        {
            players.InitAvailableIDs();
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
        if (ReInput.controllers.GetJoystickNames().Length <= 0)
        {
            for (int i = 0; i < players.totalPlayers.Count; i++)
            {
                players.totalPlayers[i].Initialize(Character_SubStates.Dummy,null,-1);
                _settingsController.SetPlayerData(players.totalPlayers[i]);
            }
        }
        else
        {
            players.AddToJoystickNames(ReInput.controllers.GetJoystickNames());
            if (ReInput.controllers.GetJoystickNames().Length == 1)
            {
                players.totalPlayers[0].characterProfile = playerProfiles[0].chosenCharacter;
                players.totalPlayers[0].Initialize(Character_SubStates.Controlled, playerProfiles[0].chosenAmplifier, players.availableIds[0]);
                players.AddUsedID(players.joystickNames[0]);
                _settingsController.SetPlayerData(players.totalPlayers[0]);

                players.totalPlayers[1].characterProfile = playerProfiles[1].chosenCharacter;
                players.totalPlayers[1].Initialize(Character_SubStates.Dummy, null, -1);
                _settingsController.SetPlayerData(players.totalPlayers[1]);
            }
            else
            {
                for (int i = 0; i < players.totalPlayers.Count; i++)
                {
                    if (players.totalPlayers[i].playerID == -1)
                    {
                        players.totalPlayers[i].characterProfile = playerProfiles[i].chosenCharacter;
                        players.totalPlayers[i].Initialize(Character_SubStates.Controlled, playerProfiles[i].chosenAmplifier, players.availableIds[0]);
                        players.AddUsedID(players.joystickNames[i]);
                        _settingsController.SetPlayerData(players.totalPlayers[i]);
                    }
                    else { continue; }
                }
            }
        }
        _settingsController.SetTeleportPositions(_eventSystem);
    }
    public void DesyncPlayers(ControllerStatusChangedEventArgs args)
    {
        players.SubtractFromJoystickNames(ReInput.controllers.GetJoystickNames());
        for(int i = 0; i < players.totalPlayers.Count; i++) 
        {
            if (!players.UsedID.Item1.Contains(players.totalPlayers[i].playerID)) 
            {
                players.totalPlayers[i].Initialize(Character_SubStates.Dummy, null, -1);
            }
        }
    }

    public void UnloadFightingArena()
    {
        if (SceneManager.GetActiveScene().name == "MainGame_Arena")
        {
            Menu_Manager.subsequentLoad = true;
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


