using DG.Tweening;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using FightingGame_FrameData;

public class Character_HurtboxController : MonoBehaviour
{
    public int _angleZ;
    [SerializeField] private Character_Base _base;
    [SerializeField] private TMP_Text _hurtboxStateText;
    [SerializeField] HurtBoxSize curSize;
    [SerializeField] HurtBoxType newHurtboxType;
    [SerializeField] bool groundCheck;
    [SerializeField] private HeightPositionMarkerSet h_Markers;

    private List<Vector3> hurtBoxesSizes = new List<Vector3>();
    private List<Vector3> hurtBoxRotations = new List<Vector3>();

    public List<float> hurtBoxesPositions = new List<float>();

    public Base_Collider collisionBox;

    public Transform reference;
    public HurtBox triggerBox;
    [SerializeField] private HurtBox _extendedHurtbox;
    public float check;
    private int totalTechTime = 5;
    public float throwTechTime;
    public bool throwTeched;

    public void SetCollisionHurtboxStartSize(Character_CollisionSizing HurtBoxSizing)
    {
        throwTechTime = totalTechTime * Base_FrameCode.ONE_FRAME;
        throwTeched = false;
        collisionBox.SetBaseCollider(0.5f, _base.characterProfile.Height / 100f, HurtBoxSizing, ColliderType.Collision);
    }
    public void SetMarkers(HeightPositionMarkerSet _hMarkers)
    {
        h_Markers = _hMarkers;
    }
    public void SetTriggerHurtboxStartSize(Character_HurtBoxSizing HurtBoxSizing, Character_Base _base)
    { 
        triggerBox.SetHurtboxSizing(HurtBoxSizing,true, _base);
        //SetupVectorInfo(HurtBoxSizing);
    }
    private void FixedUpdate()
    {
        IsGrounded();
        collisionBox.SetSlipMat(!groundCheck);
    }
    private void Update()
    {
        triggerBox.transform.rotation = _base.gameObject.transform.rotation;
        if (triggerBox.huBType != newHurtboxType)
        {
            triggerBox.huBType = newHurtboxType;
        }
        SetHitboxSize();
    }
    public void ResetExtendedHurtbox() 
    {
        _extendedHurtbox.SetHurtBoxSize(0, 0, true);
    }
    public bool CheckIfThrowTeched() 
    {
        if(_base._cStateMachine._playerState.current.State == _base._cStateMachine.hitStateRef) 
        {
            if (_base._subState != Character_SubStates.Dummy)
            {
                if (_base.CheckThrowButton().Button_State._state != ButtonStateMachine.InputState.released)
                {
                    throwTeched = true;
                    return true;
                }
            }
            return false;
        }
        return false;
    }

    #region Ground Check
    public bool IsGrounded() 
    {
        try
        {
            Collider[] cols = Physics.OverlapBox
                (collisionBox.currentCollider.bounds.center,
                collisionBox.currentCollider.bounds.extents,
                collisionBox.currentCollider.transform.rotation);
            foreach (Collider c in cols)
            {
                if (c.transform.root == transform.root)
                {
                    continue;
                }
                if (c.transform.root.gameObject.GetComponent<Character_Base>())
                {
                    continue;
                }
                else
                {
                    if (c.gameObject.tag == "Ground")
                    {
                        groundCheck = true;
                        return groundCheck;
                    }
                    else
                    {
                        continue;
                    }
                }
            }
            groundCheck = false;
            return groundCheck;
        }
        catch (UnassignedReferenceException) { return false; }
    }
    #endregion

    public void SetHitboxSize()
    {
        #region Height Of HitBox
        float height = Vector3.Distance(h_Markers.topPoint.transform.position, h_Markers.basePoint.transform.position);
        triggerBox.transform.localScale = new Vector3(1f, height, 1f);
        #endregion

        #region CenterPoint of Hitbox
        Bounds centerPoint = new Bounds(h_Markers.centerPoint.transform.localPosition, Vector3.zero);
        centerPoint.SetMinMax(h_Markers.basePoint.transform.position, h_Markers.topPoint.transform.position);
        if (!limitCheck(centerPoint.center.y, h_Markers.topPoint.transform.localPosition.y, h_Markers.basePoint.transform.position.y))
        {
            triggerBox.transform.localPosition = new Vector3(triggerBox.transform.localPosition.x, centerPoint.center.y, triggerBox.transform.localPosition.z);
        }
        #endregion

        #region Rotation of Hitbox
        float yPoint = h_Markers.topPoint.transform.position.y - h_Markers.basePoint.transform.position.y;
        float xPoint = h_Markers.topPoint.transform.position.x - h_Markers.basePoint.transform.position.x;
        float angle = Mathf.Atan2(yPoint, xPoint);
        if (angle < 0f)
        {
            angle += Mathf.PI * 2f;
        }

        _angleZ = (int)(angle * (180f / Mathf.PI) - 90f);
        _angleZ = _angleZ / 10;
        _angleZ = _angleZ * 10;
        Vector3 hurtboxRotation = new Vector3(0f, 0f, -ClampAngle(_angleZ));
        triggerBox.transform.localEulerAngles = hurtboxRotation;
        //collisionBox.transform.localEulerAngles = hurtboxRotation;
        if (curSize == HurtBoxSize.Downed && _base._cAnimator.isHit)
        {
            Vector3 hurtboxPosition = new Vector3(0f, -0.25f, 0f);
            //collisionBox.transform.localPosition = hurtboxPosition;
        }
        else
        {
            //collisionBox.transform.localPosition = new Vector3(0f, 0.35f, 0f);
        }
        #endregion
    }
    int ClampAngle(int angle) 
    {
        if((90 - angle) < 15) 
        {
            curSize = HurtBoxSize.Downed;
            return 90;
        }
        if ((90 - angle) >= 70)
        {
            curSize = HurtBoxSize.Standing;
            return 0;
        }
        return 0;
    }
    bool limitCheck(float centerY, float topY, float bottomY) 
    {
        if(centerY >= topY)
        {
            return true;
        }
        if (centerY <= bottomY)
        {
            return true;
        }
        return false;
        
    }
    #region HurtBox Size Manipulation

    public void SetupVectorInfo(Character_HurtBoxSizing HurtBoxSizing) 
    {
        hurtBoxesSizes.Add(HurtBoxSizing.hurtboxSizing);
        hurtBoxesSizes.Add(new Vector3(HurtBoxSizing.hurtboxSizing.x, HurtBoxSizing.hurtboxSizing.y - (HurtBoxSizing.hurtboxSizing.y / 2), HurtBoxSizing.hurtboxSizing.z));
        hurtBoxesSizes.Add(new Vector3(HurtBoxSizing.hurtboxSizing.y, HurtBoxSizing.hurtboxSizing.x, HurtBoxSizing.hurtboxSizing.z));
        hurtBoxesPositions.Add(triggerBox.transform.localPosition.y);
        hurtBoxesPositions.Add(-0.35f);
        float layFlatValue = _base.pSide.thisPosition._directionFacing == Character_Face_Direction.FacingRight ? 90 : -90f;
        hurtBoxRotations.Add(new Vector3(0, 0, 0f));
        hurtBoxRotations.Add(new Vector3(0, 0, layFlatValue));
    }
    #endregion

    #region Hurtbox State Manipulation
    public void SetHurboxState(HurtBoxType newType = HurtBoxType.NoBlock)
    {
        if (newHurtboxType != newType)
        {
            newHurtboxType = newType;
            triggerBox.huBType = newHurtboxType;
            triggerBox.SetHurtboxState(triggerBox.huBType);
        }
    }
    public void SetHurboxStateOnCommand(CustomCallback callback)
    {
        triggerBox.huBType = callback.chosenType;
        triggerBox.SetHurtboxState(triggerBox.huBType);
    }
    #endregion
}
[Serializable]
public enum HurtBoxSize
{
    Standing,
    Downed,
}