using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using FightingGame_FrameData;

[Serializable]
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
    [Space(15)]
    #endregion

    private Character_Base character;
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
    bool isFollowUpAttack;
    Character_Face_Direction currentFacingDirection;
    public void SetIsFollowUpAttack(bool state) 
    {
        isFollowUpAttack = state;
    }

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
        character = _playerAnim._base;
    }
    public void GetPlacementLocation(Character_Base curBase)
    {
        if (curBase._cHitboxManager.GetActiveHitBox().hitboxProperties?.InputTimer != null)
        {
            curBase._cHitboxManager.DisableCurrentHitbox();
               character._cHitboxManager.IterateHitBox();
        }
        HitBox newHitBox = curBase._cHitboxManager.GetActiveHitBox();

        if (HitBox != null)
        {
            if (HitBox.hitboxProperties != null)
            {
                HitBox.hitboxProperties = null;
            }
        }
        HitBox = newHitBox;
    }
    float ReturnBiasOnHitbox() 
    {
        currentFacingDirection = character.pSide.thisPosition._directionFacing;
        if (character.pSide.thisPosition._directionFacing == Character_Face_Direction.FacingRight)
        {
           return bias = 0;
        }
        else
        {
            return bias = (hb_placement.x * 2);
        }
    }
    Vector3 ReturnHITPosToVector3()
    {
        return new Vector3(hb_placement.x - ReturnBiasOnHitbox(), hb_placement.y, hb_placement.z);
    }
    Vector3 ReturnHURTPosToVector3()
    {
        return new Vector3(hu_placement.x - ReturnBiasOnHitbox(), hu_placement.y, hu_placement.z);
    }

    public override void OnInit(Character_Base curBase, Attack_BaseProperties newAttackProperties = null)
    {
        init = true;
        character._aFrameDataMeter.SetSliderInformation(HitBox.hitboxProperties.AttackAnims._frameData);
        GetPlacementLocation(curBase);
        HitBox.PlaceHurtBox(extendedHitBox, ReturnHURTPosToVector3(), hu_orientation, hu_size.x, hu_size.y, hurtType);
        if (newAttackProperties != null)
        {
            HitBox.SetHitBoxProperties(newAttackProperties);
        }
        _playerCAnimator.SetCanTransitionIdle(false);
        _playerCAnimator._base._aManager.SetStartNextAttack(false);
    }
    public override void OnStartup(Character_Base curBase)
    {
        startup = true;
        extendedHitBox.ActivateHurtbox(extendedHitBox);
        extendedHitBox.SetHurtboxState(extendedHitBox.huBType);
        character._cHurtBox.SetHurboxState(extendedHitBox.huBType);
        HitBox.PlaceHitBox(HitBox, ReturnHITPosToVector3(), hb_orientation, hb_size.x, hb_size.y, attackType);
    }
    public override void OnStay(Character_Base curBase)
    {
    }
    public override void OnActive(Character_Base curBase)
    {
        active = true;
        if (currentFacingDirection != character.pSide.thisPosition._directionFacing)
        {
            HitBox.PlaceHurtBox(extendedHitBox, ReturnHURTPosToVector3(), hu_orientation, hu_size.x, hu_size.y, hurtType);
            HitBox.PlaceHitBox(HitBox, ReturnHITPosToVector3(), hb_orientation, hb_size.x, hb_size.y, attackType);
        }
        HitBox.ActivateHitbox(HitBox, extendedHitBox, animName, _hitCount,_playerCAnimator.lastAttack);
    }
    public override void OnRecov(Character_Base curBase)
    {
        inactive = true;
        if (HitBox != null)
        {
            HitBox.DestroyHitbox(HitBox, extendedHitBox);
        }
        else 
        {
            character._cHitboxManager.DisableCurrentHitbox();
        }
        DebugMessageHandler.instance.DisplayErrorMessage(1, $"Entered recov");
        _playerCAnimator._base._aManager.SetStartNextAttack(true);

        if (HitBox.hitboxProperties != null)
        {
            if (HitBox.hitboxProperties._moveType == MoveType.Counter)
            {
                extendedHitBox.CounterMoveProperty = null;
                character._cHurtBox.SetHurboxState();
            }
        }
    }
    public override void OnRecovEnd()
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
        requiredHitboxCallBacks.Add(new RequiredCallback(() => OnRecovEnd(), _frameData.recoveryEnd, lastFrame));
    }
    public void AddCustomCallbacks(AttackHandler_Attack throwAttackCallbacks = null)
    {
        if (throwAttackCallbacks != null)
        {
            customHitboxCallBacks = new List<CustomCallback>();
            for (int i = 0; i < throwAttackCallbacks._frameData._extraPoints.Count; i++)
            {
                throwAttackCallbacks._frameData._extraPoints[i].hitFrameBool = false;
                CustomCallback customCallback = new CustomCallback(
                    throwAttackCallbacks._frameData._extraPoints[i].call, 
                    throwAttackCallbacks._frameData._extraPoints[i].hitFramePoints,
                    throwAttackCallbacks._frameData._extraPoints[i].hitFrameBool, 
                    throwAttackCallbacks._frameData._extraPoints[i].camPos,
                    throwAttackCallbacks._frameData._extraPoints[i].camRotation, 
                    throwAttackCallbacks._frameData._extraPoints[i].Force,
                    throwAttackCallbacks._frameData._extraPoints[i].projectileSpeed, 
                    throwAttackCallbacks._frameData._extraPoints[i].snapMovement,
                    throwAttackCallbacks._frameData._extraPoints[i].customDamage, 
                    throwAttackCallbacks._frameData._extraPoints[i].awaitEnum);
                customHitboxCallBacks.Add(customCallback);
            }
        }
        else
        {
            customHitboxCallBacks = new List<CustomCallback>();
            for (int i = 0; i < _frameData._extraPoints.Count; i++)
            {
                _frameData._extraPoints[i].hitFrameBool = false;
                CustomCallback customCallback = 
                    new CustomCallback(_frameData._extraPoints[i].call, 
                    _frameData._extraPoints[i].hitFramePoints,
                    _frameData._extraPoints[i].hitFrameBool,
                    _frameData._extraPoints[i].camPos,
                    _frameData._extraPoints[i].camRotation, 
                    _frameData._extraPoints[i].Force,
                    _frameData._extraPoints[i].projectileSpeed,
                    _frameData._extraPoints[i].snapMovement,
                    _frameData._extraPoints[i].customDamage,
                    _frameData._extraPoints[i].awaitEnum);
                customHitboxCallBacks.Add(customCallback);
            }
        }
    }
    public IEnumerator TickAnimFrameCount(Attack_BaseProperties lastAttack)
    {
        if (lastAttack._moveType == MoveType.Counter)
        {
            extendedHitBox.SetCounterMoveProperty(lastAttack);
        }
        frameCount = 0;
        if (_playerCAnimator.lastAttack._moveType == MoveType.Super)
        {
            character._cAttackTimer.PauseTimerOnSuperSuccess();
        }
        while (frameCount <= lastAttack.AttackAnims.animLength)
        {
            if (character.ReturnIfPaused())
            {
                yield return new WaitForSeconds(Base_FrameCode.ONE_FRAME);
            }
            else
            {
                float frameIterator = Base_FrameCode.ONE_FRAME * character._cHitstun.animSpeed;
                float waitTime = Base_FrameCode.ONE_FRAME / character._cHitstun.animSpeed;
                if (character._cHitstun.animSpeed == 0.25f)
                {
                    frameCount = frameCount - (frameCount * character._cHitstun.animSpeed);
                }
                try
                {
                    float curFuncTimeStamp = Base_FrameCode.ONE_FRAME * requiredHitboxCallBacks[0].timeStamp;
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
                            if (frameCount >= Base_FrameCode.ONE_FRAME * customHitboxCallBacks[0].timeStamp && customHitboxCallBacks[0].funcBool == false)
                            {
                                character.ReceiveCustomCallBack(customHitboxCallBacks[0]);
                                customHitboxCallBacks.RemoveAt(0);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    //HitBox.DestroySelf();
                    Debug.LogError(e.ToString());
                    frameCount = lastAttack.AttackAnims.animLength + 1f;
                    Debug.Log("Null Check");
                    Debug.Log($"Inactive frame: {lastAttack.AttackAnims._frameData.inactive}");
                    Debug.Log($"Last Attack null?: {lastAttack == null}");
                    Debug.Log($"Inactive bool state: {inactive}");
                    Debug.Break();
                }
                frameCount += frameIterator;
                yield return new WaitForSeconds(waitTime);
            }
        }
        _playerCAnimator.SetCanTransitionIdle(true);
        if (character._cAttackTimer._type == TimerType.Special) 
        {
            character._cAttackTimer.ClearAttackLanded();
        }
        else if (character._cAttackTimer._type == TimerType.Super && lastAttack._moveType == MoveType.Super)
        {
            character._cAttackTimer.ClearSuperLanded();
        }
        else if (character._cAttackTimer._type == TimerType.Throw && lastAttack._moveType == MoveType.Throw)
        {
            character._cAttackTimer.ClearThrowLanded();
        }
        else
        {
            character._cAttackTimer.ClearAttackLanded();
            _playerCAnimator.SetCanTransitionIdle(true);
        }
        if (requiredHitboxCallBacks.Count == 1)
        {
            requiredHitboxCallBacks[0].func();
            requiredHitboxCallBacks.RemoveAt(0);
        }
        if (lastAttack._moveType == MoveType.Throw)
        {
            if (!lastAttack.hitConnected)
            {
                _playerCAnimator.CountUpNegativeFrames(lastAttack.AttackAnims._frameData.totalRecovery);
            }
        }
        else
        {
            _playerCAnimator.CountUpNegativeFrames(lastAttack.AttackAnims._frameData.totalRecovery);
        }
    }
    public IEnumerator TickAnimCustomCount(AttackHandler_Attack customProp, int curAnim = -1, int animCount = 1, Callback superIteratorCallback = null)
    {
        frameCount = 0;
        if (!_playerCAnimator.canTick)
        {
            _playerCAnimator.canTick = true;
        }
        if (_playerCAnimator.lastAttack._moveType == MoveType.Throw)
        {
            character._cAttackTimer.PauseTimerOnThrowSuccess();
        }
        if (_playerCAnimator.lastAttack._moveType == MoveType.Super)
        {
            character._cAttackTimer.PauseTimerOnSuperSuccess();
        }
        while (frameCount <= customProp.animLength)
        {
            if (character.ReturnIfPaused())
            {
                yield return new WaitForSeconds(Base_FrameCode.ONE_FRAME);
            }
            else
            {
                float frameIterator = Base_FrameCode.ONE_FRAME * character._cHitstun.animSpeed;
                float waitTime = Base_FrameCode.ONE_FRAME / character._cHitstun.animSpeed;
                frameCount = frameCount * character._cHitstun.animSpeed;
                try
                {
                    float curFuncTimeStamp = Base_FrameCode.ONE_FRAME * requiredHitboxCallBacks[0].timeStamp;
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
                            float curCustomTimeStamp = Base_FrameCode.ONE_FRAME * customHitboxCallBacks[0].timeStamp;
                            if (frameCount >= curCustomTimeStamp && customHitboxCallBacks[0].funcBool == false)
                            {
                                Debug.Log($"{customProp.animName}: CustomCallback 0, Hit!!");
                                character.ReceiveCustomCallBack(customHitboxCallBacks[0], superIteratorCallback);
                                if (customHitboxCallBacks[0].awaitEnum.keyRef != WaitingEnumKey.NA) 
                                {
                                    customHitboxCallBacks.RemoveAt(0);
                                    yield break;
                                }
                                customHitboxCallBacks.RemoveAt(0);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    //HitBox.DestroySelf();
                    Debug.LogError(e.ToString());
                    frameCount = customProp.animLength + 1f;
                    Debug.Log("Null Check");
                    Debug.Log($"Inactive frame: {customProp._frameData.inactive}");
                    Debug.Log($"Last Attack null?: {customProp == null}");
                    Debug.Log($"Inactive bool state: {inactive}");
                    Debug.Break();
                }
                frameCount += frameIterator;
                yield return new WaitForSeconds(waitTime);
            }
        }
        Attack_BaseProperties thisAttack = HitBox?.hitboxProperties;
        if (thisAttack?.InputTimer == null) 
        {
            thisAttack = character._cAnimator.lastAttack;
        }
        if (thisAttack.InputTimer != null)
        {
            if (thisAttack._moveType == MoveType.Throw || thisAttack._moveType == MoveType.CommandGrab)
            {
                character._cAttackTimer.ClearThrowLanded();
                _playerCAnimator.SetCanTransitionIdle(true);
            }
            else if (thisAttack._moveType == MoveType.Super)
            {
                if (curAnim >= animCount)
                {
                    character._cAttackTimer.ClearSuperLanded();
                    _playerCAnimator.SetCanTransitionIdle(true);
                }
            }
            else
            {
                character._cAttackTimer.ClearAttackLanded();
                _playerCAnimator.SetCanTransitionIdle(true);
            }
        }
        else 
        {
            character._cAttackTimer.ClearAttackLanded();
            _playerCAnimator.SetCanTransitionIdle(true);
        }
        if (requiredHitboxCallBacks.Count == 1)
        {
            requiredHitboxCallBacks[0].func();
            requiredHitboxCallBacks.RemoveAt(0);
        }
        _playerCAnimator.CountUpNegativeFrames(customProp._frameData.totalRecovery);
    }
}

[Serializable]
public class HitCount
{
    public int _count, _startCount;
    [Range(0, 30)] public int _refreshRate, _startRefreshRate;
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
    public int init, startup, active, inactive, recoveryEnd;
    public int totalRecovery;
    public List<ExtraFrameHitPoints> _extraPoints;
    public void SetRecoveryFrames(float sampleRate, float animLength)
    {
        int totalFrames = (int)(Mathf.Ceil(animLength / (1 / sampleRate)));
        totalRecovery = (int)(recoveryEnd - inactive);
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
    public AwaitClass awaitEnum;
    public float forceFloat,projectileSpeedFloat;
    public Vector3 camPositionVector, camRotateVector;
    public bool snapMovement;
    public CustomDamageField customDamage;
    public CustomCallback (HitPointCall _customCall, 
        float _timeStamp, 
        bool _funcBool, 
        Vector3 position, 
        Vector3 rotation, 
        float _forceFloat = -1, 
        float _projectileFloat = -1, 
        bool isSnapping = false, 
        CustomDamageField _customDamage = null, 
        AwaitClass _awaitEnum = null)
    {
        customCall = _customCall;
        timeStamp = _timeStamp;
        funcBool = _funcBool;
        camPositionVector = position;
        camRotateVector = rotation;
        snapMovement = isSnapping;
        customDamage = _customDamage;
        forceFloat = _forceFloat;
        projectileSpeedFloat = _projectileFloat;
        awaitEnum = _awaitEnum;
    }
}
[Serializable]
public class ExtraFrameHitPoints
{
    public int hitFramePoints;
    public HitPointCall call;
    public bool hitFrameBool;
    public AwaitClass awaitEnum;
    public float Force, projectileSpeed;
    public Vector3 camPos, camRotation;
    public bool snapMovement;
    public CustomDamageField customDamage;
}

[Serializable, Flags]
public enum HitPointCall
{
    ShootProjectile = 1 << 0,
    Force_Right = 1 << 1,
    Force_Up = 1 << 2,
    TeleportForward = 1 << 3,
    KillStance = 1 << 4,
    ToggleArmor = 1 << 5,
    ToggleInvincible = 1 << 6,
    ToggleAntiAir = 1 << 7,
    ActivateMobilityAction = 1 << 8,
    ClearMobility = 1 << 9,
    UnFreeze = 1 << 10,
    ToggleFreeze_Self = 1 << 11,
    ToggleFreeze_Other = 1 << 12,
    ToggleFreeze_Both = 1 << 13,
    TeleportBackward = 1 << 14,

    PanPosOnTarget = 1 << 15,
    PanRotateOnTarget = 1 << 16,
    PanZoomOnTarget = 1 << 17,

    DealCustomDamage = 1 << 18,
    ForceSideSwitch = 1 << 19,
    AwaitSequenceSignifier = 1 << 20,
    LockPos = 1 << 21,
    UnlockPos = 1 << 22,
}