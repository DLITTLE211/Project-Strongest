using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Threading.Tasks;

public class CharacterSelect_LoadArena : MonoBehaviour
{
    [SerializeField] private GameObject _mainMenuCamera;
    [SerializeField] private CharacterSelect_Setup _characterSelectSetup;
    private bool _arenaLoaded;
    public static ChosenCharacter leftPlayerChosenProfile, rightPlayerChosenProfile;
    public static Stage_StageAsset chosenStage;
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

            leftPlayerChosenProfile = _characterSelectSetup.GetLeftPlayerProfile();
            rightPlayerChosenProfile = _characterSelectSetup.GetRightPlayerProfile();
            chosenStage = _characterSelectSetup.GetChosenStage();
            Task[] tasks = new Task[]
            {
            _characterSelectSetup.DisableCharacterCursors(),
            _characterSelectSetup.ToggleStageSelectState(false),
            _characterSelectSetup.ToggleCharacterSelectInfo(false,0),
            _characterSelectSetup.TogglePlayerInfo(0),
            };
            await Task.WhenAll(tasks);
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
