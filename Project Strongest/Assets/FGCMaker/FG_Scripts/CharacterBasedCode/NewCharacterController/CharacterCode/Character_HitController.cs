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

    public Attack_KnockDown last_KD;
    private float recoveryTime;
    Attack_BaseProperties currentProperty;
    CustomDamageField currentCustomDamageField;
    IEnumerator activeHitResponseRoutine, recoverRoutine;
    public float currentHitstun;
    public float hitStunScaling;

    [SerializeField] private bool smallHitRecovering;
    [SerializeField] private bool bigHitRecovering;
    [SerializeField] private bool airRecoverPossible;
    [SerializeField] private bool _isRecovering;
    [SerializeField] private bool blockedAttack;
    public bool Recovering { get{ return _isRecovering;  } }


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
                if (!refField[i].hitLevel.HasFlag(currentCustomDamageField.hitLevel))
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
        ClearRecoveryRoutine();
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
                ClearRecoveryRoutine();
                CallLockedHitResponse(FilterGroundLockReactions(currentAttack.hitLevel));
                return;
            }
            else
            {
                ClearRecoveryRoutine();
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
        ClearRecoveryRoutine();
        HitAnimationField hitReaction = FilterGroundLockReactions(currentAttack.hitLevel);
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
    void CheckAndStartHitResponse(HitAnimationField curField)
    {
        ClearHitResponseRoutine();
        activeHitResponseRoutine = DoHitResponse(curField);
        StartCoroutine(activeHitResponseRoutine);
    }
    public void ClearHitResponseRoutine() 
    {
        if (activeHitResponseRoutine != null)
        {
            StopCoroutine(activeHitResponseRoutine);
        }
    }
    public void ClearRecoveryRoutine()
    {
        if (recoverRoutine != null)
        {
            StopCoroutine(recoverRoutine);
            _isRecovering = false;
        }
        _base._cComboCounter.StopFadeRoutine();
    }
    IEnumerator DoHitResponse(HitAnimationField curField)
    {
        float hitStunInFrames = curField.animLength + (currentHitstun * Base_FrameCode.ONE_FRAME);
        try
        {
            ClearRecoveryRoutine();
            SetStunMeterValue(hitStunInFrames);
            _base._cAnimator.PlayNextAnimation(curField.animHash, 0, true);
            _base._cAnimator.SetCanRecover(true);
            _base._cHitstun.CallHitStun(hitStunInFrames);
        }
        catch (StackOverflowException) 
        {
            Debug.LogError("Stack Overflow Hit");
        }
        Vertical_KnockBack currentKnockBack = GetActiveVerticalKnockback();
        while (hitStunInFrames > 0)
        {
            if (_base.ReturnIfPaused())
            {
                yield return new WaitForSeconds(Base_FrameCode.ONE_FRAME);
            }
            else
            {
                if (currentKnockBack != null)
                {
                    if (currentKnockBack.verticalKBP != Attack_KnockBack_Vertical.No_KUD)
                    {
                        yield return new WaitForSeconds(0.45f);
                        while (!_base._cHurtBox.IsGrounded())
                        {
                            hitStunInFrames -= (Base_FrameCode.ONE_FRAME * _base._cHitstun.animSpeed);
                            UpdateMeterValue(Base_FrameCode.ONE_FRAME);
                            yield return new WaitForSeconds(Base_FrameCode.ONE_FRAME);
                        }
                        hitStunInFrames = 0;
                        ClearMeterValue();
                    }
                    hitStunInFrames -= (Base_FrameCode.ONE_FRAME * _base._cHitstun.animSpeed);
                    UpdateMeterValue(Base_FrameCode.ONE_FRAME);
                    yield return new WaitForSeconds(Base_FrameCode.ONE_FRAME);
                }
                else
                {
                    hitStunInFrames -= (Base_FrameCode.ONE_FRAME * _base._cHitstun.animSpeed);
                    UpdateMeterValue(Base_FrameCode.ONE_FRAME);
                    yield return new WaitForSeconds(Base_FrameCode.ONE_FRAME);
                }
            }
        }
        _base._cComboCounter.SetComboStateFalse();
        if (curField.hitReactionType == HitReactionType.KnockdownHit)
        {
            ClearRecoveryRoutine();
            _isRecovering = true;
            if (currentCustomDamageField != null)
            {
                HurtBoxType landingBoxType = currentCustomDamageField.KnockDown == Attack_KnockDown.HKD ? HurtBoxType.HardKnockdown : HurtBoxType.SoftKnockdown;
                _base._cHurtBox.SetHurboxState(landingBoxType);
                recoverRoutine = DoRecovery(currentCustomDamageField.KnockDown, curField, false);
            }
            else if (currentProperty != null)
            {
                HurtBoxType landingBoxType = currentProperty.KnockDown == Attack_KnockDown.HKD ? HurtBoxType.HardKnockdown : HurtBoxType.SoftKnockdown;
                _base._cHurtBox.SetHurboxState(landingBoxType);
                recoverRoutine = DoRecovery(currentProperty.KnockDown, curField,false);
            }
            yield return new WaitUntil(() => _base._cHurtBox.IsGrounded());
            StartCoroutine(recoverRoutine);
        }
        else
        {
            blockedAttack = false;
            _base._cHealth.StartHealthRegen();
            if(!_base._cHurtBox.IsGrounded())
            {
                HitAnimationField recoveryAnim = CheckRecoveryAnim(Attack_KnockDown.NONE);
                _base._cAnimator.PlayNextAnimation(recoveryAnim.animHash, 0, true);
                yield return new WaitForSeconds(recoveryAnim.animLength);
            }
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
            currentCustomDamageField = null;
            currentProperty = null;
        }
    }

    IEnumerator DoDeathResponse(HitAnimationField curField)
    {
        _base._cAnimator.PlayNextAnimation(curField.animHash, 0, true);
        yield return new WaitForSeconds(curField.animLength);

        _base._cComboCounter.SetComboStateFalse();

        if (curField.hitReactionType == HitReactionType.KnockdownHit)
        {
            ClearRecoveryRoutine();
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
    public Vertical_KnockBack GetActiveVerticalKnockback() 
    {
        if(currentCustomDamageField != null) 
        {
            return currentCustomDamageField.verticalKBP;
        }
        else 
        {
            if (currentProperty != null)
            {
                return currentProperty.verticalKBP;
            }
        }
        return null;
    }
    #region Successful Hit Code
    void SetRecoverable()
    {
        _cAnimator.isHit = false; 
        _base._cAnimator.SetCanRecover(false);
    }
    public void HandleHitState(Attack_BaseProperties currentAttack, float StunValue, float calculatedScaling, bool blockedAttack)
    {
        smallHitRecovering = false;
        bigHitRecovering = false;
        airRecoverPossible = false;
        _cAnimator.isHit = true;
        currentProperty = currentAttack;
        Callback<Attack_BaseProperties> funcCall = null;
        currentHitstun = StunValue;
        hitStunScaling = calculatedScaling;
        SearchHitResponseDictionary(currentAttack, blockedAttack);
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
        for (int i = 0; i < refField.Count; i++)
        {
            if (!refField[i].knockdownAnimType.HasFlag(_knockDownType))
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
    bool CheckNextAttackCatchPostLanding()
    {
        Attack_BaseProperties _opponentProperty = _base.opponentPlayer._cAnimator.lastAttack;
        if (_opponentProperty != null)
        {
            if (_opponentProperty.hitConnected == true && _opponentProperty != currentProperty)
            {
                if (lockMoveTypes.Contains(_opponentProperty._moveType))
                {
                    ForceLockHitAnim(_opponentProperty.hitLevel);
                }
                return true;
            }
        }
        return false;
    }

    public void ForceLockHitAnim(HitLevel _level) 
    {
        CallLockedHitResponse(FilterGroundLockReactions(_level));
    }

    IEnumerator DoRecovery(Attack_KnockDown knockDownType, HitAnimationField playGroundedAnim,bool isDead)
    {
        if (playGroundedAnim.hitLevel != HitLevel.Crumple)
        {
            _base._cHitstun.EndHitStun();
            int animHash = Animator.StringToHash("Landing_After_AirHit");
            _base._cAnimator.PlayNextAnimation(animHash, 0, true);
            yield return new WaitForSeconds(0.4f);
        }
        else
        {
            _base._cHitstun.EndHitStun();
            yield return new WaitForSeconds(0.25f);
        }

        #region Choosing Recov Anim
        HitAnimationField recoveryAnim = CheckRecoveryAnim(knockDownType);
        Debug.LogError($"Chosen Getup Animation: {recoveryAnim.animName}");
        yield return new WaitForEndOfFrame();
        #endregion
        if (!isDead)
        {
            _base._cHurtBox.SetHurboxState(HurtBoxType.Invincible);

            if (currentCustomDamageField == null)
            {
                if (CheckNextAttackCatchPostLanding())
                {
                    _base._cHealth.StartHealthRegen();
                    currentCustomDamageField = null;
                    currentProperty = null;
                    yield break;
                }
            }
            _base._cAnimator.PlayNextAnimation(recoveryAnim.animHash, 0, true);
            yield return new WaitForSeconds(recoveryAnim.animLength);
            if (bigHitRecovering)
            {
                bigHitRecovering = false;
            }
            recoverRoutine = null;
            _isRecovering = false;
            _base._cHealth.StartHealthRegen();
            SetRecoverable();
            currentCustomDamageField = null;
            currentProperty = null;
            blockedAttack = false;
        }
    }
    #endregion
    void SetStunMeterValue(float TopValue)
    {
        _hitStunSlider.maxValue = TopValue;
        _hitStunSlider.value = _hitStunSlider.maxValue;
    }
    void UpdateMeterValue(float subtractValue)
    {
        _hitStunSlider.value -= subtractValue;
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