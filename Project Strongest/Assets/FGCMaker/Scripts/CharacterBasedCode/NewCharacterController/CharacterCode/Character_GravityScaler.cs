using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_GravityScaler : MonoBehaviour
{
    [SerializeField] private Character_Base _base;
    private Rigidbody _rb => _base.myRb;
    [SerializeField,Range(0f,15f)] private float curGravity, gravity;
    int hitCountScaler => _base._cComboCounter.ReturnCurrentComboCount();
    [SerializeField] bool isFrozen;
    private void Start()
    {
        gravity = 5f;
        isFrozen = false;
    }

    public void HandleGravityFreeze(bool state)
    {
        if (state)
        {
            if (!isFrozen)
            {
                isFrozen = true;
                gravity = 0f;
            }
        }
        else
        {
            if (isFrozen)
            {
                isFrozen = false;
                gravity = 5f;
            }
        }
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
            gravity = 0;
        }
    }

    public void UpdateGravityScaleOnHit(float hitStunValue) 
    {
        curGravity += (hitStunValue/ hitCountScaler);
    }
}
