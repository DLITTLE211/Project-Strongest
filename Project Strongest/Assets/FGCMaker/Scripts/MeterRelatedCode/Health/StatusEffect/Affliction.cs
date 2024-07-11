using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "New Status Effect", menuName = "Affliction")]
public class Affliction : StatusEffect
{
    public NegativeStatusEffect affliction;
    private void Awake()
    {
        Messenger.AddListener(Events.SendAfflictionToTarget,SendEffect);
    }
    void SendEffect() 
    {
        AssignStatusEffect(affliction);
    }
    public void AssignStatusEffect(NegativeStatusEffect effect)
    {
        Messenger.Broadcast<NegativeStatusEffect>(Events.SendNegativeEffect, effect);
    }
}
