using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

[System.Serializable]
public class HurtBox : CollisionDetection
{
    public HurtBoxType huBType;
    public ColliderType colliderType;
    private Attack_BaseProperties currentHitProperties;
    public Attack_BaseProperties CounterMoveProperty;
    public Dictionary<HurtBoxType, Callback> hitResponseDictionary;

    private HitBox currentHitbox;
    private Transform target;
    Callback endingFunction;

    List<HitBoxType> unblockableAttacks = new List<HitBoxType>()
    {
        HitBoxType.Throw,
        HitBoxType.CommandGrab_Air,
        HitBoxType.CommandGrab_Ground,
        HitBoxType.Unblockable,
    };
    public void SetHurtboxSizing(Character_HurtBoxSizing hu_Sizing)
    {
        SetHurtBoxSize(0, 0, false, ColliderType.Trigger, hu_Sizing);
        SetHurtboxState(huBType);
        SetupHitResponseDicitonary();
    }
    public void SetCounterMoveProperty(Attack_BaseProperties counterProperty)
    {
        CounterMoveProperty = counterProperty;
    }
    public void SetupHitResponseDicitonary()
    {
        hitResponseDictionary = new Dictionary<HurtBoxType, Callback>();
        for (int i = 0; i < Enum.GetNames(typeof(HurtBoxType)).Length; i++)
        {
            HurtBoxType curType = (HurtBoxType)i;
            if (curType == HurtBoxType.NoBlock)
            {
                hitResponseDictionary.Add(curType, () => DoHitResponse());
            }
            else 
            {
                hitResponseDictionary.Add(curType, DetermineIfAttackPossible);
            }
        }
    }
    void DetermineIfAttackPossible() 
    {
        if (CheckAttackIfBlockLow())
        {
            //DoHitResponse(true);
        }
        if (CheckAttackIfBlockHigh())
        {
            //DoHitResponse(true);
        }
        if (CheckAttackIfParryLow())
        {
            //DoHitResponse(true);
        }
        if (CheckAttackIfParryHigh())
        {
            //DoHitResponse(true);
        }
        if (CheckAttackIfSoftKnockdown())
        {
            //DoHitResponse(true);
        }
        if (CheckAttackIfHardKnockdown())
        {
            //DoHitResponse(true);
        }
        if (CheckAttackIfInvincible())
        {
            //DoHitResponse(true);
        }
        if (CheckAttackIfArmor())
        {
            //DoHitResponse(true);
        }
        if (CheckAttackIfFullParry())
        {
            //DoHitResponse(true);
        }
        DoHitResponse(false);
    }

    bool CheckAttackIfBlockLow() 
    {
        return false;
    }
    bool CheckAttackIfBlockHigh()
    {
        return false;
    }
    bool CheckAttackIfParryLow()
    {
        return false;
    }
    bool CheckAttackIfParryHigh()
    {
        return false;
    }
    bool CheckAttackIfSoftKnockdown()
    {
        return false;
    }
    bool CheckAttackIfHardKnockdown()
    {
        return false;
    }
    bool CheckAttackIfInvincible()
    {
        return false;
    }
    bool CheckAttackIfArmor()
    {
        return false;
    }
    bool CheckAttackIfFullParry()
    {
        return false;
    }



    void FindAttackResponse() 
    {
        Callback hitResponse = null;
        if (hitResponseDictionary.TryGetValue(huBType, out hitResponse)) 
        {
            hitResponse();
        }
    }
    public void ReceieveHitBox(HitBox _hitbox, Transform _target,Callback endFunc)
    {
        currentHitbox = _hitbox;
        target = _target;
        endingFunction = endFunc;
        FindAttackResponse();

        switch (huBType)
        {
            case HurtBoxType.NoBlock:
                //Send Do Damage;
                OnSuccessfulHit(_hitbox, target, endFunc);
                break;
            case HurtBoxType.BlockLow:
                if (_hitbox.HBType == HitBoxType.Low)
                {
                    OnSuccessfulBlock(_hitbox, target, endFunc);
                }
                else 
                {
                    if (_hitbox.HBType == HitBoxType.Throw)
                    {
                        //Send Throw;
                    }
                    else if (_hitbox.HBType == HitBoxType.CommandGrab_Ground)
                    {
                        if (this.GetComponentInParent<Character_Base>()._cHurtBox.IsGrounded()) 
                        {
                            //Send Command Throw;
                            return;
                        }
                        //Send Not Connected;
                    }
                    else if (_hitbox.HBType == HitBoxType.CommandGrab_Air)
                    {
                        if (!this.GetComponentInParent<Character_Base>()._cHurtBox.IsGrounded())
                        {
                            //Send Command Throw;
                            return;
                        }
                        //Send Not Connected;
                    }
                    else 
                    {
                        //Send Do Damage;
                    }
                }
                break;
            case HurtBoxType.BlockHigh:
                if (_hitbox.HBType == HitBoxType.High ^ _hitbox.HBType == HitBoxType.Overhead)
                {
                    OnSuccessfulBlock(_hitbox, target, endFunc);
                }
                else
                {
                    if (_hitbox.HBType == HitBoxType.Throw)
                    {
                        //Send Throw;
                    }
                    else if (_hitbox.HBType == HitBoxType.CommandGrab_Ground)
                    {
                        if (this.GetComponentInParent<Character_Base>()._cHurtBox.IsGrounded())
                        {
                            //Send Command Throw;
                            return;
                        }
                        //Send Not Connected;
                    }
                    else if (_hitbox.HBType == HitBoxType.CommandGrab_Air)
                    {
                        if (!this.GetComponentInParent<Character_Base>()._cHurtBox.IsGrounded())
                        {
                            //Send Command Throw;
                            return;
                        }
                        //Send Not Connected;
                    }
                    else
                    {
                        //Send Do Damage;
                    }
                }
                break;
            case HurtBoxType.ParryLow:
                if (_hitbox.HBType == HitBoxType.Low ^ _hitbox.HBType == HitBoxType.Unblockable)
                {
                    //Send ParrierForceIdle();
                }
                else
                {
                    if (_hitbox.HBType == HitBoxType.Throw)
                    {
                        //Send Throw With Counter Hit;
                    }
                    else if (_hitbox.HBType == HitBoxType.CommandGrab_Ground)
                    {
                        if (this.GetComponentInParent<Character_Base>()._cHurtBox.IsGrounded())
                        {
                            //Send Command Throw;
                            return;
                        }
                        //Send Not Connected;
                    }
                    else if (_hitbox.HBType == HitBoxType.CommandGrab_Air)
                    {
                        if (!this.GetComponentInParent<Character_Base>()._cHurtBox.IsGrounded())
                        {
                            //Send Command Throw;
                            return;
                        }
                        //Send Not Connected;
                    }
                    else if (_hitbox.HBType == HitBoxType.High)
                    {
                        //Send High With Counter Hit;
                    }
                }
                break;
            case HurtBoxType.ParryHigh:
                if (_hitbox.HBType == HitBoxType.High ^ _hitbox.HBType == HitBoxType.Overhead ^ _hitbox.HBType == HitBoxType.Unblockable)
                {
                    //Send ParrierForceIdle();
                }
                else
                {
                    if (_hitbox.HBType == HitBoxType.Throw)
                    {
                        //Send Throw With Counter Hit;
                    }
                    else if (_hitbox.HBType == HitBoxType.CommandGrab_Ground)
                    {
                        if (this.GetComponentInParent<Character_Base>()._cHurtBox.IsGrounded())
                        {
                            //Send Command Throw;
                            return;
                        }
                        //Send Not Connected;
                    }
                    else if (_hitbox.HBType == HitBoxType.CommandGrab_Air)
                    {
                        if (!this.GetComponentInParent<Character_Base>()._cHurtBox.IsGrounded())
                        {
                            //Send Command Throw;
                            return;
                        }
                        //Send Not Connected;
                    }
                    else if (_hitbox.HBType == HitBoxType.Low)
                    {
                        //Send Low With Counter Hit;
                    }
                }
                break;
            case HurtBoxType.FullParry:
                if (_hitbox.HBType == HitBoxType.High ^ _hitbox.HBType == HitBoxType.Overhead ^ _hitbox.HBType == HitBoxType.Unblockable ^ _hitbox.HBType == HitBoxType.Low)
                {
                    OnSuccessfulCounter(_hitbox, target, endFunc);
                }
                else
                {
                    if (_hitbox.HBType == HitBoxType.Throw)
                    {
                        //Send Throw With Counter Hit;
                    }
                    else if (_hitbox.HBType == HitBoxType.CommandGrab_Ground)
                    {
                        if (this.GetComponentInParent<Character_Base>()._cHurtBox.IsGrounded())
                        {
                            //Send Command Throw;
                            return;
                        }
                        //Send Not Connected;
                    }
                    else if (_hitbox.HBType == HitBoxType.CommandGrab_Air)
                    {
                        if (!this.GetComponentInParent<Character_Base>()._cHurtBox.IsGrounded())
                        {
                            //Send Command Throw;
                            return;
                        }
                        //Send Not Connected;
                    }
                }
                break;
            case HurtBoxType.SoftKnockdown:
                //Send Hit on SoftKnockdown
                break;
            case HurtBoxType.HardKnockdown:
                if (_hitbox.HBType == HitBoxType.Low)
                {
                    //Send Hit with Low;
                }
                break;
            case HurtBoxType.Invincible:
                //Send Hit InvulBody
                break;
            case HurtBoxType.Armor:
                if (_hitbox.HBType == HitBoxType.Throw)
                {
                    //Send Throw;
                }
                else if (_hitbox.HBType == HitBoxType.CommandGrab_Ground)
                {
                    //Send Command Throw;
                }
                else if (_hitbox.HBType == HitBoxType.CommandGrab_Air)
                {
                    //Send Not Connected;
                }
                else if (_hitbox.HBType == HitBoxType.Unblockable)
                {
                    //Send Hit with Unblockable;
                }
                else 
                {
                    //Send reduce unit of Armor by 1
                }
                break;
            default:
                DebugMessageHandler.instance.DisplayErrorMessage(1, $"Invalid HurtboxType Detected.");
                break;
        }
    }

    void DoHitResponse(bool blockedAttack = false) 
    {
        if (blockedAttack)
        {

        }
        else 
        {

        }
    }
    public async void OnSuccessfulBlock(HitBox _hitbox, Transform target, Callback endFunc)
    {
        Character_Base Base_Target = target.GetComponent<Character_Base>();
        Character_Base Base_Attacker = _hitbox.GetComponentInParent<Character_Base>();
        if (_hitbox.hitboxProperties != null)
        {
            currentHitProperties = _hitbox.hitboxProperties;
        }
        HitCount hitCount = currentHitProperties.AttackAnims._hitCount;
        if (hitCount._count > 1)
        {
            if (_hitbox.HBType != HitBoxType.nullified)
            {
                currentHitProperties.hitConnected = true;
                StartCoroutine(DoMultiHit_OnBlock(_hitbox, hitCount, Base_Target, Base_Attacker));
            }
        }
        else if (hitCount._count <= 1)
        {
            if (_hitbox.HBType != HitBoxType.nullified)
            {
                Attack_BaseProperties currentAttack = Base_Attacker._cHitboxManager.GetActiveHitBox().hitboxProperties;

                currentAttack.hitConnected = true;
                Base_Attacker.comboList3_0.NewCheckAndApply(Base_Target, Base_Attacker, true, currentHitProperties);
                await Character_Hitstop.Instance.CallHitStop(currentAttack, currentAttack.hitstopValue/5f, Base_Target);
                //Base_Target._cHitController.HandleBlockState(currentAttack);
                //await Base_Target._cHitstun.ApplyHitStun(currentAttack.hitstunValue/5f);
                _hitbox.DestroyHitbox(_hitbox, Base_Attacker.pSide.thisPosition.GiveHurtBox());
            }
        }
        endFunc();
    }
    public async void OnSuccessfulHit(HitBox _hitbox, Transform target, Callback endFunc) 
    {
        Character_Base Base_Target = target.GetComponent<Character_Base>();
        Character_Base Base_Attacker = _hitbox.GetComponentInParent<Character_Base>();
        if (_hitbox.hitboxProperties != null)
        {
            currentHitProperties = _hitbox.hitboxProperties;
        }
        HitCount hitCount = currentHitProperties.AttackAnims._hitCount;
        if (hitCount._count > 1)
        {
            if (_hitbox.HBType != HitBoxType.nullified)
            {
                currentHitProperties.hitConnected = true;
                StartCoroutine(DoMultiHit(_hitbox, hitCount, Base_Target, Base_Attacker));
            }
        }
        else if (hitCount._count <= 1)
        {
            if (_hitbox.HBType != HitBoxType.nullified)
            {
                currentHitProperties.hitConnected = true;

                Base_Attacker.comboList3_0.NewCheckAndApply(Base_Target, Base_Attacker, false, currentHitProperties);
                await Character_Hitstop.Instance.CallHitStop(currentHitProperties, currentHitProperties.hitstopValue, Base_Target);
                //Base_Target._cHitController.HandleHitState(currentHitProperties);
                Base_Target._cGravity.UpdateGravityScaleOnHit(currentHitProperties.hitstunValue);
                //await Base_Target._cHitstun.ApplyHitStun(currentHitProperties.hitstunValue);
                _hitbox.DestroyHitbox(_hitbox, Base_Attacker.pSide.thisPosition.GiveHurtBox());
            }
        }
        endFunc();
    }
    
    public async void OnSuccessfulCounter(HitBox _hitbox, Transform target, Callback endFunc)
    {
        Character_Base Base_Target = _hitbox.GetComponentInParent<Character_Base>();
        Character_Base Base_Attacker = target.GetComponentInParent<Character_Base>();
        if (Base_Attacker._cAnimator.lastAttack != null) 
        {
            CounterMoveProperty = Base_Attacker._cAnimator.lastAttack;
        }
        
        if (_hitbox.HBType != HitBoxType.nullified)
        {
            Attack_BaseProperties currentAttack = Base_Attacker._cHitboxManager.GetActiveHitBox().hitboxProperties;
            currentAttack.hitConnected = true;
            Base_Attacker.comboList3_0.NewCheckAndApply(Base_Target, Base_Attacker, false, currentHitProperties);
            await Character_Hitstop.Instance.CallHitStop(currentAttack, currentAttack.hitstopValue, Base_Target);
            Base_Target._cGravity.UpdateGravityScaleOnHit(currentAttack.hitstunValue);
            //await Base_Target._cHitstun.ApplyHitStun(currentAttack.hitstunValue);
            _hitbox.DestroyHitbox(_hitbox, Base_Attacker.pSide.thisPosition.GiveHurtBox());
        }
        endFunc();
    }

    #region Multi-Hit Functions
    #region Hit Portion Functions
    IEnumerator DoMultiHit(HitBox _hitbox, HitCount hitCount, Character_Base Base_Target, Character_Base Base_Attacker)
    {
        //Base_Target.comboList3_0.Check(attackName, Base_Target);
        Base_Target._cAnimator.isHit = true;
        HandleMultiHitProperties(_hitbox, hitCount, Base_Target, Base_Attacker);
        Base_Target._cGravity.UpdateGravityScaleOnHit(_hitbox.hitboxProperties.hitstunValue);// Base_Attacker._cAnimator.lastAttack.hitstunValue);
        for (int i = 0; i < hitCount._count; i++)
        {
            yield return new WaitForSeconds(hitCount._refreshRate);
            Base_Attacker.comboList3_0.NewCheckAndApply(Base_Target, Base_Attacker, false, currentHitProperties);
        }
        _hitbox.DestroyHitbox(_hitbox, Base_Attacker.pSide.thisPosition.GiveHurtBox());
        hitCount.ResetRefresh();
        hitCount.ResetHitCount();
        //Base_Attacker._cAnimator.ClearLastAttack();
    }
    async void HandleMultiHitProperties(HitBox _hitbox, HitCount hitCount, Character_Base Base_Target, Character_Base Base_Attacker)
    {
        Base_Target._cForce.SendKnockBackOnHit(_hitbox.hitboxProperties);
        //Base_Target._cHitController.HandleHitState(_hitbox.hitboxProperties);
        //await Base_Target._cHitstun.ApplyHitStun(_hitbox.hitboxProperties.hitstunValue);
        await Character_Hitstop.Instance.CallHitStop(_hitbox.hitboxProperties, _hitbox.hitboxProperties.hitstopValue, Base_Target);
    }
    #endregion

    #region Block Portion Functions
    IEnumerator DoMultiHit_OnBlock(HitBox _hitbox, HitCount hitCount, Character_Base Base_Target, Character_Base Base_Attacker)
    {
        //Base_Target.comboList3_0.Check(attackName, Base_Target);
        Base_Target._cAnimator.isHit = true;
        HandleMultiHitProperties_OnBlock(_hitbox, hitCount, Base_Target, Base_Attacker);
        for (int i = 0; i < hitCount._count; i++)
        {
            yield return new WaitForSeconds(hitCount._refreshRate);
            Base_Attacker.comboList3_0.NewCheckAndApply(Base_Target, Base_Attacker, true, currentHitProperties);
        }
        _hitbox.DestroyHitbox(_hitbox, Base_Attacker.pSide.thisPosition.GiveHurtBox());
        hitCount.ResetRefresh();
        hitCount.ResetHitCount();
        //Base_Attacker._cAnimator.ClearLastAttack();
    }
    async void HandleMultiHitProperties_OnBlock(HitBox _hitbox, HitCount hitCount, Character_Base Base_Target, Character_Base Base_Attacker)
    {
        Base_Target._cForce.SendKnockBackOnHit(_hitbox.hitboxProperties);
        Base_Target._cHitController.HandleBlockState(_hitbox.hitboxProperties);
        //await Base_Target._cHitstun.ApplyHitStun(_hitbox.hitboxProperties.hitstunValue/5f);
        await Character_Hitstop.Instance.CallHitStop(_hitbox.hitboxProperties, _hitbox.hitboxProperties.hitstopValue / 5f, Base_Target);
    }
    #endregion
    #endregion

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Wall")
        {
            if (currentHitProperties != null)
            {
                Character_Force thisPlayerForce = this.gameObject.transform.root.GetComponent<Character_Base>()._cForce;
                Character_HurtboxController thisPlayerGroundedState = this.gameObject.transform.root.GetComponent<Character_Base>()._cHurtBox;
                if (currentHitProperties.lateralKBP.lateralKBP == Attack_KnockBack_Lateral.FullForceWallBounce)
                {
                    StartCoroutine(thisPlayerForce.DoWallLaunch());
                    Debug.Log("Will Wall Bounce on Hit");
                }
            }

            else
            {
                Character_Base thisBase = this.gameObject.transform.root.GetComponent<Character_Base>();
                Player_SideRecognition thisPSide = this.gameObject.transform.root.GetComponent<Character_Base>().pSide;
                thisBase._cForce.AddLateralForceOnCommand(0.75f);
                Debug.Log("Hit Wall Frame 1");
            }
        }
        else if (other.gameObject.tag == "Ground") 
        {
            if (currentHitProperties != null)
            {
                Character_Force thisPlayerForce = this.gameObject.transform.root.GetComponent<Character_Base>()._cForce;
                Character_HurtboxController thisPlayerGroundedState = this.gameObject.transform.root.GetComponent<Character_Base>()._cHurtBox;
                if (currentHitProperties.verticalKBP.verticalKBP == Attack_KnockBack_Vertical.FullForceGroundBounce)
                {
                    if (thisPlayerGroundedState.IsGrounded() == false)
                    {
                        thisPlayerForce.DoGroundBounce(currentHitProperties);
                        Debug.Log("Will Ground Bounce");
                    }
                }
            }
        }
    }
}
