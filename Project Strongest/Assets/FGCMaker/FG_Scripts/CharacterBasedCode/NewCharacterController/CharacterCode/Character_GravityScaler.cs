using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_GravityScaler : MonoBehaviour
{
    [SerializeField] private Character_Base _base;
    [SerializeField, Range(0f, 15f)] private float curGravity, gravity;
    private Rigidbody _rb => _base.myRb;
    [SerializeField] private int hitCountScaler => _base._cComboCounter.ReturnCurrentComboCount();
    [SerializeField] bool isFrozen;
    private void Start()
    {
        gravity = 10f;
        isFrozen = false;
    }
    

    public void HandleGravityFreeze(bool state)
    {
        if (isFrozen != state) 
        {
            isFrozen = state;
        }
        SetGravity(isFrozen);
    }
    void SetGravity(bool gravityState) 
    {
        gravity = gravityState == true ? 0f : 10f;
    }
    public float ReturnCurrentGravity() 
    {
        return curGravity;
    }

    private void Update()
    {
        if (!isFrozen)
        {
            if ((int)_rb.velocity.y == 0)
            {
                if (_base._cHurtBox.IsGrounded())
                {
                    curGravity = 0;
                }
                else
                {
                    curGravity = gravity;
                }
            }
            else
            {
                if (_rb.velocity.y < 0)
                {

                    curGravity = gravity + _rb.velocity.y;
                }
                else
                {
                    curGravity = gravity - _rb.velocity.y;
                }
            }
            _rb.AddForce(transform.up * -curGravity, ForceMode.Acceleration);
        }
        else 
        {
            curGravity = 0;
        }
    }

    public void UpdateGravityScaleOnHit(float hitStunValue) 
    {
        float value = hitStunValue * _base._cDamageCalculator.GetCurrentScaling();
        curGravity += value;
    }
}
