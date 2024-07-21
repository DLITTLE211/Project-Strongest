using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEditor;

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
    private Character_Animator _playerCAnimator;
    public FrameData _frameData;
    public HitCount _hitCount;
    private float bias;
    private float frameCount;

    List<RequiredCallback> requiredHitboxCallBacks;
    List<CustomCallback> customHitboxCallBacks;
    bool init;
    bool startup;
    bool active;
    bool inactive;
    bool lastFrame;

    public void SetAttackAnim(Character_Animator _playerAnim = null)
    {
        _playerCAnimator = _playerAnim;
        playerAnim = _playerAnim.myAnim;
        animLength = animClip.length;
        _frameData.SetRecoveryFrames(animClip.frameRate, animLength);
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
        catch (Exception)
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
            bias = (hb_placement.x * 2);
        }
    }
    Vector3 ReturnHITPosToVector3()
    {
        return new Vector3(hb_placement.x - bias, hb_placement.y, hb_placement.z);
    }
    Vector3 ReturnHURTPosToVector3()
    {
        return new Vector3(hu_placement.x - bias, hu_placement.y, hu_placement.z);
    }

    public override void OnInit(Character_Base curBase, Attack_BaseProperties newAttackProperties = null)
    {
        init = true;
        GetPlacementLocation(curBase);
        HitBox.PlaceHurtBox(extendedHitBox, ReturnHURTPosToVector3(), hu_orientation, hu_size.x, hu_size.y, hurtType);
        if (newAttackProperties != null)
        {
            HitBox.hitboxProperties = newAttackProperties;
        }
        DebugMessageHandler.instance.DisplayErrorMessage(1, $"Entered init");
        _playerCAnimator.SetCanTransitionIdle(false);
        _playerCAnimator._base._aManager.SetStartNextAttack(false);
    }
    public override void OnStartup(Character_Base curBase)
    {
        startup = true;
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
        active = true;
        HitBox.ActivateHitbox(HitBox, extendedHitBox, animName, _hitCount);
        DebugMessageHandler.instance.DisplayErrorMessage(1, $"Entered active");
    }
    public override void OnRecov(Character_Base curBase)
    {
        inactive = true;
        HitBox.DestroyHitbox(HitBox, extendedHitBox);
        DebugMessageHandler.instance.DisplayErrorMessage(1, $"Entered recov");
        _playerCAnimator._base._aManager.SetStartNextAttack(true);

    }
    public override void OnExit()
    {
        lastFrame = true;
        if (HitBox.gameObject.activeInHierarchy)
        {
            HitBox.DestroyHitbox(HitBox, extendedHitBox);
        }
        HitBox.hitboxProperties = null;
    }

    public void AddRequiredCallbacks(Character_Base curBase, Attack_BaseProperties newAttackProperties = null)
    {
        if (requiredHitboxCallBacks == null)
        {
            requiredHitboxCallBacks = new List<RequiredCallback>();
        }
        else
        {
            if (requiredHitboxCallBacks.Count > 0)
            {
                requiredHitboxCallBacks.Clear();
            }
        }

        init = false;
        startup = false;
        active = false;
        inactive = false;
        lastFrame = false;

        if (newAttackProperties != null)
        {

            requiredHitboxCallBacks.Add(new RequiredCallback(() => OnInit(curBase, newAttackProperties), _frameData.init, init));
        }
        else
        {
            requiredHitboxCallBacks.Add(new RequiredCallback(() => OnInit(curBase), _frameData.init, init));
        }
        requiredHitboxCallBacks.Add(new RequiredCallback(() => OnStartup(curBase), _frameData.startup, startup));
        requiredHitboxCallBacks.Add(new RequiredCallback(() => OnActive(curBase), _frameData.active, active));
        requiredHitboxCallBacks.Add(new RequiredCallback(() => OnRecov(curBase), _frameData.inactive, inactive));
        requiredHitboxCallBacks.Add(new RequiredCallback(() => OnExit(), _frameData.lastFrame, lastFrame));
    }
    public void AddCustomCallbacks()
    {
        customHitboxCallBacks = new List<CustomCallback>();
        for (int i = 0; i < _frameData._extraPoints.Count; i++)
        {
            _frameData._extraPoints[i].hitFrameBool = false;
            CustomCallback customCallback = new CustomCallback(_frameData._extraPoints[i].call, _frameData._extraPoints[i].hitFramePoints, 
                _frameData._extraPoints[i].hitFrameBool,_frameData._extraPoints[i].camPos, 
                _frameData._extraPoints[i].camRotation, _frameData._extraPoints[i].Force, 
                _frameData._extraPoints[i].projectileSpeed);
            customHitboxCallBacks.Add(customCallback);
        }
    }
    public IEnumerator TickAnimFrameCount(Attack_BaseProperties lastAttack)
    {
        frameCount = 0;
        float waitTime = 1f / 60f;
        while (frameCount <= lastAttack.AttackAnims.animLength)
        {
            try
            {
                float curFuncTimeStamp = waitTime * requiredHitboxCallBacks[0].timeStamp;
                if (requiredHitboxCallBacks.Count > 0)
                {
                    if (frameCount >= curFuncTimeStamp && requiredHitboxCallBacks[0].funcBool == false)
                    {
                        requiredHitboxCallBacks[0].func();
                        requiredHitboxCallBacks.RemoveAt(0);
                    }
                }
                if (customHitboxCallBacks != null)
                {
                    if (customHitboxCallBacks.Count > 0)
                    {
                        if (frameCount >= waitTime * customHitboxCallBacks[0].timeStamp && customHitboxCallBacks[0].funcBool == false)
                        {
                            Messenger.Broadcast<CustomCallback>(Events.CustomCallback, customHitboxCallBacks[0]);
                            customHitboxCallBacks.RemoveAt(0);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
                frameCount = lastAttack.AttackAnims.animLength + 1f;
                Debug.Log("Null Check");
                Debug.Log($"Inactive frame: {lastAttack.AttackAnims._frameData.inactive}");
                Debug.Log($"Last Attack null?: {lastAttack == null}");
                Debug.Log($"Inactive bool state: {inactive}");
                Debug.Break();
            }
            frameCount += 1f * waitTime;
            yield return new WaitForSeconds(waitTime);
        }
        _playerCAnimator.SetCanTransitionIdle(true);
        if (requiredHitboxCallBacks.Count == 1)
        {
            requiredHitboxCallBacks[0].func();
            requiredHitboxCallBacks.RemoveAt(0);
        }
        Messenger.Broadcast<int>(Events.AddNegativeFrames, lastAttack.AttackAnims._frameData.recovery);
    }
    public IEnumerator TickAnimThrowCount(AttackHandler_Attack throwProp)
    {
        frameCount = 0;
        float waitTime = 1f / 60f;
        while (frameCount <= throwProp.animLength)
        {
            try
            {
                float curFuncTimeStamp = waitTime * requiredHitboxCallBacks[0].timeStamp;
                if (requiredHitboxCallBacks.Count > 0)
                {
                    if (frameCount >= curFuncTimeStamp && requiredHitboxCallBacks[0].funcBool == false)
                    {
                        requiredHitboxCallBacks[0].func();
                        requiredHitboxCallBacks.RemoveAt(0);
                    }
                }
                if (customHitboxCallBacks != null)
                {
                    if (customHitboxCallBacks.Count > 0)
                    {
                        if (frameCount >= waitTime * customHitboxCallBacks[0].timeStamp && customHitboxCallBacks[0].funcBool == false)
                        {
                            Messenger.Broadcast<CustomCallback>(Events.CustomCallback, customHitboxCallBacks[0]);
                            customHitboxCallBacks.RemoveAt(0);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
                frameCount = throwProp.animLength + 1f;
                Debug.Log("Null Check");
                Debug.Log($"Inactive frame: {throwProp._frameData.inactive}");
                Debug.Log($"Last Attack null?: {throwProp == null}");
                Debug.Log($"Inactive bool state: {inactive}");
                Debug.Break();
            }
            frameCount += 1f * waitTime;
            yield return new WaitForSeconds(waitTime);
        }
        _playerCAnimator.SetCanTransitionIdle(true);
        if (requiredHitboxCallBacks.Count == 1)
        {
            requiredHitboxCallBacks[0].func();
            requiredHitboxCallBacks.RemoveAt(0);
        }
        Messenger.Broadcast<int>(Events.AddNegativeFrames, throwProp._frameData.recovery);
    }
}

[Serializable]
public class HitCount
{
    public int _count, _startCount;
    [Range(0, 0.25f)] public float _refreshRate, _startRefreshRate;
    public void ResetRefresh()
    {
        _refreshRate = _startRefreshRate;
    }
    public void ResetHitCount()
    {
        if (_startCount < 1)
        {
            _startCount = 1;
        }
        _count = _startCount;
    }
}
[Serializable]
public class FrameData
{
    public int init, startup, active, inactive, recovery, lastFrame;
    public List<ExtraFrameHitPoints> _extraPoints;
    public void SetRecoveryFrames(float sampleRate, float animLength)
    {
        int totalFrames = (int)(Mathf.Ceil(animLength / (1 / sampleRate)));
        lastFrame = totalFrames;
        recovery = (int)(totalFrames - inactive);
        if (_extraPoints.Count > 0)
        {
            for (int i = 0; i < _extraPoints.Count; i++)
            {
                _extraPoints[i].hitFrameBool = false;
            }
        }
    }
    public void ResetExtraFrames()
    {
        if (_extraPoints.Count > 0)
        {
            for (int i = 0; i < _extraPoints.Count; i++)
            {
                _extraPoints[i].hitFrameBool = false;
            }
        }
    }
}
[Serializable]
public class RequiredCallback
{
    public Callback func;
    public float timeStamp;
    public bool funcBool;
    public RequiredCallback(Callback _func, float _timeStamp, bool _funcBool)
    {
        func = _func;
        timeStamp = _timeStamp;
        funcBool = _funcBool;
    }
}
[Serializable]
public class CustomCallback
{
    public HitPointCall customCall;
    public float timeStamp;
    public bool funcBool;

    public float forceFloat,projectileSpeedFloat;
    public Vector3 camPositionVector, camRotateVector;
    public bool snapMovement;
    public CustomDamageField customDamage;
    public CustomCallback(HitPointCall _customCall, float _timeStamp, bool _funcBool, Vector3 position, Vector3 rotation, float _forceFloat = -1, float _projectileFloat = -1, bool isSnapping = false, CustomDamageField _customDamage = null)
    {
        customCall = _customCall;
        timeStamp = _timeStamp;
        funcBool = _funcBool;
        camPositionVector = position;
        camRotateVector = rotation;
        snapMovement = isSnapping;
        if (_customDamage != null)
        {
            customDamage = _customDamage;
        }
        if (_forceFloat > 0) 
        {
            forceFloat = _forceFloat;
        }
        if (_projectileFloat > 0)
        {
            projectileSpeedFloat = _projectileFloat;
        }
        
    }
}
[Serializable]
public class ExtraFrameHitPoints
{
    public int hitFramePoints;
    public HitPointCall call;
    public bool hitFrameBool;
    public float Force, projectileSpeed;
    public Vector3 camPos, camRotation;
    public bool snapMovement;
    public CustomDamageField customDamage;
}

[Serializable, Flags]
public enum HitPointCall
{
    ShootProjectile = 2,
    Force_Small = 4,
    Force_Medium = 8,
    Force_Large = 16,
    TeleportForward = 35,
    KillStance = 64,
    ToggleArmor = 125,
    ToggleInvincible = 256,
    ToggleAntiAir = 512,
    ActivateMobilityAction = 1024,
    ClearMobility = 2048,
    UnFreeze = 4096,
    ToggleFreeze_Self = 8192,
    ToggleFreeze_Other = 16384,
    ToggleFreeze_Both = 32768,
    TeleportBackward = 65536,

    PanPosOnTarget = 131072,
    PanRotateOnTarget = 262144,
    PanZoomOnTarget = 524288,

    DealCustomDamage = 1048576,
    ForceSideSwitch = 2097152,
}