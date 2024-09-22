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
            TotalAfflictions[i].SetAffliction();
            TotalAfflictions[i]._afflictionObject.SetActive(false);
        }
        appliedAffliction.Clear();
    }
    public void ApplyAffliction(Affliction_Object _appliedAffliction,int index)
    {
        Affliction activeAffliction = TotalAfflictions[index]._afflictionObject.GetComponent<Affliction>();
        TotalAfflictions[index]._afflictionObject.SetActive(true);
        activeAffliction.SetDurationValues();
        activeAffliction.durationSlider.value = 1;
        activeAffliction.ActivateAffliction(() => DeactivateAffliction(TotalAfflictions[index], index));
        appliedAffliction.Add(TotalAfflictions[index]);
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
                _base.opponentPlayer._afflictionManager.ApplyAffliction(TotalAfflictions[Value], i);
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
    public void SetAffliction() 
    {
        _afflictionBase = _afflictionObject.GetComponent<Affliction>();
        
    }
}