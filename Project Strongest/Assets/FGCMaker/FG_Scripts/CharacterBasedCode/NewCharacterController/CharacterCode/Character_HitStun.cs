using UnityEngine;
using System;
using System.Collections;
using FightingGame_FrameData;

public class Character_HitStun : MonoBehaviour
{
    [SerializeField] private Character_Animator _cAnimator;
    public float animSpeed;
    [SerializeField] bool isFrozen;
    [SerializeField] private IEnumerator hitStunRoutine;
    void Start()
    {
        isFrozen = false;
        SetStartCharacterAnimSpeed();
    }
    public void SetAnimator(Character_Animator myAnim)
    {
        _cAnimator = myAnim;
    }
    public float GetCurrentAnimSpeed() { return animSpeed; }
    public void HandleAnimatorFreeze(bool state, float speed = 1f)
    {
        if (isFrozen != state)
        {
            isFrozen = state;
        }
        SetTargetFreezeState(speed);
    }
    void SetTargetFreezeState(float speed)
    {
        _cAnimator.myAnim.speed = speed;
        _cAnimator.shadowAnim.speed = speed;
        animSpeed = speed;
    }
    public bool IsFrozen() 
    {
        return isFrozen;
    }
    void SetStartCharacterAnimSpeed() 
    {
        animSpeed = 1;
    }
    public void KillStunRoutine() 
    {
        if (hitStunRoutine != null)
        {
            StopCoroutine(hitStunRoutine);
            hitStunRoutine = null;
        }
    }
    void ActivateHitStun(float hitstunValue) 
    {
        KillStunRoutine();
        hitStunRoutine = ApplyHitstun(hitstunValue);
        StartCoroutine(hitStunRoutine);
    }
    public void CallHitStun(float hitstunValue) 
    {
        ActivateHitStun(hitstunValue);
    }
    IEnumerator ApplyHitstun(float hitStunValue)
    {
        animSpeed = 0.45f;
        float stunTime = 0;
        _cAnimator.SetCanRecover(true);
        while (stunTime < hitStunValue)
        {
            if (_cAnimator._base.ReturnIfPaused())
            {
                yield return new WaitForSeconds(Base_FrameCode.ONE_FRAME);
            }
            else
            {
                stunTime += Base_FrameCode.ONE_FRAME;
                yield return new WaitForSeconds(Base_FrameCode.ONE_FRAME);
            }
        }
        animSpeed = 1f;
        Messenger.Broadcast<int, string>(Events.SendReturnTime, 0, _cAnimator._base.gameObject.name);
    }
    public void EndHitStun() 
    {
        KillStunRoutine(); 
        animSpeed = 1f;
    }

    private void Update()
    {
        if (_cAnimator != null)
        {
            _cAnimator.myAnim.speed = animSpeed;
            _cAnimator.shadowAnim.speed = animSpeed;
        }
    }
}
