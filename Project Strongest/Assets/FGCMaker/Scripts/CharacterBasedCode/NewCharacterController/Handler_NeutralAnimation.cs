using System;
using UnityEngine;

[System.Serializable]
public class Handler_NeutralAnimation : AttackHandler_Base
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
    public override void OnInit(Character_Base curBase, Attack_BaseProperties attack = null)
    {
        // DebugMessageHandler.instance.DisplayErrorMessage(1, $"Entered Init");
    }
    public override void OnStartup(Character_Base curBase)
    {
        // DebugMessageHandler.instance.DisplayErrorMessage(1, $"Entered startup");
    }
    public override void OnStay(Character_Base curBase)
    {
        DebugMessageHandler.instance.DisplayErrorMessage(1, $"Entered stay");
    }
    public override void OnActive(Character_Base curBase)
    {
        DebugMessageHandler.instance.DisplayErrorMessage(1, $"Entered active");
    }
    public override void OnRecov(Character_Base curBase)
    {
        DebugMessageHandler.instance.DisplayErrorMessage(1, $"Entered recov");
    }
    public override void OnExit()
    {
    }
}
