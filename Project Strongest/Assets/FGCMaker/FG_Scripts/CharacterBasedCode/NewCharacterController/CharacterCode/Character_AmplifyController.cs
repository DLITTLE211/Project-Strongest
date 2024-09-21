using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_AmplifyController : MonoBehaviour
{
    [SerializeField] private Amplifiers chosenAmplifier;

    public void SetChosenAmplifier(Amplifiers _chosenAmplifier) 
    {
        chosenAmplifier = _chosenAmplifier;
    }
    
}
