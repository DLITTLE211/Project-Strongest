using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Threading.Tasks;

public class CharacterSelect_LoadArena : MonoBehaviour
{
    [SerializeField] private GameObject _mainMenuCamera;
    [SerializeField] private VersusMenu_DisplayController _displayController;
    [SerializeField] private CSSubMenu_CharacterSelectController _characterSelectController;
    [SerializeField] private CSSubMenu_StageSelectController _stageSelectController;
    [SerializeField] private CharacterSelect_Setup _characterSelectSetup;
    private bool _arenaLoaded;
    public static ChosenCharacter leftPlayerChosenProfile, rightPlayerChosenProfile;
    public static Stage_StageAsset chosenStage;
    public static Round_Info _roundInfo;
    public static Character_AvailableID curPlayerData;
    private void Awake()
    {
        _arenaLoaded = false;
    }
    public async void OnCharactersAndStageSelected() 
    {
        await LoadArena();
    }

    async Task LoadArena()
    {
        DontDestroyOnLoad(_mainMenuCamera.gameObject);
        if (SceneManager.GetActiveScene().name == "MainGame_MenuScene")
        {
            curPlayerData = _characterSelectSetup.players;
            _roundInfo = _stageSelectController.GetRoundInfomation();
            leftPlayerChosenProfile = _characterSelectController.GetLeftPlayerProfile();
            rightPlayerChosenProfile = _characterSelectController.GetRightPlayerProfile();
            chosenStage = _stageSelectController.GetChosenStage();
            if (_characterSelectSetup.currentSet.gameMode != GameMode.Training)
            {
                _displayController.CloseAndDisplayPlayerData();
            }
            Task[] tasks = new Task[]
            {
            _characterSelectSetup.DisableCharacterCursors(),
            _characterSelectSetup.TogglePlayerInfo(0),
            };
            await Task.WhenAll(tasks);
            await Task.Delay(1500);
            _characterSelectSetup.ClearListeners();
            for (int i = 0; i < _characterSelectController._playerCursors.Count; i++)
            {
                CharacterSelect_Page currentPage = _characterSelectController._playerCursors[i].cursorPage;
                currentPage.ResetNamePlatePosition();
            }
            _stageSelectController.ResetStagePositionData();
            if (_characterSelectSetup.currentSet.gameMode != GameMode.Training)
            {
                AsyncOperation newOP = SceneManager.LoadSceneAsync("MainGame_Arena", LoadSceneMode.Additive);
                newOP.completed += DelayEnableArenaObject;
            }
            else
            {
                _mainMenuCamera.SetActive(false);
                SceneManager.LoadSceneAsync("MainGame_Arena", LoadSceneMode.Additive);
            }
        }
    }
    async void DelayEnableArenaObject(AsyncOperation lastOp)
    {
        lastOp.completed -= DelayEnableArenaObject;
        await Task.Delay(2000);
        DelayDisableVersusObject();
    }
    async void DelayDisableVersusObject()
    {
        await Task.Delay(1850);
        _displayController.OpenDisplay();
        await Task.Delay(2000);
        _mainMenuCamera.SetActive(false);
        GameManager.instance.SetLoadComplete();
    }
    public void OnApplicationQuit()
    {
        SceneManager.LoadScene("MainGame_MenuScene", LoadSceneMode.Additive);
        SceneManager.UnloadSceneAsync("MainGame_Arena");
        _arenaLoaded = false;
    }
}
