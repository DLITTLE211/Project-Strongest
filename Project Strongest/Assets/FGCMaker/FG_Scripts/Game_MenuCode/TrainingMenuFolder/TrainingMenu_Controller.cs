using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class TrainingMenu_Controller : MonoBehaviour
{ 
    [SerializeField] private Transform buttonSpawnPoint;
    [SerializeField] private GameObject spawnedButton;
    [SerializeField] private List<TrainingButtonObject> trainingButtonDictionary = new List<TrainingButtonObject>();
    List<(string,UnityAction)> funcList;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public TrainingButtonObject ReturnTopButton() 
    {
        return trainingButtonDictionary[0];
    }
    public void SetupTrainingButtons()
    {
        funcList = new List<(string, UnityAction)>()
        {
            ("Health Settings",() => ActivateHealthMenu()),
            ("Meter Settings",() => ActivateMeterMenu()),
            ("Dummy Settings",() => ActivateDummySettingsMenu()),
            ("Info Display",() => ActivateInfoDisplayMenu()),
            ("Move Lists",() => ActivateMoveListMenu()),
            ("Character Select",() => ReturnToCharacterSelect()),
            ("Main Menu",() => ReturnToMainMenu()),
        };
        for (int i = 0; i < 7; i++)
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
                curButtonNavigation.selectOnUp = trainingButtonDictionary[trainingButtonDictionary.Count-1].menuButton;
                trainingButtonDictionary[i].menuButton.navigation = curButtonNavigation;
                continue;
            }
            if (i == trainingButtonDictionary.Count - 1)
            {
                curButtonNavigation.selectOnUp = trainingButtonDictionary[i-1].menuButton;
                curButtonNavigation.selectOnDown = trainingButtonDictionary[0].menuButton;
                trainingButtonDictionary[i].menuButton.navigation = curButtonNavigation;
                continue;
            }
            else 
            {
                curButtonNavigation.selectOnUp = trainingButtonDictionary[i-1].menuButton;
                curButtonNavigation.selectOnDown = trainingButtonDictionary[i+1].menuButton;
                trainingButtonDictionary[i].menuButton.navigation = curButtonNavigation;
                continue;
            }
        }
    }
    public void ActivateHealthMenu() 
    {
        Debug.Log("Hit Health");
    }
    public void ActivateMeterMenu()
    {
        Debug.Log("Hit Meter");
    }
    public void ActivateDummySettingsMenu()
    {
        Debug.Log("Hit DummySettings");
    }
    public void ActivateInfoDisplayMenu()
    {
        Debug.Log("Hit InfoDisplay");
    }
    public void ActivateMoveListMenu()
    {
        Debug.Log("Hit MoveList");
    }
    public void ReturnToCharacterSelect()
    {
        Debug.Log("Hit CS");
    }
    public void ReturnToMainMenu()
    {
        Debug.Log("Hit Main");
    }
}
