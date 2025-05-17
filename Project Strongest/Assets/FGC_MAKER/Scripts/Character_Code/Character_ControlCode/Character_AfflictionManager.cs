using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using FightingGame_FrameData;

public class Character_AfflictionManager : MonoBehaviour
{
    [SerializeField] private Character_Base _base;
    [SerializeField] private Dictionary<StatusEffect.Effect_Affliction,Affliction_Object> _totalAfflictions;
    [SerializeField] private List<Affliction_Object> TotalAfflictions;
    [SerializeField] private List<Affliction_Object> appliedAffliction;
    IEnumerator applicationWindow;
    public void ClearAppliedAffliction() 
    {
        if(appliedAffliction != null) 
        {
            appliedAffliction = null;
        }
    }
    public void Start()
    {
        _totalAfflictions = new Dictionary<StatusEffect.Effect_Affliction, Affliction_Object>();
        appliedAffliction = new List<Affliction_Object>();
        for (int i = 0; i < TotalAfflictions.Count; i++)
        {
            TotalAfflictions[i].SetAffliction(_base);
            TotalAfflictions[i]._afflictionObject.SetActive(false);
        }
        for (int i = 0; i < TotalAfflictions.Count; i++)
        {
            StatusEffect.Effect_Affliction currentAffliction = TotalAfflictions[i]._afflictionType;
            Affliction_Object currentObject = TotalAfflictions[i];
            _totalAfflictions.Add(currentAffliction, currentObject);
        }
        appliedAffliction.Clear();
    }
    #region Affliction Applier Code
    public void OpenAfflictionApplicationWindow(AfflictionSet currentSet) 
    {
        if(applicationWindow != null) 
        {
            StopCoroutine(applicationWindow);
            applicationWindow = null;
        }
        applicationWindow = CheckAfflictionApplicationWindow(currentSet);
        StartCoroutine(applicationWindow);
    }

    IEnumerator CheckAfflictionApplicationWindow(AfflictionSet currentSet) 
    {
        List<StatusEffect.Effect_Affliction> availableAfflictions = new List<StatusEffect.Effect_Affliction>();
        if(currentSet._weakAffliction != StatusEffect.Effect_Affliction.None) 
        {
            availableAfflictions.Add(currentSet._weakAffliction);
        }
        if (currentSet._mediumAffliction != StatusEffect.Effect_Affliction.None)
        {
            availableAfflictions.Add(currentSet._mediumAffliction);
        }
        if (currentSet._strongAffliction != StatusEffect.Effect_Affliction.None)
        {
            availableAfflictions.Add(currentSet._strongAffliction);
        }
        float frameCount = 0;
        float maxTime = 30f * Base_FrameCode.ONE_FRAME;
        StatusEffect.Effect_Affliction inputtedAffliction = StatusEffect.Effect_Affliction.None;
        while (frameCount < maxTime) 
        {
            if (CheckInputForAffliction(availableAfflictions,out inputtedAffliction)) 
            {
                SendAfflicion(inputtedAffliction);
            }
            frameCount += Base_FrameCode.ONE_FRAME;
            yield return new WaitForSeconds(Base_FrameCode.ONE_FRAME);
        }
    }
    bool CheckInputForAffliction(List<StatusEffect.Effect_Affliction> totalAfflictions, out StatusEffect.Effect_Affliction inputtedAffliction) 
    {
        /*if () 
        {

        }
        if ()
        {

        }
        if ()
        {

        }*/
        inputtedAffliction = StatusEffect.Effect_Affliction.None;
        return false;
    }
    public void SendAfflicion(StatusEffect.Effect_Affliction c)
    {
        Affliction_Object newAffliction = new Affliction_Object();
        if (_totalAfflictions.TryGetValue(c, out newAffliction))
        {
            if (!_base._cSuperMeter.CanTakeFromMeter(newAffliction._afflictionBase.meterRequirement))
            {
                return;
            }
            _base._cSuperMeter.TakeMeter(newAffliction._afflictionBase.meterRequirement);
            _base.opponentPlayer._afflictionManager.ApplyAffliction(newAffliction);
        }
    }
    public void ApplyAffliction(Affliction_Object _appliedAffliction)
    {
        Affliction activeAffliction = _appliedAffliction._afflictionObject.GetComponent<Affliction>();
        _appliedAffliction._afflictionObject.SetActive(true);
        activeAffliction.SetDurationValues();
        activeAffliction.durationSlider.value = 1;
        activeAffliction.ActivateAffliction(() => DeactivateAffliction(_appliedAffliction, _appliedAffliction._afflictionObject));
        appliedAffliction.Add(_appliedAffliction);
        ApplyBaseAfflictions();
    }
    void DeactivateAffliction(Affliction_Object _appliedAffliction, GameObject thisAffliction)
    {
        _appliedAffliction._afflictionBase.ForceEndAffliction();
        appliedAffliction.Remove(_appliedAffliction);
        thisAffliction.SetActive(false);
    }
    #endregion


    public void ApplyAffliction(int index)
    {
        Affliction activeAffliction = TotalAfflictions[index]._afflictionObject.GetComponent<Affliction>();
        TotalAfflictions[index]._afflictionObject.SetActive(true);
        activeAffliction.SetDurationValues();
        activeAffliction.durationSlider.value = 1;
        activeAffliction.ActivateAffliction(() => DeactivateAffliction(TotalAfflictions[index], index));
        appliedAffliction.Add(TotalAfflictions[index]);
        ApplyBaseAfflictions();
    }
    void DeactivateAffliction(Affliction_Object _appliedAffliction,int _index) 
    {
        _appliedAffliction._afflictionBase.ForceEndAffliction();
        appliedAffliction.Remove(_appliedAffliction);
        TotalAfflictions[_index]._afflictionObject.SetActive(false);
    }
    public void ApplyBaseAfflictions(float value = -1) 
    {
        for (int i = 0; i < appliedAffliction.Count; i++)
        {
            if (appliedAffliction[i] != null)
            {
                if (appliedAffliction[i]._afflictionBase.currentState != ActiveState.Active)
                {
                    appliedAffliction[i]._afflictionBase.ActivateAffliction(value, appliedAffliction[i]._afflictionBase.afflictionType);
                }
            }
        }
    }
    public void CheckSingleUseAfflictions()
    {
        for (int i = 0; i < appliedAffliction.Count; i++)
        {
            if (appliedAffliction[i] != null)
            {
                if (appliedAffliction[i]._afflictionBase.currentState == ActiveState.Active 
                    && appliedAffliction[i]._afflictionBase._isConsumed 
                    && appliedAffliction[i]._afflictionBase._isSingleUse)
                {
                    appliedAffliction[i]._afflictionBase.KillSingleAffliction();
                }
            }
        }
    }
    public bool Activate = false;
    [Range(0, 14)] public int value;
    public void Update()
    {
        if (Activate) 
        {
            TestSendAffliction(value);
            Activate = false;
        }
    }
    public void TestSendAffliction(int Value) 
    {
        Affliction _affliction = TotalAfflictions[Value]._afflictionBase;
        for (int i = 0; i < TotalAfflictions.Count; i++) 
        {
            if (TotalAfflictions[i]._afflictionBase.affliction == _affliction.affliction)
            {
                if (!_base._cSuperMeter.CanTakeFromMeter(TotalAfflictions[i]._afflictionBase.meterRequirement))
                {
                    return;
                }
                _base._cSuperMeter.TakeMeter(TotalAfflictions[i]._afflictionBase.meterRequirement);
                _base.opponentPlayer._afflictionManager.ApplyAffliction(i);
                break;
            }
            continue;
        }
    }
}
[Serializable]
public class Affliction_Object 
{
    public StatusEffect.Effect_Affliction _afflictionType;
    public GameObject _afflictionObject;
    public Affliction _afflictionBase;
    public void SetAffliction(Character_Base _base) 
    {
        _afflictionBase = _afflictionObject.GetComponent<Affliction>();
        _afflictionBase.SetTextValue();
        _afflictionBase._base = _base;
        _afflictionBase.currentState = ActiveState.Inactive;
    }
}