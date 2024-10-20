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
    //[SerializeField] internal int normalGatlingCount;
    public MoveType MoveTypeHierarchy { get { return curTypeHierarchy; } }
    IEnumerator RechargeRoutine;
    void Start()
    {
        Combo = new List<Attack_BaseProperties>();
        _AttackAnimQueue = new Queue<Attack_BaseProperties>();  
        CanTransitionAnimation = true;
        SetStringCancelCheck();
     //   normalGatlingCount = 3;
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
    public void ClearAttacks()
    {
        if (_base._cAttackTimer._type == TimerType.Normal)
        {
            _AttackAnimQueue.Clear();
            _cAnimator.inputWindowOpen = true;
            _cAnimator.EndAnim();
            Combo.Clear();
            currentCount = Combo.Count;
            _cAnimator.negativeFrameCount = 0;
            _cAnimator.inStanceState = false;
            _cAnimator.inRekkaState = false;
            if (_base.comboList3_0 != null)
            {
                IState currentState = _base._cStateMachine._playerState.GetCurrentState();
                IState attackingState = _base._cStateMachine.attackingStateRef;
                _base.comboList3_0.ClearAttackData(currentState, attackingState);
            }
        }
    }

    public void ResetMoveHierarchy()
    {
        curTypeHierarchy = MoveType.Normal;
  //      normalGatlingCount = 3;
    }
    public void ReceiveAttack(Attack_BaseProperties attack, Callback SetAttackOnSuccess)
    {
        GetAttackCriteriaifNotNull(attack, SetAttackOnSuccess); 
    }
    public void GetAttackCriteriaifNotNull(Attack_BaseProperties newAttack, Callback SetAttackOnSuccess)
    {
        if (Combo.Count == 0 && currentCount == 0)
        {
            Combo.Add(newAttack);
            ChecFirstAttackCriteria(newAttack,true, SetAttackOnSuccess);
        }
        else
        {
            Combo.Add(newAttack);
            CheckNextAttackCriteria(newAttack, false, Combo.Count-1, SetAttackOnSuccess);
        }
    }
    void ChecFirstAttackCriteria(Attack_BaseProperties newAttack, bool isFirstAttack, Callback SetAttackOnSuccess)
    {
        if (!CheckStringPriority(Combo[0].cancelProperty, newAttack, newAttack.cancelProperty, isFirstAttack))
        {
            Combo.RemoveAt(0);
            return;
        }
        if (!CheckMeterCriteria(newAttack))
        {
            Combo.RemoveAt(0);
            return;
        }
        if (!CheckGroundCriteria(newAttack))
        {
            Combo.RemoveAt(0);
            return;
        }
        SetAttackOnSuccess();
        DoAttack(newAttack);
    }
    void CheckNextAttackCriteria(Attack_BaseProperties newAttack, bool isFirstAttack, int index, Callback SetAttackOnSuccess)
    {
        int newAttackHierarchy = (int)newAttack._moveType;
        int lastAttackHierachy = (int)curTypeHierarchy;
        if (newAttackHierarchy < lastAttackHierachy) 
        {
            Combo.RemoveAt(index);
            return;
        }
        if (!CheckMoveType(newAttack, false, index)) 
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
        if (newAttack._moveType != MoveType.Rekka && _cAnimator.inRekkaState)
        {
            _cAnimator.SetRekkaBool(false);
        }
        if (newAttack._moveType != MoveType.Stance && _cAnimator.inStanceState)
        {
            _cAnimator.SetStanceBool(false);
        }
        SetAttackOnSuccess();
        DoAttack(newAttack);
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
            if (StateComparison(lastState, newAttackCancelInfo, Cancel_State.Light_String_Normal_Start, Cancel_State.Light_String_Normal_FollowUp))
            {
                return true;
            }
            if (StateComparison(lastState, newAttackCancelInfo, Cancel_State.Heavy_String_Normal_Start, Cancel_State.Heavy_String_Normal_FollowUp))
            {
                return true;
            }
            else
            {
                if (newAttackCancelInfo.cancelFrom < _cAnimator.currentAttackLevel)
                {
                    return false;
                }
                else
                {
                    if (lastState.cancelTo == Cancel_State.Light_Normal_Attack)
                    {
                        List<Cancel_State> availableCancelStates = new List<Cancel_State>();
                        availableCancelStates.Add(Cancel_State.Light_Normal_Attack);
                        availableCancelStates.Add(Cancel_State.Heavy_Normal_Attack);
                        availableCancelStates.Add(Cancel_State.Command_Normal_Attack);
                        if (availableCancelStates.Contains(newAttack.cancelProperty.cancelFrom))
                        {
                            return true;
                        }
                    }
                    if (lastState.cancelTo == Cancel_State.Heavy_Normal_Attack)
                    {
                        if (newAttack.cancelProperty.cancelFrom == Cancel_State.Command_Normal_Attack)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
        }
    }
    bool StateComparison(Attack_CancelInfo lastState, Attack_CancelInfo newState, Cancel_State desiredLastState, Cancel_State desiredNextState) 
    {
        bool FirstCheck = (lastState.cancelTo == desiredNextState || lastState.CurrentLevel == desiredNextState);
        bool SecondCheck = (newState.cancelFrom == desiredLastState || newState.cancelFrom == desiredNextState);
        return FirstCheck && SecondCheck;
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
        if (lastState.cancelTo == Cancel_State.NotCancellable ^ lastState.cancelTo == Cancel_State.Maximum_Attack)
        {
            string message = lastState.cancelTo == Cancel_State.NotCancellable ? "Last attack was noncacncelablle. ending..." : "You've reached the maximum Attack Level. ending...";
            Debug.Log(message);
            return false;
        }
        else
        {
            if (lastState.cancelTo == Cancel_State.Rekka_Input_FollowUp && newAttack.cancelProperty.cancelFrom == Cancel_State.Rekka_Input_Start)
            {
                Attack_RekkaSpecialMove curRekka = _base.comboList3_0.GetRekkaRouteAttack(newAttack);
                if (curRekka.inRekkaState)
                {
                    if (!curRekka.usedRekkas.Contains(newAttack))
                    {
                        Debug.Log("New Attack is within current activate Rekka. Doing action");
                        return true;
                    }
                }
                return false;
            }
            if (lastState.cancelTo == Cancel_State.Super_Attack)
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
                int newAttackHierarchy = (int)newAttack.cancelProperty.CurrentLevel;
                int lastAttackHierachy = (int)lastState.cancelTo;
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
    }
    public void SetStartNextAttack(bool state) 
    {
        CanTransitionAnimation = state;
    }
    void DoAttack(Attack_BaseProperties _newAttack)
    {
        currentCount = Combo.Count;
        curTypeHierarchy = _newAttack._moveType;
        if (CanTransitionAnimation)
        {
            PlayAttack(_newAttack);
        }
        else 
        {
           StartCoroutine(AwaitClear(_newAttack));
        }
    }
    IEnumerator AwaitClear(Attack_BaseProperties _newAttack)
    {
        _AttackAnimQueue.Enqueue(_newAttack);
        while (_AttackAnimQueue.Count > 0)
        {
            yield return new WaitForSeconds(2 * (1f / 60f));
            if (_AttackAnimQueue.Count > 0)
            {
                Debug.LogError("Queued Attack successfully Released");
                PlayAttack(_AttackAnimQueue.Dequeue());
            }
        }
    }
    void PlayAttack(Attack_BaseProperties attack)
    {
        if (attack._moveType == MoveType.Normal)
        {
     //       normalGatlingCount--;
        }
        _cAnimator.SetNextAttackStartVariables(attack);
   //     ClearRoutine();
    //    RechargeRoutine = RechargeGatlings();
   //     StartCoroutine(RechargeRoutine);
    }
    void ClearRoutine() 
    {
        if(RechargeRoutine!= null) 
        {
            StopCoroutine(RechargeRoutine);
            RechargeRoutine = null;
        }
    }
    IEnumerator RechargeGatlings() 
    {
        yield return new WaitForSeconds(2f);
 //       normalGatlingCount = 3;
    }
}
