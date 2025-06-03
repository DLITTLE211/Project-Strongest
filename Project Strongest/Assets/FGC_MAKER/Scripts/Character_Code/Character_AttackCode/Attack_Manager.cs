using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using System;

public class Attack_Manager : MonoBehaviour
{
    public Character_Base _base;
    [SerializeField] private Character_Animator _cAnimator;
    public Character_Animator C_Animator { set { _cAnimator = value; } }
    public List<Attack_BaseProperties> Combo;
    public int currentCount;
    private bool CanTransitionAnimation;

    Queue<Attack_BaseProperties> _AttackAnimQueue;
    private List<Cancel_State> stringCancelStates;
    [SerializeField] private MoveType curTypeHierarchy;
    public MoveType MoveTypeHierarchy { get { return curTypeHierarchy; } }
    IEnumerator RechargeRoutine;
    void Start()
    {
        Combo = new List<Attack_BaseProperties>();
        _AttackAnimQueue = new Queue<Attack_BaseProperties>();  
        CanTransitionAnimation = true;
        SetStringCancelCheck();
    }
    void SetStringCancelCheck() 
    {
        stringCancelStates = new List<Cancel_State>();
        stringCancelStates.Add(Cancel_State.Light_Normal_Attack);
        stringCancelStates.Add(Cancel_State.Light_String_Normal_Start);
        stringCancelStates.Add(Cancel_State.Light_String_Normal_FollowUp);
        stringCancelStates.Add(Cancel_State.Heavy_Normal_Attack);
        stringCancelStates.Add(Cancel_State.Heavy_String_Normal_Start);
        stringCancelStates.Add(Cancel_State.Heavy_String_Normal_FollowUp);
    }
    public Attack_CancelInfo GetLastCancelInfo() 
    {
        if (Combo.Count > 0)
        {
            return Combo[Combo.Count - 1].cancelProperty;
        }
        return null;
    }
    public void ClearAttacks()
    {
        //if (_base._cAttackTimer._type == TimerType.Normal)
        //{
            _AttackAnimQueue.Clear();
            _cAnimator.inputWindowOpen = true;
            Combo.Clear();
            currentCount = Combo.Count;
            _cAnimator.inStanceState = false;
            _cAnimator.inRekkaState = false;
            if (_base.comboList3_0 != null)
            {
                IState currentState = _base._cStateMachine._playerState.GetCurrentState();
                IState attackingState = _base._cStateMachine.attackingStateRef;
                _base.comboList3_0.ClearAttackData(currentState, attackingState);
            }
        //}
    }

    public void ResetMoveHierarchy()
    {
        curTypeHierarchy = MoveType.Normal;
    }
    public void ReceiveAttack(Attack_BaseProperties attack, Callback SetAttackOnSuccess, AfflictionSet afflictionSet = null)
    {
        GetAttackCriteriaifNotNull(attack, SetAttackOnSuccess, afflictionSet); 
    }
    public void GetAttackCriteriaifNotNull(Attack_BaseProperties newAttack, Callback SetAttackOnSuccess,AfflictionSet afflictionSet = null)
    {
        if (Combo.Count == 0 && currentCount == 0)
        {
            Combo.Add(newAttack);
            ChecFirstAttackCriteria(newAttack,true, SetAttackOnSuccess, afflictionSet);
        }
        else
        {
            Combo.Add(newAttack);
            CheckNextAttackCriteria(newAttack, false, Combo.Count-1, SetAttackOnSuccess, afflictionSet);
        }
    }
    void ChecFirstAttackCriteria(Attack_BaseProperties newAttack, bool isFirstAttack, Callback SetAttackOnSuccess, AfflictionSet afflictionSet = null)
    {
        if (!CheckStringPriority(Combo[0].cancelProperty, newAttack, newAttack.cancelProperty, isFirstAttack))
        {
            RemoveAtIndex(0);
            return;
        }
        if (!CheckAttackRequiresNoInstall(newAttack))
        {
            RemoveAtIndex(0);
            return;
        }
        if (!CheckAttackRequiresInstall(newAttack))
        {
            RemoveAtIndex(0);
            return;
        }
        if (!CheckMeterCriteria(newAttack))
        {
            RemoveAtIndex(0);
            return;
        }
        if (!CheckGroundCriteria(newAttack))
        {
            RemoveAtIndex(0);
            return;
        }
        SetAttackOnSuccess();
        DoAttack(newAttack, afflictionSet);
    }
    void CheckNextAttackCriteria(Attack_BaseProperties newAttack, bool isFirstAttack, int index, Callback SetAttackOnSuccess, AfflictionSet afflictionSet = null)
    {
        int newAttackHierarchy = (int)newAttack._moveType;
        int lastAttackHierachy = (int)curTypeHierarchy;
        if (newAttackHierarchy < lastAttackHierachy)
        {
            RemoveAtIndex(index);
            return;
        }
        if (!CheckMoveType(newAttack, false, index))
        {
            RemoveAtIndex(index);
            return;
        }
        if (!CheckAttackRequiresInstall(newAttack))
        {
            RemoveAtIndex(index);
            return;
        }
        if (!CheckAttackRequiresNoInstall(newAttack))
        {
            RemoveAtIndex(0);
            return;
        }
        if (!CheckMeterCriteria(newAttack))
        {
            RemoveAtIndex(index);
            return;
        }
        if (!CheckGroundCriteria(newAttack))
        {
            RemoveAtIndex(index);
            return;
        }
        if (newAttack._moveType != MoveType.Rekka && _cAnimator.inRekkaState)
        {
            _cAnimator.SetRekkaBool(false);
        }
        if (newAttack._moveType != MoveType.Stance && _cAnimator.inStanceState)
        {
            _cAnimator.SetStanceBool(false);
        }
        SetAttackOnSuccess();
        DoAttack(newAttack, afflictionSet);
    }
    bool CheckMoveType(Attack_BaseProperties newAttack, bool isFirstAttack, int index = 0)
    {
        Attack_BaseProperties lastBase = Combo[index - 1];
        switch (newAttack._moveType) 
        {
            case MoveType.Normal:
                if (!CheckStringPriority(lastBase.cancelProperty, newAttack, newAttack.cancelProperty, isFirstAttack))
                {
                    if (!(CheckCancelCriteria(lastBase.cancelProperty, newAttack, newAttack.cancelProperty)))
                    {
                        return false;
                    }
                }
                break;
            case MoveType.String_Normal:
                if (!CheckStringPriority(lastBase.cancelProperty, newAttack, newAttack.cancelProperty, isFirstAttack))
                {
                    if (!(CheckCancelCriteria(lastBase.cancelProperty, newAttack, newAttack.cancelProperty)))
                    {
                        return false;
                    }
                }
                if (_base._cComboDetection.ReturnActiveFollowUp() != null)
                {
                    if (!_base._cComboDetection.ReturnActiveFollowUp().CheckAttackContains(newAttack))
                    {
                        return false;
                    }
                }
                break;
            case MoveType.Command_Normal:
                if (!CheckStringPriority(lastBase.cancelProperty, newAttack, newAttack.cancelProperty, isFirstAttack))
                {
                    if (!(CheckCancelCriteria(lastBase.cancelProperty, newAttack, newAttack.cancelProperty)))
                    {
                        return false;
                    }
                }
                break;
            default:
                if (!(CheckCancelCriteria(lastBase.cancelProperty, newAttack, newAttack.cancelProperty)))
                {
                    return false;
                }
                break;
        }
        return true;
    }
    public bool CheckStringPriority(Attack_CancelInfo lastState, Attack_BaseProperties newAttack, Attack_CancelInfo newAttackCancelInfo, bool firstAttack)
    {
        if (firstAttack)
        {
            return true;
        }
        else
        {
            if (lastState.nextAvailableAttackRoute.HasFlag(newAttack.cancelProperty.CurrentLevel))
            {
                return true;
            }
            return false;
        }
    }
    public bool CheckAttackRequiresInstall(Attack_BaseProperties newAttack)
    {
        if (!newAttack.isInInstall)
        {
            return true;
        }
        else
        {
            if (_base._cSubStateController.VerifyInstallState())
            {
                return true;
            }
            return false;
        }
    }
    public bool CheckAttackRequiresNoInstall(Attack_BaseProperties newAttack) 
    {
        if (!newAttack.isNotInInstall) 
        {
            return true;
        }
        else
        {
            if (!_base._cSubStateController.VerifyInstallState()) 
            {
                return true;
            }
            return false;
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
            if (_base._cSuperMeter.CanTakeFromMeter(newAttack._meterRequirement)) 
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
    public bool CheckCancelCriteria(Attack_CancelInfo lastState, Attack_BaseProperties newAttack, Attack_CancelInfo newAttackCancelInfo)
    {
        if (lastState.nextAvailableAttackRoute.HasFlag(Cancel_State.NotCancellable) || lastState.CurrentLevel == Cancel_State.Maximum_Attack)
        {
            string message = "Last attack was NONCANCELLABLE from previous attack. ending...";
            Debug.LogWarning(message);
            return false;
        }
        else
        {
            if (lastState.CurrentLevel == Cancel_State.Rekka_Input_FollowUp && newAttack.cancelProperty.CurrentLevel == Cancel_State.Rekka_Input_Start)
            {
                Attack_RekkaSpecialMove curRekka = _base.comboList3_0.GetRekkaRouteAttack(newAttack);
                if (curRekka != null)
                {
                    if (curRekka.inRekkaState)
                    {
                        if (!curRekka.usedRekkas.Contains(newAttack))
                        {
                            Debug.Log("New Attack is within current activate Rekka. Doing action");
                            return true;
                        }
                    }
                }
                return false;
            }
            if (lastState.CurrentLevel == Cancel_State.Super_Attack)
            {
                int lastInputtedAttackIndex = Combo.Count - 2;
                if(lastInputtedAttackIndex <= 0) 
                {
                    return true;
                }
                if (newAttack == Combo[lastInputtedAttackIndex])
                {
                    return false;
                }
                return true;
            }
            else
            {
                if (lastState.nextAvailableAttackRoute.HasFlag(newAttack.cancelProperty.CurrentLevel))
                {
                    Debug.Log(" New Attack is possible in this routing. Doing action");
                    return true;
                }
                else 
                {
                    _base._cAttackTimer.ResetTimer();
                    Debug.Log(" New Attack is not possible in this routing. Ending action");
                    return false;
                }
            }
        }
    }
    public void SetStartNextAttack(bool state) 
    {
        CanTransitionAnimation = state;
    }

    public void OVERRIDE_DOATTACK(Attack_BaseProperties _base, AfflictionSet afflictionSet) 
    {
        DoAttack(_base, afflictionSet);
    }
    void DoAttack(Attack_BaseProperties _newAttack, AfflictionSet afflictionSet)
    {
        currentCount = Combo.Count;
        curTypeHierarchy = _newAttack._moveType;
        if (CanTransitionAnimation)
        {
            PlayAttack(_newAttack, afflictionSet);
        }
        else 
        {
           StartCoroutine(AwaitClear(_newAttack, afflictionSet));
        }
    }
    IEnumerator AwaitClear(Attack_BaseProperties _newAttack, AfflictionSet afflictionSet)
    {
        _AttackAnimQueue.Enqueue(_newAttack);
        while (_AttackAnimQueue.Count > 0)
        {
            yield return new WaitForSeconds(2 * (1f / 60f));
            if (_AttackAnimQueue.Count > 0)
            {
                Debug.LogError("Queued Attack successfully Released");
                PlayAttack(_AttackAnimQueue.Dequeue(), afflictionSet);
            }
        }
    }
    void PlayAttack(Attack_BaseProperties attack, AfflictionSet afflictionSet)
    {
        if (attack.hitblocked) 
        {
            attack.hitblocked = false;
        }
        _cAnimator.SetNextAttackStartVariables(attack, afflictionSet);
    }
    public void RemoveAtIndex(int index) 
    {
        try 
        {
            Combo.RemoveAt(index);
        }
        catch(ArgumentOutOfRangeException) 
        {
            ClearAttacks();
        }
    }
}
