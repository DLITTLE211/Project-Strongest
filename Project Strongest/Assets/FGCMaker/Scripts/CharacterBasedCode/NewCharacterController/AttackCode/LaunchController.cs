using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LaunchController : MonoBehaviour
{
    public MeshRenderer sphereMesh;
    public Transform thisTransform;
    public Rigidbody _myRB;
    public float travelSpeed;
    [SerializeField] private Attack_BaseProperties projectileProperty;
    public Attack_BaseProperties Property { get { return projectileProperty; } set { projectileProperty = value; } }
    public HitBox projectileHitbox;
    private void Start()
    {
        thisTransform = this.gameObject.transform;
        projectileHitbox = GetComponent<HitBox>();
    }

    public void Launch(Character_Base _Base, Vector2 size, HitBoxType hbType)
    {
        projectileHitbox.HBType = hbType;
        projectileHitbox.SetHitboxSize(projectileHitbox, size.x, size.y);
        if (_Base.pSide.thisPosition._directionFacing == Character_Face_Direction.FacingRight)
        {
            travelSpeed = travelSpeed * 1;
        }
        else 
        {

            travelSpeed = travelSpeed * -1;
        }
        _myRB.AddForce(this.transform.right * travelSpeed, ForceMode.VelocityChange);
    }
    public void SetProperty(Attack_BaseProperties property) 
    {
        Property = property;
        projectileHitbox.hitboxProperties = Property;
    }
    public void SendHitTarget(Transform target)
    {
        if (projectileHitbox.HBType != HitBoxType.nullified)
        {
            projectileHitbox.SendHitStateAndHurtBox(projectileHitbox, target);
            projectileHitbox.currentCollider.enabled = false;
            KillProjectile();
        }
    }
    public void KillProjectile()
    {
        if (projectileHitbox.HBType != HitBoxType.nullified)
        {
            projectileHitbox.SetHitColliderType(projectileHitbox, HitBoxType.nullified);
        }
        float time = (4 * (1 / 60f));
        sphereMesh.material.DOFade(0, time);
        thisTransform.DOScale(1.25f, time).OnComplete(() =>
        {
            Destroy(this.gameObject);
        });
    }
}
