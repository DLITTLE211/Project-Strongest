using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class TrainingButtonObject : MonoBehaviour
{
        public Button menuButton;
        public string name;
        public TMP_Text buttonFuncName;
        public void Make(string _name, UnityAction func)
        {
            name = _name;
            buttonFuncName.text = name;
            UnityAction buttonAction = func;
            menuButton.onClick.AddListener(buttonAction);
        }
    public void OnApplicationQuit()
    {
        menuButton.onClick.RemoveAllListeners();
    }
}
