using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public abstract class AttackHandler_Base 
{
    public Animator playerAnim;
    public AnimationClip animClip;
    public string animName;
    public float animLength;

    public abstract void OnInit(Character_Base curBase, Attack_BaseProperties newAttackProperties = null);
    public abstract void OnStartup(Character_Base curBase);
    public abstract void OnActive(Character_Base curBase);
    public abstract void OnStay(Character_Base curBase);
    public abstract void OnRecov(Character_Base curBase);
    public abstract void OnExit();
}
[Serializable]
public class ResponseAnim_Base
{
    public AnimationClip animClip;
    public string animName;
    public float animLength;
    public List<int> actionableHitPoint;
    public List<float> actionableHitPointInFrames;
    public List<float> timeDifference;
    public HitReactionType reaction;
    public void SetCurrentAnim() 
    {
        animLength = animClip.length;
        GetActionableHitPointInFrames();
    }
    public void GetActionableHitPointInFrames()
    {
        actionableHitPointInFrames = new List<float>();
        timeDifference = new List<float>();
        for (int i = 0; i < actionableHitPoint.Count; i++) 
        {
            actionableHitPointInFrames.Add((actionableHitPoint[i] * (1f / animClip.frameRate)));
            timeDifference.Add((animLength - (actionableHitPoint[i] * (1f / animClip.frameRate))));
        }
    }
    public void PlayAnimation(Character_Animator anim) 
    {
        anim.PlayNextAnimation(Animator.StringToHash(animName), 0f);
    }
    public void PlayAnimation(Character_Animator anim, string animName)
    {
        anim.PlayNextAnimation(Animator.StringToHash(animName), 0f);
    }
}
