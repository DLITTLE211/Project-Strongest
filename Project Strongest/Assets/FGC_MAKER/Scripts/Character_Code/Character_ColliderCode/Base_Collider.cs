using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base_Collider : CollisionDetection
{
    // Start is called before the first frame update
    public PhysicMaterial physicsMat;
    public Collider headCollider;
    public Transform modelRotation;
    public Character_ColliderPush pushCollider;
    public void SetBaseCollider(float sizeX = 0, float sizeY = 0, Character_CollisionSizing sizing = null,ColliderType collisionType = ColliderType.Trigger)
    {
        if (currentCollider == null)
        {
            this.gameObject.AddComponent<CapsuleCollider>();
            headCollider = this.gameObject.GetComponent<CapsuleCollider>();
            headCollider.GetComponent<CapsuleCollider>().center = sizing.headHurtboxPositioning;
            headCollider.GetComponent<CapsuleCollider>().radius = sizing.headRadius;
            headCollider.GetComponent<CapsuleCollider>().height = sizing.headHeight;

            boxColliderSpawnPoint.AddComponent<CapsuleCollider>();
            currentCollider = boxColliderSpawnPoint.GetComponent<CapsuleCollider>();
            currentCollider.GetComponent<CapsuleCollider>().center = sizing.bodyHurtboxPositioning;
            currentCollider.GetComponent<CapsuleCollider>().radius = sizing.bodyRadius;
            currentCollider.GetComponent<CapsuleCollider>().height = sizing.bodyHeight;
            if (collisionType == ColliderType.Trigger)
            {
                currentCollider.isTrigger = true;
            }
            else if (collisionType == ColliderType.Collision)
            {
                currentCollider.isTrigger = false;
            }
            pushCollider.SetCharacterBase(this.gameObject.GetComponentInParent<Character_Base>());
        }
        headCollider.GetComponent<CapsuleCollider>().material = physicsMat;
        xSize = sizeX;
        ySize = sizeY;
        this.transform.localScale = new Vector2(xSize, ySize);
    }
}
