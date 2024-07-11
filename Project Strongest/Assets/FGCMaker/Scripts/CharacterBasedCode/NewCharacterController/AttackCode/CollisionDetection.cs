using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
[Serializable]
public class CollisionType 
{
}
[Serializable]
public enum HurtBoxType
{
    NoBlock,
    BlockLow,
    BlockHigh,
    ParryLow,
    ParryHigh,
    SoftKnockdown,
    HardKnockdown,
    Invincible,
    Armor,
    FullParry
}
[Serializable]
public enum HitBoxType 
{
    Low,
    High,
    Overhead,
    Anti_Air,
    Unblockable,
    CommandGrab,
    Throw,
    nullified,
}
[Serializable]
public enum ColliderType
{
    Trigger,
    Collision,
}
public class CollisionDetection : MonoBehaviour
{
    public float xSize, ySize;
    public GameObject boxColliderSpawnPoint;
    public Collider currentCollider;
    public SpriteRenderer cRenderer;
    public TMP_Text testText;
    private HitBox lastHitbox;
    public CollisionType collisionType;

    public void SetHurtBoxSize(float sizeX = 0, float sizeY = 0, ColliderType collisionType = ColliderType.Trigger)
    {
        if (currentCollider == null)
        {
            if (collisionType == ColliderType.Trigger)
            {
                boxColliderSpawnPoint.AddComponent<CapsuleCollider>();
                currentCollider = boxColliderSpawnPoint.GetComponent<CapsuleCollider>();
                currentCollider.GetComponent<CapsuleCollider>().radius = 0.25f;
                currentCollider.isTrigger = true;
            }
            else if (collisionType == ColliderType.Collision)
            {
                currentCollider.isTrigger = false;
            }
        }
        xSize = sizeX;
        ySize = sizeY;
        this.transform.localScale = new Vector3(sizeX, sizeY, sizeY);
    }
    public void SetHitboxSize(HitBox hitboxInfo, float sizeX = 0, float sizeY = 0)
    {
        if (hitboxInfo.currentCollider == null)
        {
            hitboxInfo.boxColliderSpawnPoint.AddComponent<SphereCollider>();
            hitboxInfo.currentCollider = boxColliderSpawnPoint.GetComponent<SphereCollider>();
            hitboxInfo.currentCollider.isTrigger = true;
        }
        xSize = sizeX;
        ySize = sizeY;
        this.transform.localScale = new Vector3(sizeX, sizeY, sizeY);
    }
    public void SetRendererColor(Color32 newColor) 
    {
        cRenderer.color = newColor;
    }

    public void SetText(string message = null) 
    {
        if(message == null) 
        {
            message = "";
            testText.text = message;
           return;
        }
        testText.text = message;
    }
    public void SetHitColliderType(HitBox collision, HitBoxType _newType) 
    {
        collision.HBType = _newType;
        collision.SetHitboxSize(collision);
    }

    public void PlaceHitBox(HitBox _hitbox, Vector3 _position, Vector3 _rotation,float _sizeX,float _sizeY, HitBoxType _hitType) 
    {
        Quaternion _rotate = new Quaternion(_rotation.x, _rotation.y, 0,0);
        
        _hitbox.SetHitboxSize(_hitbox, _sizeX, _sizeY);
        _hitbox.transform.localPosition = _position;
        _hitbox.transform.localRotation = _rotate;

        _hitbox.HBType = _hitType;
        lastHitbox = _hitbox;
    }
    public void PlaceHurtBox(HurtBox hurtBox, Vector3 _position, Vector3 _rotation, float _sizeX, float _sizeY, HurtBoxType _hurtType = HurtBoxType.NoBlock)
    {
        Quaternion _rotate = new Quaternion(_rotation.x, _rotation.y, 0, 0);
        hurtBox.SetHurtBoxSize(_sizeX, _sizeY);
        hurtBox.transform.localPosition = _position;
        hurtBox.transform.localRotation = _rotate;

        hurtBox.huBType = _hurtType;
    }
    public void ActivateHurtbox(HurtBox hurtBox)
    {
        hurtBox.gameObject.SetActive(true);
    }
    public void ActivateHitbox(HitBox _hitbox, HurtBox hurtbox,string attackName, HitCount _hitCount)
    {
        hurtbox.gameObject.SetActive(true);
        lastHitbox.gameObject.SetActive(true);
        CheckForCollision(_hitbox);
    }
    public void DestroyHitbox(HitBox _hitbox, HurtBox hurtbox)
    {
        CheckForCollision(_hitbox);
        lastHitbox.SetHitColliderType(_hitbox, HitBoxType.nullified);
        hurtbox.gameObject.SetActive(false);
        _hitbox = lastHitbox;
    }
    void CheckForCollision(HitBox _hitbox) 
    {
        Collider[] cols =  Physics.OverlapBox(
            _hitbox.currentCollider.bounds.center,
            _hitbox.currentCollider.bounds.extents,
            _hitbox.currentCollider.transform.rotation,
            LayerMask.GetMask("Players"));

        foreach (Collider c in cols)
        {
            if (c.isTrigger == false)
            {
                continue;
            }
            if (c.transform.root != transform.root)
            {
                Transform target = c.transform.root;
                if (_hitbox.HBType != HitBoxType.nullified)
                {
                    _hitbox.SendHitStateAndHurtBox(_hitbox, _hitbox.hitboxProperties.AttackAnims._hitCount, target);
                    _hitbox.SetHitColliderType(_hitbox, HitBoxType.nullified);
                }

                DebugMessageHandler.instance.DisplayErrorMessage(3, c.name);
                break;
            }
            else { continue; }
        }
    }
}
