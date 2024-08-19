using System;
using UnityEngine;

[Serializable]
public class Attack_NonSpecialAttack : Attack_NonSpecial_Base,  IAttackFunctionality 
{
    [SerializeField] private int curInput,curAttack;
    [SerializeField] private int lastDirection;
    public (MoveInput, AttackInput) _newinput;
    private Character_Base _curBase;
    public int leewayTime;

    public void SetStarterInformation(Character_Base _base)
    {
        if (_attackInput._correctInput.Count > 1)
        {
            leewayTime = 40;
        }
        _curBase = _base;
        ResetCombo();
        for (int i = 0; i < _attackInput._correctInput.Count; i++)
        {
            _attackInput._correctInput[i].SetAttackInfo(_attackInput._correctInput[i]._correctSequence, _attackInput._correctInput[i].property._attackName);
        }
        SetComboTimer();
    }
    public void SetComboTimer()
    {
        for (int i = 0; i < _attackInput._correctInput.Count; i++)
        {
            _attackInput._correctInput[i].property.InputTimer = _curBase._cAttackTimer;
        }
    }
    #region Attack Base Code

    public override void ResetCombo()
    {
        curAttack = 0;
        for (int i = 0; i < _attackInput._correctInput.Count; i++)
        {
            _attackInput._correctInput[i].property.hitConnected = false;
        }
    }
    
    #endregion

    public int GetFollowUpAttackInt() 
    { 
        return curAttack; 
    }
    public void ResetAttackData()
    {
        ResetCombo();
    }

    public void DoFollowUpAttack(int attack, Callback SendAttackOnSucess)
    {
        if (curAttack > _attackInput._correctInput.Count)
        {
            curAttack = _attackInput._correctInput.Count;
            Debug.LogError($"Current Attack input exceeds string count. Returning...");
            return;
        }
        if (!_attackInput._correctInput[curAttack-1].property.hitConnected)
        {
            Debug.LogError($"Previous Attack in string did not connect. Returning...");
            return;
        }
        Attack_BaseProperties newNormalAttack = _attackInput._correctInput[curAttack].property;
        newNormalAttack.InputTimer.SetTimerType(TimerType.Normal, (leewayTime * (1 / 60f)));
        _curBase._aManager.ReceiveAttack(newNormalAttack, () => StringNormalFollowUpFunctions(newNormalAttack, SendAttackOnSucess));
        curAttack++;
    }
    public void StringNormalFollowUpFunctions(Attack_BaseProperties newProperty, Callback SendAttackOnSucess)
    {
        SendAttackOnSucess();
        _curBase.comboList3_0.SetFollowAttack(newProperty);
    }
    public void PreformAttack(Callback SendAttackOnSucess)
    {

        Attack_BaseProperties newNormalAttack = _attackInput._correctInput[0].property;
        ResetCombo();
        if (_attackInput._correctInput.Count > 1)
        {
            newNormalAttack.InputTimer.SetTimerType(TimerType.Normal, (leewayTime * (1/60f)));
        }
        _curBase._aManager.ReceiveAttack(newNormalAttack, SendAttackOnSucess);
        curAttack++;
    }

    public void SendCounterHitInfo(Character_Base curBase)
    {
        throw new NotImplementedException();
    }
    public void SendCounterHitInfo(Character_Base target, Attack_BaseProperties main)
    {
        target._cDamageCalculator.ReceiveCounterHitMultiplier(main.counterHitDamageMult);
    }

    public void SendSuccessfulDamageInfo(Character_Base attacker, Character_Base target, bool blockedAttack, Attack_BaseProperties main, Attack_BaseProperties followUp = null)
    {
        if (followUp != null)
        {
            if (!blockedAttack)
            {
                SendCounterHitInfo(target, followUp);
                target._cDamageCalculator.TakeDamage(followUp);
            }
            else
            {
                target._cDamageCalculator.TakeChipDamage(followUp);
            }
        }
        else
        {
            if (!blockedAttack)
            {
                SendCounterHitInfo(target);
                target._cDamageCalculator.TakeDamage(main);
            }
            else
            {
                target._cDamageCalculator.TakeChipDamage(main);
            }
        }
    }
    public MoveType GetAttackMoveType()
    {
        return _attackInput._correctInput[0].property._moveType;
    }

    public void HandleDamageDealing(Character_Base attacker, Character_Base target, bool blockedAttack, Attack_BaseProperties main, Attack_BaseProperties followUp = null)
    {
        SendSuccessfulDamageInfo(attacker, target, blockedAttack, main, followUp);
    }
}
