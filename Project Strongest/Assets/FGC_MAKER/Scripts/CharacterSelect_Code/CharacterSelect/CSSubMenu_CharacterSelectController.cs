using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSSubMenu_CharacterSelectController : CharacterSelect_SubMenuBase
{
    [SerializeField] private List<Character_Profile> _activeProfiles;
    protected override void ActivateSubMenu()
    {

    }
    protected override void DeactivateSubMenu()
    {

    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C)) 
        {
            ActivateSubMenu();
        }
    }

}
