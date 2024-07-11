using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base_Collider : CollisionDetection
{
    // Start is called before the first frame update
    public PhysicMaterial physicsMat;
    public Collider headCollider;
    public Transform modelRotation;
    public void SetBaseCollider(float sizeX = 0, float sizeY = 0, ColliderType collisionType = ColliderType.Trigger)
    {
        if (currentCollider == null)
        {
            this.gameObject.AddComponent<CapsuleCollider>();
            headCollider = this.gameObject.GetComponent<CapsuleCollider>();
            headCollider.GetComponent<CapsuleCollider>().center = new Vector3(0, 0.27f, 0);
            headCollider.GetComponent<CapsuleCollider>().radius = 0.35f;
            headCollider.GetComponent<CapsuleCollider>().height = 0;

            boxColliderSpawnPoint.AddComponent<CapsuleCollider>();
            currentCollider = boxColliderSpawnPoint.GetComponent<CapsuleCollider>();
            currentCollider.GetComponent<CapsuleCollider>().center = new Vector3(0, -0.15f, 0);
            currentCollider.GetComponent<CapsuleCollider>().height = 0.83f;
            if (collisionType == ColliderType.Trigger)
            {
                currentCollider.isTrigger = true;
            }
            else if (collisionType == ColliderType.Collision)
            {
                currentCollider.isTrigger = false;
            }
        }
        headCollider.GetComponent<CapsuleCollider>().material = physicsMat;
        xSize = sizeX;
        ySize = sizeY;
        this.transform.localScale = new Vector2(xSize, ySize);
    }
}
