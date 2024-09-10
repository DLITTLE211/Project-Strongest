using TMPro;
using UnityEngine;

public class Character_DamageCalculator : MonoBehaviour
{
    [SerializeField] private float curRawDamage,curChipDamage;
    [SerializeField] private float calculatedDamage;
    [SerializeField] private float calculatedRecovDamage;
    [SerializeField] private float calculatedScaling;
    [SerializeField] private float calculatedMeterScaling;

    [SerializeField] private float counterHitMult; 
    [SerializeField] private float afflictionDebuffDamage;
    [SerializeField] private float currentComboHitCount;

    public Character_Health _healtController;
    public Character_ComboCounter _oppCounter;
    public Character_Base _base;

    [SerializeField] private TMP_Text _damageText;
    float damageTextAmount;
    #region Damage Functions
    public void TakeCustomDamage(CustomCallback callback = null)
    {
        _base.opponentPlayer._cComboCounter.OnHit_CountUp();
        curRawDamage = callback.customDamage.rawAttackDamage;
        if (CheckAfflictionState())
        {
            afflictionDebuffDamage = _healtController.currentAffliction.effectNumber;
        }
        if (!CheckCounterHitState())
        {
            counterHitMult = 1;
        }
        float counterHitCalculation = (curRawDamage * counterHitMult) - curRawDamage;
        float counterHitValue = counterHitCalculation <= 0 ? 1 : counterHitCalculation;
        float defenseValue = _healtController.defenseValue / 100;

        calculatedDamage = ((counterHitValue + curRawDamage) + afflictionDebuffDamage) - (calculatedScaling + defenseValue);
        calculatedRecovDamage = calculatedDamage - (calculatedDamage * 0.80f);
        UpdateDamageText(calculatedDamage);
        if (calculatedRecovDamage <= 0)
        {
            calculatedRecovDamage = 0;
        }
        _healtController.ApplyMainHealthDamage(Mathf.Abs(calculatedDamage));
        _healtController.ApplyRecoveryHealthDamage(Mathf.Abs(calculatedRecovDamage));
        _base._cHitController.ForceCustomLockAnim(callback.customDamage, callback.customDamage.isFinalAttack);
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
        if (CheckAfflictionState())
        {
            afflictionDebuffDamage = _healtController.currentAffliction.effectNumber;
        }
        if (!CheckCounterHitState())
        {
            counterHitMult = 1;
        }
        currentComboHitCount = getCurrentComboHitCount();

        float counterHitCalculation = (curRawDamage * counterHitMult) - curRawDamage;
        float counterHitValue = counterHitCalculation == 0 ? 1 : counterHitCalculation;
        float defenseValue = _healtController.defenseValue / 100;

        calculatedDamage = ((counterHitValue + curRawDamage) + afflictionDebuffDamage) - (calculatedScaling + defenseValue);
        calculatedRecovDamage = calculatedDamage - (calculatedDamage * 0.80f);

        if (currentAttack._meterRequirement <= 0)
        {
            calculatedMeterScaling += _oppCounter.CurrentHitCount <= 1 ? 0 : currentAttack._meterAwardedOnHit / (currentComboHitCount * 0.5f);
            float scaledMeterValue = Mathf.Abs((currentAttack._meterAwardedOnHit - calculatedMeterScaling));
            _base.opponentPlayer._cSuperMeter.AddMeter(scaledMeterValue);
        }
        UpdateDamageText(calculatedDamage);
        if (calculatedRecovDamage <= 0)
        {
            calculatedRecovDamage = 0;
        }
        _healtController.ApplyMainHealthDamage(Mathf.Abs(calculatedDamage));
        _healtController.ApplyRecoveryHealthDamage(Mathf.Abs(calculatedRecovDamage));
        if (!armoredAttack)
        {
            _base._cHitController.HandleHitState(currentAttack, currentAttack.attackMainStunValues.hitstunValue, calculatedScaling, false);
        }
        ApplyScalingForNextAttack(currentAttack);
    }
    private void TakeChipDamage(Attack_BaseProperties currentAttack)
    {
        curChipDamage = currentAttack.rawChipDamage;
        if (CheckAfflictionState())
        {
            afflictionDebuffDamage = _healtController.currentAffliction.effectNumber;
        }
        float defenseValue = _healtController.defenseValue / 100;

        calculatedDamage = curChipDamage  - defenseValue;
        calculatedRecovDamage = calculatedDamage - (calculatedDamage * 0.80f);
        UpdateDamageText(calculatedDamage);
        if (calculatedRecovDamage <= 0)
        {
            calculatedRecovDamage = 0;
        }
        _healtController.ApplyMainHealthDamage(Mathf.Abs(calculatedDamage));
        _healtController.ApplyRecoveryHealthDamage(Mathf.Abs(calculatedRecovDamage));
        _base._cHitController.HandleHitState(currentAttack, currentAttack.attackMainStunValues.blockStunValue, calculatedScaling, true);
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
