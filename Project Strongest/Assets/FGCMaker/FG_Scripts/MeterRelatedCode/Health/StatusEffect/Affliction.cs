using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "New Status Effect", menuName = "Affliction")]
public class Affliction : StatusEffect
{
    public Effect_Affliction affliction;

    void SendEffect() 
    {
        AssignStatusEffect(affliction);
    }
    public void AssignStatusEffect(Effect_Affliction effect)
    {
    }
}
