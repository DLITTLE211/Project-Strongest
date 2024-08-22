using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using System;

public class Character_HitController : MonoBehaviour
{
    //[SerializeField] private HitReactions _characterHitAnimations;
    [SerializeField] private Character_Animator _cAnimator;
    [SerializeField] private Character_Base _base;
    [SerializeField] private HitAnimationHolder _characterTotalHitReactions;
    Dictionary<HitLevel, Callback> reactionFunctionDictionary;

    bool recoverTrigger;
    public bool smallHitRecovering, bigHitRecovering, airRecoverPossible;
    public bool crouchBlocking, standingBlocking;
    private bool hasRecovered;
    public Attack_KnockDown last_KD;
    private float recoveryTime;
    Attack_BaseProperties currentProperty;
    IEnumerator activeHitResponseRoutine, recoverRoutine;
    List<MoveType> lockMoveTypes = new List<MoveType>()
    {
        MoveType.Counter,
        MoveType.CommandGrab,
        MoveType.Throw,
        MoveType.Super,
    };
    public void Start()
    {
        recoveryTime = 0;
    }
    public void SetHitReactions(Character_Animator myAnim,HitReactions newHitReactions) 
    {
        //_characterHitAnimations = newHitReactions;
        SetAnimator(myAnim);
    }
    public void SetAnimator(Character_Animator myAnim)
    {
        _cAnimator = myAnim;
        SetUpHitAnimations();
    }
    void SetUpHitAnimations()
    {
        _characterTotalHitReactions.Setup();
        for (int i = 0; i < _base.characterProfile.hitResponseAnimations.Count; i++)
        {
            CharacterAnimResponse responseAnim = _base.characterProfile.hitResponseAnimations[i];
            AnimationClip curClip = _base.characterProfile.hitResponseAnimations[i]._clip;
            HitAnimationField newHitAnimField = new HitAnimationField(curClip, curClip.length, curClip.name, responseAnim._hitReactionType, responseAnim._hitResponseLevel, responseAnim.groundedReaction, responseAnim.isLowResponse);

            if (newHitAnimField.hitReactionType == HitReactionType.StandardBlock ^ newHitAnimField.hitReactionType == HitReactionType.GuardBreakBlock)
            {
                _characterTotalHitReactions.block_Anims.Add(newHitAnimField);
                continue;
            }
            if (newHitAnimField.hitReactionType == HitReactionType.Getup)
            {
                _characterTotalHitReactions.getUp_Anims.Add(newHitAnimField);
                continue;
            }
            else
            {
                if (responseAnim._hitResponseLevel == HitLevel.SlightKnockback ^ responseAnim._hitResponseLevel == HitLevel.MediumKnockback)
                {
                    if (!responseAnim.groundedReaction)
                    {
                        _characterTotalHitReactions.AirlightReactions.Add(newHitAnimField);
                        continue;
                    }
                    else 
                    {
                        if (!responseAnim.isLowResponse)
                        {
                            _characterTotalHitReactions.lightHighReactions.Add(newHitAnimField);
                            continue;
                        }
                        else
                        {
                            _characterTotalHitReactions.lightLowReactions.Add(newHitAnimField);
                            continue;
                        }
                    }
                }
                else
                {
                    _characterTotalHitReactions.bigReactions.Add(newHitAnimField);
                    continue;
                }
            }
        }
        reactionFunctionDictionary = new Dictionary<HitLevel, Callback>();
        for (int i = 0; i < Enum.GetNames(typeof(HitLevel)).Length; i++) 
        {
            HitLevel curHitReaction = (HitLevel)i;
            if (curHitReaction == HitLevel.SlightKnockback ^ curHitReaction == HitLevel.MediumKnockback)
            {
                reactionFunctionDictionary.Add(curHitReaction, SmallHitDetect);
                continue;
            }
            else 
            {
                reactionFunctionDictionary.Add(curHitReaction, BigHitDetect);
                continue;
            }
        }
        recoverRoutine = null;
        activeHitResponseRoutine = null;
    }
    bool IsBlocking()
    {
        IState currentState = _base._cStateMachine._playerState.current.State;
        bool blocking = currentState == _base._cStateMachine.crouchBlockRef ^ currentState == _base._cStateMachine.standBlockRef;
        return blocking;
    }

    List<HitAnimationField> CheckCurrentHitState() 
    {
        if (!_base._cHurtBox.IsGrounded())
        {
            return _characterTotalHitReactions.AirlightReactions;
        }
        else 
        {
            if (_base._cHitboxManager.GetCurrentHitbox().HBType == HitBoxType.Low)
            {
                return _characterTotalHitReactions.lightLowReactions;
            }
            else 
            {
                return _characterTotalHitReactions.lightHighReactions;
            }
        }
    }

    void SmallHitDetect()
    {
        if (IsBlocking())
        {
            BlockHitDetect();
            return;
        }
        if (lockMoveTypes.Contains(currentProperty._moveType)) 
        {
            LockHitDetect();
            return;
        }
        List<HitAnimationField> reactionList = CheckCurrentHitState();
        int randomHitReaction = 0;
        if (reactionList.Count > 1) 
        {
            randomHitReaction = UnityEngine.Random.Range(0, reactionList.Count);
        }
        HitAnimationField hitReaction = reactionList[randomHitReaction];
        CheckAndStartHitResponse(hitReaction);
    }
    void BigHitDetect() 
    {
        if (IsBlocking())
        {
            BlockHitDetect();
            return;
        }
        if (lockMoveTypes.Contains(currentProperty._moveType))
        {
            LockHitDetect();
            return;
        }
        int randomHitReaction = UnityEngine.Random.Range(0, _characterTotalHitReactions.bigReactions.Count);
        HitAnimationField hitReaction = _characterTotalHitReactions.bigReactions[randomHitReaction];
        //CheckAndStartHitResponse();
    }
    void LockHitDetect()
    {
        //CheckAndStartHitResponse();
    }
    void BlockHitDetect()
    {
        //CheckAndStartHitResponse();
    }
    void CheckAndStartHitResponse(HitAnimationField curField)
    {
        if (activeHitResponseRoutine != null)
        {
            StopCoroutine(activeHitResponseRoutine);
        }
        activeHitResponseRoutine = DoHitResponse(curField);
        StartCoroutine(activeHitResponseRoutine);
    }
    IEnumerator DoHitResponse(HitAnimationField curField)
    {
        recoverTrigger = false;
        float waitTime = (currentProperty.hitstunValue * (1 / 60f));
        float oneFrame = (1 / 60f);
        while (waitTime > 0) 
        {
            waitTime -= oneFrame;
            yield return new WaitForSeconds(waitTime);
        }
        if (curField.hitReactionType == HitReactionType.KnockdownHit)
        {
            if (recoverRoutine != null)
            {
                StopCoroutine(recoverRoutine);
            }
            recoverRoutine = DoRecovery(currentProperty.KnockDown);
            StartCoroutine(recoverRoutine);
        }
        else
        {
            if (smallHitRecovering)
            {
                smallHitRecovering = false;
            }
            if (crouchBlocking)
            {
                crouchBlocking = false;
            }
            if (standingBlocking)
            {
                standingBlocking = false;
            }
            if (airRecoverPossible)
            {
                airRecoverPossible = false;
            }
            _base._cAnimator.SetCanRecover(true);
            _base._cHurtBox.SetHurboxState();
        }
    }
    #region Successful Hit Code
    void MakeRecoverable(float frameCount)
    {
        Debug.Log(frameCount * 60 + "on init");
        _cAnimator.isHit = false; 
        _base._cAnimator.SetCanRecover(true);
        recoverTrigger = true;
    }
    public async Task ReactToHit(ResponseAnim_Base hitanim, Attack_BaseProperties attackProperty)
    {
        recoverTrigger = false;
        float waitTime = hitanim.actionableHitPointInFrames[0];

        float hitstunInFrames = (attackProperty.hitstunValue * (1 / 60f));
        float totalAnimTimePlusHitStun = hitanim.animLength + hitstunInFrames;

        float frameCount = 0;
        recoveryTime += totalAnimTimePlusHitStun;
        while (frameCount <= totalAnimTimePlusHitStun)
        {
            if (frameCount >= (waitTime + hitstunInFrames) && recoverTrigger == false)
            {
                MakeRecoverable(frameCount);
            }
            frameCount += (1 / 60f);
            await Task.Delay((int)((1 / 60f) * 1000f));
        }
        while (_base._cAnimator._canRecover == true)
        {
            if (_base._cAnimator._canRecover == true)
            {
                _base._cAnimator.SetCanRecover(false);
            }
            await Task.Delay((int)((1 / 60f) * 1000f));
        }
        if (!_base._cAnimator._canRecover && !_base._cAnimator.isHit)
        {
            await RecoverAfterHit();
        }
    }
    public void HandleHitState(Attack_BaseProperties attackProperty)
    {
        smallHitRecovering = false;
        bigHitRecovering = false;
        airRecoverPossible = false;
        hasRecovered = false;
        _cAnimator.isHit = true;
        currentProperty = attackProperty;
        Callback funcCall = null;

        if (reactionFunctionDictionary.TryGetValue(currentProperty.hitLevel, out funcCall))
        {
            funcCall();
        }
    }
    /*async void SetGroundedHitReaction(Attack_BaseProperties attackProperty)
    {
        switch (attackProperty.hitLevel)
        {
            case HitLevel.SlightKnockback:
                smallHitRecovering = true;
                //ApplyHitInfo(attackProperty, _characterHitAnimations.ground_hitAnims, 0);
                //await ReactToHit(_characterHitAnimations.ground_hitAnims[0], attackProperty);
                break;
            case HitLevel.MediumKnockback:
                smallHitRecovering = true;
                //ApplyHitInfo(attackProperty, _characterHitAnimations.ground_hitAnims, 1);
                //await ReactToHit(_characterHitAnimations.ground_hitAnims[1], attackProperty);
                break;
            case HitLevel.SoaringHit:
                bigHitRecovering = true;
                _base._cHurtBox.ChangeHeightOnDowned();
                _base._cHurtBox.SetHitboxSize(HurtBoxSize.Downed);
                //ApplyHitInfo(attackProperty, _characterHitAnimations.ground_hitAnims, 2);
                //await ReactToHit(_characterHitAnimations.ground_hitAnims[2], attackProperty);
                break;
            case HitLevel.Crumple:
                bigHitRecovering = true;
                _base._cHurtBox.ChangeHeightOnDowned();
                //ApplyHitInfo(attackProperty, _characterHitAnimations.ground_hitAnims, 3);
               // await ReactToHit(_characterHitAnimations.ground_hitAnims[3], attackProperty);
                break;
            case HitLevel.Spiral:
                bigHitRecovering = true;
                _base._cHurtBox.ChangeHeightOnDowned();
                _base._cHurtBox.SetHitboxSize(HurtBoxSize.Downed);
                //ApplyHitInfo(attackProperty, _characterHitAnimations.ground_hitAnims, 4);
                //await ReactToHit(_characterHitAnimations.ground_hitAnims[4], attackProperty);
                break;
            default:
                Debug.LogError($"{attackProperty.hitLevel} type hit provided not accounted for in switch case");
                Debug.Break();
                break;
        }
    }
    async void SetAirHitReaction(Attack_BaseProperties attackProperty)
    {
        airRecoverPossible = true;
        switch (attackProperty.hitLevel)
        {
            case HitLevel.SlightKnockback:
                smallHitRecovering = true;
                //ApplyHitInfo(attackProperty, _characterHitAnimations.air_HitAnims, 0);
                //await ReactToHit(_characterHitAnimations.air_HitAnims[0], attackProperty);
                break;
            case HitLevel.MediumKnockback:
                smallHitRecovering = true;
                _base._cHurtBox.SetHitboxSize(HurtBoxSize.Downed);
               // ApplyHitInfo(attackProperty, _characterHitAnimations.air_HitAnims, 1);
                //await ReactToHit(_characterHitAnimations.air_HitAnims[1], attackProperty);
                break;
            case HitLevel.SoaringHit:
                bigHitRecovering = true;
                _base._cHurtBox.ChangeHeightOnDowned();
                _base._cHurtBox.SetHitboxSize(HurtBoxSize.Downed);
                //ApplyHitInfo(attackProperty, _characterHitAnimations.air_HitAnims, 1);
                //await ReactToHit(_characterHitAnimations.air_HitAnims[1], attackProperty);
                break;
            case HitLevel.Crumple:
                bigHitRecovering = true;
                airRecoverPossible = false;
                _base._cHurtBox.ChangeHeightOnDowned();
                _base._cHurtBox.SetHitboxSize(HurtBoxSize.Downed);
                //ApplyHitInfo(attackProperty, _characterHitAnimations.air_HitAnims, 1, Attack_KnockDown.SKD);
                //await ReactToHit(_characterHitAnimations.air_HitAnims[1], attackProperty);
                break;
            case HitLevel.Spiral:
                bigHitRecovering = true;
                airRecoverPossible = false;
                _base._cHurtBox.ChangeHeightOnDowned();
                _base._cHurtBox.SetHitboxSize(HurtBoxSize.Downed);
                //ApplyHitInfo(attackProperty, _characterHitAnimations.ground_hitAnims, 4, Attack_KnockDown.SKD);
                //await ReactToHit(_characterHitAnimations.ground_hitAnims[4], attackProperty);
                break;
            default:
                Debug.LogError($"{attackProperty.hitLevel} type hit provided not accounted for in switch case");
                Debug.Break();
                break;
        }
    }
    public void ApplyHitInfo(Attack_BaseProperties attackProperty, List<ResponseAnim_Base> animation ,int level, Attack_KnockDown possibleKnockdown = Attack_KnockDown.NONE) 
    {
        animation[level].PlayAnimation(_cAnimator, animation[level].animName);
        if(possibleKnockdown != Attack_KnockDown.NONE)
        {
            last_KD = possibleKnockdown;
        }
        else 
        {
            last_KD = attackProperty.KnockDown;
        }
    }*/
    public bool ReturnNotRecovering() 
    {
        bool notRecovering = !bigHitRecovering && !smallHitRecovering;
        return notRecovering;
    }
    public async Task RecoverAfterHit()
    {
        while (recoveryTime > 0)
        {
            await Task.Delay((int)((1 / 60f) * 1000f));
        }
        if (last_KD == Attack_KnockDown.NONE)
        {
            StartCoroutine(RegularHitRecover());
        }
        else
        {
            StartCoroutine(DoGetUp());
        }

    }

    IEnumerator RegularHitRecover()
    {
        yield return null;
        if (airRecoverPossible)
        {
            if (!hasRecovered)
            {
                hasRecovered = true;
                _cAnimator.SetShake(false);
                //_characterHitAnimations.air_RecoverAnims[0].PlayAnimation(_cAnimator, _characterHitAnimations.air_RecoverAnims[0].animName);
               // _base._cHurtBox.ChangeHeightOnStanding(_characterHitAnimations.air_RecoverAnims[0].actionableHitPointInFrames[0]);
                //yield return new WaitForSeconds(_characterHitAnimations.air_RecoverAnims[0].actionableHitPointInFrames[0]);
                _cAnimator.EndShake();
                _cAnimator.isHit = false;
                airRecoverPossible = false;
               // yield return new WaitForSeconds(_characterHitAnimations.air_RecoverAnims[0].timeDifference[0]);
                _cAnimator.isHit = false;
                SetHitStateFalse();
            }
        }
        else 
        {
            if (!hasRecovered)
            {
                hasRecovered = true;
                _cAnimator.SetShake(false);
                _cAnimator.EndShake();
                _cAnimator.isHit = false;
                airRecoverPossible = false;
                _cAnimator.isHit = false;
                SetHitStateFalse();
            }
        }
    }

    IEnumerator DoRecovery(Attack_KnockDown knockDownType) 
    {
        if (knockDownType == Attack_KnockDown.HKD) 
        {
        }
        if (knockDownType == Attack_KnockDown.SKD)
        {

        }
        yield return null;
        if (bigHitRecovering)
        {
            bigHitRecovering = false;
        }
        recoverRoutine = null;
    }
    IEnumerator DoGetUp()
    {
        yield return null; 
        if (_base._cHurtBox.IsGrounded())
        {
            _cAnimator.SetShake(false);
            SetHurtboxOnHit(last_KD);

            if (!hasRecovered)
            {
                if (last_KD == Attack_KnockDown.SKD)
                {
                    hasRecovered = true;
                    //_characterHitAnimations.getUp_Anims[0].PlayAnimation(_cAnimator, _characterHitAnimations.getUp_Anims[0].animName);
                    //yield return new WaitForSeconds(_characterHitAnimations.getUp_Anims[1].actionableHitPointInFrames[0]);
                    _base._cAnimator.myAnim.SetTrigger("SKD");
                   // _base._cHurtBox.ChangeHeightOnStanding(_characterHitAnimations.getUp_Anims[1].actionableHitPointInFrames[0]);
                    _cAnimator.EndShake();
                    _cAnimator.isHit = false;
                   // yield return new WaitForSeconds(_characterHitAnimations.getUp_Anims[1].timeDifference[0]);
                    _cAnimator.isHit = false;
                    SetHitStateFalse();
                }
                else if (last_KD == Attack_KnockDown.HKD)
                {
                    hasRecovered = true;
                    //_characterHitAnimations.getUp_Anims[0].PlayAnimation(_cAnimator);
                    //yield return new WaitForSeconds(_characterHitAnimations.getUp_Anims[2].actionableHitPointInFrames[0]);
                    //_base._cAnimator.myAnim.SetTrigger("HKD");
                   // _base._cHurtBox.ChangeHeightOnStanding(_characterHitAnimations.getUp_Anims[0].actionableHitPointInFrames[0]);
                    _cAnimator.EndShake();
                    _cAnimator.isHit = false;
                    //yield return new WaitForSeconds(_characterHitAnimations.getUp_Anims[2].timeDifference[0]);
                    _cAnimator.isHit = false;
                    SetHitStateFalse();
                }
            }
        }
        else
        {
            if (!hasRecovered)
            {
                StartCoroutine(RecallGetUp());
            }
        }
    }
    IEnumerator RecallGetUp() 
    {
        yield return new WaitForSeconds(1 / 60f);
        StartCoroutine(DoGetUp());
    }
    public void SetHitStateFalse()
    {
        if(smallHitRecovering) 
        {
            smallHitRecovering = false;
        }
        if (bigHitRecovering)
        {
            bigHitRecovering = false;
        }
        _base._cAnimator.SetCanRecover(false);
        _base._cHurtBox.SetHurboxState();
    }
    public void SetHurtboxOnHit(Attack_KnockDown knockDown)
    {
        switch (knockDown) 
        {
            case Attack_KnockDown.HKD:
                _base._cHurtBox.SetHurboxState(HurtBoxType.HardKnockdown);
                break;
            case Attack_KnockDown.SKD:
                _base._cHurtBox.SetHurboxState(HurtBoxType.SoftKnockdown);
                break;
        }
    }
    #endregion


    public async void HandleBlockState(Attack_BaseProperties attackProperty)
    {
        standingBlocking = true;
        switch (attackProperty.hitLevel)
        {
            case HitLevel.SlightKnockback:
                smallHitRecovering = true;
                //ApplyHitInfo(attackProperty, _characterHitAnimations.standingblock_Anims, 0);
                //await ReactToBlock(_characterHitAnimations.standingblock_Anims[0], attackProperty);
                break;
            case HitLevel.MediumKnockback:
                smallHitRecovering = true;
                //ApplyHitInfo(attackProperty, _characterHitAnimations.standingblock_Anims, 1);
                //await ReactToBlock(_characterHitAnimations.standingblock_Anims[0], attackProperty);
                break;
            case HitLevel.SoaringHit:
                smallHitRecovering = true;
                //ApplyHitInfo(attackProperty, _characterHitAnimations.standingblock_Anims, 2);
                //await ReactToBlock(_characterHitAnimations.standingblock_Anims[0], attackProperty);
                break;
            case HitLevel.Crumple:
                bigHitRecovering = true;
                //ApplyHitInfo(attackProperty, _characterHitAnimations.standingblock_Anims, 3);
                //await ReactToBlock(_characterHitAnimations.standingblock_Anims[1], attackProperty);
                break;
            case HitLevel.Spiral:
                bigHitRecovering = true;
                //ApplyHitInfo(attackProperty, _characterHitAnimations.standingblock_Anims, 4);
                //await ReactToBlock(_characterHitAnimations.standingblock_Anims[1], attackProperty);
                break;
            default:
                Debug.LogError($"{attackProperty.hitLevel} type hit provided not accounted for in switch case");
                Debug.Break();
                break;
        }

    }
    /*
    public async Task ReactToBlock(ResponseAnim_Base hitanim, Attack_BaseProperties attackProperty)
    {
        recoverTrigger = false;
        float waitTime = hitanim.actionableHitPointInFrames[0];

        float hitstunInFrames = (attackProperty.hitstunValue * (1 / 60f)) /10f;
        float totalAnimTimePlusHitStun = hitanim.animLength + hitstunInFrames;

        float frameCount = 0;
        recoveryTime += totalAnimTimePlusHitStun;
        while (frameCount <= totalAnimTimePlusHitStun)
        {
            if (frameCount >= (waitTime + hitstunInFrames) && recoverTrigger == false)
            {
                MakeRecoverable(frameCount);
            }
            frameCount += (1 / 60f);
            await Task.Delay((int)((1 / 60f) * 1000f));
        }
        while (_base._cAnimator._canRecover == true)
        {
            if (_base._cAnimator._canRecover == true)
            {
                _base._cAnimator.SetCanRecover(false);
            }
            await Task.Delay((int)((1 / 60f) * 1000f));
        }
        if (!_base._cAnimator._canRecover && !_base._cAnimator.isHit)
        {
            await RecoverAfterBlock();
        }
    }*/
    public bool ReturnStandBlock()
    {
        bool notRecovering = standingBlocking;
        return notRecovering;
    }
    public bool ReturnCrouchBlock()
    {
        bool notRecovering = crouchBlocking;
        return notRecovering;
    }
    public async Task RecoverAfterBlock()
    {
        while (recoveryTime > 0)
        {
            await Task.Delay((int)((1 / 60f) * 1000f));
        }
        hasRecovered = false;
        StartCoroutine(BlockRecover());
    }
    public void SetBlockStateFalse()
    {
        if (smallHitRecovering)
        {
            smallHitRecovering = false;
        }
        if (bigHitRecovering)
        {
            bigHitRecovering = false;
        }
        if (standingBlocking)
        {
            standingBlocking = false;
            _base._cAnimator.SetCanRecover(false);
            _base._cHurtBox.SetHurboxState(HurtBoxType.BlockHigh);
        }
        if (crouchBlocking)
        {
            crouchBlocking = false;
            _base._cAnimator.SetCanRecover(false);
            _base._cHurtBox.SetHurboxState(HurtBoxType.BlockLow);
        }
    }
    IEnumerator BlockRecover()
    {
        yield return null;
        if (standingBlocking)
        {
            if (!hasRecovered)
            {
                hasRecovered = true;
                _cAnimator.SetShake(false);
                _cAnimator.EndShake();
                _cAnimator.isHit = false;
                //yield return new WaitForSeconds(_characterHitAnimations.standingblock_Anims[0].timeDifference[0]);
                SetBlockStateFalse();
            }
        }
        else if (crouchBlocking) 
        {
            if (!hasRecovered)
            {
                hasRecovered = true;
                _cAnimator.SetShake(false);
                _cAnimator.EndShake();
                _cAnimator.isHit = false;
                //yield return new WaitForSeconds(_characterHitAnimations.crouchingblock_Anims[0].timeDifference[0]);
                SetBlockStateFalse();
            }
        }
        else
        {
            if (!hasRecovered)
            {
                hasRecovered = true;
                _cAnimator.SetShake(false);
                _cAnimator.EndShake();
                _cAnimator.isHit = false;
                SetBlockStateFalse();
            }
        }
    }
}

[Serializable]
public class HitAnimationHolder
{
    public List<HitAnimationField> AirlightReactions;
    public List<HitAnimationField> lightHighReactions;
    public List<HitAnimationField> lightLowReactions;
    public List<HitAnimationField> bigReactions;
    public List<HitAnimationField> getUp_Anims;
    public List<HitAnimationField> block_Anims;
    public void Setup() 
    {
        AirlightReactions = new List<HitAnimationField>();
        lightHighReactions = new List<HitAnimationField>();
        lightLowReactions = new List<HitAnimationField>();
        bigReactions = new List<HitAnimationField>();
        getUp_Anims = new List<HitAnimationField>();
        block_Anims = new List<HitAnimationField>();
    }
}


[Serializable]
public class HitAnimationField 
{
    public AnimationClip anim;
    public float animLength;
    public string animName;
    public int animHash;
    public HitReactionType hitReactionType;
    public HitLevel hitLevel;
    public bool isGroundedReaction;
    public bool isLowReaction;
    public HitAnimationField(AnimationClip _anim, float _animLength, string _animName,HitReactionType _hitReactionType, HitLevel _hitLevel, bool _isGroundedReaction = true, bool _isLowReaction = false) 
    {
        anim = _anim;
        animLength = _animLength;
        animName = _animName;
        animHash = Animator.StringToHash(animName);
        hitReactionType = _hitReactionType;
        hitLevel = _hitLevel;
        isGroundedReaction = _isGroundedReaction;
        isLowReaction = _isLowReaction;
    }
}
[Serializable]
public enum HitReactionType 
{
    StandardHit = 0,
    KnockdownHit = 1,
    StandardBlock = 2,
    GuardBreakBlock = 3,
    Getup = 4,
}