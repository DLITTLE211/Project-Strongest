using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class Menu_RoundSelectState : Menu_BaseState
{
    [SerializeField] private CharacterSelect_Setup _characterSelect;
    [SerializeField] private CharacterSelect_Cursor _player1_Cursor, _player2_Cursor;
    public override void OnEnter()
    {

    }
    public override void OnExit()
    {

    }
    public override void OnUpdate()
    {

        _characterSelect.CursorController(_player1_Cursor);
        _characterSelect.CursorController(_player2_Cursor);
    }
    public override void Select(CharacterSelect_Cursor _currentCursor) 
    {

    }
    public override void Cancel(CharacterSelect_Cursor _currentCursor) 
    {

    }
}
