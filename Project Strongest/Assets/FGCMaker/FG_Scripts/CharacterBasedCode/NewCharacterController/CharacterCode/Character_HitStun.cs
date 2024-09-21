using UnityEngine;
using System;
using System.Collections;

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
    void ActivateHitStun(float hitstunValue) 
    {
        if(hitStunRoutine != null) 
        {
            StopCoroutine(hitStunRoutine);
            hitStunRoutine = null;
        }
        hitStunRoutine = ApplyHitstun(hitstunValue);
        StartCoroutine(hitStunRoutine);
    }
    public void CallHitStun(float hitstunValue) 
    {
        ActivateHitStun(hitstunValue);
    }
    IEnumerator ApplyHitstun(float hitStunValue)
    {
        animSpeed = 0.05f;
        float stunTime = 0;
        float oneFrame = 1f / 60f;
        _cAnimator.SetCanRecover(true);
        while (stunTime < hitStunValue)
        {
            if (_cAnimator._base.ReturnIfPaused())
            {
                yield return new WaitForSeconds(oneFrame);
            }
            else
            {
                stunTime += oneFrame;
                yield return new WaitForSeconds(oneFrame);
            }
        }
        animSpeed = 1f;
        Messenger.Broadcast<int, string>(Events.SendReturnTime, 0, _cAnimator._base.gameObject.name);
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
