using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageWallColliderIgnore : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "CameraWall") 
        {
            Physics.IgnoreCollision(collision.gameObject.GetComponent<Collider>(),this.GetComponent<Collider>());
        }
    }
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "CameraWall")
        {
            Physics.IgnoreCollision(collision.gameObject.GetComponent<Collider>(), this.GetComponent<Collider>());
        }
    }
}
