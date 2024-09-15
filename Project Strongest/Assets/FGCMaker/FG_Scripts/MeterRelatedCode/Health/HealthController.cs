using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HealthController : MonoBehaviour
{
    public MainMeterController health_Main;
    public MainMeterController health_Recov;
    public StunMeterController stunController;
    public float recoverHealthRate;
    public Affliction currentAffliction;
    public float defenseValue;
    public bool canRecover;

    // Start is called before the first frame update
    
    void Start()
    {
        health_Main.SetStartMeterValues(100);
        health_Recov.SetStartMeterValues(100);
        Messenger.AddListener<Affliction>(Events.SendAfflictionToTarget, ApplyAffliction);
    }
    public void ApplyAffliction(Affliction curAffliction) 
    {
        currentAffliction = curAffliction;
    }

    public void ApplyMainHealthDamage(float damageValue) 
    {
        StopCoroutine(RecoverHealthWaitTime());
        canRecover = false;
        DOTween.Kill(health_Main.meterSlider);
        health_Main.currentValue -= damageValue;
        health_Main.SetCurrentMeterValue(health_Main.currentValue);
        stunController.ApplyStun(4 * 0.01f);
        stunController.currentAffliction = currentAffliction;
        StartCoroutine(RecoverHealthWaitTime());
    }
    public void ApplyRecoveryHealthDamage(float damageValue)
    {
        health_Recov.currentValue -= damageValue;
        health_Recov.SetCurrentMeterValue(health_Recov.currentValue);
    }
    IEnumerator RecoverHealthWaitTime() 
    {
        yield return new WaitForSeconds(0.5f);
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
