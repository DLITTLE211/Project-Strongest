using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;

public class Menu_BaseState : MonoBehaviour
{
    public virtual void OnEnter() 
    { }
    public virtual void OnExit()
    { }
    public virtual void OnUpdate() { }
    public virtual void Select(CharacterSelect_Cursor _currentCursor) { }
    public virtual void Cancel(CharacterSelect_Cursor _currentCursor) { }
    public virtual void CycleLeft(CharacterSelect_Cursor _currentCursor) { }
    public virtual void CycleRight(CharacterSelect_Cursor _currentCursor) { }

    public async Task DelayWithTime(int time) 
    {
        await Task.Delay(time);
    }

    public Menu_BaseState()
    {
        
    }
}
