using TMPro;
using UnityEngine;

public class Character_DamageCalculator : MonoBehaviour
{
    [SerializeField] private float curRawDamage;
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
    public HitPointCall customDamageCall;
    private void Start()
    {
        Messenger.AddListener<CustomCallback>(Events.CustomCallback, ApplyForceOnCustomCallback);
    }
    void ApplyForceOnCustomCallback(CustomCallback callback)
    {
        if (customDamageCall.HasFlag(callback.customCall))
        {
            switch (callback.customCall)
            {
                case HitPointCall.DealCustomDamage:
                    TakeDamage(callback.customDamage);
                    break;
            }
        }
    }

    #region Damage Functions
    public void TakeDamage(CustomDamageField currentAttack)
    {
        _base.opponentPlayer._cComboCounter.OnHit_CountUp();
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

        calculatedDamage = ((counterHitValue + curRawDamage) + afflictionDebuffDamage) - (calculatedScaling);
        calculatedRecovDamage = (calculatedDamage / 2) / currentComboHitCount;

        _healtController.ApplyMainHealthDamage(Mathf.Abs(calculatedDamage));
        UpdateDamageText(calculatedDamage);
        _healtController.ApplyRecoveryHealthDamage(Mathf.Abs(calculatedRecovDamage));
    }
    public void TakeDamage(Attack_BaseProperties currentAttack)
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

        calculatedDamage = ((counterHitValue + curRawDamage) + afflictionDebuffDamage) - (calculatedScaling);
        calculatedRecovDamage = (calculatedDamage / 2) / currentComboHitCount;

        if (currentAttack._meterRequirement <= 0)
        {
            calculatedMeterScaling += _oppCounter.CurrentHitCount <= 1 ? 0 : currentAttack._meterAwardedOnHit / (currentComboHitCount * 0.5f);
            float scaledMeterValue = Mathf.Abs((currentAttack._meterAwardedOnHit - calculatedMeterScaling));
            _base.opponentPlayer._cSuperMeter.AddMeter(scaledMeterValue);
        }

        _healtController.ApplyMainHealthDamage(Mathf.Abs(calculatedDamage));
        UpdateDamageText(calculatedDamage);
        _healtController.ApplyRecoveryHealthDamage(Mathf.Abs(calculatedRecovDamage));
        ApplyScalingForNextAttack(currentAttack);
    }
    public void TakeChipDamage(Attack_BaseProperties _curRawDamage)
    {
        curRawDamage = _curRawDamage.rawAttackDamage;
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

        calculatedScaling += _oppCounter.CurrentHitCount <= 1 ? 0 : (defenseValue + (currentComboHitCount * _curRawDamage.attackScalingPercent / 100));
        calculatedDamage = ((counterHitValue * curRawDamage) + afflictionDebuffDamage) - (calculatedScaling);
        calculatedRecovDamage = (calculatedDamage / 2) / currentComboHitCount;
        float calculatedChipDamage = calculatedDamage / 10f;
        float calculatedChipRecovDamage = 0;
        if (currentComboHitCount == 0)
        {
            calculatedChipRecovDamage = (calculatedChipDamage / 2) / 0.75f;
        }
        else
        {
            calculatedChipRecovDamage = (calculatedChipDamage / 2) / currentComboHitCount;
        }

        if (_curRawDamage._meterRequirement <= 0)
        {
            calculatedMeterScaling += _oppCounter.CurrentHitCount <= 1 ? 0 : _curRawDamage._meterAwardedOnHit / (currentComboHitCount * 0.5f);
            float scaledMeterValue = (Mathf.Abs((_curRawDamage._meterAwardedOnHit - calculatedMeterScaling))) / 100f;

            _base.opponentPlayer._cSuperMeter.AddMeter(scaledMeterValue);
        }

        _healtController.ApplyMainHealthDamage(Mathf.Abs(calculatedChipDamage));
        UpdateDamageText(calculatedChipDamage);

        _healtController.ApplyRecoveryHealthDamage(Mathf.Abs(calculatedChipRecovDamage));
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
                calculatedScaling = currentAttack.attackScalingPercent * 0.005f;
            }
            else
            {
                calculatedScaling = currentAttack.attackScalingPercent * 0.01f;
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
