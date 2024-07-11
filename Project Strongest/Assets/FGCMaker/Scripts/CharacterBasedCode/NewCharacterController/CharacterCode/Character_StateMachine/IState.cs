using UnityEngine;

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
    protected static readonly int groundIdleHash = Animator.StringToHash("Idle");
    protected static readonly int airIdleHash = Animator.StringToHash("AirIdle");
    protected static readonly int crouchHash = Animator.StringToHash("Crouch");
    protected static readonly int sblockHash = Animator.StringToHash("Block");
    protected static readonly int cblockHash = Animator.StringToHash("CrouchBlock");
    protected static readonly int eSblockHash = Animator.StringToHash("ExitBlock");
    protected static readonly int eCblockHash = Animator.StringToHash("ExitCrouchBlock");
    protected static readonly int moveFHash = Animator.StringToHash("F_Walk");
    protected static readonly int moveBHash = Animator.StringToHash("B_Walk");
    protected static readonly int dashFHash = Animator.StringToHash("F_Dash");
    protected static readonly int dashBHash = Animator.StringToHash("B_Dash");
    protected static readonly int jumpHash = Animator.StringToHash("Jump");
    protected const float _crossFade = 0.25f;

    protected BaseState(Character_Base playerBase) 
    {
        this._base = playerBase;
        _baseAnim = playerBase._cAnimator.myAnim;
        _cAnim = playerBase._cAnimator;
        _baseForce = playerBase._cForce;
    }
    public virtual void OnEnter() {}
    public virtual void OnStay() {}
    public virtual void OnRecov() {}
    public virtual void OnExit() {}
    public virtual void OnUpdate() {}
    public virtual void OnFixedUpdate() {}
}