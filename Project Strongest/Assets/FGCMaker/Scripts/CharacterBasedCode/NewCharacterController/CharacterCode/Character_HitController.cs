using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using System;
using System.Linq;

public class Character_HitController : MonoBehaviour
{
    [SerializeField] private Character_Animator _cAnimator;
    [SerializeField] private Character_Base _base;
    //[SerializeField] private HitAnimationHolder _characterTotalHitReactions;
    [SerializeField] private HitAnimationHolder2 characterTotalHitReactions;
    Dictionary<HitLevel, Callback<Attack_BaseProperties>> reactionFunctionDictionary;

    bool recoverTrigger;
    public bool smallHitRecovering, bigHitRecovering, airRecoverPossible;
    public bool crouchBlocking, standingBlocking;
    private bool hasRecovered;
    public Attack_KnockDown last_KD;
    private float recoveryTime;
    Attack_BaseProperties currentProperty;
    IEnumerator activeHitResponseRoutine, recoverRoutine;
    public float currentHitstun, hitStunScaling;


    List<MoveType> lockMoveTypes = new List<MoveType>()
    {
        MoveType.Counter,
        MoveType.CommandGrab,
        MoveType.Throw,
        MoveType.Super,
    };

    List<HitLevel> blockTypes = new List<HitLevel>()
    {
        HitLevel.StandardBlock,
        HitLevel.GuardBreak,
    };
    public void Start()
    {
        hitStunScaling = 0;
        currentHitstun = 0;
        recoveryTime = 0;
    }
    public bool ReturnNotRecovering()
    {
        bool notRecovering = !bigHitRecovering && !smallHitRecovering;
        return notRecovering;
    }
    public void SetHitReactions(Character_Animator myAnim, HitReactions newHitReactions)
    {
        SetAnimator(myAnim);
    }
    public void SetAnimator(Character_Animator myAnim)
    {
        _cAnimator = myAnim;
        SetUpHitAnimations();
    }
    void SetUpHitAnimations()
    {
        characterTotalHitReactions.Setup();
        for (int i = 0; i < _base.characterProfile.properHitResponseAnimations.Count; i++)
        {
            HitAnimationField curHitAnim = _base.characterProfile.properHitResponseAnimations[i];
            curHitAnim.DoAnimationInfoSetup();
            if (curHitAnim.hitReactionType == HitReactionType.Getup)
            {
                characterTotalHitReactions.getUpReactions.Add(_base.characterProfile.properHitResponseAnimations[i]);
                continue;
            }
            if (curHitAnim.hitReactionType == HitReactionType.StandardBlock || curHitAnim.hitReactionType == HitReactionType.GuardBreakBlock) 
            {
                characterTotalHitReactions.blockReactions.Add(_base.characterProfile.properHitResponseAnimations[i]);
                continue;
            }
            else 
            {
                characterTotalHitReactions.hitReactions.Add(_base.characterProfile.properHitResponseAnimations[i]);
                continue;
            }
        }

        reactionFunctionDictionary = new Dictionary<HitLevel, Callback<Attack_BaseProperties>>();
        reactionFunctionDictionary.Add(HitLevel.SlightKnockback, SmallHitDetect);
        reactionFunctionDictionary.Add(HitLevel.MediumKnockback, SmallHitDetect);
        reactionFunctionDictionary.Add(HitLevel.SoaringHit, BigHitDetect);
        reactionFunctionDictionary.Add(HitLevel.Spiral, BigHitDetect);
        reactionFunctionDictionary.Add(HitLevel.Crumple, BigHitDetect);
        reactionFunctionDictionary.Add(HitLevel.Scorpion, BigHitDetect);
        reactionFunctionDictionary.Add(HitLevel.StandardBlock, BlockHitDetect);
        reactionFunctionDictionary.Add(HitLevel.GuardBreak, BlockHitDetect);
        recoverRoutine = null;
        activeHitResponseRoutine = null;
    }
    bool IsBlocking()
    {
        IState currentState = _base._cStateMachine._playerState.current.State;
        bool blocking = currentState == _base._cStateMachine.crouchBlockRef ^ currentState == _base._cStateMachine.standBlockRef;
        return blocking;
    }

    List<HitAnimationField> FilterHitReactions(Attack_BaseProperties currentAttack)
    {
        List<HitAnimationField> refField = new List<HitAnimationField>(characterTotalHitReactions.hitReactions);
        bool IsGrounded = _base._cHurtBox.IsGrounded();
        for (int i = 0; i < refField.Count; i++)
        {
            bool falseGroundedResponse = IsGrounded && !refField[i].isGroundedReaction;
            bool falseLowedResponse = refField[i].isLowReaction && currentAttack.AttackAnims.attackType != HitBoxType.Low;
            if (!(currentAttack.hitLevel.HasFlag(refField[i].hitLevel)))
            {
                refField[i] = null;
                continue;
            }
            if (falseGroundedResponse)
            {
                refField[i] = null;
                continue;
            }
            if (!falseGroundedResponse)
            {
                refField[i] = null;
                continue;
            }
            if (falseLowedResponse)
            {
                refField[i] = null;
                continue;
            }
            if (!falseLowedResponse)
            {
                refField[i] = null;
                continue;
            }
            if (!currentAttack.hitLevel.HasFlag(HitLevel.Spiral) && refField[i].hitLevel == HitLevel.Spiral)
            {
                refField[i] = null;
                continue;
            }
            if (refField[i].hitLevel == HitLevel.Crumple && !currentAttack.hitLevel.HasFlag(HitLevel.Crumple))
            {
                refField[i] = null;
                continue;
            }
        }
        for (int i = refField.Count-1; i >= 0; i--)
        {
            if (refField[i] == null)
            {
                refField.RemoveAt(i);
            }
        }

        return refField;
    }

    HitAnimationField FilterGroundLockReactions(CustomDamageField currentAttack)
    {
        List<HitAnimationField> refField = new List<HitAnimationField>(characterTotalHitReactions.hitReactions);
        for (int i = 0; i < refField.Count; i++)
        {
            if (!refField[i].hitLevel.HasFlag(currentAttack.hitLevel))
            {
                refField[i] = null;
                continue;
            }
        }
        for (int i = refField.Count - 1; i >= 0; i--)
        {
            if (refField[i] == null)
            {
                refField.RemoveAt(i);
            }
        }

        return refField[0];
    }
    List<HitAnimationField> FilterBlockReactions(Attack_BaseProperties currentAttack)
    {
        List<HitAnimationField> refField = characterTotalHitReactions.hitReactions;
        for (int i = 0; i < refField.Count; i++)
        {
            if (!(currentAttack.hitLevel.HasFlag(refField[i].hitLevel)))
            {
                refField.Remove(refField[i]);
                continue;
            }
            if(currentAttack.AttackAnims.attackType != HitBoxType.Low && refField[i].isLowReaction) 
            {
                refField.Remove(refField[i]);
            }
        }
        return refField;
    }

    void SmallHitDetect(Attack_BaseProperties currentAttack = null)
    {
        List<HitAnimationField> hitReactionList = FilterHitReactions(currentAttack);
        int randomHitReaction = 0;
        if (hitReactionList.Count > 1)
        {
            randomHitReaction = UnityEngine.Random.Range(0, hitReactionList.Count);
        }
        HitAnimationField hitReaction = hitReactionList[randomHitReaction];
        CheckAndStartHitResponse(hitReaction);
    }
    void BigHitDetect(Attack_BaseProperties currentAttack = null)
    {
        if (currentAttack.hitLevel.HasFlag(HitLevel.Crumple))
        {

        }
        else 
        {
            List<HitAnimationField> hitReactionList = FilterHitReactions(currentAttack);
            int randomHitReaction = 0;
            if (hitReactionList.Count > 1)
            {
                randomHitReaction = UnityEngine.Random.Range(0, hitReactionList.Count);
            }
            HitAnimationField hitReaction = hitReactionList[randomHitReaction];
            CheckAndStartHitResponse(hitReaction);
        }
    }
    void LockHitDetect(CustomDamageField currentAttack, bool finalAttack)
    {
        HitAnimationField hitReaction = FilterGroundLockReactions(currentAttack);
        if (hitReaction != null)
        {
            if (finalAttack) 
            {
                CheckAndStartHitResponse(hitReaction);
            }
            else 
            {
                CallLockedHitResponse(hitReaction);
            }
        }
    }
    void BlockHitDetect(Attack_BaseProperties currentAttack)
    {
        List<HitAnimationField> blockReactionList = FilterBlockReactions(currentAttack);
        int randomHitReaction = 0;
        if (blockReactionList.Count > 1)
        {
            randomHitReaction = UnityEngine.Random.Range(0, blockReactionList.Count);
        }
        HitAnimationField blockReaction = blockReactionList[randomHitReaction];
        CheckAndStartHitResponse(blockReaction);
    }
    void CallLockedHitResponse(HitAnimationField curField)
    {
        _base._cAnimator.PlayNextAnimation(curField.animHash, 0, true);
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
    void ClearRecoveryRoutine()
    {
        if (recoverRoutine != null)
        {
            StopCoroutine(recoverRoutine);
        }
    }
    IEnumerator DoHitResponse(HitAnimationField curField)
    {
        ClearRecoveryRoutine();
        recoverTrigger = false;
        float oneFrame = (1 / 60f);
        float hitStunInFrames = curField.animLength + (currentHitstun * oneFrame);
        _base._cAnimator.PlayNextAnimation(curField.animHash,0,true);
        _base._cAnimator.SetShake(true);
        _base._cAnimator.SetCanRecover(true);
        while (hitStunInFrames > 0) 
        {
            hitStunInFrames -= (oneFrame);// + hitStunScaling);
            yield return new WaitForSeconds(oneFrame);
        }
        _base._cAnimator.EndShake();
        if (curField.hitReactionType == HitReactionType.KnockdownHit)
        {
            ClearRecoveryRoutine();
            recoverRoutine = DoRecovery(currentProperty.KnockDown);
            StartCoroutine(recoverRoutine);
        }
        else
        {
            if (recoverRoutine != null)
            {
                StopCoroutine(recoverRoutine);
            }
            if (smallHitRecovering)
            {
                smallHitRecovering = false;
            }
            if (airRecoverPossible)
            {
                airRecoverPossible = false;
            }
            SetRecoverable();
            _base._cHurtBox.SetHurboxState();
        }
    }
    #region Successful Hit Code
    void SetRecoverable()
    {
        _cAnimator.isHit = false; 
        _base._cAnimator.SetCanRecover(false);
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
                //MakeRecoverable(frameCount);
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
    public void HandleHitState(Attack_BaseProperties currentAttack, float StunValue, float calculatedScaling, bool blockedAttack)
    {
        smallHitRecovering = false;
        bigHitRecovering = false;
        airRecoverPossible = false;
        hasRecovered = false;
        _cAnimator.isHit = true;
        currentProperty = currentAttack;
        Callback<Attack_BaseProperties> funcCall = null;
        currentHitstun = StunValue;
        hitStunScaling = calculatedScaling;
        SearchHitResponseDictionary(currentAttack, blockedAttack);
       /* if (reactionFunctionDictionary.TryGetValue(currentProperty.hitLevel, out funcCall))
        {
            funcCall(currentAttack);
        }*/
    }
    public void ForceCustomLockAnim(CustomDamageField currentAttack, bool finalAttack)
    {
        LockHitDetect(currentAttack, finalAttack);
    }
    void SearchHitResponseDictionary(Attack_BaseProperties attackHitLevel,bool blockedAttack)
    {
        KeyValuePair<HitLevel, Callback<Attack_BaseProperties>> entry = new KeyValuePair<HitLevel, Callback<Attack_BaseProperties>>();
        for (int i = 0; i < reactionFunctionDictionary.Count; i++)
        {
            entry = reactionFunctionDictionary.ElementAt(i);
            if (!attackHitLevel.hitLevel.HasFlag(entry.Key)) 
            {
                continue;
            }
            else 
            {
                if (blockedAttack)
                {
                    if (blockTypes.Contains(entry.Key))
                    {
                        entry.Value(currentProperty);
                    }
                    continue;
                }
                else 
                {
                    if (!blockTypes.Contains(entry.Key))
                    {
                        entry.Value(currentProperty);
                    }
                    continue;
                }
            }
        }
    }



    public async Task RecoverAfterHit()
    {
        while (recoveryTime > 0)
        {
            //await Task.Delay((int)((1 / 60f) * 1000f));
        }
        if (last_KD == Attack_KnockDown.NONE)
        {
            //StartCoroutine(RegularHitRecover());
        }
        else
        {
            //StartCoroutine(DoGetUp());
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
    HitAnimationField CheckRecoveryAnim(bool isGrounded = true)
    {
       /* List<HitAnimationField> refField = characterTotalHitReactions.getUpReactions;
        for (int i = 0; i < refField.Count; i++) 
        {
            if (!_base._cHurtBox.IsGrounded())
            {
                if (currentProperty.KnockDown.HasFlag(Attack_KnockDown.SKD))
                {
                    if (refField[i].isGroundedReaction == true)
                    {
                        refField.Remove(refField[i]);
                        continue;
                        if (currentProperty.KnockDown == Attack_KnockDown.NONE)
                        {
                            return _characterTotalHitReactions.getUp_Anims[i];
                        }
                        else
                        {
                            if (_characterTotalHitReactions.getUp_Anims[i].knockdownAnimType == currentProperty.KnockDown)
                            {
                                return _characterTotalHitReactions.getUp_Anims[i];
                            }
                        }
                    }
                }
            }
            break;
        }
        if (currentProperty.KnockDown != Attack_KnockDown.HKD)
        {
            return _characterTotalHitReactions.groundedGetUp_Anims[0];
        }
        else
        {
            return _characterTotalHitReactions.groundedGetUp_Anims[1];
        }*/
        return null;
    }

    IEnumerator DoRecovery(Attack_KnockDown knockDownType)
    {
        if (knockDownType.HasFlag(Attack_KnockDown.SKD) || knockDownType.HasFlag(Attack_KnockDown.HKD))
        {
            HitAnimationField recoveryAnim = CheckRecoveryAnim();
            yield return new WaitUntil(() => _base._cHurtBox.IsGrounded());
            _base._cAnimator.PlayNextAnimation(Animator.StringToHash("Landing_After_AirHit"), 0, true);
            yield return new WaitForSeconds(0.4f);
            _base._cAnimator.PlayNextAnimation(recoveryAnim.animHash, 0, true);
            yield return new WaitForSeconds(recoveryAnim.animLength);
            if (bigHitRecovering)
            {
                bigHitRecovering = false;
            }
        }
        else
        {
            HitAnimationField recoveryAnim = CheckRecoveryAnim(_base._cHurtBox.IsGrounded());
            _base._cAnimator.PlayNextAnimation(recoveryAnim.animHash, 0, true);
            yield return new WaitForSeconds(recoveryAnim.animLength);
        }
        recoverRoutine = null;
        SetRecoverable();
        _base._cHurtBox.SetHurboxState();
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
    public HitAnimationField crumpleReaction;
    public List<HitAnimationField> getUp_Anims;
    public List<HitAnimationField> groundedGetUp_Anims;
    public List<HitAnimationField> block_Anims;
    public void Setup() 
    {
        AirlightReactions = new List<HitAnimationField>();
        lightHighReactions = new List<HitAnimationField>();
        lightLowReactions = new List<HitAnimationField>();
        bigReactions = new List<HitAnimationField>();
        getUp_Anims = new List<HitAnimationField>();
        groundedGetUp_Anims = new List<HitAnimationField>();
        block_Anims = new List<HitAnimationField>();
    }
}

[Serializable]
public class HitAnimationHolder2
{
    public List<HitAnimationField> hitReactions;
    public List<HitAnimationField> blockReactions;
    public List<HitAnimationField> getUpReactions;
    public void Setup()
    {
        hitReactions = new List<HitAnimationField>();
        blockReactions = new List<HitAnimationField>();
        getUpReactions = new List<HitAnimationField>();
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
    public Attack_KnockDown knockdownAnimType;
    public HitAnimationField(AnimationClip _anim, float _animLength, string _animName,HitReactionType _hitReactionType, HitLevel _hitLevel, bool _isGroundedReaction = true, bool _isLowReaction = false, Attack_KnockDown _knockdownAnimType = Attack_KnockDown.NONE) 
    {
        anim = _anim;
        animLength = _animLength;
        animName = _animName;
        animHash = Animator.StringToHash(animName);
        hitReactionType = _hitReactionType;
        hitLevel = _hitLevel;
        isGroundedReaction = _isGroundedReaction;
        isLowReaction = _isLowReaction;
        knockdownAnimType = _knockdownAnimType;
    }
    public void DoAnimationInfoSetup() 
    {
        animLength = anim.length;
        animName = anim.name;
        animHash = Animator.StringToHash(animName);
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