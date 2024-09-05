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
    [SerializeField] private Character_Base _base;
    public Transform ModelTransform { get { return modelTransform; } }
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
    bool NeutralStateTurn() 
    {
        List<IState> slowTurnStates = new List<IState>() 
        {
            _base._cStateMachine.idleStateRef,
            _base._cStateMachine.crouchStateRef,
            _base._cStateMachine.jumpRef,
        };
        bool fullCheck = slowTurnStates.Contains(_base._cStateMachine._playerState.current.State);
        return fullCheck;
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
        float speed = 0.15f;
        if (!NeutralStateTurn()) 
        {
            speed = 0;
        }
        modelTransform.localScale = new Vector3(1f, 1f, flipSide);
        modelTransform.DORotate(direction, speed).OnStart(() =>
        {
            if (_face == Character_Face_Direction.FacingRight)
            {
                CollisionDetectionTransform.DOLocalMoveX(-0.25f, speed);
            }
            else if (_face == Character_Face_Direction.FacingLeft)
            {
                CollisionDetectionTransform.DOLocalMoveX(0.25f, speed);
            }
        }).OnComplete(() =>
        {
            _directionFacing = _face; 
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
        return null;
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
