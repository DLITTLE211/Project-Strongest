using TMPro;
using UnityEngine;
using System;
using System.Collections.Generic;

public class Character_DamageCalculator : MonoBehaviour
{
    [SerializeField] private float curRawDamage,curChipDamage;
    [SerializeField] private float calculatedDamage;
    [SerializeField] private float calculatedRecovDamage;
    [SerializeField] private float calculatedScaling;
    [SerializeField] private float calculatedMeterScaling;

    [SerializeField] private float counterHitMult; 
    [SerializeField] private float afflictionDebuffDamage;

    [SerializeField] private float buffDamageMultiplier;
    [SerializeField] private float chipBuffDamageMultiplier;
    [SerializeField] private float buffDefensiveMultiplier;

    public Character_Health _healtController;
    public Character_ComboCounter _oppCounter;
    public Character_Base _base;

    [SerializeField] private TMP_Text _damageText;
    float damageTextAmount;
    bool victoryHit;
    bool allowDeathCheck;
    public bool isDead;
    #region Damage Functions
    private void Start()
    {
        victoryHit = false;
   }
    public void SetAllowDeathCheck() 
    {
        allowDeathCheck = GameManager.instance._gameModeSet.gameMode != GameMode.Training;
    }
    public void ResetBuffs() 
    {
        SetBuffMultiplier();
        SetChipBuffMultiplier();
    }
    public void SetBuffMultiplier(float value = 0f) 
    {
        buffDamageMultiplier = value;
    }
    public void SetChipBuffMultiplier(float value = 0f)
    {
        chipBuffDamageMultiplier = value;
    }
    public void SetDefensiveMultiplier(float value = 0f)
    {
        buffDefensiveMultiplier = value;
    }
    public void TakeCustomDamage(CustomCallback callback = null)
    {
        if (callback.customDamage.rawAttackDamage <= 0)
        {
            return;
        }
        _base.opponentPlayer._cComboCounter.OnHit_CountUp();
        curRawDamage = callback.customDamage.rawAttackDamage;
        if (buffDamageMultiplier > 0) 
        {
            curRawDamage = curRawDamage + (curRawDamage * buffDamageMultiplier);
        }
        if (!CheckCounterHitState())
        {
            counterHitMult = 1;
        }
        float counterHitCalculation = (curRawDamage * counterHitMult) - curRawDamage;
        float counterHitValue = counterHitCalculation <= 0 ? 1 : counterHitCalculation;
        float defenseValue = (buffDefensiveMultiplier > 0 ? (_healtController.defenseValue + buffDefensiveMultiplier) / 100 : _healtController.defenseValue / 100);
        _base._cForce.SendKnockBackOnHit(callback.customDamage);
        calculatedDamage = ((counterHitValue + curRawDamage) + afflictionDebuffDamage) - (calculatedScaling + defenseValue);
        if (callback.customDamage.isScaling)
        {
            calculatedRecovDamage = calculatedDamage - (calculatedDamage * 0.35f);
        }
        else 
        {
            calculatedRecovDamage = calculatedDamage;
        }
        UpdateDamageText(calculatedDamage);
        if (calculatedRecovDamage <= 0)
        {
            calculatedRecovDamage = 0;
        }
        bool isDeadOnHit = _healtController.TestIfDeadDamage(calculatedDamage);
        if (isDeadOnHit && allowDeathCheck && !victoryHit && callback.customDamage.isFinalAttack)
        {

            isDead = isDeadOnHit;
            DeathCheck();
            _base._cHitController.DeathHitDetect(null,callback.customDamage);
        }
        else
        {
            _healtController.ApplyMainHealthDamage(Mathf.Abs(calculatedDamage));
            _healtController.ApplyRecoveryHealthDamage(Mathf.Abs(calculatedRecovDamage));
            _base._cHitController.ForceCustomLockAnim(callback.customDamage, callback.customDamage.isFinalAttack);
        }
    }
    public void SetVictoryHitState(bool state) 
    {
        victoryHit = state;
    }
    void DeathCheck() 
    {
        if (GameManager.instance._gameModeSet.gameMode != GameMode.Training)
        {
            SetVictoryHitState(true);
            _healtController.ClearHealthValues();
            GameManager.instance.CallPlayerDeath(_base.opponentPlayer);
        }
    }
    public void ReceiveDamage(Attack_BaseProperties currentAttack, bool blocked, bool armoredAttack = false)
    {
        if (blocked)
        {
            TakeChipDamage(currentAttack);
        }
        else
        {
            TakeDamage(currentAttack,armoredAttack);
        }
    }
    private void TakeDamage(Attack_BaseProperties currentAttack,bool armoredAttack)
    {
        curRawDamage = currentAttack.rawAttackDamage;
        if (buffDamageMultiplier > 0)
        {
            curRawDamage = curRawDamage + (curRawDamage * buffDamageMultiplier);
        }
        if (!CheckCounterHitState())
        {
            counterHitMult = 1;
        }

        float counterHitCalculation = (curRawDamage * counterHitMult) - curRawDamage;
        float counterHitValue = counterHitCalculation == 0 ? 1 : counterHitCalculation;
        float defenseValue = (buffDefensiveMultiplier > 0 ? (_healtController.defenseValue + buffDefensiveMultiplier) / 100 : _healtController.defenseValue / 100);

        calculatedDamage = (counterHitValue + curRawDamage) - (calculatedScaling + defenseValue);
        calculatedRecovDamage = calculatedDamage - (calculatedDamage * 0.575f);

        _base.opponentPlayer._amplifyController.CheckAmplifyActiveCall(currentAttack,false);
        if (currentAttack._meterRequirement <= 0)
        {
            calculatedMeterScaling += _oppCounter.CurrentHitCount <= 1 ? 0 : ((currentAttack._meterAwardedOnHit * 0.05f)* calculatedScaling);
            float scaledMeterValue = currentAttack._meterAwardedOnHit - calculatedMeterScaling;
            if (scaledMeterValue > 0)
            {
                _base.opponentPlayer._cSuperMeter.AddMeter(scaledMeterValue);
            }
        }
        UpdateDamageText(calculatedDamage);
        if (calculatedRecovDamage <= 0)
        {
            calculatedRecovDamage = 0;
        }
        bool isDeadOnHit = _healtController.TestIfDeadDamage(calculatedDamage);
        if (isDeadOnHit && allowDeathCheck && !victoryHit )
        {
            isDead = isDeadOnHit;
            DeathCheck();
            _base._cHitController.DeathHitDetect(currentAttack, null);
        }
        else
        {
            _healtController.ApplyMainHealthDamage(Mathf.Abs(calculatedDamage));
            _healtController.ApplyRecoveryHealthDamage(Mathf.Abs(calculatedRecovDamage));
            if (!armoredAttack)
            {
                _base._cHitController.HandleHitState(currentAttack, currentAttack.attackMainStunValues.hitstopValue, currentAttack.attackMainStunValues.hitstunValue, calculatedScaling, false);
            }
            ApplyScalingForNextAttack(currentAttack);
        }
    }
    private void TakeChipDamage(Attack_BaseProperties currentAttack)
    {
        curChipDamage = currentAttack.rawChipDamage;
        if (buffDamageMultiplier > 0)
        {
            curChipDamage = curChipDamage + (curChipDamage * buffDamageMultiplier);
        }
        else if(chipBuffDamageMultiplier > 0)
        {
            curChipDamage = curChipDamage + (curChipDamage * chipBuffDamageMultiplier);
        }
        float defenseValue = (buffDefensiveMultiplier > 0 ? (_healtController.defenseValue + buffDefensiveMultiplier) / 100 : _healtController.defenseValue / 100);

        calculatedDamage = curChipDamage  - defenseValue;
        calculatedRecovDamage = calculatedDamage - (calculatedDamage * 0.80f);
        UpdateDamageText(calculatedDamage);
        _base.opponentPlayer._amplifyController.CheckAmplifyActiveCall(currentAttack, true);
        if (calculatedRecovDamage <= 0)
        {
            calculatedRecovDamage = 0;
        }
        bool isDeadOnHit = _healtController.TestIfDeadDamage(calculatedDamage);
        if (isDeadOnHit && allowDeathCheck && !victoryHit)
        {
            isDead = isDeadOnHit;
            DeathCheck();
            _base._cHitController.DeathHitDetect(currentAttack, null);
        }
        else
        {
            _healtController.ApplyMainHealthDamage(Mathf.Abs(calculatedDamage));
            _healtController.ApplyRecoveryHealthDamage(Mathf.Abs(calculatedRecovDamage));
            _base._cHitController.HandleHitState(currentAttack, currentAttack.attackMainStunValues.blockStopValue, currentAttack.attackMainStunValues.blockStunValue, calculatedScaling, true);
        }
    }
    #endregion

    void ApplyScalingForNextAttack(Attack_BaseProperties currentAttack)
    {
        if (_oppCounter.CurrentHitCount <= 1)
        {
            calculatedScaling = 0;
        }
        else
        {
            if (_oppCounter.CurrentHitCount > 7)
            {
                calculatedScaling += currentAttack.attackScalingPercent * 0.005f;
            }
            else
            {
                calculatedScaling += currentAttack.attackScalingPercent * 0.01f;
            }
        }
    }
    public void ReceiveCustomAfflictionDamage(float damageAmount) 
    {
        _healtController.ApplyMainHealthDamage(Mathf.Abs(damageAmount));
        _healtController.ApplyRecoveryHealthDamage(Mathf.Abs(damageAmount));
    }
    public float GetCurrentScaling() 
    {
        return calculatedScaling * 1.75f;
    }
    public void ResetScaling() 
    {
        calculatedScaling = 0;
        calculatedMeterScaling = 0;
    }
    public void ReceiveCounterHitMultiplier(float cHMultiplier)
    {
        counterHitMult = cHMultiplier;
    }
    bool CheckAfflictionState()
    {
        if (_healtController.currentAffliction != null)
        {
            return true;
        }
        return false;
    }
    bool CheckCounterHitState()
    {
        if (_oppCounter.CurrentHitCount == 1 && _base._cStateMachine._playerState.CurrentStateString == "State_Attacking")
        {
            return true;
        }
        return false;
    }
    float getCurrentComboHitCount()
    {
        return _oppCounter.CurrentHitCount;
    }
    public void ClearDamageText() 
    {
        damageTextAmount = 0;
        UpdateDamageText();
    }
    void UpdateDamageText(float damageAmount = 0) 
    {
        damageTextAmount += damageAmount;
        if (damageTextAmount <= 0)
        {
            _damageText.text = "";
            return;
        }
        _damageText.text = $"Current Damage: ({damageTextAmount})";
    }
}
