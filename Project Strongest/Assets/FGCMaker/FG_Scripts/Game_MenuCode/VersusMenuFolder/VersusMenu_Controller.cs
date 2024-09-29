using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class VersusMenu_Controller : MonoBehaviour
{
    [SerializeField] private Transform buttonSpawnPoint;
    [SerializeField] private GameObject spawnedButton;
    [SerializeField] private List<TrainingButtonObject> trainingButtonDictionary = new List<TrainingButtonObject>();
    List<(string, UnityAction)> funcList;
    [SerializeField] private List<UI_SubMenuBase> trainingMenuObject;
    public MenuType chosenMenuType;
    [SerializeField] private EventSystem eventSystem;
    [SerializeField] private UI_HealthSettings_SubMenu healthReturnValues;
    [SerializeField] private UI_MeterSetting_SubMenu meterReturnValues;
    [SerializeField] private UI_DummyController_SubMenu EnemySettingReturnValues;
    [SerializeField] private UI_Movelist_SubMenu movelistMenu;
    [SerializeField] private UI_InfoDisplay_SubMenu InfoDisplayReturnValues;
    public TrainingButtonObject ReturnTopButton()
    {
        return trainingButtonDictionary[0];
    }
    private void Start()
    {
        DeactivateMenuOnStart();
    }

    public void SetP1MoveListInformation(Character_MoveList movelist, string characterName)
    {
        movelistMenu.SetPlayer1MoveListData(movelist, characterName);
    }
    public void SetP2MoveListInformation(Character_MoveList movelist, string characterName)
    {
        movelistMenu.SetPlayer2MoveListData(movelist, characterName);
    }
    public void SetupTrainingButtons(EventSystem _eventSystem)
    {
        eventSystem = _eventSystem;
        funcList = new List<(string, UnityAction)>()
        {
            ("Move Lists",() => ActivateMoveListMenu()),
            ("Character Select",() => ReturnToCharacterSelect()),
            ("Main Menu",() => ReturnToMainMenu()),
        };
        for (int i = 0; i < funcList.Count; i++)
        {
            GameObject buttonObject = Instantiate(spawnedButton, buttonSpawnPoint);
            TrainingButtonObject buttonScript = buttonObject.GetComponent<TrainingButtonObject>();
            buttonScript.Make(funcList[i].Item1, funcList[i].Item2);
            trainingButtonDictionary.Add(buttonScript);
        }
        for (int i = 0; i < trainingButtonDictionary.Count; i++)
        {
            Navigation curButtonNavigation = new Navigation();
            curButtonNavigation.mode = Navigation.Mode.Explicit;
            if (i == 0)
            {
                curButtonNavigation.selectOnDown = trainingButtonDictionary[1].menuButton;
                curButtonNavigation.selectOnUp = trainingButtonDictionary[trainingButtonDictionary.Count - 1].menuButton;
                trainingButtonDictionary[i].menuButton.navigation = curButtonNavigation;
                continue;
            }
            if (i == trainingButtonDictionary.Count - 1)
            {
                curButtonNavigation.selectOnUp = trainingButtonDictionary[i - 1].menuButton;
                curButtonNavigation.selectOnDown = trainingButtonDictionary[0].menuButton;
                trainingButtonDictionary[i].menuButton.navigation = curButtonNavigation;
                continue;
            }
            else
            {
                curButtonNavigation.selectOnUp = trainingButtonDictionary[i - 1].menuButton;
                curButtonNavigation.selectOnDown = trainingButtonDictionary[i + 1].menuButton;
                trainingButtonDictionary[i].menuButton.navigation = curButtonNavigation;
                continue;
            }
        }
    }
    public void ActivateMoveListMenu()
    {
        chosenMenuType = MenuType.MoveListDisplay;
        ActivateMenu();
        Debug.Log("Hit MoveList");
    }
    void ActivateMenu()
    {
        for (int i = 0; i < trainingMenuObject.Count; i++)
        {
            if (trainingMenuObject[i]._menuType == chosenMenuType)
            {
                trainingMenuObject[i].gameObject.SetActive(true);
                trainingMenuObject[i].EnableMenu(SetActiveButton);
                break;
            }
            continue;
        }
    }
    public void DeactivateMenuOnStart()
    {
        for (int i = 0; i < trainingMenuObject.Count; i++)
        {
            trainingMenuObject[i].DisableMenu(() => trainingMenuObject[i].gameObject.SetActive(false));
        }
    }
    public void DeactivateMenu()
    {
        for (int i = 0; i < trainingMenuObject.Count; i++)
        {
            if (trainingMenuObject[i]._menuType == chosenMenuType)
            {
                trainingMenuObject[i].DisableMenu(() => trainingMenuObject[i].gameObject.SetActive(false));
                break;
            }
            continue;
        }
        eventSystem.SetSelectedGameObject(trainingButtonDictionary[0].gameObject);
    }

    void SetActiveButton(GameObject button)
    {
        eventSystem.SetSelectedGameObject((GameObject)button);
    }
    public void ReturnToCharacterSelect()
    {
        Menu_Manager.instance.DelayChosenPage(() => Menu_Manager.instance.TrainingSelected());
        GameManager.instance.UnloadFightingArena();
        Debug.Log("Hit CS");
    }
    public void ReturnToMainMenu()
    {
        Menu_Manager.instance.DelayChosenPage(() => Menu_Manager.instance.ActivateMainMenuPage());
        GameManager.instance.UnloadFightingArena();
        Debug.Log("Hit Main");
    }
}
