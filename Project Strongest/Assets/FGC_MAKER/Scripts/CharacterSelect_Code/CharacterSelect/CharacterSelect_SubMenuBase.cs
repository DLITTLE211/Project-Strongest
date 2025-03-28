using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelect_SubMenuBase : MonoBehaviour
{
    public bool allowBase;
    protected virtual void ActivateSubMenu() 
    {

    }
    protected virtual void DeactivateSubMenu()
    {

    }
    public virtual void OnUpdate()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            ActivateSubMenu();
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            DeactivateSubMenu();
        }
    }
}
