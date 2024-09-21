using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "New Status Effect", menuName = "Affliction")]
public class Affliction : StatusEffect
{
    public Effect_Affliction affliction;
    public float activeDuration;
    public bool _isConsumed;
    void SendEffect() 
    {
        AssignStatusEffect(affliction);
    }
    public void AssignStatusEffect(Effect_Affliction effect)
    {
    }
}
