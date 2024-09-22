using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using FightingGame_FrameData;

public class Character_AfflictionManager : MonoBehaviour
{
    [SerializeField] private Character_Base _base;
    [SerializeField] private List<Affliction_Object> TotalAfflictions;
    [SerializeField] private List<Affliction_Object> appliedAffliction = new List<Affliction_Object>();
    [SerializeField] private IEnumerator ApplyAfflictionRoutine;
    public void ClearAppliedAffliction() 
    {
        if(appliedAffliction != null) 
        {
            appliedAffliction = null;
        }
    }
    public void Start()
    {
        for(int i = 0; i < TotalAfflictions.Count; i++) 
        {
            TotalAfflictions[i]._afflictionObject.SetActive(false);
        }
    }
    public void ApplyAffliction(Affliction_Object _appliedAffliction,int AfflictionIndex) 
    {
        Affliction currentAffliction = _appliedAffliction._afflictionObject.GetComponent<Affliction>();
        currentAffliction.durationSlider.value = 1;
        currentAffliction.ActivateAffliction(() => DeactivateAffliction(_appliedAffliction,AfflictionIndex));
        appliedAffliction.Add(_appliedAffliction);
    }
    void DeactivateAffliction(Affliction_Object _appliedAffliction,int _index) 
    {
        appliedAffliction.Remove(_appliedAffliction);
        TotalAfflictions[_index]._afflictionObject.SetActive(false);
    }
    public void CheckToApplyAffliction(Attack_BaseProperties _specialMoveProperty) 
    {
        if (appliedAffliction != null) 
        {
            StopCoroutine(ApplyAfflictionRoutine);
            ApplyAfflictionRoutine = null;
        }
        /*
         if(_specialMoveProperty.AfflictionSet.Total < 1)
         {
            return;
         }
         */
        ApplyAfflictionRoutine = OnDelayApplyAffliction();
        StartCoroutine(ApplyAfflictionRoutine);
    }
    IEnumerator OnDelayApplyAffliction(/*AfflictionSet _afflictionSet*/) 
    {
        float waitTime = Base_FrameCode.ONE_FRAME * 10f;
        yield return new WaitForSeconds(waitTime);
        /*
        if(_afflictionSet) 
        {
            SendAfflictionToTarget(Affliction _affliction); 
        }
        */
    }
    public bool Activate;
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
        Affliction _affliction = TotalAfflictions[Value]._afflictionObject.GetComponent<Affliction>();
        _base.opponentPlayer._afflictionManager.ApplyAffliction(TotalAfflictions[Value], (int)_affliction.affliction);
    }
}
[Serializable]
public class Affliction_Object 
{
    public StatusEffect.Effect_Affliction _afflictionType;
    public GameObject _afflictionObject;
}