using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_SystemButtons : MonoBehaviour
{
    public List<Character_ButtonInput> _systemButtons;

    public void Start()
    {
        _systemButtons = new List<Character_ButtonInput>();
    }
    public void AddSystemButton(Character_ButtonInput systemButton) 
    {
        _systemButtons.Add(systemButton);
    }
}
