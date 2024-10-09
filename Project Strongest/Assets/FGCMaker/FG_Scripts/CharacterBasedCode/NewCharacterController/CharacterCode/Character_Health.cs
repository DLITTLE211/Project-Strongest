using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class Character_Health : MonoBehaviour
{
    [SerializeField] private Character_Base _base;
    [SerializeField] private Image _chosenCharacterProfileImage;
    public MainMeterController health_Main;
    public MainMeterController health_Recov;
    public Character_StunController stunController;
    public float recoverHealthRate;
    public Affliction currentAffliction;
    [Range(75f, 200f)] public float defenseValue;
    public bool canRecover;
    private Character_Profile curProfile;
    IEnumerator healthRegenRoutine;
    public void ClearRegenRoutine() 
    {
        if (healthRegenRoutine != null) 
        {
            StopCoroutine(healthRegenRoutine);
            healthRegenRoutine = null;
        }
    }
    public bool TestIfDeadDamage(float damage)
    {
        bool mainHealthIsZero = (health_Main.currentValue - damage) <= 0;
        float tenP = health_Main.maxValue * 0.1f;
        bool recoveryHealthIsTenP = health_Recov.currentValue <= (tenP);
        return mainHealthIsZero && recoveryHealthIsTenP;
    }
    public void ClearHealthValues()
    {
        health_Main.currentValue = 0;
        health_Recov.currentValue = 0;
    }
    public void SetHealthInformation(Character_Profile profile)
    {
        curProfile = profile;
        _chosenCharacterProfileImage.sprite = profile.CharacterProfileImage;
        SetStartingHealthValues();
    }
    public void SetStartingHealthValues() 
    {
        health_Main.SetStartMeterValues(curProfile.MaxHealth);
        health_Recov.SetStartMeterValues(curProfile.MaxHealth);
        recoverHealthRate = curProfile.HealthRegenRate;
    }
    public void ApplyMainHealthDamage(float damageValue)
    {
        canRecover = false;
        DOTween.Kill(health_Main.meterSlider);
        health_Main.currentValue -= damageValue;
        health_Main.SetCurrentMeterValue(health_Main.currentValue);
        stunController.ApplyStun(damageValue * 0.01f);
    }
    public void StartHealthRegen()
    {
        healthRegenRoutine = RecoverHealthWaitTime();
        StartCoroutine(healthRegenRoutine);
    }
    public void ApplyRecoveryHealthDamage(float damageValue)
    {
        health_Recov.currentValue -= damageValue;
        health_Recov.SetCurrentMeterValue(health_Recov.currentValue);
    }
    IEnumerator RecoverHealthWaitTime()
    {
        IState hitState = _base._cStateMachine.hitStateRef;
        while (_base._cStateMachine._playerState.GetCurrentState() == hitState) 
        {
            yield return new WaitForEndOfFrame();
        }
        recoverHealth();
    }
    void CheckMeterValue()
    {
        if (health_Main.currentValue != health_Main.meterSlider.value)
        {
            health_Main.currentValue = health_Main.meterSlider.value;
        }
    }
    public void recoverHealth()
    {
        if (health_Main.currentValue >= health_Recov.currentValue)
        {
            canRecover = false;
            return;
        }
        else
        {
            canRecover = true;
            health_Main.meterSlider.DOValue(health_Recov.currentValue, recoverHealthRate).OnUpdate(CheckMeterValue).OnComplete(() =>
            {
                SetFinalRecovValue();
            });
        }
    }
    public void SetFinalRecovValue()
    {
        health_Main.currentValue = health_Recov.currentValue;
        health_Main.SetCurrentMeterValue(health_Main.currentValue);
        canRecover = false;
    }
}
