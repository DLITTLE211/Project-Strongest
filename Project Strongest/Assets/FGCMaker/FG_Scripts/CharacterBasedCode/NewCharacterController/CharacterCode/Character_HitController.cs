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
    [SerializeField] private bool crouchBlocking;
    [SerializeField] private bool standingBlocking;
    [SerializeField] private bool _isRecovering;
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
    #endregion

    HitAnimationField FindAnimationOfType(Attack_BaseProperties currentAttack = null, CustomDamageField currentCustom = null) 
    {
        List<HitAnimationField> refField = new List<HitAnimationField>(characterTotalHitReactions.hitReactions);
        List<HitAnimationField> prunedList = new List<HitAnimationField>();
        bool lowHit = currentAttack.AttackAnims.attackType == HitBoxType.Low;
        bool IsGrounded = _base._cHurtBox.IsGrounded();
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
    }

    void SmallHitDetect(Attack_BaseProperties currentAttack = null)
    {
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
        ClearRecoveryRoutine();
        float hitStunInFrames = curField.animLength + (currentHitstun * Base_FrameCode.ONE_FRAME);
        SetStunMeterValue(hitStunInFrames);
        _base._cAnimator.PlayNextAnimation(curField.animHash,0,true);
        _base._cAnimator.SetCanRecover(true);
        _base._cHitstun.CallHitStun(hitStunInFrames);
        while (hitStunInFrames > 0) 
        {
            if (_base.ReturnIfPaused())
            {
                yield return new WaitForSeconds(Base_FrameCode.ONE_FRAME);
            }
            else
            {
                hitStunInFrames -= (Base_FrameCode.ONE_FRAME * _base._cHitstun.animSpeed);
                UpdateMeterValue(Base_FrameCode.ONE_FRAME);
                yield return new WaitForSeconds(Base_FrameCode.ONE_FRAME);
            }
        }
        if (curField.hitReactionType == HitReactionType.KnockdownHit)
        {
            ClearRecoveryRoutine();
            _isRecovering = true;
             recoverRoutine = DoRecovery(currentProperty.KnockDown, curField);
            yield return new WaitUntil(() => _base._cHurtBox.IsGrounded());
            StartCoroutine(recoverRoutine);
        }
        else
        {
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
            _base._cHurtBox.SetHurboxState();
        }
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

    IEnumerator DoRecovery(Attack_KnockDown knockDownType, HitAnimationField playGroundedAnim)
    {
        if (playGroundedAnim.hitLevel != HitLevel.Crumple)
        {
            yield return new WaitForEndOfFrame();
            if (CheckNextAttackCatchPostLanding())
            {
                yield break;
            }
            int animHash = Animator.StringToHash("Landing_After_AirHit");
            _base._cAnimator.PlayNextAnimation(animHash, 0, true);
            yield return new WaitForSeconds(0.4f);
        }
        else
        {
            yield return new WaitForSeconds(0.25f);
        }

        HitAnimationField recoveryAnim = CheckRecoveryAnim(knockDownType);
        Debug.LogError($"Chosen Getup Animation: {recoveryAnim.animName}");
        yield return new WaitForEndOfFrame();
        if (CheckNextAttackCatchPostLanding())
        {
            yield break;
        }
        _base._cAnimator.PlayNextAnimation(recoveryAnim.animHash, 0, true);
        yield return new WaitForSeconds(recoveryAnim.animLength);
        if (bigHitRecovering)
        {
            bigHitRecovering = false;
        }
        recoverRoutine = null;
        _isRecovering = false;
        SetRecoverable();
        _base._cHurtBox.SetHurboxState();
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
    public bool ReturnStandBlock()
    {
        return standingBlocking;
    }
    public bool ReturnCrouchBlock()
    {
        return crouchBlocking;
    }
}