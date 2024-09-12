using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.Events;

public class TrainingMenu_Controller : MonoBehaviour
{
    [SerializeField] private Transform buttonSpawnPoint;
    [SerializeField] private GameObject spawnedButton;
    [SerializeField] private Button trainingButtons;
    [SerializeField] private List<TrainingButtonObject> trainingButtonDictionary = new List<TrainingButtonObject>();
    // Start is called before the first frame update
    void Start()
    {
        SetupTraininButtons();
    }
    void SetupTraininButtons() 
    {
        for(int i = 0; i < 6; i++) 
        {
            GameObject buttonObject = Instantiate(spawnedButton, buttonSpawnPoint);
            TrainingButtonObject buttonScript = buttonObject.GetComponent<TrainingButtonObject>();
            switch (i) 
            {
                case 0:
                    buttonScript.Make("Health Settings", ActivateHealthMenu);
                    trainingButtonDictionary.Add(buttonScript);
                    break;
                case 1:
                    buttonScript.Make("Meter Settings", ActivateMeterMenu);
                    trainingButtonDictionary.Add(buttonScript);
                    break;
                case 2:
                    buttonScript.Make("Dummy Controls", ActivateDummySettingsMenu);
                    trainingButtonDictionary.Add(buttonScript);
                    break;
                case 3:
                    buttonScript.Make("Info Display", ActivateInfoDisplayMenu);
                    trainingButtonDictionary.Add(buttonScript);
                    break;
                case 4:
                    buttonScript.Make("Move Lists", ActivateMoveListMenu);
                    trainingButtonDictionary.Add(buttonScript);
                    break;
                case 5:
                    buttonScript.Make("Character Select", ReturnToCharacterSelect);
                    trainingButtonDictionary.Add(buttonScript);
                    break;
                case 6:
                    buttonScript.Make("Main Menu", ReturnToMainMenu);
                    trainingButtonDictionary.Add(buttonScript);
                    break;
            }
        }
    }
    void ActivateHealthMenu() 
    {

    }
    void ActivateMeterMenu()
    {

    }
    void ActivateDummySettingsMenu()
    {

    }
    void ActivateInfoDisplayMenu()
    {

    }
    void ActivateMoveListMenu()
    {

    }
    void ReturnToCharacterSelect()
    {

    }
    void ReturnToMainMenu()
    {

    }
}
