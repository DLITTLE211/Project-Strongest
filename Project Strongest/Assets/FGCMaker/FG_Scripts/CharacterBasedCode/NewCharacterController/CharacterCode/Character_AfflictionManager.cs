using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_AfflictionManager : MonoBehaviour
{
    [SerializeField] private Affliction appliedAffliction;
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
}
