using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "New Status Effect", menuName = "Amplifier")]
public class Amplifiers : StatusEffect 
{
    public Effect_Amplify amplifier;
    void SendEffect()
    {
        AssignStatusEffect(amplifier);
    }
    public void AssignStatusEffect(Effect_Amplify effect)
    {
    }
    public void AddToMeter() 
    {

    }
}
