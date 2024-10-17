using UnityEngine;
using System.Collections.Generic;

public interface IState 
{
    void OnEnter();
    void OnStay();
    void OnRecov();
    void OnExit();
    void OnUpdate();
    void OnFixedUpdate();
}

public abstract class BaseState : IState 
{
    protected readonly Character_Base _base;
    protected readonly Character_Animator _cAnim;
    protected readonly Animator _baseAnim;
    protected readonly Character_Force _baseForce;
    public List<string> allAnimationNames;
    protected static readonly int groundIdleHash = Animator.StringToHash("Idle");
    protected static readonly int secondaryIdleHash = Animator.StringToHash("Idle_2");
    protected static readonly int crouchHash = Animator.StringToHash("Crouch");
    protected static readonly int secondaryCrouchHash = Animator.StringToHash("Crouch_2");
    protected static readonly int sblockHash = Animator.StringToHash("Standing_Block");
    protected static readonly int cblockHash = Animator.StringToHash("Crouching_Block");
    protected static readonly int moveFHash = Animator.StringToHash("Walk_Forward");
    protected static readonly int moveBHash = Animator.StringToHash("Walk_Backward");
    protected const float _crossFade = 0.25f;

    protected BaseState(Character_Base playerBase) 
    {
        this._base = playerBase;
        playerBase.gameObject.SetActive(true);
        _baseAnim = playerBase._cAnimator.myAnim;
        _baseAnim.gameObject.SetActive(true);
        _cAnim = playerBase._cAnimator;
        _cAnim.gameObject.SetActive(true);
        _baseForce = playerBase._cForce;
        _baseForce.gameObject.SetActive(true);
    }
    void SetAnimationNames() 
    {
        allAnimationNames = new List<string>();
        for (int i = 0; i < _base.characterProfile.AllCharacterAnimations.Count; i++) 
        {
            allAnimationNames.Add(_base.characterProfile.AllCharacterAnimations[i].name);
        }
    }
    public void ForceIdleAnimPlay()
    {
        _cAnim.PlayNextAnimation(groundIdleHash,0);
    }
    public virtual void OnEnter() {}
    public virtual void OnStay() {}
    public virtual void OnRecov() {}
    public virtual void OnExit() {}
    public virtual void OnUpdate() 
    {
        GameManager.instance.sideManager.CheckPlayerPositions();
    }
    public virtual void OnFixedUpdate() {}
}