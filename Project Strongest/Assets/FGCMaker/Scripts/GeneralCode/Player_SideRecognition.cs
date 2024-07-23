using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;

public class Player_SideRecognition : MonoBehaviour
{
    public Character_Position thisPosition;
}
[System.Serializable]
public class Character_Position
{
    [SerializeField] private Transform CollisionDetectionTransform;
    [SerializeField] private Transform modelTransform;
    public Transform ModelTransform { get { return modelTransform; } }
    public HitBox[] hitBoxes;
    public HitBox projectile_HitBox;
    public HurtBox extendedHurbox;
    public Transform _targetCharacter;
    public float LW_Distance, RW_Distance;
    public Character_Face_Direction _directionFacing;
    private Vector3 leftFace = new Vector3(0f, 180f, 0f), rightFace = new Vector3(0f, 0f, 0f);
    Tween switchFaceDirection;
    public void SetFacingState(Character_Face_Direction face) 
    {
        if (_directionFacing != face) 
        {
            switch (face) 
            {
                case Character_Face_Direction.FacingLeft:
                    TurnModel(leftFace, -1f, face);
                    break;
                case Character_Face_Direction.FacingRight:
                    TurnModel(rightFace, 1f, face);
                    break;
            }
        }
    }

    void TurnModel(Vector3 direction, float flipSide, Character_Face_Direction _face)
    {
        if (modelTransform == null)
        {
            return;
        }
        if (DOTween.IsTweening(modelTransform))
        {
            return;
        }
        if (modelTransform.localEulerAngles == direction)
        {
            _directionFacing = _face;
            return;
        }
        modelTransform.localScale = new Vector3(1f, 1f, flipSide);
        modelTransform.DORotate(direction, 0.25f).OnComplete(() =>
        {
            _directionFacing = _face; 
            if (_directionFacing == Character_Face_Direction.FacingRight)
            {
                CollisionDetectionTransform.localPosition = new Vector3(-1 * 0.25f, 0, 0);
            }
            else if (_directionFacing == Character_Face_Direction.FacingLeft)
            {
                CollisionDetectionTransform.localPosition = new Vector3(1 * 0.25f, 0, 0);
            }
        });
    }
    public void SetModelTransform(Transform _modelTransform) 
    {
        modelTransform = _modelTransform;
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
    Neither,
}
