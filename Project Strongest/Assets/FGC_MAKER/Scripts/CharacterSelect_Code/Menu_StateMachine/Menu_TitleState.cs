using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class Menu_TitleState : Menu_BaseState
{
    [SerializeField] private GameObject _titleScreenObject;
    public override void OnEnter()
    {
        SetTitleScreenState(true);
    }
    public void SetTitleScreenState(bool state) 
    {
        _titleScreenObject.SetActive(state);
    }
    public override void OnExit()
    {
        SetTitleScreenState(false);
    }
    public override void OnUpdate() 
    {

    }
}
