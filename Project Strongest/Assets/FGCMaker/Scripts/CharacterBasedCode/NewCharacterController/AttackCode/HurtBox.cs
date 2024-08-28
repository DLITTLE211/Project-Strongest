using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
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
    public Dictionary<AttackBoolCheck, Callback> AttackCheckDictionary;

    private HitBox currentHitbox;
    private Transform target;
    Callback endingFunction;
    HitboxTypeList refList;
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
        refList = new HitboxTypeList();
        hitResponseDictionary = new Dictionary<HurtBoxType, Callback>();
        for (int i = 0; i < Enum.GetNames(typeof(HurtBoxType)).Length; i++)
        {
            HurtBoxType curType = (HurtBoxType)i;
            if (curType == HurtBoxType.NoBlock)
            {
                hitResponseDictionary.Add(curType, () => ReceiveAttackData());
            }
            else 
            {
                hitResponseDictionary.Add(curType, DetermineIfAttackPossible);
            }
        }
    }
    void SetAttackCheckDictionary() 
    {
        AttackCheckDictionary = new Dictionary<AttackBoolCheck, Callback>();

        #region FullCheckList

        #region LowCheck
        AttackBoolCheck lowblock = delegate()
        {
            return CheckAttackIfBlockLow();
        };
        AttackCheckDictionary.Add(lowblock, () =>ReceiveAttackData(true));
        #endregion

        #region HighCheck
        AttackBoolCheck highBlock = delegate ()
        {
            return CheckAttackIfBlockHigh();
        };
        AttackCheckDictionary.Add(highBlock, () => ReceiveAttackData(true));
        #endregion

        #region ParryLowCheck
        AttackBoolCheck ParryLowCheck = delegate ()
        {
            return CheckAttackIfParryLow();
        };
        AttackCheckDictionary.Add(ParryLowCheck, () => ReceiveParryData());
        #endregion

        #region ParryHighCheck
        AttackBoolCheck ParryHighCheck = delegate ()
        {
            return CheckAttackIfParryHigh();
        };
        AttackCheckDictionary.Add(ParryHighCheck, () => ReceiveParryData());
        #endregion

        #region SKD
        AttackBoolCheck SKDCheck = delegate ()
        {
            return CheckAttackIfSoftKnockdown();
        };
        AttackCheckDictionary.Add(SKDCheck, null);
        #endregion

        #region HKD
        AttackBoolCheck HKDCheck = delegate ()
        {
            return CheckAttackIfHardKnockdown();
        };
        AttackCheckDictionary.Add(HKDCheck, null);
        #endregion

        #region InvicibilityCheck
        AttackBoolCheck FullInvulCheck = delegate ()
        {
            return CheckAttackIfInvincible();
        };
        AttackCheckDictionary.Add(FullInvulCheck, null);
        #endregion

        #region Armor Check
        AttackBoolCheck ArmorCheck = delegate ()
        {
            return CheckAttackIfArmor();
        };
        AttackCheckDictionary.Add(ArmorCheck, () => ReceiveAttackData(true));
        #endregion

        #region Full Counter Check
        AttackBoolCheck FullCounter = delegate ()
        {
            return CheckAttackIfFullCounter();
        };
        AttackCheckDictionary.Add(FullCounter, () => ReceiveCounterData());
        #endregion

        #region Low Immunity
        AttackBoolCheck LowImmune = delegate ()
        {
            return CheckAttackIfLowImmune();
        };
        AttackCheckDictionary.Add(LowImmune,null);
        #endregion

        #region High Immunity
        AttackBoolCheck HighImmune = delegate ()
        {
            return CheckAttackIfHighImmune();
        };
        AttackCheckDictionary.Add(HighImmune, null);
        #endregion
        #endregion
    }

    void FindAttackResponse()
    {
        Callback hitResponse = null;
        if (AttackCheckDictionary == null) 
        {
            SetAttackCheckDictionary();
        }
        if (hitResponseDictionary.TryGetValue(huBType, out hitResponse))
        {
            hitResponse();
        }
    }
    void DetermineIfAttackPossible() 
    {
        for (int i = 0; i < AttackCheckDictionary.Count; i++) 
        {
            KeyValuePair<AttackBoolCheck, Callback> boolCallbackCombo = AttackCheckDictionary.ElementAt(i);
            if (boolCallbackCombo.Key()) 
            {
                if (boolCallbackCombo.Value != null)
                {
                    boolCallbackCombo.Value();
                }
                return;
            }
            continue;
        }
        ReceiveAttackData(false);
        return;
    }

    #region Boolean Check Field
    bool CheckAttackIfBlockLow() 
    {
        bool lowAttack = refList.LowOKList.Contains(currentHitbox.HBType);
        bool lowBlock = huBType == HurtBoxType.BlockLow;
        return lowAttack && lowBlock;
    }
    bool CheckAttackIfBlockHigh()
    {
        bool highAttack = refList.HighAttacks.Contains(currentHitbox.HBType);
        bool highBlock = huBType == HurtBoxType.BlockHigh;
        return highAttack && highBlock;
    }
    bool CheckAttackIfParryLow()
    {
        bool lowAttack = currentHitbox.HBType == HitBoxType.Low;
        bool parryLow = huBType == HurtBoxType.ParryLow;
        return lowAttack && parryLow;
    }
    bool CheckAttackIfParryHigh()
    {
        bool highAttack = currentHitbox.HBType == HitBoxType.High;
        bool parryHigh = huBType == HurtBoxType.ParryHigh;
        return highAttack && parryHigh;
    }
    bool CheckAttackIfFullCounter()
    {
        bool armorThroughAttack = !refList.GrabList.Contains(currentHitbox.HBType);
        bool parryFull = huBType == HurtBoxType.FullCounter;
        return armorThroughAttack && parryFull;
    }
    bool CheckAttackIfSoftKnockdown()
    {
        return huBType == HurtBoxType.SoftKnockdown;
    }
    bool CheckAttackIfHardKnockdown()
    {
        bool lowAttack = (refList.HighAttacks.Contains(currentHitbox.HBType) && !refList.GrabList.Contains(currentHitbox.HBType)  && currentHitbox.hitboxProperties.KnockDown.HasFlag(Attack_KnockDown.NONE));
        bool hardKnockdown = huBType == HurtBoxType.HardKnockdown;
        return lowAttack && hardKnockdown;
    }
    bool CheckAttackIfInvincible()
    {
        return huBType == HurtBoxType.Invincible;
    }
    bool CheckAttackIfArmor()
    {
        bool armorThroughAttack = !refList.GrabList.Contains(currentHitbox.HBType);
        bool armorState = huBType == HurtBoxType.Armor;
        return armorThroughAttack && armorState;
    }
    bool CheckAttackIfHighImmune()
    {
        bool highAttack = refList.HighAttacks.Contains(currentHitbox.HBType);
        bool armorState = huBType == HurtBoxType.HighImmune;
        return highAttack && armorState;
    }
    bool CheckAttackIfLowImmune()
    {
        bool lowAttack = currentHitbox.HBType == HitBoxType.Low;
        bool parryFull = huBType == HurtBoxType.LowImmune;
        return lowAttack && parryFull;
    }
    #endregion

    public void ReceieveHitBox(HitBox _hitbox, Transform _target,Callback endFunc)
    {
        currentHitbox = _hitbox;
        currentHitProperties = currentHitbox.hitboxProperties;
        target = _target;
        endingFunction = endFunc;
        FindAttackResponse();
    }
    async void ReceiveCounterData()
    {
        Character_Base Base_Target = currentHitbox.GetComponentInParent<Character_Base>();
        Character_Base Base_Attacker = target.GetComponentInParent<Character_Base>();
        if (Base_Attacker._cAnimator.lastAttack != null)
        {
            CounterMoveProperty = Base_Attacker._cAnimator.lastAttack;
        }

        if (currentHitbox.HBType != HitBoxType.nullified)
        {
            Attack_BaseProperties currentAttack = Base_Attacker._cHitboxManager.GetActiveHitBox().hitboxProperties;
            currentAttack.hitConnected = true;
            Base_Attacker.comboList3_0.NewCheckAndApply(Base_Target, Base_Attacker, false, currentHitProperties);
            //await Character_Hitstop.Instance.CallHitStop(currentAttack, currentAttack.hitstopValue, Base_Target);
            Base_Target._cGravity.UpdateGravityScaleOnHit(currentAttack.hitstunValue);
            //await Base_Target._cHitstun.ApplyHitStun(currentAttack.hitstunValue);
            currentHitbox.DestroyHitbox(currentHitbox, Base_Attacker.pSide.thisPosition.GiveHurtBox());
        }
        endingFunction();
        endingFunction = null;
    }
    void ReceiveParryData() 
    {
        Character_Base Base_Target = currentHitbox.GetComponentInParent<Character_Base>();
        Character_Base Base_Attacker = target.GetComponentInParent<Character_Base>();
        Base_Attacker._aManager.ClearAttacks();
        Base_Attacker._cHitstop.TriggerHitStop(currentHitProperties, (currentHitProperties.hitstopValue), Base_Target);
        endingFunction();
        endingFunction = null;
    }
    void ReceiveAttackData(bool blockedAttack = false) 
    {
        StartCoroutine(HandleHitResponse(blockedAttack));
    }
    IEnumerator HandleHitResponse(bool BlockedAttack)
    {
        Character_Base Base_Target = target.GetComponent<Character_Base>();
        Character_Base Base_Attacker = currentHitbox.GetComponentInParent<Character_Base>();
        HitCount hitCount = currentHitProperties.AttackAnims._hitCount;
        int curHit = 0;
        while (curHit < hitCount._count) 
        {
            currentHitProperties.hitConnected = true;
            Base_Attacker.comboList3_0.NewCheckAndApply(Base_Target, Base_Attacker, BlockedAttack, currentHitProperties);
            if (BlockedAttack)
            {
                Base_Attacker._cHitstop.TriggerHitStop(currentHitProperties, (currentHitProperties.hitstopValue / 10f), Base_Target);
            }
            else
            {
                Base_Attacker._cHitstop.TriggerHitStop(currentHitProperties, (currentHitProperties.hitstopValue), Base_Target);
                Base_Target._cGravity.UpdateGravityScaleOnHit(currentHitProperties.hitstunValue);
            }
            if (hitCount._refreshRate > 0)
            {
                yield return new WaitForSeconds(hitCount._refreshRate * (1 / 60f));
            }
            else
            {
                yield return new WaitForSeconds(1 / 60f);
            }
            curHit++;
        }
        hitCount.ResetRefresh();
        hitCount.ResetHitCount();
        endingFunction();
        endingFunction = null;
        currentHitbox.DestroyHitbox(currentHitbox, currentHitbox.hitboxProperties.AttackAnims.extendedHitBox);
    }

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


[Serializable]
public class HitboxTypeList 
{
    public List<HitBoxType> GrabList = new List<HitBoxType>()
    {
        HitBoxType.Throw,
        HitBoxType.CommandGrab_Air,
        HitBoxType.CommandGrab_Ground,
    }; public List<HitBoxType> LowOKList = new List<HitBoxType>()
    {
        HitBoxType.High,
        HitBoxType.Anti_Air,
        HitBoxType.Low,
    };
    public List<HitBoxType> HighAttacks = new List<HitBoxType>()
    {
        HitBoxType.High,
        HitBoxType.Overhead,
        HitBoxType.Anti_Air,
    };
}
[Serializable]
public delegate bool AttackBoolCheck();