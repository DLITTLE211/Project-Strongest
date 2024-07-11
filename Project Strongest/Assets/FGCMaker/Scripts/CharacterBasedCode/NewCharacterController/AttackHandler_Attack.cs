using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AttackHandler_Attack : AttackHandler_Base
{
    #region HitBox Variables
    [Space(15)]
    public HitBox HitBox;
    public HitBoxType attackType;
    [SerializeField] internal Vector3 hb_placement;
    [SerializeField] internal Vector3 hb_orientation = new Vector3(0, 0, 0);
    [SerializeField] internal Vector2 hb_size;
    [Space(15)]
    #endregion

    #region HurtBox Variables
    public HurtBox extendedHitBox;
    public HurtBoxType hurtType;
    [SerializeField] internal Vector3 hu_placement;
    [SerializeField] internal Vector3 hu_orientation = new Vector3(0, 0, 0);
    [SerializeField] internal Vector2 hu_size;
    #endregion

    public FrameData _frameData;

    public HitCount _hitCount;
    private float bias;
    public void SetAttackAnim(Character_Animator _playerAnim = null)
    {
        playerAnim = _playerAnim.myAnim;
        _frameData.SetRecoveryFrames(animClip.frameRate,animLength);
        try
        {
            animName = animClip.name;
            animLength = animClip.length;
            _hitCount.ResetHitCount();
            _hitCount.ResetRefresh();

            if (extendedHitBox == null)
            {
                extendedHitBox = _playerAnim._base.pSide.thisPosition.GiveHurtBox();
                extendedHitBox.gameObject.SetActive(false);
            }
        }
        catch (Exception e)
        {
            return;
        }
    }
    public void GetPlacementLocation(Character_Base curBase)
    {
        HitBox newHitBox = curBase.pSide.thisPosition.ReturnSideHitBox(this);
        if (HitBox == null)
        {
            HitBox = newHitBox;
        }
        else 
        {
            if (HitBox != newHitBox)
            {
                HitBox = newHitBox;
            }
        }
        if (HitBox.name.Contains(("r").ToString().ToUpper()))
        {
            bias = 0;
        }
        else
        {
            bias = (hb_placement.x*2);
        }
    }
    Vector3 ReturnHITPosToVector3()
    {
        return new Vector3(hb_placement.x - bias,hb_placement.y,hb_placement.z);
    }
    Vector3 ReturnHURTPosToVector3()
    {
        return new Vector3(hu_placement.x - bias, hu_placement.y, hu_placement.z);
    }
    public override void OnInit(Character_Base curBase, Attack_BaseProperties newAttackProperties = null)
    {
       
        GetPlacementLocation(curBase);
        HitBox.PlaceHurtBox(extendedHitBox, ReturnHURTPosToVector3(), hu_orientation, hu_size.x, hu_size.y, hurtType);
        if (newAttackProperties != null)
        {
            HitBox.hitboxProperties = newAttackProperties;
        }
        DebugMessageHandler.instance.DisplayErrorMessage(1, $"Entered startup");
    }
    public override void OnStartup(Character_Base curBase)
    {
        extendedHitBox.ActivateHurtbox(extendedHitBox);
        HitBox.PlaceHitBox(HitBox, ReturnHITPosToVector3(), hb_orientation, hb_size.x, hb_size.y, attackType);
        DebugMessageHandler.instance.DisplayErrorMessage(1, $"Entered startup");
    }
    public override void OnStay(Character_Base curBase)
    {
        DebugMessageHandler.instance.DisplayErrorMessage(1, $"Entered stay");
    }
    public override void OnActive(Character_Base curBase)
    {
        HitBox.ActivateHitbox(HitBox, extendedHitBox,animName, _hitCount);
        DebugMessageHandler.instance.DisplayErrorMessage(1, $"Entered active");
    }
    public override void OnRecov(Character_Base curBase)
    {
        HitBox.DestroyHitbox(HitBox, extendedHitBox);
        DebugMessageHandler.instance.DisplayErrorMessage(1, $"Entered recov");
    }
    public override void OnExit()
    {
        if (HitBox.gameObject.activeInHierarchy) 
        {
            HitBox.DestroyHitbox(HitBox, extendedHitBox);
        }
        HitBox.hitboxProperties = null;
    }
}

[Serializable]
public class HitCount
{
    public int _count, _startCount;
    [Range(0,0.25f)]public float _refreshRate, _startRefreshRate;
    public void ResetRefresh()
    {
        _refreshRate = _startRefreshRate;
    }
    public void ResetHitCount()
    {
        if(_startCount < 1) 
        {
            _startCount = 1;
        }
        _count = _startCount;
    }
}
[Serializable]
public class FrameData
{
    public int init, startup, active, inactive,recovery, lastFrame;
    public List<ExtraFrameHitPoints> _extraPoints;
    public void SetRecoveryFrames(float sampleRate,float animLength) 
    {
        int totalFrames = (int)(Mathf.Ceil(animLength / (1 / sampleRate)));
        lastFrame = totalFrames;
        recovery = (int)(totalFrames - inactive);
        if (_extraPoints.Count > 0)
        {
            for (int i = 0; i < _extraPoints.Count; i++)
            {
                _extraPoints[i].hitFrameBools = false;
            }
        }
    }
    public void ResetExtraFrames() 
    {
        if (_extraPoints.Count > 0)
        {
            for (int i = 0; i < _extraPoints.Count; i++)
            {
                _extraPoints[i].hitFrameBools = false;
            }
        }
    }
}
[Serializable]
public class ExtraFrameHitPoints 
{
    public int hitFramePoints;
    public HitPointCall call;
    public bool hitFrameBools;
}

[Serializable]
public enum HitPointCall 
{
    Phase,
    ShootProjectile,
    Force_Small,
    Force_Medium,
    Force_Large,
    Teleport,
    KillStance,
    ToggleArmor,
    ToggleInvincible,
    ToggleAntiAir,
    ActivateMobilityAction,
    ClearMobility,
    UnFreeze,
    ToggleFreeze_Self,
    ToggleFreeze_Other,
    ToggleFreeze_Both,
}