using System.Collections.Generic;
using System;
using System.Collections;
using UnityEngine;
using System.Threading.Tasks;
using DG.Tweening;
using FightingGame_FrameData;

public class Character_Animator : MonoBehaviour
{
    public List<SkinnedMeshRenderer> _modelMeshCount;
    #region HitAnimNums
    public bool isHit;
    #endregion
    [SerializeField] private HeightPositionMarkerSet heightMarkerObjects;
    [Space(15)]
    [SerializeField] public Animator myAnim;
    [SerializeField] public Animator shadowAnim;
    [SerializeField] public Character_Base _base;
    [SerializeField] private Character_InputTimer_Attacks _timer => _base._cAttackTimer;

    #region Enums
    [Header("Enums")]
    public lastMovementState _lastMovementState;
    public lastAttackState _lastAttackState;
    #endregion

    [SerializeField] public Character_MobilityOption activatedInput;
    [SerializeField] public Attack_BaseProperties lastAttack;
     public AttackHandler_Attack _lastAnim;
    public Transform _model;
    public bool inRekkaState,inStanceState;
    public bool canBlock;
    public bool canTick;
    public bool inputWindowOpen;
    public bool _canRecover;
    public bool canTransitionIdle;
    Vector3 startPos;
    public Cancel_State currentAttackLevel;

    bool hitNewAnim;
    public bool customSuperHit;
    public IEnumerator BasicAttackRoutine, ThrowAttackRoutine, SuperAttackRoutine;
    int matCount;
    int lastHashPlayed;

    [SerializeField] private VictoryAnimation _victoryAnim;
    [SerializeField] private GameObject soda;
    private void Start()
    {
        customSuperHit = false;
        inputWindowOpen = true;
        startPos = _model.localPosition;
        inRekkaState = false;
        inStanceState = false;
    }
    public void SetModelColors(Character_ColorData _colorData, int skinIndex) 
    {
        matCount = 0;
        List<Material> _matList = new List<Material>();
        for(int i = 0; i < _colorData._modelMaterials.Count; i++) 
        {
            Material _curMat = new Material(_colorData._modelMaterials[i]);
            _curMat.SetTexture("_Base_Color_Mat", _colorData.ColorSets[skinIndex]._imageSet[i]);
            _curMat.name = $"{_colorData._modelMaterials[i].name}_Color_{skinIndex+1}";
            _matList.Add(_curMat);
        }
        for (int i = 0; i < _modelMeshCount.Count; i++) 
        {
            int meshMatCount = _modelMeshCount[i].materials.Length;
            _modelMeshCount[i].materials = GetNewColorSet(_matList,meshMatCount);
        }
        _matList = null;
    }
    public Material[] GetNewColorSet(List<Material> _matList, int meshCount) 
    {
        Material[] newMaterialMat = new Material[meshCount];
        List<Material> matList = new List<Material>();
        for (int j = 0; j < meshCount; j++)
        {
            matList.Add(_matList[matCount]);
            matCount++;
        }
        newMaterialMat = matList.ToArray();
        return newMaterialMat;
    }
    public HeightPositionMarkerSet ReturnMarkers() 
    {
        return heightMarkerObjects;
    }
    public bool RoutineActive() 
    {
        return BasicAttackRoutine == null && ThrowAttackRoutine == null && SuperAttackRoutine == null;
    }
    public void SetStanceBool(bool state) 
    {
        inStanceState = state; 
    }
    public void SetRekkaBool(bool state)
    {
        inRekkaState = state;
    }

    public void PlayNextAnimation(int animHash, float crossFadeTime, bool attackOverride = false, float overrideTime = 0f, bool lockedHit = false, bool inReverse = false)
    {
        if(lastHashPlayed != animHash) 
        {
            lastHashPlayed = animHash;
        }
        else 
        {
            if (!attackOverride)
            {
                return;
            }
        }
        if (myAnim == null) 
        {
            return;
        }
        if (lockedHit)
        {
            shadowAnim.Play(animHash, 0, overrideTime);
            myAnim.Play(animHash, 0, overrideTime);
            _base._cHitstun.HandleAnimatorFreeze(true,0f);
        }
        else
        {
            if (_base._cHitstun.IsFrozen())
            {
                _base._cHitstun.HandleAnimatorFreeze(false);
            }
            if (attackOverride)
            {
                shadowAnim.Play(animHash, 0, overrideTime);
                myAnim.Play(animHash, 0, overrideTime);
            }
            else
            {
                if (inReverse)
                {
                    myAnim.CrossFade(animHash, crossFadeTime, 0, -1);
                    shadowAnim.CrossFade(animHash, crossFadeTime, 0, -1);
                }
                else
                {
                    myAnim.CrossFade(animHash, crossFadeTime, 0, 0);
                    shadowAnim.CrossFade(animHash, crossFadeTime, 0, 0);
                }
            }
        }
    }

    public void SetCanRecover(bool state) 
    {
        _canRecover = state;
    }

    #region AnimEvent Functions
    public void SetOpponentFreeze(CustomCallback callback = null)
    {
        _base.opponentPlayer._cForce.HandleForceFreeze(true);
        _base.opponentPlayer._cHitstun.HandleAnimatorFreeze(true, 0.45f);
        _base.opponentPlayer._cGravity.HandleGravityFreeze(true);
    }
    public void SetSelfFreeze(CustomCallback callback = null)
    {
        
        _base._cHitstun.HandleAnimatorFreeze(true, 0.45f);
        _base._cGravity.HandleGravityFreeze(true);
    }
    public void FreezeBoth(CustomCallback callback = null) 
    {
        SetSelfFreeze();
        SetOpponentFreeze();
    }
    public void SetSelfUnfreeze(CustomCallback callback = null)
    {
        _base._cForce.HandleForceFreeze(false);
        _base._cGravity.HandleGravityFreeze(false);
        if (_base._cStateMachine._playerState.current.State != _base._cStateMachine.hitStateRef)
        {
            _base._cHitstun.HandleAnimatorFreeze(false);
        }
        else 
        {
            _base._cHitstun.HandleAnimatorFreeze(true,0f);
        }
    }
    public bool CheckAttackAndMobility()
    {
        bool activatedInputCheck = (activatedInput == null || (activatedInput.movementPriority == 0 && (activatedInput.mobilityOptionName == null || activatedInput.mobilityOptionName == "")));
        bool lastAttackCheck = (lastAttack == null || (lastAttack.InputTimer == null && (lastAttack._attackName == null || lastAttack._attackName == "")));

        if (activatedInputCheck && lastAttackCheck) 
        {
            return true;
        }
        return false;
    }
    public bool CheckMobilityStateState()
    {
        if (activatedInput == null)
        {
            return true;
        }
        if (activatedInput.movementPriority == 0)
        {
            return true;
        }
        if (activatedInput.mobilityOptionName == null || activatedInput.mobilityOptionName == "")
        {
            return true;
        }
        return false;
    }
    public bool CheckAttackState() 
    {
        if (lastAttack == null)
        {
            return false;
        }
        if (lastAttack.InputTimer == null)
        {
            return false;
        }
        return true;
    }
    public bool CheckAttackState(bool check)
    {
        if (CheckAttackState())
        {
            if (check)
            {
                return true;
            }
            else 
            {
                return false;
            }
        }
        return true;
    }
    #endregion
    public void SetActivatedInput(Character_MobilityOption inputToActivate)
    {
        if (!hitNewAnim)
        {
            if (lastAttack != null)
            {
                if ((CheckAttackState(lastAttack.dashCancelable) && inputToActivate.movementPriority == 2))
                {
                    hitNewAnim = true;
                    activatedInput = inputToActivate;
                    _lastMovementState = lastMovementState.populated;
                    _base.character_MobilityOptions.ActivateMobilityOption(inputToActivate);
                }
                else if ((CheckAttackState(lastAttack.JumpCancelable) && inputToActivate.movementPriority != 2))
                {
                    hitNewAnim = true;
                    activatedInput = inputToActivate;
                    SetSelfUnfreeze();
                    _base._cForce.CallLockKinematic();
                    _lastMovementState = lastMovementState.populated;
                    _base.character_MobilityOptions.ActivateMobilityOption(inputToActivate);
                }
            }
            else
            {
                hitNewAnim = true;
                activatedInput = inputToActivate;
                _lastMovementState = lastMovementState.populated;
                _base.character_MobilityOptions.ActivateMobilityOption(inputToActivate);
            }
        }
    }

    public void SetLastAttack(Attack_BaseProperties _attack)
    {
        if (_attack != null)
        {
            lastAttack = _attack;
            lastAttack.AttackAnims.AddRequiredCallbacks(_base, lastAttack);
            if (lastAttack.AttackAnims.HitBox != null)
            {
                lastAttack.AttackAnims.HitBox.hitboxProperties = _attack;
            }
            _lastAttackState = lastAttackState.populated;
            currentAttackLevel = lastAttack.cancelProperty.CurrentLevel;
        }
    }

    public void ClearLastActivatedInput()
    {
        NullifyMobilityOption();
    }
    public void SetNextAttackStartVariables(Attack_BaseProperties nextattack)
    {
        canTick = true;
        SetLastAttack(nextattack);
        if (lastAttack.AttackAnims._frameData._extraPoints.Count > 0)
        {
            lastAttack.AttackAnims.AddCustomCallbacks();
        }
        StartFrameCount();

    }
    public void StartFrameCount()
    {
        if (BasicAttackRoutine != null)
        {
            StopCoroutine(BasicAttackRoutine);
            BasicAttackRoutine = null;
        }
        lastAttack.AttackAnims.SetIsFollowUpAttack(false);

        _lastAnim = lastAttack.AttackAnims;
        PlayNextAnimation(lastAttack.attackHashes, 2 * (1f / lastAttack.AttackAnims.animClip.frameRate),true);
        BasicAttackRoutine = lastAttack.AttackAnims.TickAnimFrameCount(lastAttack);
        StartCoroutine(BasicAttackRoutine);
    }
    public void StartThrowFrameCount(Attack_BaseProperties throwProperty, AttackHandler_Attack throwCustom)
    {
        if (BasicAttackRoutine != null)
        {
            StopCoroutine(BasicAttackRoutine);
            BasicAttackRoutine = null;
        }
        _lastAnim = throwCustom;
        lastAttack = throwProperty;
        throwCustom.SetIsFollowUpAttack(true);
        PlayNextAnimation(Animator.StringToHash(throwCustom.animName), 2 * (1f / throwCustom.animClip.frameRate), true);
        ThrowAttackRoutine = throwCustom.TickAnimCustomCount(throwCustom);
        StartCoroutine(ThrowAttackRoutine);
    }
    public void StartSuperFrameCount(Attack_BaseProperties superProperty, int curAnim,int animCount,AttackHandler_Attack superCustom, Callback nextAnimIterator = null)
    {
        if (BasicAttackRoutine != null)
        {
            StopCoroutine(BasicAttackRoutine);
            BasicAttackRoutine = null;
        }
        lastAttack = superProperty;
        customSuperHit = true;
        _base._cAttackTimer.PauseTimerOnSuperSuccess();

        _lastAnim = superCustom;
        PlayNextAnimation(Animator.StringToHash(superCustom.animName), 2 * (1f / superCustom.animClip.frameRate),true);
        superCustom.SetIsFollowUpAttack(true);
        if (SuperAttackRoutine != null) 
        {
            StopCoroutine(SuperAttackRoutine);
            SuperAttackRoutine = null;
        }
        SuperAttackRoutine = superCustom.TickAnimCustomCount(superCustom, curAnim, animCount, nextAnimIterator);
        StartCoroutine(SuperAttackRoutine);
    }

    public void SetCanTransitionIdle(bool state) 
    {
        canTransitionIdle = state;
    }


    #region Projectile Code
    public void ShootProjectile()
    {
        GameObject projectile = Instantiate(_base.pSide.thisPosition.projectile_HitBox.gameObject, _base.gameObject.transform);
        projectile.gameObject.transform.localPosition = lastAttack.AttackAnims.hu_placement;
        projectile.gameObject.transform.localRotation = Quaternion.identity;
        projectile.gameObject.transform.localScale = Vector3.zero;
        projectile.transform.DOScale(.45f, 0.25f).OnComplete(() =>
        {
            try 
            { 
                projectile.GetComponent<LaunchController>().SetProperty(lastAttack);
                projectile.GetComponent<LaunchController>().Launch(_base, lastAttack.AttackAnims.hb_size, lastAttack.AttackAnims.attackType);
                StartCoroutine(KillProjectile(projectile));
            }
            catch(NullReferenceException)
            {
                Attack_BaseProperties reserveProperty = _base._aManager.Combo[_base._aManager.Combo.Count - 1];
                projectile.GetComponent<LaunchController>().SetProperty(reserveProperty);
                projectile.GetComponent<LaunchController>().Launch(_base, reserveProperty.AttackAnims.hb_size, reserveProperty.AttackAnims.attackType);
                StartCoroutine(KillProjectile(projectile));
            }
         });
    }

    IEnumerator KillProjectile(GameObject hitbox)
    {
        yield return new WaitForSeconds(150* (1 / 60f));
        if (lastAttack != null)
        {
            ClearLastAttack();
        }
        Destroy(hitbox);
    }
    #endregion


    #region End Of Animation Clean-Up
    public void CountUpNegativeFrames()
    {
        if (BasicAttackRoutine != null) 
        {
            StopCoroutine(BasicAttackRoutine);
            BasicAttackRoutine = null;
        }
        if (ThrowAttackRoutine != null)
        {
            StopCoroutine(ThrowAttackRoutine);
            ThrowAttackRoutine = null;
        }
        if (SuperAttackRoutine != null)
        {
            StopCoroutine(SuperAttackRoutine);
            SuperAttackRoutine = null;
        }
        _base._cHitboxManager.IterateHitBox();
    }
    public void NullifyMobilityOption()
    {
        _base._cForce.ResetPriority();
        hitNewAnim = false;
        activatedInput = null;
        _lastMovementState = lastMovementState.nullified;
    }
    public void ClearLastAttack()
    {
        if (customSuperHit) 
        {
            customSuperHit= false;
        }
        _base._cForce.CallUnlockKinematic();
        inputWindowOpen = true;
        _base._cForce.ResetPriority();
        currentAttackLevel = Cancel_State.NotCancellable;
    }
    public void KillAttackOnRoutineEnd()
    {
        _lastAttackState = lastAttackState.nullified;
        lastAttack = null;
    }
    public void EndAnim()
    {
        if (canTick)
        {
            Debug.Log("Hit End Anim");
            canTick = false;
            if (lastAttack != null)
            {
                ClearLastAttack();
            }
        }
    }
    #endregion
    public void FullBaseAttackDataClear(Attack_BaseProperties thisAttack, FrameData _frameData)
    {
        if (_base._cAttackTimer._type == TimerType.Special)
        {
            _base._cAttackTimer.ClearAttackLanded();
        }
        else if (_base._cAttackTimer._type == TimerType.Super && lastAttack._moveType == MoveType.Super)
        {
            _base._cAttackTimer.ClearSuperLanded();
        }
        else if (_base._cAttackTimer._type == TimerType.Throw && lastAttack._moveType == MoveType.Throw)
        {
            _base._cAttackTimer.ClearThrowLanded();
        }
        else
        {
            _base._cAttackTimer.ClearAttackLanded();
            SetCanTransitionIdle(true);
        }
        if (thisAttack._moveType == MoveType.Throw)
        {
            if (!thisAttack.hitConnected)
            {
                CountUpNegativeFrames();
                SetCanTransitionIdle(true);
            }
            else if (_base.opponentPlayer._cHurtBox.throwTeched)  
            {
                CountUpNegativeFrames();
                SetCanTransitionIdle(true);
            }
        }
        else
        {
            CountUpNegativeFrames();
            SetCanTransitionIdle(true);
        }
        _base._aFrameDataMeter.GetAdvantageValue(_frameData);
        KillAttackOnRoutineEnd();
        EndAnim();
    }
    public void FullCustomAttackDataClear(Attack_BaseProperties thisAttack, int curAnim, int animCount, List<RequiredCallback> requiredHitboxCallBacks, FrameData _frameData)
    {
        if (thisAttack?.InputTimer == null)
        {
            thisAttack = lastAttack;
        }
        if (thisAttack.InputTimer != null)
        {
            if (thisAttack._moveType == MoveType.Throw || thisAttack._moveType == MoveType.CommandGrab)
            {
                _base._cAttackTimer.ClearThrowLanded();
            }
            else if (thisAttack._moveType == MoveType.Super)
            {
                if (curAnim >= animCount)
                {
                    _base._cAttackTimer.ClearSuperLanded();
                }
            }
            else
            {
                _base._cAttackTimer.ClearAttackLanded();
            }
        }
        else
        {
            _base._cAttackTimer.ClearAttackLanded();
        }
        if (requiredHitboxCallBacks.Count == 1)
        {
            requiredHitboxCallBacks[0].func();
            requiredHitboxCallBacks.RemoveAt(0);
        }
        _base._aFrameDataMeter.GetAdvantageValue(_frameData);
        EndAnim();
        KillAttackOnRoutineEnd();
        CountUpNegativeFrames();
        SetCanTransitionIdle(true);
    }

    #region End Of Round Animation Handler
    protected static readonly int winPerfect = Animator.StringToHash("Win_HighHealth");
    protected static readonly int winNormal = Animator.StringToHash("Win_LowHealth");
    protected static readonly int lossRound = Animator.StringToHash("Loss_Normal");
    public void PlayRoundWinAnim() 
    {
        float halfWayMark = _base._cHealth.health_Main.maxValue / 2f;
        if (_base._cHealth.health_Main.currentValue >= halfWayMark) 
        {
            PlayNextAnimation(winPerfect,0.25f);
        }
        else 
        {
            PlayNextAnimation(winNormal, 0.25f);
        }
    }
    public void PlayRoundLossAnim()
    {
        if (!_base._cDamageCalculator.isDead)
        {
            PlayNextAnimation(lossRound, 0.25f);
        }
    }
    public void PlayVictoryAnim()
    {
        _base.Deactivate();
        _base._cStateMachine.enabled = false;
        _victoryAnim._animName = _victoryAnim._animationClip.name;
        _victoryAnim.activatePointInFrames = _victoryAnim.activatePoint * Base_FrameCode.ONE_FRAME;
        _victoryAnim._animLength = _victoryAnim._animationClip.length;
        _victoryAnim._animHash = Animator.StringToHash(_victoryAnim._animName);
        StartCoroutine(PlayAnimSequence(_victoryAnim));
    }
    IEnumerator PlayAnimSequence(VictoryAnimation _currentAction)
    {
        float frameCount = 0;
        SetCanTransitionIdle(false);
        bool pointHit = false;
        float waitTime = Base_FrameCode.ONE_FRAME;
        float endingFrame = _victoryAnim._animLength;
        PlayNextAnimation(_victoryAnim._animHash, 0.25f);
        while (frameCount <= endingFrame)
        {
            #region Mobility Anim Checks
            if (frameCount > _victoryAnim.activatePointInFrames && !pointHit)
            {
                pointHit = true;
                soda.gameObject.SetActive(true);
            }
            frameCount += waitTime;
            yield return new WaitForSeconds(waitTime);
            #endregion
        }
    }
    #endregion
}
[Serializable]
public enum lastAttackState 
{
    nullified, 
    populated
}
[Serializable]
public enum lastMovementState 
{ 
    nullified, 
    populated 
}
[Serializable]
public class HeightPositionMarkerSet 
{
    public Transform topPoint;
    public Transform centerPoint;
    public Transform basePoint;
}
[Serializable]
public class WidthPositionMarkerSet
{
    public Transform leftFootPoint;
    public Transform leftHandPoint;
    public Transform rightFootPoint;
    public Transform rightHandPoint;
}
[Serializable]
public class VictoryAnimation : MobilityOption_Anim
{
    public float activatePoint;
    public float activatePointInFrames;
}