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
    // Start is called before the first frame update
    void Start()
    {
        SetHitboxSize(this, xSize, ySize);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        CheckAttackState();
    }

    public void CheckAttackState() 
    {
        switch (HBType)
        {
            case HitBoxType.Low:
                SetRendererColor(new Color32(0,255,0,255)); //Green
                break;
            case HitBoxType.High:
                SetRendererColor(new Color32(255, 0, 0, 255)); //Red
                break;
            case HitBoxType.Overhead:
                SetRendererColor(new Color32(255, 0, 255, 255)); //Purple
                break;
            case HitBoxType.Anti_Air:
                SetRendererColor(new Color32(149,255, 8, 255)); //Purple
                break;
            case HitBoxType.Unblockable:
                SetRendererColor(new Color32(0, 135, 135, 255)); //IDK
                break;
            case HitBoxType.CommandGrab:
                SetRendererColor(new Color32(0, 0, 255, 255)); //Blue
                break;
            case HitBoxType.Throw:
                SetRendererColor(new Color32(255, 255, 0, 255)); //Yellow
                break;
            case HitBoxType.nullified:
                SetRendererColor(new Color32(0, 0, 0, 15)); //Black
                SetHitboxSize(this);
                SetText();
                return;
            default:
                DebugMessageHandler.instance.DisplayErrorMessage(1, $"Invalid HitboxType Detected.");
                break;
        }
        SetText($"Current HitboxType: {HBType}");
    }
    void CheckEffects()
    {
        for(int i = 0; i < attackAffliction.Count; i++)
        {
            Messenger.Broadcast<Affliction>(Events.SendAfflictionToTarget,attackAffliction[i]);
        }
    }
    public void SendHitStateAndHurtBox(HitBox thisHitbox,HitCount _hitCount, Transform target)
    {
        target.GetComponentInChildren<HurtBox>().ReceieveHitBox(thisHitbox, _hitCount, target);
        //Messenger.Broadcast<HitBox, string, HitCount>(Events.SendHitToHurtBox, thisHitbox, attackName, _hitCount);
        CheckEffects();
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
                        this.GetComponent<LaunchController>().SendHitTarget(target);
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
