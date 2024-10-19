using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_InputTimer : MonoBehaviour
{
    [SerializeField] protected float _frameCountTimer, _startFrameCountTimer;
    [SerializeField] private bool _checkForInput;

    public float FrameCountTimer 
    { 
        get{ return _frameCountTimer; }
        set{ _frameCountTimer = value; }
    }
    public float StartFrameCountTimer
    {
        get { return _startFrameCountTimer; }
        set { _startFrameCountTimer = value; }
    }
    public bool CheckForInput
    {
        get { return _checkForInput; }
        set { _checkForInput = value; }
    }
}
