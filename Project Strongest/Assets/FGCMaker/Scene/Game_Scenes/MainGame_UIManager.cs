using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainGame_UIManager : MonoBehaviour
{
    public List<MainGame_UI> _uiObjects;
    public void SetActiveUI(GameMode _curMode)
    {
        for (int i = 0; i < _uiObjects.Count; i++)
        {
            if (_curMode == GameMode.Training) 
            {
                _uiObjects[i].gameObject.SetActive(true);
            }
            else 
            {
                if (_uiObjects[i].type != UI_Type.Training) 
                {
                    _uiObjects[i].gameObject.SetActive(true);
                }
                continue;
            }
        }
    }

}
