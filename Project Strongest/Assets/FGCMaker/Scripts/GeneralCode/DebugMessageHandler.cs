using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;
public class DebugMessageHandler : MonoBehaviour
{
    public static DebugMessageHandler instance;
    public bool filterLog,filterWarning,filterError;
    private void Awake()
    {
        instance = this;
    }
    /// <summary>
    /// Handles any and all debug messages in game
    /// </summary>
    /// <param name="type"></param>
    /// <param name="message"></param>
    public void DisplayErrorMessage(int type,string message) 
    {
        switch (type) 
        {
            case 1:
                if (!filterLog)
                {
                    Debug.Log($"{message}");
                }
                break;
            case 2:
                if (!filterWarning)
                {
                    Debug.LogWarning($"{message}");
                }
                break;
            case 3:
                if (!filterError)
                {
                    Debug.LogError($"{message}");
                }
                break;
        }
    }
}
