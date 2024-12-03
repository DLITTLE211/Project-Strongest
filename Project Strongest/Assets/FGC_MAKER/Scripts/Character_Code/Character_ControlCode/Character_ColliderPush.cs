using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_ColliderPush : MonoBehaviour
{
    public Character_Base _base;
    Character_Base opponentFace;
    CapsuleCollider _self;
    public Collider[] tempColliders;
    private const float pushBackMultiplier = 0.35f;
    private const float strongPushBackMultiplier = 0.535f;
    public void SetCharacterBase(Character_Base newBase) 
    {
        _base = newBase;
        _self = GetComponent<CapsuleCollider>();
    }
    private void Update()
    {
        if (!_base._cHurtBox.IsGrounded())
        {
            CheckSecondPlayerOverlap();
        }
    }

    void CheckSecondPlayerOverlap() 
    {
        try
        {
            Collider[] cols = Physics.OverlapBox
                (_self.bounds.center, _self.bounds.extents, _self.transform.rotation);
            tempColliders = cols;
            for (int i = 0; i < cols.Length; i++)
            {
                if (cols[i].transform.root == transform.root)
                {
                    continue;
                }
                else
                {
                    if (cols[i].gameObject.tag == "CollisionBox" || cols[i].gameObject.tag == "PCharacter")
                    {
                        PushBackObject(cols[i]);
                        return;
                    }
                }
            }
        }
        catch (UnassignedReferenceException) 
        {

        }
    }

    void PushBackObject(Collider otherPlayer)
    {
        opponentFace = otherPlayer.gameObject.GetComponentInParent<Character_Base>();
        opponentFace._cForce.beingPushed = true;
        if (_base._cStateMachine._playerState.current.State == _base._cStateMachine.hitStateRef)
        {
            opponentFace._cForce.InstantForceAway(-strongPushBackMultiplier);
        }
        else 
        {
            opponentFace._cForce.InstantForceAway(-pushBackMultiplier);
        }
    }
}
