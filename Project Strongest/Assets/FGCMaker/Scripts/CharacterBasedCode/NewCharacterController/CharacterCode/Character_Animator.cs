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

    float frameCount;

    internal int negativeFrameCount;

    bool init;
    bool startup;
    bool active;
    bool inactive;
    Vector3 startPos;

    private void Start()
    {
        inputWindowOpen = true;
        startPos = _model.localPosition;
        Messenger.AddListener(Events.ClearLastInput, ClearLastActivatedInput);
    }
    public void PlayNextAnimation(int animHash, float crossFadeTime, bool attackOverride = false)
    {
        if (attackOverride)
        {
            myAnim.Play(animHash, 0, 0);
        }
        else 
        {
            myAnim.CrossFade(animHash, crossFadeTime, 0, 0);
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
                StartMobiltyFrameCount(inputToActivate, anim, totalWaitTime);
            }
            else if ((CheckAttackState(lastAttack.JumpCancelable) && inputToActivate.movementPriority != 2))
            {
                activatedInput = inputToActivate;
                SetSelfUnfreeze();
                _base._cForce.CallLockKinematic();
                ClearLastAttack();
                _lastMovementState = lastMovementState.populated;
                StartMobiltyFrameCount(inputToActivate, anim, totalWaitTime);
            }
        }
        else
        {

            activatedInput = inputToActivate;
            _lastMovementState = lastMovementState.populated;
            StartMobiltyFrameCount(inputToActivate, anim, totalWaitTime);
        }
    }
    IEnumerator WaitToKillInput(float time) 
    {
        yield return new WaitForSeconds(time);
        NullifyMobilityOption();
    }
    public void StartMobiltyFrameCount(Character_Mobility inputToActivate, MobilityAnimation anim, float totalWaitTime)
    {
        StartCoroutine(TickMobilityAnimation(inputToActivate, anim, totalWaitTime));
    }

    IEnumerator TickMobilityAnimation(Character_Mobility inputToActivate, MobilityAnimation anim, float totalWaitTime)
    {
        float waitTime = 1f / 60f;
        SetStartValues();

        PlayNextAnimation(Animator.StringToHash(anim.animName[0]), 0.25f);
        while (frameCount <= anim.animLength[0])
        {
            #region Mobility Anim Checks
            for (int i = 0; i < anim.frameData._extraPoints.Count; i++)
            {
                ExtraFrameHitPoints newHitPoint = anim.frameData._extraPoints[i];
                if (frameCount >= waitTime * newHitPoint.hitFramePoints && newHitPoint.hitFrameBools == false)
                {
                    CheckCallState(newHitPoint, inputToActivate);
                    newHitPoint.hitFrameBools = true;
                }
            }
            frameCount += waitTime;
            yield return new WaitForSeconds(waitTime);
            #endregion
        }
        StartCoroutine(WaitToKillInput(totalWaitTime));
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
            if (lastAttack.AttackAnims.HitBox != null) 
            {
                lastAttack.AttackAnims.HitBox.hitboxProperties = _attack;
            }
            _lastAttackState = lastAttackState.populated;
        }
    }

    public void ClearLastActivatedInput()
    {
        activatedInput.activeMove = false;
        CountUpNegativeFrames(activatedInput.mobilityAnim.frameData.recovery);
    }

    private void Update()
    {
        if (Shake)
        {
            StartCoroutine(CallShake());
        }
    }
    public void SetStartValues()
    {
        frameCount = 0;
        init = false;
        startup = false;
        active = false;
        inactive = false;
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
        if (nextattack.AttackAnims._frameData._extraPoints.Count > 0)
        {
            for (int i = 0; i < lastAttack.AttackAnims._frameData._extraPoints.Count; i++)
            {
                lastAttack.AttackAnims._frameData._extraPoints[i].hitFrameBools = false;
            }
        }
        StartFrameCount(nextattack);

    }
    public void StartFrameCount(Attack_BaseProperties nextattack)
    {
        StartCoroutine(TickAnimFrameCount());
    }

    IEnumerator TickAnimFrameCount()
    {
        float waitTime = 1f / 60f;
        SetStartValues();

        while (frameCount <= lastAttack.AttackAnims.animLength)
        {
            try
            {
                if (frameCount >= waitTime * lastAttack.AttackAnims._frameData.init && init == false)
                {
                    inputWindowOpen = false;
                    Debug.Log(frameCount * 60 + "on init");
                    lastAttack.AttackAnims.OnInit(_base, lastAttack);
                    init = true;
                }
                if (frameCount >= waitTime * lastAttack.AttackAnims._frameData.startup && startup == false)
                {
                    Debug.Log(frameCount * 60 + "on startup");
                    lastAttack.AttackAnims.OnStartup(_base);
                    startup = true;
                }
                if (frameCount >= waitTime * lastAttack.AttackAnims._frameData.active && active == false)
                {
                    Debug.Log(frameCount * 60 + "on active");
                    lastAttack.AttackAnims.OnActive(_base);
                    active = true;
                }
                if (frameCount >= waitTime * lastAttack.AttackAnims._frameData.inactive && inactive == false)
                {
                    inputWindowOpen = true;
                    Debug.Log(frameCount * 60 + "on recovery");
                    inactive = true;
                    lastAttack.AttackAnims.OnRecov(_base);
                }
                if (lastAttack.AttackAnims._frameData._extraPoints.Count > 0)
                {
                    for (int i = 0; i < lastAttack.AttackAnims._frameData._extraPoints.Count; i++)
                    {
                        ExtraFrameHitPoints newHitPoint = lastAttack.AttackAnims._frameData._extraPoints[i];
                        if (frameCount >= waitTime * newHitPoint.hitFramePoints && newHitPoint.hitFrameBools == false)
                        {
                            CheckCallState(newHitPoint);
                            newHitPoint.hitFrameBools = true;
                        }
                    }
                }
            }
            catch (Exception)
            {
                frameCount = lastAttack.AttackAnims.animLength + 1f;
                Debug.Log("Null Check");
                Debug.Log($"Inactive frame: {lastAttack.AttackAnims._frameData.inactive}");
                Debug.Log($"Last Attack null?: {lastAttack == null}");
                Debug.Log($"Inactive bool state: {inactive}");
                Debug.Break();
            }
            frameCount += 1f * waitTime;
            yield return new WaitForSeconds(waitTime);
        }
      
        CountUpNegativeFrames(lastAttack.AttackAnims._frameData.recovery);
    }
    void CheckCallState(ExtraFrameHitPoints newHitPoint, Character_Mobility mobility = null) 
    {
        switch (newHitPoint.call)
        {
            case HitPointCall.ActivateMobilityAction:
                _base._extraMoveAsset.CallMobilityAction(mobility);
                break;
            case HitPointCall.ClearMobility:
                ClearLastActivatedInput();
                break;
            case HitPointCall.ToggleFreeze_Both:
                SetSelfFreeze();
                SetOpponentFreeze();
                break;
            case HitPointCall.UnFreeze:
                _base.opponentPlayer._cAnimator.SetSelfUnfreeze();
                SetSelfUnfreeze();
                break;
            case HitPointCall.ToggleFreeze_Self:
                SetSelfFreeze();
                break;
            case HitPointCall.ToggleFreeze_Other:
                _base.opponentPlayer._cAnimator.SetSelfFreeze();
                break;
            case HitPointCall.Phase:
                //ToggleOpponentFreeze();
                //ClearLastAttack();
                break;
            case HitPointCall.ShootProjectile:
                ShootProjectile();
                break;
            case HitPointCall.Force_Small:
                AddForceOnAttack(3.15f);
                break;

            case HitPointCall.Force_Medium:
                AddForceOnAttack(6);
                break;

            case HitPointCall.Force_Large:
                AddForceOnAttack(10);
                break;
            case HitPointCall.Teleport:
                break;

            case HitPointCall.KillStance:
                ClearLastAttack();
                _base._cAttackTimer.SetTimerType();
                break;
        }
    }
    public void AddForceOnAttack(float forceValue)
    {
        _base._cForce.AddForceOnCommand(forceValue);
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

        if (lastAttack != null)
        {
            if (lastAttack._moveType == MoveType.Stance && _base.comboList3_0.GetInnerStanceAttack(lastAttack).Item1.inStanceState) 
            {
                return;
            }
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
