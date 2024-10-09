using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class HitAnimationField 
{
    public AnimationClip anim;
    public float animLength;
    public string animName;
    public int animHash;
    public HitReactionType hitReactionType;
    public HitLevel hitLevel;
    public bool isGroundedReaction;
    public bool isLowReaction;
    public Attack_KnockDown knockdownAnimType;
    public HitAnimationField(AnimationClip _anim, float _animLength, string _animName, HitReactionType _hitReactionType, HitLevel _hitLevel, bool _isGroundedReaction = true, bool _isLowReaction = false, Attack_KnockDown _knockdownAnimType = Attack_KnockDown.NONE)
    {
        anim = _anim;
        animLength = _animLength;
        animName = _animName;
        animHash = Animator.StringToHash(animName);
        hitReactionType = _hitReactionType;
        hitLevel = _hitLevel;
        isGroundedReaction = _isGroundedReaction;
        isLowReaction = _isLowReaction;
        knockdownAnimType = _knockdownAnimType;
    }
    public void DoAnimationInfoSetup()
    {
        animLength = anim.length;
        animName = anim.name;
        animHash = Animator.StringToHash(animName);
    }
}

[Serializable]
public class HitAnimationHolder
{
    public List<HitAnimationField> hitReactions;
    public List<HitAnimationField> blockReactions;
    public List<HitAnimationField> getUpReactions;
    public void Setup()
    {
        hitReactions = new List<HitAnimationField>();
        blockReactions = new List<HitAnimationField>();
        getUpReactions = new List<HitAnimationField>();
    }
}
[Serializable]
public enum HitReactionType
{
    StandardHit = 0,
    KnockdownHit = 1,
    StandardBlock = 2,
    GuardBreakBlock = 3,
    Getup = 4,
}