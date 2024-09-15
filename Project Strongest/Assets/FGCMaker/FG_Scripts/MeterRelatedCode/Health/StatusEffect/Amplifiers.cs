using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "New Status Effect", menuName = "Amplifier")]
public class Amplifiers : StatusEffect 
{
    public PositiveStatusEffect amplifier;
    private void Awake()
    {
        Messenger.AddListener(Events.SendAmpliferToSelf, SendEffect);
    }
    void SendEffect()
    {
        AssignStatusEffect(amplifier);
    }
    public void AssignStatusEffect(PositiveStatusEffect effect)
    {
        Messenger.Broadcast<PositiveStatusEffect>(Events.SendPositiveEffect, effect);
    }
}
