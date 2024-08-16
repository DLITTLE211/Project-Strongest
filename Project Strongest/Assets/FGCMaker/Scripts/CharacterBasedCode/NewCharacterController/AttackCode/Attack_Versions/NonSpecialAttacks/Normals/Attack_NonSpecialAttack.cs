using System;
using UnityEngine;

[Serializable]
public class Attack_NonSpecialAttack : Attack_NonSpecial_Base,  IAttackFunctionality 
{
    [SerializeField] private int curInput,curAttack;
    [SerializeField] private int lastDirection;
    public (Attack_BaseInput.MoveInput, Attack_BaseInput.AttackInput) _newinput;
    private Character_Base _curbase;

    AttackData attackData;

    public void SetStarterInformation(Character_Base _base)
    {
        _curbase = _base;
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
            _attackInput._correctInput[i].property.InputTimer = _curbase._cAttackTimer;
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

    /*public void RewardAttack(Character_Base curBase) 
    {
        this._attackInput._correctInput[0].property.InputTimer.ResetTimerSuccess();
        
        PreformAttack(curInput, curAttack, curBase);
        if (curInput > _attackInput._correctInput.Count)
        {
            curInput = _attackInput._correctInput.Count;

        }
        else
        {
            curInput++;
            curAttack++;
        }
    }*/
   
    public int GetFollowUpAttackInt() 
    { 
        return curAttack; 
    }
    public void ResetAttackData()
    {
        ResetCombo();
    }

    public void DoFollowUpAttack(int attack)
    {
        if (curAttack > _attackInput._correctInput.Count)
        {
            curAttack = _attackInput._correctInput.Count;
            Debug.LogError($"Current Attack input exceeds string count. Returning...");
            return;
        }
        if (CheckStateMatchAttackState(curAttack))
        {
            Debug.LogError($"Current Attack airState information does not match playerstate. Returning...");
            return;
        }
        attackData = new AttackData(_curbase, null, null, -1, null, null, _attackInput._correctInput[curAttack]);
        attackData.curBase._aManager.ReceiveAttack(attackData.normalAttack.property);
        curAttack++;
    }
    public void PreformAttack()
    {
        if (CheckStateMatchAttackState(0))
        {
            Debug.LogError($"Current Attack airState information does not match playerstate. Returning...");
            return;
        }
        attackData = new AttackData(_curbase, null, null, -1, null, null, _attackInput._correctInput[0]);
        ResetCombo();
        attackData.curBase._aManager.ReceiveAttack(attackData.normalAttack.property);
        curAttack++;
    }

    bool CheckStateMatchAttackState(int curAttack) 
    {
        if (this._attackInput._correctInput[curAttack].property._airInfo == AirAttackInfo.AirOnly && _curbase._cHurtBox.IsGrounded())
        {
             return false;
        }
        if (this._attackInput._correctInput[curAttack].property._airInfo == AirAttackInfo.GroundOnly && !_curbase._cHurtBox.IsGrounded())
        {
             return false;
        }
        return true;
    }
    public void PreformAttack(int currentInput, int currentAttack,Character_Base curBase)
    {
        //_pathdata.SetPathData(currentInput, currentAttack);
        
        //curBase.comboList3_0.UpdatePathData(_pathdata);
        curBase._aManager.ReceiveAttack(_attackInput._correctInput[currentInput].property);
    }

    public void SendCounterHitInfo(Character_Base curBase)
    {
        throw new NotImplementedException();
    }
    public void SendCounterHitInfo(Attack_BaseProperties attack, Character_Base target)
    {
        target._cDamageCalculator.ReceiveCounterHitMultiplier(attack.counterHitDamageMult);
    }

    public void SendSuccessfulDamageInfo(Character_Base curBase, bool blockedAttack)
    {
        throw new NotImplementedException();
    }
    public void SendSuccessfulDamageInfo(Attack_BaseProperties attack, Character_Base target, bool blockedAttack = false)
    {
        if (!blockedAttack)
        {
            target._cDamageCalculator.TakeDamage(attack);
        }
        else 
        {
            target._cDamageCalculator.TakeChipDamage(attack);
        }
    }


    public MoveType GetAttackMoveType()
    {
        return _attackInput._correctInput[0].property._moveType;
    }
}
