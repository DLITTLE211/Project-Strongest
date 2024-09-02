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
    FullCounter,
    LowImmune,
    HighImmune,
}
[Serializable]
public enum HitBoxType 
{
    Low,
    High,
    Overhead,
    Anti_Air,
    Unblockable,
    CommandGrab_Ground,
    Throw,
    nullified,
    CommandGrab_Air,
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
    public CollisionType collisionType;
    public bool allowHitCheck;
    private Dictionary<HitBoxType, Color> hitboxColor = new Dictionary<HitBoxType, Color>();
    private Dictionary<HurtBoxType, Color32> hurtboxColor = new Dictionary<HurtBoxType, Color32>();
    private void Start()
    {
        allowHitCheck = false;
    }
    public void SetupHitboxDictionary()
    {
        hitboxColor = new Dictionary<HitBoxType, Color>();
        hitboxColor.Add(HitBoxType.Low, Color.green);
        hitboxColor.Add(HitBoxType.High, Color.red);
        hitboxColor.Add(HitBoxType.Overhead, Color.blue);
        hitboxColor.Add(HitBoxType.Anti_Air, Color.white);
        hitboxColor.Add(HitBoxType.Unblockable, Color.grey);
        hitboxColor.Add(HitBoxType.CommandGrab_Ground, Color.cyan);
        hitboxColor.Add(HitBoxType.Throw, Color.yellow);
        hitboxColor.Add(HitBoxType.nullified, Color.black);
        hitboxColor.Add(HitBoxType.CommandGrab_Air, Color.magenta);
    }
    public void SetupHurtboxDictionary()
    {
        hurtboxColor = new Dictionary<HurtBoxType, Color32>();
        hurtboxColor.Add(HurtBoxType.NoBlock, new Color32(255,0,170,95));
        hurtboxColor.Add(HurtBoxType.BlockHigh, new Color32(102, 222, 255, 95));
        hurtboxColor.Add(HurtBoxType.BlockLow, new Color32(197, 255, 102, 95));
        hurtboxColor.Add(HurtBoxType.ParryLow, new Color32(236, 220, 188, 95));
        hurtboxColor.Add(HurtBoxType.ParryHigh, new Color32(155, 97, 52, 95));
        hurtboxColor.Add(HurtBoxType.SoftKnockdown, new Color32(57, 207, 255, 95));
        hurtboxColor.Add(HurtBoxType.HardKnockdown, new Color32(135, 135, 135, 95));
        hurtboxColor.Add(HurtBoxType.Invincible, new Color32(255, 255, 255, 95));
        hurtboxColor.Add(HurtBoxType.Armor, new Color32(0, 0, 0, 95));
        hurtboxColor.Add(HurtBoxType.FullCounter, new Color32(188, 106, 106, 95));
    }
    public void SetHitboxState(HitBoxType _hitProperty)
    {
        if (hitboxColor.ContainsKey(_hitProperty))
        {
            SetRendererColor(hitboxColor[_hitProperty]);
            SetText($"Current HitboxType: {_hitProperty}");
        }
        //DebugMessageHandler.instance.DisplayErrorMessage(1, $"Invalid HitboxType Detected.");
    }
    public void SetHurtboxState(HurtBoxType _hurtProperty)
    {
        if(hurtboxColor.Count == 0) 
        {
            SetupHurtboxDictionary();
        }
        if (hurtboxColor.ContainsKey(_hurtProperty))
        {
            SetRendererColor(hurtboxColor[_hurtProperty]);
            SetText($"Current HitboxType: {_hurtProperty}");
            return;
        }
        //DebugMessageHandler.instance.DisplayErrorMessage(1, $"Invalid HitboxType Detected.");
    }
    public void SetHurtBoxSize( float sizeX, float sizeY, bool isHurtboxTypeBox, ColliderType collisionType = ColliderType.Trigger, Character_HurtBoxSizing sizing = null)
    {
        if (currentCollider == null)
        {
            if (collisionType == ColliderType.Trigger)
            {
                if (!isHurtboxTypeBox)
                {
                    boxColliderSpawnPoint.AddComponent<CapsuleCollider>();
                    currentCollider = boxColliderSpawnPoint.GetComponent<CapsuleCollider>();
                    currentCollider.GetComponent<CapsuleCollider>().radius = 0.25f;
                }
                else
                {
                    boxColliderSpawnPoint.AddComponent<BoxCollider>();
                    currentCollider = new Collider();
                    currentCollider = boxColliderSpawnPoint.GetComponent<BoxCollider>();
                }
                currentCollider.isTrigger = true;
            }
            else if (collisionType == ColliderType.Collision)
            {
                currentCollider.isTrigger = false;
            }
        }
        if (sizing != null)
        {
            if (!isHurtboxTypeBox)
            {
                currentCollider.GetComponent<CapsuleCollider>().center = new Vector3(0, 0, 0.5f);
                currentCollider.transform.localPosition = sizing.hurtboxPosition;
                currentCollider.transform.localScale = sizing.hurtboxSizing;
                currentCollider.GetComponent<CapsuleCollider>().radius = sizing.radius;
            }
            else
            {
                currentCollider.GetComponent<BoxCollider>().center = new Vector3(0, 0, 0.5f);
                currentCollider.transform.localPosition = sizing.hurtboxPosition;
                currentCollider.transform.localScale = sizing.hurtboxSizing;
            }
        }
        else
        {
            xSize = sizeX;
            ySize = sizeY;
            this.transform.localScale = new Vector3(sizeX, sizeY, sizeY);
        }
    }
    public void SetHitboxSize(HitBox hitboxInfo, float sizeX = 0, float sizeY = 0)
    {
        if (hitboxInfo != null)
        {
            if (hitboxInfo.currentCollider == null)
            {
                hitboxInfo.boxColliderSpawnPoint.AddComponent<BoxCollider>();
                hitboxInfo.currentCollider = boxColliderSpawnPoint.GetComponent<BoxCollider>();
                hitboxInfo.currentCollider.isTrigger = true;
            }
            else 
            {
                hitboxInfo.currentCollider.GetComponent<BoxCollider>().size = new Vector3(1, 1, 1);
                if (!hitboxInfo.currentCollider.isTrigger)
                {
                    hitboxInfo.currentCollider.isTrigger = true;
                }
            }
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
        if (_hitbox.hitboxProperties != null)
        {
            SetHitboxState(_hitbox.hitboxProperties.AttackAnims.attackType);
        }
        if (!_hitbox.gameObject.activeInHierarchy) 
        {
            _hitbox.gameObject.SetActive(true);
        }
    }
    public void PlaceHurtBox(HurtBox hurtBox, Vector3 _position, Vector3 _rotation, float _sizeX, float _sizeY, HurtBoxType _hurtType = HurtBoxType.NoBlock)
    {
        Quaternion _rotate = new Quaternion(_rotation.x, _rotation.y, 0, 0);
        hurtBox.SetHurtBoxSize(_sizeX, _sizeY,true);
        hurtBox.transform.localPosition = _position;
        hurtBox.transform.localRotation = _rotate;
        hurtBox.huBType = _hurtType;
    }
    public void ActivateHurtbox(HurtBox hurtBox)
    {
        hurtBox.gameObject.SetActive(true);
        if (hurtBox.hitResponseDictionary == null)
        {
            hurtBox.SetupHitResponseDicitonary();
        }
    }
    public void ActivateHitbox(HitBox _hitbox, HurtBox hurtbox,string attackName, HitCount _hitCount, Attack_BaseProperties property)
    {
        hurtbox.gameObject.SetActive(true);
        _hitbox.hitboxProperties = property;
        allowHitCheck = true;
       // CheckForCollision();
    }
    public void DestroyHitbox(HitBox _hitbox, HurtBox hurtbox = null)
    {
        allowHitCheck = false;
        _hitbox.SetHitColliderType(_hitbox, HitBoxType.nullified);
        if (hurtbox != null)
        {
            hurtbox.SetHurtBoxSize(0, 0, true);
        }
    }

    public void ClearAdditionalHit(HitBox _hitbox) 
    {
        DestroyHitbox(_hitbox);
    }
}
