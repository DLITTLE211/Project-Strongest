using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class PlayerAnimation 
{
    public Animator playerAnim;
    public AnimationClip animClip;
    public string animName;
    public float animLength;
    public enum AnimState 
    {
        NotPlaying= 0,
        Startup = 1,
        Active = 2,
        Recovery = 3,
    }
    public AnimState neutralAnimState;


    public abstract void OnStartup();
    public abstract void OnActive();
    public abstract void OnStay();
    public abstract void OnRecov();
    public abstract void OnExit();
}
[Serializable]
public class NeutralAnim : PlayerAnimation
{
    public void SetNeutralAnim(Animator _playerAnim = null)
    {
        playerAnim = _playerAnim;
        try
        {
            animName = animClip.name;
            animLength = animClip.length;
        }
        catch (Exception e)
        {
            return;
        }
    }
    public override void OnStartup() 
    {
       // DebugMessageHandler.instance.DisplayErrorMessage(1, $"Entered startup");
    }
    public override void OnStay()
    {
        DebugMessageHandler.instance.DisplayErrorMessage(1, $"Entered stay");
    }
    public override void OnActive()
    {
        DebugMessageHandler.instance.DisplayErrorMessage(1, $"Entered active");
    }
    public override void OnRecov()
    {
        if (animClip.isLooping)
        {
            OnStartup();
        }
        DebugMessageHandler.instance.DisplayErrorMessage(1, $"Entered recov");
    }
    public override void OnExit()
    {}
}