using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Character_HurtboxController : MonoBehaviour
{
    [SerializeField] private Character_Base _base;
    [SerializeField] private TMP_Text _hurtboxStateText;
    [SerializeField] HurtBoxSize curSize;
    [SerializeField] HurtBoxType newHurtboxType;
    [SerializeField] bool groundCheck;

    private List<Vector3> hurtBoxesSizes = new List<Vector3>();
    private List<Vector3> hurtBoxRotations = new List<Vector3>();

    public List<float> hurtBoxesPositions = new List<float>();

    public Base_Collider collisionBox;

    public Transform reference;
    public HurtBox triggerBox;
    public float check;

    private void Start()
    {
        SetupVectorInfo();
    }
    public void SetHurtboxStartSize() 
    {
        collisionBox.SetBaseCollider(0.5f, _base.characterProfile.Height / 100f, ColliderType.Collision);
    }
    private void FixedUpdate()
    {
        IsGrounded();
        if (triggerBox.huBType != newHurtboxType) 
        {
            triggerBox.huBType = newHurtboxType;
        }
    }
    private void Update()
    {
        triggerBox.transform.rotation = _base.gameObject.transform.rotation;
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
                        groundCheck = false;
                        return groundCheck;
                    }
                }
            }
            groundCheck = false;
            return groundCheck;
        }
        catch (UnassignedReferenceException) { return false; }
    }
    #endregion

   
    #region HurtBox Size Manipulation
    public void SetHitboxSize(HurtBoxSize newSize) 
    {
        if (newSize != curSize)
        {
            //DOTween.Complete(triggerBox.transform);
            switch (newSize)
            {
                case HurtBoxSize.Standing:
                    triggerBox.transform.DOScale(hurtBoxesSizes[0], (2 * (1 / 60f)));
                    triggerBox.transform.DOLocalMoveY(hurtBoxesPositions[0], (2 * (1 / 60f)));
                   // collisionBox.transform.DOLocalRotate(hurtBoxRotations[0], (2 * (1 / 60f)));
                    break;
                case HurtBoxSize.Crouching:
                    triggerBox.transform.DOScale(hurtBoxesSizes[1], (2 * (1 / 60f)));
                    triggerBox.transform.DOLocalMoveY(hurtBoxesPositions[1], (2 * (1 / 60f)));
                   // collisionBox.transform.DOLocalRotate(hurtBoxRotations[0], (2 * (1 / 60f)));
                    break;
                case HurtBoxSize.Downed:
                    triggerBox.transform.DOScale(hurtBoxesSizes[2], (2 * (1 / 60f)));
                    triggerBox.transform.DOLocalMoveY(hurtBoxesPositions[1], (2 * (1 / 60f)));
                   // collisionBox.transform.DOLocalRotate(hurtBoxRotations[1], (2 * (1 / 60f)));
                    break;
            }
            curSize = newSize;
        }
    }
    public void ChangeHeightOnDowned()
    {
        collisionBox.transform.DOLocalMoveY(-0.15f, (2 * (1 / 60f)));
    }
    public void ChangeHeightOnStanding(float time)
    {
        collisionBox.transform.DOLocalMoveY(0.35f, time);
    }

    public void SetupVectorInfo() 
    {
        hurtBoxesSizes.Add(triggerBox.transform.localScale);
        hurtBoxesSizes.Add(new Vector3(triggerBox.transform.localScale.x, triggerBox.transform.localScale.y - (triggerBox.transform.localScale.y / 2), triggerBox.transform.localScale.z));
        hurtBoxesSizes.Add(new Vector3(triggerBox.transform.localScale.y, triggerBox.transform.localScale.x, triggerBox.transform.localScale.z));
        hurtBoxesPositions.Add(triggerBox.transform.localPosition.y);
        hurtBoxesPositions.Add(-0.35f);
        float layFlatValue = _base.pSide.thisPosition._directionFacing == Character_Face_Direction.FacingRight ? 90 : -90f;
        hurtBoxRotations.Add(new Vector3(0, 0, 0f));
        hurtBoxRotations.Add(new Vector3(0, 0, layFlatValue));
    }
    #endregion

    #region Hurtbox State Manipulation
    public void SetHurboxState(HurtBoxType newType)
    {
        newHurtboxType = newType;
        UpdateHurtboxStateText(newHurtboxType);
    }
    public void UpdateHurtboxStateText(HurtBoxType newType)
    {
        _hurtboxStateText.text = $"HurtboxState: {newType.ToString()}";
    }
    #endregion
}
[Serializable]
public enum HurtBoxSize
{
    Standing,
    Crouching,
    Downed,
}