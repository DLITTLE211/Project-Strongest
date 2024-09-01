using System.Collections.Generic;
using System;
using System.Collections;
using UnityEngine;
using System.Threading.Tasks;
using DG.Tweening;

public class Character_Animator : MonoBehaviour
{
    #region HitAnimNums
    public bool isHit;
    #endregion

    [SerializeField] public Animator myAnim;
    [SerializeField] public Animator shadowAnim;
    [SerializeField] public Character_Base _base;
    [SerializeField] private Character_InputTimer_Attacks _timer => _base._cAttackTimer;

    #region Enums
    [Header("Enums")]
    public lastMovementState _lastMovementState;
    public enum lastMovementState { nullified, populated };
    public lastAttackState _lastAttackState;
    public enum lastAttackState { nullified, populated };
    #endregion

    [SerializeField] public Character_Mobility activatedInput;
    [SerializeField] public Attack_BaseProperties lastAttack;

    public Transform _model;
    public bool inRekkaState,inStanceState;
    public bool canBlock;
    public bool canTick;
    public bool inputWindowOpen;
    public bool _canRecover;
    public bool canTransitionIdle;

    internal int negativeFrameCount;
    Vector3 startPos;
    public Cancel_State currentAttackLevel;


    [SerializeField] private HitPointCall _freezeCall;
    [SerializeField] private HitPointCall _mobilityCall;
    [SerializeField] private HitPointCall _attackCall;
    public HitPointCall FreezeCall { get { return _freezeCall; } }
    public HitPointCall MobilityCall { get { return _mobilityCall; } }
    public HitPointCall AttackCall { get { return _attackCall; } }
    bool hitNewAnim;
    public bool customSuperHit;
    public IEnumerator BasicAttackRoutine, ThrowAttackRoutine, SuperAttackRoutine;
    private void Start()
    {
        customSuperHit = false;
        inputWindowOpen = true;
        startPos = _model.localPosition;
        inRekkaState = false;
        inStanceState = false;
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
 /*   void ApplyForceOnCustomCallback(CustomCallback callback)
    {
        if (_freezeCall.HasFlag(callback.customCall))
        {
            switch (callback.customCall)
            {
                case HitPointCall.ToggleFreeze_Both:
                    SetOpponentFreeze();
                    SetSelfFreeze();
                    break;
                case HitPointCall.ToggleFreeze_Other:
                    SetOpponentFreeze();
                    break;
                case HitPointCall.ToggleFreeze_Self:
                    SetSelfFreeze();
                    break;
                case HitPointCall.UnFreeze:
                    SetSelfUnfreeze();
                    break;
            }
        }
        if (_mobilityCall.HasFlag(callback.customCall))
        {
            switch (callback.customCall)
            {
                case HitPointCall.ClearMobility:
                    ClearLastActivatedInput();
                    break;
            }
        }
        if (_attackCall.HasFlag(callback.customCall))
        {
            switch (callback.customCall)
            {
                case HitPointCall.KillStance:
                    ClearLastAttack();
                    _base._cAttackTimer.SetTimerType();
                    break;
                case HitPointCall.ShootProjectile:
                    ShootProjectile();
                    break;
            }
        }
    }*/

    public void PlayNextAnimation(int animHash, float crossFadeTime, bool attackOverride = false, float overrideTime = 0f, bool lockedHit = false)
    {
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
                myAnim.CrossFade(animHash, crossFadeTime, 0, 0);
                shadowAnim.CrossFade(animHash, crossFadeTime, 0, 0);
            }
        }
    }

    public void SetCanRecover(bool state) 
    {
        _canRecover = state;
    }

    #region AnimEvent Functions
    public void SetOpponentFreeze()
    {
        _base.opponentPlayer._cForce.HandleForceFreeze(true);
        _base.opponentPlayer._cHitstun.HandleAnimatorFreeze(true, 0.25f);
        _base.opponentPlayer._cGravity.HandleGravityFreeze(true);
    }
    public void SetSelfFreeze()
    {
        _base._cHitstun.HandleAnimatorFreeze(true, 0.25f);
        _base._cGravity.HandleGravityFreeze(true);
    }
    public void SetSelfUnfreeze()
    {
        _base._cForce.HandleForceFreeze(false);
        _base._cGravity.HandleGravityFreeze(false);
        if (_base._cStateMachine._playerState.current.State != _base._cStateMachine.hitStateRef)
        {
            _base._cHitstun.HandleAnimatorFreeze(false);
        }
    }
    public bool CheckAttackAndMobility()
    {
        bool activatedInputCheck = (activatedInput == null || (activatedInput.movementPriority == 0 && (activatedInput.movementName == null || activatedInput.movementName == "")));
        bool lastAttackCheck = (lastAttack == null || (lastAttack.InputTimer == null && (lastAttack._attackName == null || lastAttack._attackName == "")));

        if (activatedInputCheck && lastAttackCheck) 
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
    IEnumerator SetCanBlock()
    {
        yield return new WaitForSeconds(0.067f);
        canBlock = true;
    }
    #endregion

    public void SetActivatedInput(Character_Mobility inputToActivate)
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
                    StartCoroutine(_base._extraMoveAsset.TickMobilityAnimation(inputToActivate));
                }
                else if ((CheckAttackState(lastAttack.JumpCancelable) && inputToActivate.movementPriority != 2))
                {
                    hitNewAnim = true;
                    activatedInput = inputToActivate;
                    SetSelfUnfreeze();
                    _base._cForce.CallLockKinematic();
                    _lastMovementState = lastMovementState.populated;
                    StartCoroutine(_base._extraMoveAsset.TickMobilityAnimation(inputToActivate));
                }
            }
            else
            {
                hitNewAnim = true;
                activatedInput = inputToActivate;
                _lastMovementState = lastMovementState.populated;
                StartCoroutine(_base._extraMoveAsset.TickMobilityAnimation(inputToActivate));
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
            currentAttackLevel = lastAttack.cancelProperty.cancelFrom;
        }
    }

    public void ClearLastActivatedInput()
    {
        if (activatedInput != null)
        {
            NullifyMobilityOption();
        }
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
        PlayNextAnimation(lastAttack.attackHashes[0], 2 * (1f / lastAttack.AttackAnims.animClip.frameRate),true);
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
    public void CountUpNegativeFrames(int lastNegativeFrames)
    {
        negativeFrameCount += lastNegativeFrames;
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
    }
    public void NullifyMobilityOption()
    {
        _base._cForce.ResetPriority();
        hitNewAnim = false;
        activatedInput.activeMove = false;
        activatedInput = null;
        _lastMovementState = lastMovementState.nullified;
    }
    public void ClearLastAttack()
    {
        if (customSuperHit) 
        {
            customSuperHit= false;
        }
        lastAttack = null; 
        _base._cForce.CallUnlockKinematic();
        _lastAttackState = lastAttackState.nullified;
        inputWindowOpen = true;
        _base._cForce.ResetPriority();
        currentAttackLevel = Cancel_State.NotCancellable;
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
}
