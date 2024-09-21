using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FightingGame_FrameData;

public class Character_AfflictionManager : MonoBehaviour
{
    [SerializeField] private Affliction appliedAffliction;
    IEnumerator ApplyAfflictionRoutine;
    public void ClearAppliedAffliction() 
    {
        if(appliedAffliction != null) 
        {
            appliedAffliction = null;
        }
    }
    public void ApplyAffliction(Affliction _appliedAffliction) 
    {
        appliedAffliction = _appliedAffliction;
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
}
