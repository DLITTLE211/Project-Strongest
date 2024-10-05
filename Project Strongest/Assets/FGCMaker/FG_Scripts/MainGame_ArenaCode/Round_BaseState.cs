using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;

[Serializable]
public class Round_BaseState 
{
    public async virtual void OnEnter(Task[] task = null){}
    public async virtual void OnExit(Task[] task = null) {}
}
