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
    protected static readonly int crouchHash = Animator.StringToHash("Crouch");
    protected static readonly int sblockHash = Animator.StringToHash("Standing_Block");
    protected static readonly int cblockHash = Animator.StringToHash("Crouching_Block");
    //protected static readonly int eSblockHash = Animator.StringToHash("ExitBlock");
    //protected static readonly int eCblockHash = Animator.StringToHash("ExitCrouchBlock");
    protected static readonly int moveFHash = Animator.StringToHash("Walk_Forward");
    protected static readonly int moveBHash = Animator.StringToHash("Walk_Backward");
    protected static readonly int dashFHash = Animator.StringToHash("Dash_Forward");
    protected static readonly int dashBHash = Animator.StringToHash("Dash_Backward");
    protected static readonly int jumpHash = Animator.StringToHash("Jump_Neutral");
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
        //SetAnimationNames();
    }
    void SetAnimationNames() 
    {
        allAnimationNames = new List<string>();
        for (int i = 0; i < _base.characterProfile.AllCharacterAnimations.Count; i++) 
        {
            allAnimationNames.Add(_base.characterProfile.AllCharacterAnimations[i].name);
        }
    }
    public virtual void OnEnter() {}
    public virtual void OnStay() {}
    public virtual void OnRecov() {}
    public virtual void OnExit() {}
    public virtual void OnUpdate() {}
    public virtual void OnFixedUpdate() {}
}