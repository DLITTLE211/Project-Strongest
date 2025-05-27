using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Threading.Tasks;

public class CharacterSelect_LoadArena : MonoBehaviour
{
    [SerializeField] private GameObject _mainMenuCamera;
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
        if (SceneManager.GetActiveScene().name == "MainGame_MenuScene")
        {
            curPlayerData = _characterSelectSetup.players;
            _roundInfo = _stageSelectController.GetRoundInfomation();
            leftPlayerChosenProfile = _characterSelectController.GetLeftPlayerProfile();
            rightPlayerChosenProfile = _characterSelectController.GetRightPlayerProfile();
            chosenStage = _stageSelectController.GetChosenStage();
            Task[] tasks = new Task[]
            {
            _characterSelectSetup.DisableCharacterCursors(),
            //_characterSelectSetup.ToggleCharacterSelectInfo(false,0),
            _characterSelectSetup.TogglePlayerInfo(0),
            };
            await Task.WhenAll(tasks);
            for(int i = 0; i < _characterSelectController._playerCursors.Count; i++) 
            {
                CharacterSelect_Page currentPage = _characterSelectController._playerCursors[i].cursorPage;
                currentPage.ResetNamePlatePosition();
            }
            _stageSelectController.ResetStagePositionData();
            _mainMenuCamera.SetActive(false);
            SceneManager.UnloadSceneAsync("MainGame_MenuScene");
            SceneManager.LoadScene("MainGame_Arena", LoadSceneMode.Additive);
        }
    }
    public void OnApplicationQuit()
    {
        SceneManager.LoadScene("MainGame_MenuScene", LoadSceneMode.Additive);
        SceneManager.UnloadSceneAsync("MainGame_Arena");
        _arenaLoaded = false;
    }
}
