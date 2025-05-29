using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using FightingGame_FrameData;

public class Character_HitController : MonoBehaviour
{
    [SerializeField] private Slider _hitStunSlider;
    [SerializeField] private Character_Animator _cAnimator;
    [SerializeField] private Character_Base _base;
    [SerializeField] private HitAnimationHolder characterTotalHitReactions;
    Dictionary<HitLevel, Callback<Attack_BaseProperties>> reactionFunctionDictionary;
    List<Attack_KnockBack_Vertical> knockUpHitTypes = new List<Attack_KnockBack_Vertical>();
    public Attack_KnockDown last_KD;
    private float recoveryTime;
    Attack_BaseProperties currentProperty;
    CustomDamageField currentCustomDamageField;
    IEnumerator activeHitResponseRoutine, recoverRoutine;
    IEnumerator DownedFrameTickRoutine;
    IEnumerator DelayGetupRoutine;
    public float currentHitstun;
    public float currentHitstop;
    public float hitStunAmount;
    public float hitStunScaling;
    public float strunAmountIncrease;

    [SerializeField] private bool smallHitRecovering;
    [SerializeField] private bool bigHitRecovering;
    [SerializeField] private bool airRecoverPossible;
    [SerializeField] private bool _isRecovering;
    [SerializeField] private bool blockedAttack;
    public bool Recovering { get{ return _isRecovering;  } }

    public void SetObjectState(bool state)
    {
        _hitStunSlider.gameObject.SetActive(state);
    }
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
        currentHitstop = 0;
        recoveryTime = 0;
        knockUpHitTypes.Add(Attack_KnockBack_Vertical.KnockUP);
    }
    public bool ReturnNotRecovering()
    {
        bool notRecovering = !bigHitRecovering && !smallHitRecovering;
        return notRecovering;
    }
    public void SetHitReactions(Character_Animator myAnim)
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
                if (curHitAnim._getupType == GetupType.Neutral && curHitAnim.knockdownAnimType != Attack_KnockDown.HKD && curHitAnim.isGroundedReaction)
                {
                    characterTotalHitReactions.neutralGetupReaction = curHitAnim;
                    characterTotalHitReactions.getUpReactions.Add(_base.characterProfile.properHitResponseAnimations[i]);
                    continue;
                }
                if (curHitAnim._getupType == GetupType.Back && curHitAnim.knockdownAnimType != Attack_KnockDown.HKD && curHitAnim.isGroundedReaction)
                {
                    characterTotalHitReactions.backGetupReaction = curHitAnim;
                    characterTotalHitReactions.getUpReactions.Add(_base.characterProfile.properHitResponseAnimations[i]);
                    continue;
                }
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

    #region Filter Animation Functions
    HitAnimationField FilterGroundLockReactions(HitLevel _levelOfAttack = default)
    {
        List<HitAnimationField> refField = new List<HitAnimationField>(characterTotalHitReactions.hitReactions);
        if (currentCustomDamageField != null)
        {
            for (int i = 0; i < refField.Count; i++)
            {
                if (!currentCustomDamageField.hitLevel.HasFlag(refField[i].hitLevel))
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
        }
        else
        {
            for (int i = 0; i < refField.Count; i++)
            {
                if (!_levelOfAttack.HasFlag(refField[i].hitLevel))
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

        }

        return refField[0];
    }
    List<HitAnimationField> FilterBlockReactions(Attack_BaseProperties currentAttack)
    {
        List<HitAnimationField> refField = new List<HitAnimationField>(characterTotalHitReactions.blockReactions);
        List<HitAnimationField> prunedList = new List<HitAnimationField>();
        for (int i = refField.Count-1; i >= 0; i--)
        {
            if (!(currentAttack.hitLevel.HasFlag(refField[i].hitLevel)))
            {
                prunedList.Add(refField[i]);
                continue;
            }
            if (currentAttack.AttackAnims.attackType != HitBoxType.Low)
            {
                if (refField[i].isLowReaction)
                {
                    prunedList.Add(refField[i]);
                    continue;
                }
            }
            if (currentAttack.AttackAnims.attackType == HitBoxType.Low)
            {
                if (!refField[i].isLowReaction)
                {
                    prunedList.Add(refField[i]);
                    continue;
                }
            }
        }
        foreach (HitAnimationField reaction in prunedList) 
        {
            if (refField.Contains(reaction)) 
            {
                refField.Remove(reaction);
            }
        }
        return refField;
    }
    #endregion

    HitAnimationField FindAnimationOfType(Attack_BaseProperties currentAttack = null, CustomDamageField currentCustom = null) 
    {
        List<HitAnimationField> refField = new List<HitAnimationField>(characterTotalHitReactions.hitReactions);
        List<HitAnimationField> prunedList = new List<HitAnimationField>();
        bool lowHit = false;
        bool IsGrounded = false;
        #region For CurrentAttack Variable
        lowHit = currentAttack.AttackAnims.attackType == HitBoxType.Low;
        IsGrounded = _base._cHurtBox.IsGrounded();
        for (int i = 0; i < refField.Count; i++)
        {
            if (currentAttack.hitLevel.HasFlag(refField[i].hitLevel))
            {
                prunedList.Add(refField[i]);// = null;
            }
            continue;
        }
        if (!currentAttack.hitLevel.HasFlag(HitLevel.Spiral))
        {
            for (int i = prunedList.Count - 1; i > -1; i--)
            {
                if (IsGrounded)
                {
                    if (!prunedList[i].isGroundedReaction)
                    {
                        prunedList.RemoveAt(i);
                        continue;
                    }
                }
                else
                {
                    if (prunedList[i].isGroundedReaction)
                    {
                        prunedList.RemoveAt(i);
                        continue;
                    }
                }
                if (lowHit)
                {
                    if (!prunedList[i].isLowReaction)
                    {
                        prunedList.RemoveAt(i);
                        continue;
                    }
                }
                else
                {
                    if (prunedList[i].isLowReaction)
                    {
                        prunedList.RemoveAt(i);
                        continue;
                    }
                }
            }
        }
        if (prunedList.Count == 1)
        {
            return prunedList[0];
        }
        else 
        {
            if (prunedList.Count == 0)
            {
                return null;
            }
            else 
            {
                int randomHitReaction = UnityEngine.Random.Range(0, prunedList.Count);
                return prunedList[randomHitReaction];
            }
        }
        #endregion
    }
    public void DeathHitDetect(Attack_BaseProperties currentAttack = null, CustomDamageField currentDamageField = null)
    {
        ClearRecoveryRoutine(true);
        HitAnimationField hitReaction = null;
        if (currentAttack != null) 
        {
            hitReaction = FindAnimationOfType(currentAttack);
            if (hitReaction == null)
            {
                hitReaction = FilterGroundLockReactions(currentAttack.hitLevel);
            }
        }
        if (currentDamageField != null)
        {
            hitReaction = FilterGroundLockReactions(currentDamageField.hitLevel);
            if (hitReaction == null)
            {
                hitReaction = FilterGroundLockReactions(currentAttack.hitLevel);
            }
        }
        StartCoroutine(DoDeathResponse(hitReaction));
    }
    void SmallHitDetect(Attack_BaseProperties currentAttack = null)
    {
        blockedAttack = false;
        HitAnimationField hitReaction = FindAnimationOfType(currentAttack);
        if (hitReaction == null) 
        {
            Debug.LogError("Null Event Reached");
            Debug.Break();
        }
        CheckAndStartHitResponse(hitReaction);
    }
    void BigHitDetect(Attack_BaseProperties currentAttack = null)
    {
        blockedAttack = false;
        if (lockMoveTypes.Contains(currentAttack._moveType))
        {
            if (_base.opponentPlayer.comboList3_0.GetCurrentSuperCustomAnimLength() > 0)
            {
                ClearRecoveryRoutine(true);
                CallLockedHitResponse(FilterGroundLockReactions(currentAttack.hitLevel));
                return;
            }
            else
            {
                ClearRecoveryRoutine(true);
                CheckAndStartHitResponse(FilterGroundLockReactions(currentAttack.hitLevel));
                return;
            }
        }
        HitAnimationField hitReaction = FindAnimationOfType(currentAttack);
        if (hitReaction == null)
        {
            Debug.LogError("Null Event Reached");
            //Debug.Break();
            hitReaction = FilterGroundLockReactions(currentAttack.hitLevel);
        }
        CheckAndStartHitResponse(hitReaction);
    }
    void LockHitDetect(CustomDamageField currentAttack, bool finalAttack)
    {
        blockedAttack = false;
        currentCustomDamageField = currentAttack;
        ClearRecoveryRoutine(true);
        HitAnimationField hitReaction = FilterGroundLockReactions(currentAttack.hitLevel);
        currentHitstun = (currentAttack.customDamageFieldStunValues.hitstunValue * Base_FrameCode.ONE_FRAME);
        currentHitstop = currentAttack.customDamageFieldStunValues.hitstopValue * Base_FrameCode.ONE_FRAME;
        if (hitReaction != null)
        {
            if (finalAttack) 
            {
                CheckAndStartHitResponse(hitReaction, currentHitstun);
            }
            else 
            {
                CallLockedHitResponse(hitReaction);
            }
        }
    }
    public void BlockHitDetect(Attack_BaseProperties currentAttack)
    {
        blockedAttack = true;
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
        _base._cAnimator.PlayNextAnimation(curField.animHash, 0, true,0.4f,true);
    }
    async void CallHitStopHitResponse(HitAnimationField curField)
    {
        _base._cAnimator.PlayNextAnimation(curField.animHash, 0, true, 4 * Base_FrameCode.ONE_FRAME, true);
    }
    void CheckAndStartHitResponse(HitAnimationField curField, float hitStunOverride = -1f)
    {
        ClearHitResponseRoutine();
        activeHitResponseRoutine = DoHitResponse(curField, hitStunOverride);
        StartCoroutine(activeHitResponseRoutine);
    }
    public void ClearHitResponseRoutine() 
    {
        if (activeHitResponseRoutine != null)
        {
            StopCoroutine(activeHitResponseRoutine);
        }
    }
    public void ClearRecoveryRoutine(bool stopFade)
    {
        if (recoverRoutine != null)
        {
            StopCoroutine(recoverRoutine);
            _isRecovering = false;
        }
        if (stopFade)
        {
            _base._cComboCounter.StopFadeRoutine();
        }
    }
    public void ClearFrameTickRoutine()
    {
        if (DownedFrameTickRoutine != null)
        {
            StopCoroutine(DownedFrameTickRoutine);
            if (currentCustomDamageField == null)
            {
                _base._aFrameDataMeter.SetHitRecoveringState(false);
            }
            DownedFrameTickRoutine = null;
        }
    }
    public void ResetHitstunIncrease() 
    {
        strunAmountIncrease = 1f;
    }
    public void IncreaseHitstunOnAffliction(float increaseValue)
    {
        strunAmountIncrease = increaseValue;
    }
    IEnumerator DoHitResponse(HitAnimationField curField, float overrideStunAmount = -1f)
    {
        ClearFrameTickRoutine();
        if (overrideStunAmount != -1f)
        {
            hitStunAmount = overrideStunAmount;
        }
        else
        {
            hitStunAmount = (currentHitstun * Base_FrameCode.ONE_FRAME);
        }
        ClearRecoveryRoutine(true);
        SetStunMeterValue(hitStunAmount);
        _base._cAnimator.PlayNextAnimation(curField.animHash, 0, true);
        _base._cAnimator.SetCanRecover(true);
        if (strunAmountIncrease > 0) 
        {
            float newHitStun = hitStunAmount + (hitStunAmount * strunAmountIncrease);
            hitStunAmount = newHitStun;
        }
        _base._cHitstun.CallHitStun(hitStunAmount);
        _base._aFrameDataMeter.SetHitRecoveringState(true);
        CallHitStopHitResponse(curField);
        Attack_KnockBack_Vertical currentKnockBack = GetActiveVerticalKnockback(blockedAttack);
        
        while (currentHitstop > 0)
        {
            currentHitstop -= Base_FrameCode.ONE_FRAME;
            _base._aFrameDataMeter.UpdateFrameOnHit(FrameType.HitStop);
            yield return new WaitForSeconds(Base_FrameCode.ONE_FRAME);
        }
        while (hitStunAmount > 0)
        {
            if (_base.ReturnIfPaused())
            {
                yield return new WaitForSeconds(Base_FrameCode.ONE_FRAME);
            }
            else
            {
                if (knockUpHitTypes.Contains(currentKnockBack))
                {
                    yield return new WaitUntil(() => !_base._cHurtBox.IsGrounded());
                    while (!_base._cHurtBox.IsGrounded())
                    {
                        hitStunAmount -= (Base_FrameCode.ONE_FRAME * _base._cHitstun.animSpeed);
                        UpdateMeterValue(Base_FrameCode.ONE_FRAME);
                        yield return new WaitForSeconds(Base_FrameCode.ONE_FRAME);
                    }
                    hitStunAmount = 0;
                    ClearMeterValue();
                }
                hitStunAmount -= (Base_FrameCode.ONE_FRAME * _base._cHitstun.animSpeed);
                UpdateMeterValue(Base_FrameCode.ONE_FRAME);
                yield return new WaitForSeconds(Base_FrameCode.ONE_FRAME);
            }
        }
        hitStunAmount = 0;
        if (curField.hitReactionType == HitReactionType.KnockdownHit)
        {
            ClearRecoveryRoutine(true);
            DownedFrameTickRoutine = UpdateFrameWhileDowned();
            _isRecovering = true;
            HurtBoxType landingBoxType = HurtBoxType.NoBlock;
            if (currentCustomDamageField != null)
            {
                landingBoxType = currentCustomDamageField.KnockDown == Attack_KnockDown.HKD ? HurtBoxType.HardKnockdown : HurtBoxType.SoftKnockdown;
                recoverRoutine = DoRecovery(currentCustomDamageField.KnockDown, curField, false);
            }
            else if (currentProperty != null)
            {
                currentProperty.hitConnected = false;
                landingBoxType = currentProperty.KnockDown == Attack_KnockDown.HKD ? HurtBoxType.HardKnockdown : HurtBoxType.SoftKnockdown;
                recoverRoutine = DoRecovery(currentProperty.KnockDown, curField, false);
            }
            yield return new WaitUntil(() => _base._cHurtBox.IsGrounded());
            _base._cHurtBox.SetHurboxState(landingBoxType);
            StartCoroutine(recoverRoutine);
            StartCoroutine(DownedFrameTickRoutine);
        }
        else
        {
            if (!_base._cHurtBox.IsGrounded())
            {
                yield return new WaitForSeconds(Base_FrameCode.ONE_FRAME);
                HitAnimationField recoveryAnim = CheckRecoveryAnim(curField.knockdownAnimType);
                _base._cAnimator.PlayNextAnimation(recoveryAnim.animHash, 0, true);
            }
            if (currentProperty != null)
            {
                currentProperty.hitConnected = false;
            }
            _base._aFrameDataMeter.SetHitRecoveringState(false);
            _base.CheckAttackActive(false);
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
            currentCustomDamageField = null;
            currentProperty = null;
            currentHitstun = 0;
            CheckStunMeter(); 
        }
    }
    void CheckStunMeter()
    {
        if (_base._cHealth.stunController.IsSTUNNED)
        {
            _base._cHealth.stunController.ActivateStunDelay(OnEndHitStanding);
        }
        else
        {
            OnEndHitStanding();
        }
    }
    void OnEndHitStanding() 
    {
        _base.Activate();
        blockedAttack = false;
        SetRecoverable();
        _base._cHealth.StartHealthRegen();
    }
    IEnumerator DoDeathResponse(HitAnimationField curField)
    {
        _base._cAnimator.PlayNextAnimation(curField.animHash, 0, true);
        yield return new WaitForSeconds(curField.animLength);


        if (curField.hitReactionType == HitReactionType.KnockdownHit)
        {
            ClearRecoveryRoutine(true);
            _isRecovering = true;
            recoverRoutine = DoRecovery(Attack_KnockDown.HKD, curField,true);
            StartCoroutine(recoverRoutine);
        }
        else
        {
            int animHash = Animator.StringToHash("Landing_After_AirHit");
            _base._cAnimator.PlayNextAnimation(animHash, 0, true);
            yield return new WaitForSeconds(0.4f);
        }
    }
   
    public Attack_KnockBack_Vertical GetActiveVerticalKnockback(bool _blockedAttack) 
    {
        if (currentCustomDamageField != null)
        {
            return _blockedAttack ? currentCustomDamageField.verticalKBP.Block_VKB_Level : currentCustomDamageField.verticalKBP.Hit_VKB_Level;
        }
        else
        {
            return _blockedAttack ? currentProperty.VerticalKB_Data.Block_VKB_Level : currentProperty.VerticalKB_Data.Hit_VKB_Level;
        }
    }
    #region Successful Hit Code
    void SetRecoverable()
    {
        _cAnimator.isHit = false; 
        _base._cAnimator.SetCanRecover(false);
    }
    public void HandleHitState(Attack_BaseProperties currentAttack, float StopValue, float StunValue,float calculatedScaling, bool blockedAttack)
    {
        smallHitRecovering = false;
        bigHitRecovering = false;
        airRecoverPossible = false;
        _cAnimator.isHit = true;
        SetCurrentProperty(currentAttack, StopValue, StunValue, calculatedScaling);
        Callback<Attack_BaseProperties> funcCall = null;
        SearchHitResponseDictionary(currentAttack, blockedAttack);
    }
    public void SetCurrentProperty(Attack_BaseProperties currentAttack, float StopValue, float StunValue, float calculatedScaling) 
    {
        currentProperty = currentAttack;
        currentHitstun = StunValue;
        currentHitstop = StopValue * Base_FrameCode.ONE_FRAME;
        hitStunScaling = calculatedScaling;
    }
    public void ForceCustomLockAnim(CustomDamageField currentAttack, bool finalAttack)
    {
        _cAnimator.isHit = true;
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
                        break;
                    }
                    continue;
                }
                else 
                {
                    if (!blockTypes.Contains(entry.Key))
                    {
                        _base._cHealth.ClearRegenRoutine();
                        entry.Value(currentProperty);
                        break;
                    }
                    continue;
                }
            }
        }
    }
    HitAnimationField CheckRecoveryAnim(Attack_KnockDown _knockDownType)
    {
        List<HitAnimationField> refField = new List<HitAnimationField>(characterTotalHitReactions.getUpReactions);
        bool IsGrounded = _base._cHurtBox.IsGrounded();
        for (int i = 0; i < refField.Count; i++)
        {
            if (!refField[i].knockdownAnimType.HasFlag(_knockDownType))
            {
                refField[i] = null;
                continue;
            }
            if(IsGrounded && !refField[i].isGroundedReaction) 
            {
                refField[i] = null;
                continue;
            }
            if (!IsGrounded && refField[i].isGroundedReaction)
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
        if(refField.Count > 0) 
        {
            return refField[0];
        }
        else 
        {
            return characterTotalHitReactions.getUpReactions[0];
        }
    }

    public void ForceLockHitAnim(HitLevel _level)
    {
        _cAnimator.isHit = true;
        ActivateOnTickRoutine();
        CallLockedHitResponse(FilterGroundLockReactions(_level));
    }

    IEnumerator DoRecovery(Attack_KnockDown knockDownType, HitAnimationField playGroundedAnim,bool isDead)
    {
        if (playGroundedAnim.hitLevel != HitLevel.Crumple)
        {
            _base._cHitstun.EndHitStun();
            int animHash = Animator.StringToHash("Landing_After_AirHit");
            _base._cAnimator.PlayNextAnimation(animHash, 0, true);
            _base.Activate();
            yield return new WaitForSeconds(0.4f);
        }
        else
        {
            _base._cHitstun.EndHitStun();
            _base.Activate();
            yield return new WaitForSeconds(0.25f);
        }
        #region Choosing Recov Anim
        HitAnimationField recoveryAnim = CheckRecoveryAnim(knockDownType);
        Debug.LogError($"Chosen Getup Animation: {recoveryAnim.animName}");
        yield return new WaitForEndOfFrame();
        #endregion
        if (!isDead)
        {
            CallDelayGetup(recoveryAnim);
        }
    }
    public void CallDelayGetup(HitAnimationField recoveryAnim) 
    {
        if(DelayGetupRoutine != null) 
        {
            StopCoroutine(DelayGetupRoutine);
            DelayGetupRoutine = null;
        }
        DelayGetupRoutine = HandleDelayGetupRoutine(recoveryAnim);
        StartCoroutine(DelayGetupRoutine);
    }
    IEnumerator HandleDelayGetupRoutine(HitAnimationField recoveryAnim) 
    {
        float startDelayGetupTime = 0;
        int maxHoldTime = 40;
        float holdInputTimeInFrames = maxHoldTime * Base_FrameCode.ONE_FRAME;
        bool isTeching;
        if (recoveryAnim.knockdownAnimType == Attack_KnockDown.HKD)
        {
            isTeching = false;
            startDelayGetupTime = holdInputTimeInFrames;
            _base._cHurtBox.SetHurboxState(HurtBoxType.Invincible);
            _base._cAnimator.PlayNextAnimation(recoveryAnim.animHash, 0, true);
            yield return new WaitForSeconds(recoveryAnim.animLength);
            EndingFunctionCalls();
            yield break;
        }
        else
        {
            if (HoldingAway())
            {
                recoveryAnim = characterTotalHitReactions.backGetupReaction;
                _base._cForce.InstantForceAway(-0.85f);
            }
            else
            {
                recoveryAnim = characterTotalHitReactions.neutralGetupReaction;
                isTeching = false;
                if (_base._subState == Character_SubStates.Controlled)
                {
                    if (_base.ReturnMovementInputs().Button_State.directionalInput <= 3)
                    {
                        while (!isTeching && (startDelayGetupTime <= holdInputTimeInFrames))
                        {
                            startDelayGetupTime += Base_FrameCode.ONE_FRAME;
                            if (HoldingAway())
                            {
                                recoveryAnim = characterTotalHitReactions.backGetupReaction;
                                _base._cForce.InstantForceAway(-0.85f);
                                isTeching = true;
                            }
                            else if (!(_base.ReturnMovementInputs().Button_State.directionalInput <= 3))
                            {
                                isTeching = true;
                            }
                            yield return new WaitForSeconds(Base_FrameCode.ONE_FRAME);
                        }
                    }
                }
            }
        }
        isTeching = false;
        startDelayGetupTime = holdInputTimeInFrames;
        _base._cHurtBox.SetHurboxState(HurtBoxType.Invincible);
        _base._cAnimator.PlayNextAnimation(recoveryAnim.animHash, 0, true);
        yield return new WaitForSeconds(recoveryAnim.animLength);
        EndingFunctionCalls();
    }
    void EndingFunctionCalls() 
    {
        CheckStunMeter_OnKnockdown();
    }
    void CheckStunMeter_OnKnockdown()
    {
        if (_base._cHealth.stunController.IsSTUNNED)
        {
            _base._cHealth.stunController.ActivateStunDelay(OnEndHitGrounded);
        }
        else
        {
            OnEndHitGrounded();
        }
    }
    void OnEndHitGrounded() 
    {

        SetRecoverable();
        if (bigHitRecovering)
        {
            bigHitRecovering = false;
        }
        _base._aFrameDataMeter.SetHitRecoveringState(false);
        recoverRoutine = null;
        _isRecovering = false;
        _base._cHealth.StartHealthRegen();
        currentCustomDamageField = null;
        currentProperty = null;
        blockedAttack = false;
        currentHitstun = 0;
        SetRecoverable();
        _base.CheckAttackActive(false);
        ClearFrameTickRoutine();
    }
    public bool HoldingAway() 
    {
        if (_base._subState == Character_SubStates.Controlled)
        {
            List<int> holdingSixList = new List<int>() { 6, 9 };
            List<int> holdingFourList = new List<int>() { 4, 7 };
            bool holdingFour = holdingFourList.Contains(_base.ReturnMovementInputs().Button_State.directionalInput) && _base.pSide.thisPosition._directionFacing == Character_Face_Direction.FacingRight;
            bool holdingSix = holdingSixList.Contains(_base.ReturnMovementInputs().Button_State.directionalInput) && _base.pSide.thisPosition._directionFacing == Character_Face_Direction.FacingLeft;
            return holdingSix ^ holdingFour;
        }
        return false;
    }
    #endregion
    void SetStunMeterValue(float TopValue)
    {
        _hitStunSlider.maxValue = TopValue;
        _hitStunSlider.value = _hitStunSlider.maxValue;
    }
    
    void UpdateMeterValue(float subtractValue)
    {
        _base._aFrameDataMeter.UpdateFrameOnHit(FrameType.Stun);
        _hitStunSlider.value -= subtractValue;
    }
    IEnumerator UpdateFrameWhileDowned() 
    {
        while (_cAnimator.isHit) 
        {
            _base._aFrameDataMeter.UpdateFrameOnHit(FrameType.Stun);
            yield return new WaitForSeconds(Base_FrameCode.ONE_FRAME);
        }
    }
    void ActivateOnTickRoutine() 
    {
        if (DownedFrameTickRoutine == null)
        {
            DownedFrameTickRoutine = UpdateFrameWhileDowned();
            _base._aFrameDataMeter.SetHitRecoveringState(true);
            StartCoroutine(DownedFrameTickRoutine);
        }
    }
    void ClearMeterValue()
    {
        _hitStunSlider.value = 0;
    }
    public bool CheckIfAttackBlocked() 
    {
        return blockedAttack;
    }
}