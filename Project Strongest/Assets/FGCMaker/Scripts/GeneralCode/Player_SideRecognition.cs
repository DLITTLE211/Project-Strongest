using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Player_SideRecognition : MonoBehaviour
{
    public Character_Position thisPosition;
}
[System.Serializable]
public class Character_Position
{
    public HitBox[] hitBoxes;
    public HitBox projectile_HitBox;
    public HurtBox extendedHurbox;
    public Transform _targetCharacter;
    public float LW_Distance, RW_Distance;
    public Character_Face_Direction _directionFacing;
    public void SetFacingState(Character_Face_Direction face) 
    {
        _directionFacing = face;
    }
    public void UpdatePlayerFacingDirection(Transform LW, Transform RW) 
    {
        float LW_Magnitude = Mathf.Abs(LW.position.x - _targetCharacter.position.x);
        float RW_Magnitude = Mathf.Abs(RW.position.x - _targetCharacter.position.x);
        if (LW_Distance != LW_Magnitude)
        {
            LW_Distance = LW_Magnitude;
        }
        if (RW_Distance != RW_Magnitude)
        {
            RW_Distance = RW_Magnitude;
        }
    }
    public HitBox ReturnSideHitBox(AttackHandler_Attack attack)
    {
        if (attack._frameData._extraPoints.Count > 0)
        {
            for (int i = 0; i < attack._frameData._extraPoints.Count; i++)
            {
                if (attack._frameData._extraPoints[i].call == HitPointCall.ShootProjectile)
                {
                    return projectile_HitBox;
                }
            }
        }
        return ReturnPhysicalSideHitBox();
    }
    public HitBox ReturnPhysicalSideHitBox()
    {
        return _directionFacing == Character_Face_Direction.FacingRight ? hitBoxes[1] : hitBoxes[0];
    }
    public HurtBox GiveHurtBox()
    {
        return extendedHurbox;
    }
}
[System.Serializable]
public enum Character_Face_Direction 
{
    FacingLeft,
    FacingRight,
}
