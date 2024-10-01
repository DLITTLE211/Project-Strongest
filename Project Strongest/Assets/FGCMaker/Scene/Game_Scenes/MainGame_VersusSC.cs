using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MainGame_VersusSC : MainGame_SettingsController
{
    public override void SetTeleportPositions(EventSystem eventSystem)
    {
       
        _pauseMenu.SetActive(false);
        _pauseMenu.GetComponent<VersusMenu_Controller>().SetupVersusButtons(eventSystem);
        _eventSystem = eventSystem;
        _eventSystem.firstSelectedGameObject = null;

        _pauseMenu.GetComponent<VersusMenu_Controller>().SetP1MoveListInformation(mainPlayer.comboList3_0, mainPlayer.characterProfile.CharacterName);
        _pauseMenu.GetComponent<VersusMenu_Controller>().SetP2MoveListInformation(secondaryPlayer.comboList3_0, secondaryPlayer.characterProfile.CharacterName);
    }
    public override void TogglePauseMenu()
    {
        base.TogglePauseMenu();
        if (_pauseMenu.activeInHierarchy)
        {
            _eventSystem.SetSelectedGameObject(_pauseMenu.GetComponent<VersusMenu_Controller>().ReturnTopButton().gameObject);
        }
        else
        {
            _eventSystem.firstSelectedGameObject = null;
        }
    }
}
