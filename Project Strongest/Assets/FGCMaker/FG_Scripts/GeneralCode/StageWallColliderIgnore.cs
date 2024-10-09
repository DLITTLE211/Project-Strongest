using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageWallColliderIgnore : MonoBehaviour
{
    public bool playerHitEndWall;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "CameraWall") 
        {
            Physics.IgnoreCollision(collision.gameObject.GetComponent<Collider>(),this.GetComponent<Collider>());
        }
        if (collision.gameObject.tag == "CollisionBox" || collision.gameObject.tag == "PCharacter")
        {
            playerHitEndWall = true;
           // collision.gameObject.GetComponentInParent<Character_Base>()._cForce.AddLateralForceOnCommand(2);
        }
    }
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "CameraWall")
        {
            Physics.IgnoreCollision(collision.gameObject.GetComponent<Collider>(), this.GetComponent<Collider>());
        }
        if (collision.gameObject.tag == "CollisionBox" || collision.gameObject.tag == "PCharacter")
        {
            playerHitEndWall = true;
            // collision.gameObject.GetComponentInParent<Character_Base>()._cForce.AddLateralForceOnCommand(2);
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "CollisionBox" || collision.gameObject.tag == "PCharacter")
        {
            playerHitEndWall = false;
        }
    }
}
