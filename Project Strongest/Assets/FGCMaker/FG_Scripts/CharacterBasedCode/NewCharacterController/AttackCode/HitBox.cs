using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class HitBox : CollisionDetection
{
    public HitBoxType HBType;
    public List<Affliction> attackAffliction;
    public Attack_BaseProperties hitboxProperties;
    string layerCheck;
    // Start is called before the first frame update

    void Start()
    {
        SetHitboxSize(this, xSize, ySize);
    }
    public void SetLayerCheck(int ID) 
    {
        layerCheck = $"Player{ID}";
    }
    public void SetHitBoxProperties(Attack_BaseProperties newProperty)
    {
        hitboxProperties = newProperty;
    }
    private void Update()
    {
        if (HBType != HitBoxType.nullified && allowHitCheck)
        {
            CheckForCollision();
        }
    }
    void CheckForCollision()
    {
        Collider[] cols = Physics.OverlapBox(
            currentCollider.bounds.center,
            currentCollider.bounds.extents,
            currentCollider.transform.rotation,
            LayerMask.GetMask(layerCheck));

        foreach (Collider c in cols)
        {
            if (c.isTrigger == false)
            {
                continue;
            }
            if (c.transform.root != transform.root)
            {
                Transform target = c.transform.root;
                if (HBType != HitBoxType.nullified)
                {
                    hitboxProperties.hitLanded = true;
                    SendHitStateAndHurtBox(this, c.GetComponentInParent<HurtBox>(),target, () => ClearAdditionalHit(this));
                    allowHitCheck = false;
                }

                DebugMessageHandler.instance.DisplayErrorMessage(3, c.name);
                break;
            }
            else { continue; }
        }
    }
    public void DestroySelf() 
    {
        SetHitColliderType(this, HitBoxType.nullified);
    }
    public void SendHitStateAndHurtBox(HitBox thisHitbox,HurtBox hitHurtbox,Transform target,Callback endFunc)
    {
        hitHurtbox.ReceieveHitBox(thisHitbox, target, endFunc);
        //target.GetComponentInChildren<HurtBox>().ReceieveHitBox(thisHitbox, target, endFunc);
    }
    private void OnTriggerEnter(Collider other)
    {
        
        Collider[] cols = Physics.OverlapBox
               (this.currentCollider.bounds.center,
               this.currentCollider.bounds.extents,
               this.currentCollider.transform.rotation);
        foreach (Collider c in cols)
        {
            if (c.transform.root == this.gameObject.transform.root)
            {
                continue;
            }

            if (c.transform.root != this.gameObject.transform.root)
            {
                if (other.gameObject.layer == 7)
                {
                    Transform target = c.transform.GetComponentInParent<Character_Base>().gameObject.transform;
                    if (this.gameObject.GetComponent<LaunchController>())
                    {
                        Debug.Log("Hit Other Player");
                        this.GetComponent<LaunchController>().SendHitTarget(target, c.GetComponentInParent<HurtBox>());
                    }
                }
                if (c.transform.gameObject.layer == 7)
                {
                    Debug.LogWarning("Reserve Check Hit");
                }
            }
        }
    }
}
