using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;

public class CSSubMenu_AmplifyPageController : CharacterSelect_SubMenuBase
{
    [SerializeField] private List<Amplifiers> _activeAmplifiers;
    [SerializeField] private List<AmplifyController_Object> amplifierObjects;
    private void OnEnable()
    {
        DeactivateSubMenu();
    }
    protected override void ActivateSubMenu()
    {
        for(int i = 0; i < amplifierObjects.Count; i++) 
        {
            amplifierObjects[i].gameObject.SetActive(true);
            amplifierObjects[i].Activate();
        }
    }
    protected override void DeactivateSubMenu()
    {
        for (int i = 0; i < amplifierObjects.Count; i++)
        {
            amplifierObjects[i].Deactivate();
        }
    }
    private void Update()
    {
        if (allowBase)
        {
            base.OnUpdate();
        }
    }
}
