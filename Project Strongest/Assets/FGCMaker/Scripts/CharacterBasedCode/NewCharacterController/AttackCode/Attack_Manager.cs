using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using System;

public class Attack_Manager : MonoBehaviour
{
    public Character_Base _base;
    public Character_Animator _cAnimator => _base._cAnimator;
    public List<HitBox> hitBoxes;
    public List<Attack_BaseProperties> Combo;
    //public Attack_BaseProperties newAttack, lastAttack;
    public int currentCount;
    // Start is called before the first frame update
    void Start()
    {
        SetHitBoxStartState();
        Combo = new List<Attack_BaseProperties>();
    }
    public void ClearAttacks() 
    {
        _cAnimator.inputWindowOpen = true;
        _cAnimator.EndAnim();
        Combo.Clear();
        currentCount = Combo.Count;
        _cAnimator.negativeFrameCount = 0;
    }
    public void ReceiveAttack(Attack_BaseProperties attack)
    {
        GetAttackCriteriaifNotNull(attack);
    }
    public void GetAttackCriteriaifNotNull(Attack_BaseProperties newAttack)
    {
        if (Combo.Count == 0)
        {
            Combo.Add(newAttack);
            ChecFirstAttackCriteria(newAttack);
        }
        else
        {
            CheckNextAttackCriteria(newAttack, Combo.Count -1);
        }
    }
    void ChecFirstAttackCriteria(Attack_BaseProperties newAttack, int index = 0)
    {
        if (!CheckStringPriority(Combo[index].cancelProperty.cancelTo, newAttack))
        {
            Combo.RemoveAt(index);
            return;
        }
        if (!CheckMeterCriteria(newAttack))
        {
            Combo.RemoveAt(index);
            return;
        }
        if (!CheckGroundCriteria(newAttack))
        {
            Combo.RemoveAt(index);
            return;
        }
        DoAttack(newAttack);
    }
    void CheckNextAttackCriteria(Attack_BaseProperties newAttack, int index = 0)
    {
        if (!(CheckCancelCriteria(Combo[index].cancelProperty.cancelTo, newAttack)))
        {
            Combo.RemoveAt(index);
            return;
        }
        if (!CheckMeterCriteria(newAttack))
        {
            Combo.RemoveAt(index);
            return;
        }
        if (!CheckStringPriority(Combo[index].cancelProperty.cancelTo, newAttack))
        {
            Combo.RemoveAt(index);
            return;
        }
        if (!CheckGroundCriteria(newAttack))
        {
            Combo.RemoveAt(index);
            return;
        }
        Combo.Add(newAttack);
        DoAttack(newAttack);
    }
    public bool CheckStringPriority(Cancel_State lastState, Attack_BaseProperties newAttack)
    {
        if (lastState == Cancel_State.String_Normal_Attack && newAttack.cancelProperty.cancelFrom == Cancel_State.String_Normal_Attack)
        {
            return true;
        }
        else 
        {
            if (lastState == Cancel_State.Rekka_Input_FollowUp)
            {
                if (_base.comboList3_0.GetRekkaRouteAttack(Combo[Combo.Count-1]).inRekkaState)
                {
                    return false;
                }
                _base.comboList3_0.GetRekkaRouteAttack(Combo[Combo.Count - 1]).usedRekkas.Add(newAttack);
            }

            return true;
        }
    }
    public bool CheckGroundCriteria(Attack_BaseProperties newAttack) 
    {
        switch (newAttack._airInfo) 
        {
            case AirAttackInfo.GroundOnly:
                if (_base._cHurtBox.IsGrounded())
                {
                    return true;
                }
                else 
                {
                    return false;
                }
            case AirAttackInfo.AirOk:
                return true;
            case AirAttackInfo.AirOnly:
                if (!(_base._cHurtBox.IsGrounded()))
                {
                    return true;
                }
                else 
                {
                    return false;
                }
        }
        return false;
    }
    public bool CheckMeterCriteria(Attack_BaseProperties newAttack)
    {
        if (newAttack._meterRequirement <= 0)
        {
            return true;
        }
        else 
        {
            if (!_base._cSuperMeter.CanTakeFromMeter(newAttack._meterRequirement)) 
            {
                _base._cSuperMeter.TakeMeter(newAttack._meterRequirement);
                return true;
            }
            else 
            {
                return false;
            }
        }
    }
    public bool CheckCancelCriteria(Cancel_State lastState, Attack_BaseProperties newAttack)
    {
        if (lastState == Cancel_State.NotCancellable ^ lastState == Cancel_State.Maximum_Attack)
        {
            string message = lastState == Cancel_State.NotCancellable ? "Last attack was noncacncelablle. ending..." :
                "You've reached the maximum Attack Level. ending...";
            Debug.Log(message);
            return false;
        }
        else if (lastState == Cancel_State.Rekka_Input_FollowUp) 
        {
            if (newAttack.cancelProperty.cancelFrom == Cancel_State.Rekka_Input_Start)
            {
                if (_base.comboList3_0.GetRekkaRouteAttack(newAttack).inRekkaState)
                {
                    if (!_base.comboList3_0.GetRekkaRouteAttack(newAttack).usedRekkas.Contains(newAttack))
                    {

                        Debug.Log("New Attack is within current activate Rekka. Doing action");
                        return true;
                    }
                    
                }
                return false;
            }
            else 
            {
                if (newAttack.cancelProperty.cancelFrom >= lastState)
                {
                    Debug.Log(" new Attack is of higher or equal value. Doing action");
                    return true;
                }
                else
                {
                    Debug.Log(" new Attack is of lower value. Ending action");
                    return false;
                }
            }
        }
        else if (lastState == Cancel_State.Super_Attack)
        {
            if (newAttack == Combo[Combo.Count-1])
            {
                return false;
            }
            return true;
        }
        else
        {
            int newAttackHierarchy = (int)newAttack.cancelProperty.cancelFrom;
            int lastAttackHierachy = (int)lastState;
            if (newAttackHierarchy >= lastAttackHierachy)
            {
                Debug.Log(" new Attack is of higher or equal value. Doing action");
                return true;
            }
            else
            {
                Debug.Log(" new Attack is of lower value. Ending action");
                return false;
            }
        }
    }
    void DoAttack(Attack_BaseProperties _newAttack)
    {
        currentCount = Combo.Count;
        PlayAttack(_newAttack);
    }
    void PlayAttack(Attack_BaseProperties attack)
    {
        if (attack.attackHashes.Count != 0)
        {
            _cAnimator.PlayNextAnimation(attack.attackHashes[0], 2 * (1f / attack.AttackAnims.animClip.frameRate), true);
            _cAnimator.SetNextAttackStartVariables(attack);
        }
    }

    #region HitBox Management Code
    public void SetHitBoxStartState() 
    {
        for (int i = 0; i < hitBoxes.Count; i++) 
        {
            hitBoxes[i].SetHitColliderType(hitBoxes[i], HitBoxType.nullified);
            hitBoxes[i].SetHitboxSize(hitBoxes[i]);
        }
    }
    #endregion
}
