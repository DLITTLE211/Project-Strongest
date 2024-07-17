
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

    [SerializeField] private bool Shake;
    public bool ISShaking { get { return Shake; } }

    public Transform _model;
    public bool canBlock;
    public bool canTick;
    public bool inputWindowOpen;
    public bool _canRecover;
    public bool canTransitionIdle;

    internal int negativeFrameCount;
    Vector3 startPos;

    [SerializeField] private Cancel_State curretnAttackLevel;


    [SerializeField] private HitPointCall FreezeCall;
    [SerializeField] private HitPointCall mobilityCall;
    [SerializeField] private HitPointCall attackCall;
    private void Start()
    {
        inputWindowOpen = true;
        startPos = _model.localPosition;
        Messenger.AddListener<int>(Events.AddNegativeFrames, CountUpNegativeFrames); 
        Messenger.AddListener<CustomCallback>(Events.CustomCallback, ApplyForceOnCustomCallback);
    }
    void ApplyForceOnCustomCallback(CustomCallback callback)
    {
        if (FreezeCall.HasFlag(callback.customCall))
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
        if (mobilityCall.HasFlag(callback.customCall))
        {
            switch (callback.customCall)
            {
                case HitPointCall.ClearMobility:
                    ClearLastActivatedInput();
                    break;
            }
        }
        if (attackCall.HasFlag(callback.customCall))
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
    }
    #region Shake Player Code
    private void Update()
    {
        if (Shake)
        {
            StartCoroutine(CallShake());
        }
        if (Input.GetKeyDown(KeyCode.Escape)) 
        {
            PlayNextAnimation(Animator.StringToHash("Walk_Backward"), 0.25f,false);
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            PlayNextAnimation(Animator.StringToHash("Walk_Forward"), 0.25f, false);
        }
    }
    IEnumerator CallShake()
    {
        float r_Xpos = UnityEngine.Random.Range(-0.05f, 0.05f);
        float r_Ypos = UnityEngine.Random.Range(-0.05f, 0.05f);
        _model.localPosition = new Vector3(r_Xpos, r_Ypos, 0f);
        yield return null;
        _model.localPosition = startPos;
    }
    public void SetShake(bool state)
    {
        if (Shake != state)
        {
            Shake = state;
            if (!state)
            {
                _model.localPosition = startPos;
            }
        }
    }
    public void EndShake()
    {
        SetShake(false);
        StopCoroutine(CallShake());
    }
    #endregion

    public void PlayNextAnimation(int animHash, float crossFadeTime, bool attackOverride = false, string triggerSet = "")
    {
        if (triggerSet != "") 
        {
            shadowAnim.SetTrigger(triggerSet);
            myAnim.SetTrigger(triggerSet);
            return;
        }
        if (attackOverride)
        {
            shadowAnim.Play(animHash,0,0);
            myAnim.Play(animHash, 0, 0);
        }
        else
        {
            myAnim.CrossFade(animHash, crossFadeTime, 0, 0);
            shadowAnim.CrossFade(animHash, crossFadeTime, 0, 0);
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
        _base.opponentPlayer._cHitstun.HandleAnimatorFreeze(true);
        _base.opponentPlayer._cGravity.HandleGravityFreeze(true);
    }
    public void SetSelfFreeze()
    {
        _base._cForce.HandleForceFreeze(true);
        _base._cHitstun.HandleAnimatorFreeze(true);
        _base._cGravity.HandleGravityFreeze(true);
    }
    public void SetSelfUnfreeze()
    {
        _base._cForce.HandleForceFreeze(false);
        _base._cHitstun.HandleAnimatorFreeze(false);
        _base._cGravity.HandleGravityFreeze(false);
    }
    public void SetHurtBoxToNoBlock() //Called in AnimEvent
    {
        _base._cHurtBox.SetHurboxState(HurtBoxType.NoBlock);
    }
    public void SetHurtBoxToBlockHigh() //Called in AnimEvent
    {
        StartCoroutine(SetCanBlock());
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

    public void SetHurtBoxToBlockLow() //Called in AnimEvent
    {
        StartCoroutine(SetCanBlock());
    }
    IEnumerator SetCanBlock()
    {
        yield return new WaitForSeconds(0.067f);
        canBlock = true;
    }
    #endregion

    public void SetActivatedInput(Character_Mobility inputToActivate, MobilityAnimation anim, float totalWaitTime)
    {
        if (lastAttack != null)
        {
            if ((CheckAttackState(lastAttack.dashCancelable) && inputToActivate.movementPriority == 2))
            {
                activatedInput = inputToActivate;
                _lastMovementState = lastMovementState.populated;
                StartCoroutine(_base._extraMoveAsset.TickMobilityAnimation(inputToActivate, anim, totalWaitTime, () => KillInput(totalWaitTime)));
            }
            else if ((CheckAttackState(lastAttack.JumpCancelable) && inputToActivate.movementPriority != 2))
            {
                activatedInput = inputToActivate;
                SetSelfUnfreeze();
                _base._cForce.CallLockKinematic();
                ClearLastAttack();
                _lastMovementState = lastMovementState.populated;
                StartCoroutine(_base._extraMoveAsset.TickMobilityAnimation(inputToActivate, anim, totalWaitTime, () => KillInput(totalWaitTime)));
            }
        }
        else
        {
            activatedInput = inputToActivate;
            _lastMovementState = lastMovementState.populated;
            StartCoroutine(_base._extraMoveAsset.TickMobilityAnimation(inputToActivate, anim, totalWaitTime, () => KillInput(totalWaitTime)));
        }
    }
    public void KillInput(float totalWaitTime)
    {
        StartCoroutine(WaitToKillInput(totalWaitTime));
    }
    IEnumerator WaitToKillInput(float time)
    {
        yield return new WaitForSeconds(time);
        NullifyMobilityOption();
    }
    public Character_Mobility returnActivatedInput()
    {
        return activatedInput;
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
            curretnAttackLevel = lastAttack.cancelProperty.cancelFrom;
        }
    }

    public void ClearLastActivatedInput()
    {
        if (activatedInput != null)
        {
            CountUpNegativeFrames(activatedInput.mobilityAnim.frameData.recovery);
        }
    }
    public void HaltTimer()
    {
        _timer.HaltTimer();
    }
    public void ResumeTimer()
    {
        _timer.ResumeTimer();
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
        PlayNextAnimation(lastAttack.attackHashes[0], 2 * (1f / lastAttack.AttackAnims.animClip.frameRate), true);
        StartCoroutine(lastAttack.AttackAnims.TickAnimFrameCount(lastAttack));
    }
    public void AddForceOnAttack(float forceValue)
    {
        _base._cForce.AddForceOnCommand(forceValue);
    }

    public void SetCanTransitionIdle(bool state) 
    {
        canTransitionIdle = state;
    }

    #region Projectile Code
    void ShootProjectile()
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
    void CountUpNegativeFrames(int lastNegativeFrames)
    {
        Debug.Log("Hit AddFrameCount");
        negativeFrameCount += lastNegativeFrames;
        if(activatedInput != null) 
        {
            NullifyMobilityOption();
        }
        if (lastAttack == null)
        {
            //if (lastAttack._moveType == MoveType.Stance && _base.comboList3_0.GetInnerStanceAttack(lastAttack).Item1.inStanceState)
            //{
                return;
           // }
            
        }
        else 
        {
            ClearLastAttack();
        }
    }
    public void NullifyMobilityOption() 
    {
        _base._cForce.ResetPriority();
        activatedInput.activeMove = false;
        activatedInput = null;
        _lastMovementState = lastMovementState.nullified;
    }
    public void ClearLastAttack()
    {
        lastAttack = null; 
        _base._cForce.CallUnlockKinematic();
        _lastAttackState = lastAttackState.nullified;
        inputWindowOpen = true;

        curretnAttackLevel = Cancel_State.NotCancellable;
    }
    public void EndAnim()
    {
        if (canTick)
        {
            Debug.Log("Hit End Anim");
            canTick = false;
            if (lastAttack != null)
            {
                if (lastAttack._moveType == MoveType.Normal)
                {
                    ClearLastAttack();
                }
            }
        }
    }
    #endregion
}
