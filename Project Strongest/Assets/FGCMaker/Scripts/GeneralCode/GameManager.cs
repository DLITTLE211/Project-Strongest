using Rewired;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private MainGame_Arena_LoadStage stageLoader;

    private List<ChosenCharacter> playerProfiles;
    private Stage_StageAsset _chosenStage;
    public Character_AvailableID players;
    void Start()
    {
        /*
         * Use (Reinput.ControllerConnected) event to Initialize Player with unused ID's, 
         * set controller active state from training dummy to Controlled player, etc.
         * include array of ints (0,1) that holds current unused ID's for player assignment
         * Upon event called, assign first available ID to left character
         * (will be made to choose character and side at later date)
         * If no new controller is plugged in yet the controller count is greater than 0, 
         * setup player one on previous described information.
         */

        ReInput.ControllerConnectedEvent += SetupPlayers;
        ReInput.ControllerDisconnectedEvent += DesyncPlayers;
        SetTargetFrameRate();
        if (SceneManager.GetActiveScene().name == "MainGame_CharacterSelect") 
        {
            SceneManager.SetActiveScene(SceneManager.GetSceneByName("MainGame_Arena"));
        } 
        if (SceneManager.GetActiveScene().name == "MainGame_Arena") 
        {
            LoadStageAsset();
            LoadPlayerAssets();
        }
        SetupPlayers();
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

                players.totalPlayers[1].characterProfile = playerProfiles[1].chosenCharacter;
                players.totalPlayers[1].Initialize(Character_SubStates.Dummy, null, -1);
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
                    }
                    else { continue; }
                }
            }
        }
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


