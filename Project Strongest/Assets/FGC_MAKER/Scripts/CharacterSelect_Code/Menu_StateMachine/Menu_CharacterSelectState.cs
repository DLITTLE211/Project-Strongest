using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class Menu_CharacterSelectState : Menu_BaseState
{
    [SerializeField] private CharacterSelect_Setup _characterSelect;
    public override void OnEnter()
    {
        _characterSelect.CallCharacterSelectObject();
        _characterSelect.SetCharacterSelectObjects();
    }
    public override void OnExit()
    {

    }
    public override void OnUpdate()
    {

    }
}
