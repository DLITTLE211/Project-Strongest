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
            if (_uiObjects[i].type == UI_Type.General)
            {
                _uiObjects[i].gameObject.SetActive(true);
                continue;
            }
            switch (_curMode)
            {
                case GameMode.Training:
                    if (_uiObjects[i].type == UI_Type.Training)
                    {
                        _uiObjects[i].gameObject.SetActive(true);
                        continue;
                    }
                    else
                    {
                        _uiObjects[i].gameObject.SetActive(false);
                        continue;
                    }
                    break;
                case GameMode.Versus:
                    if (_uiObjects[i].type == UI_Type.Versus)
                    {
                        _uiObjects[i].gameObject.SetActive(true);
                        continue;
                    }
                    else
                    {
                        _uiObjects[i].gameObject.SetActive(false);
                        continue;
                    }
                    break;
            }
        }
    }
}
