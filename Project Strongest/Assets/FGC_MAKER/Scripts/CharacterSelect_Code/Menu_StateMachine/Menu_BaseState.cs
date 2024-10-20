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

    public async Task DelayWithTime(int time) 
    {
        await Task.Delay(time);
    }

    public Menu_BaseState()
    {
        
    }
}
