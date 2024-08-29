using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;

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
    [SerializeField] private bool hasRecovered;


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

    /* bool IsBlocking()
     {
         IState currentState = _base._cStateMachine._playerState.current.State;
         bool blocking = currentState == _base._cStateMachine.crouchBlockRef ^ currentState == _base._cStateMachine.standBlockRef;
         return blocking;
     }*/


    #region Filter Animation Functions
/*    List<HitAnimationField> FilterHitReactions(Attack_BaseProperties currentAttack)
    {
        List<HitAnimationField> refField = new List<HitAnimationField>(characterTotalHitReactions.hitReactions);
        bool IsGrounded = _base._cHurtBox.IsGrounded();

        for (int i = 0; i < refField.Count; i++)
        {
            bool trueGroundedResponse = IsGrounded && !refField[i].isGroundedReaction;
            bool falseGroundedResponse = !IsGrounded && refField[i].isGroundedReaction;
            bool trueLowedResponse = refField[i].isLowReaction && currentAttack.AttackAnims.attackType != HitBoxType.Low;
            bool falseLowedResponse = !refField[i].isLowReaction && currentAttack.AttackAnims.attackType == HitBoxType.Low;
            if (!(currentAttack.hitLevel.HasFlag(refField[i].hitLevel)))
            {
                refField[i] = null;
                continue;
            }
            if (trueGroundedResponse)
            {
                refField[i] = null;
                continue;
            }
            if (falseGroundedResponse)
            {
                refField[i] = null;
                continue;
            }
            if (trueLowedResponse)
            {
                refField[i] = null;
                continue;
            }
            if (falseLowedResponse)
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
    }*/

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
        //List<HitAnimationField> hitReactionList = FilterHitReactions(currentAttack);
       /* int randomHitReaction = 0;
        if (hitReactionList.Count > 1)
        {
            randomHitReaction = UnityEngine.Random.Range(0, hitReactionList.Count);
        }
        HitAnimationField hitReaction = hitReactionList[randomHitReaction];
        CheckAndStartHitResponse(hitReaction);*/
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
                CallLockedHitResponse(FilterGroundLockReactions(currentAttack.hitLevel));
                return;
            }
            else 
            {
                CheckAndStartHitResponse(FilterGroundLockReactions(currentAttack.hitLevel));
                return;
            }
        }
        /* List<HitAnimationField> hitReactionList = FilterHitReactions(currentAttack);
         int randomHitReaction = 0;
         if (hitReactionList.Count > 1)
         {
             randomHitReaction = UnityEngine.Random.Range(0, hitReactionList.Count-1);
         }
         try
         {
             HitAnimationField hitReaction = hitReactionList[randomHitReaction];
             CheckAndStartHitResponse(hitReaction);
         }
         catch (ArgumentOutOfRangeException) 
         {
             //Debug.Break();
         }*/
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
        HitAnimationField hitReaction = FilterGroundLockReactions();
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
        float oneFrame = (1 / 60f);
        float hitStunInFrames = curField.animLength + (currentHitstun * oneFrame);
        SetStunMeterValue(hitStunInFrames);
        _base._cAnimator.PlayNextAnimation(curField.animHash,0,true);
        _base._cAnimator.SetCanRecover(true);
        _base._cHitstun.CallHitStun(hitStunInFrames);
        while (hitStunInFrames > 0) 
        {
            hitStunInFrames -= (oneFrame);
            UpdateMeterValue(oneFrame);
            yield return new WaitForSeconds(oneFrame);
        }
        if (curField.hitReactionType == HitReactionType.KnockdownHit)
        {
            ClearRecoveryRoutine();
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
        hasRecovered = false;
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

    IEnumerator DoRecovery(Attack_KnockDown knockDownType, HitAnimationField playGroundedAnim)
    {
        //if (knockDownType.HasFlag(Attack_KnockDown.HKD))
        //{
        if (playGroundedAnim.hitLevel != HitLevel.Crumple)
        {
            int animHash = Animator.StringToHash("Landing_After_AirHit");
            _base._cAnimator.PlayNextAnimation(animHash, 0, true);
            yield return new WaitForSeconds(0.4f);
        }
        HitAnimationField recoveryAnim = CheckRecoveryAnim(knockDownType);
        Debug.LogError($"Chosen Getup Animation: {recoveryAnim.animName}");
        _base._cAnimator.PlayNextAnimation(recoveryAnim.animHash, 0, true);
        yield return new WaitForSeconds(recoveryAnim.animLength);
        if (bigHitRecovering)
        {
            bigHitRecovering = false;
        }
        //}
        /*else
        {
            yield return new WaitUntil(() => _base._cHurtBox.IsGrounded());
            _base._cAnimator.PlayNextAnimation(Animator.StringToHash("Landing_After_AirHit"), 0, true);
            yield return new WaitForSeconds(0.4f);
            HitAnimationField recoveryAnim = CheckRecoveryAnim(Attack_KnockDown.SKD);
            _base._cAnimator.PlayNextAnimation(recoveryAnim.animHash, 0, true);
            yield return new WaitForSeconds(recoveryAnim.animLength);
            if (bigHitRecovering)
            {
                bigHitRecovering = false;
            }
        }*/
        /*else
        {
            HitAnimationField recoveryAnim = CheckRecoveryAnim(knockDownType);
            _base._cAnimator.PlayNextAnimation(recoveryAnim.animHash, 0, true);
            yield return new WaitForSeconds(recoveryAnim.animLength);
        }*/
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

    void SetStunMeterValue(float TopValue)
    {
        _hitStunSlider.maxValue = TopValue;
        _hitStunSlider.value = _hitStunSlider.maxValue;
    }
    void UpdateMeterValue(float subtractValue)
    {
        _hitStunSlider.value -= subtractValue;
    }
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

    public bool ReturnStandBlock()
    {
        return standingBlocking;
    }
    public bool ReturnCrouchBlock()
    {
        return crouchBlocking;
    }


  /*  public async Task RecoverAfterBlock()
    {
        while (recoveryTime > 0)
        {
            await Task.Delay((int)((1 / 60f) * 1000f));
        }
        hasRecovered = false;
        StartCoroutine(BlockRecover());
    }*/
   /* public void SetBlockStateFalse()
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
    }*/
    /*
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
    }*/
}