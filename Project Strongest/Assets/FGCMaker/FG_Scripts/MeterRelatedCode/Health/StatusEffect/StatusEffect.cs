using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class StatusEffect : MonoBehaviour
{
    public enum Effect_Amplify 
    {
        Bloodthirsty = 0,/*Timed*/
        Focus = 1,/*Timed*/
        HeavyHitter = 2,/*Timed*/
        Iron = 3, /*Permanent*/
        Might = 4, /*Timed*/
        Perfectionist = 5, /*Permanent*/
        WillPower = 6, /*Permanent*/
    };
    /*
     *  WillPower: Decrease to stun damage taken
     *  Iron: Increase to character defense value
     *  Perfectionist: Increases ChipDamage on block but greatly enhances Parry abilities
     *  HeavyHitter: Buffs damage output for Amplify Duration
     *  Might: Increase to chip damage of attacks for Amplify Duration
     *  Focus: Increase to meter gain for Amplify Duration
     *  Bloodthirsty: Steals meter from opponent on hit/block
     */
    public enum Effect_Affliction 
    {
        /*Weak Afflictions*/
        Dizzy = 0,
        Immobile = 1, 
        Paralysis = 2,
        Slashed = 3,
        Fragile = 4,
        /*Medium Afflictions*/
        Hollow = 5,
        Ravaged = 6,
        Shattered = 7,
        Weakened = 8,
        Yielded = 9,
        /*Strong Afflictions*/
        Defenseless = 10,
        Terror = 11,
        Erratic = 12,
        Sliced = 13,
        Severed = 14,
    };
    /*
     *  Dizzy: Increase to stun damage taken on last attack
     *  Immobile: Slows walk speed, dash distance, jump height and distance for a short time.
     *  Paralysis: Locks out Jump and Dash for a short time.
     *  Slashed: D.O.T. (Damage over time) after successful hit/block of attack.
     *  Fragile: Increase to chip damage taken on attacks for a short time
     *  Hollow: Decrease to damage output for a short time
     *  Ravaged: Increased Hit/Block stun from attacks for a short time
     *  Shattered: Increase to damage taken for a short duration.
     *  Weakened: Decrease Meter Gain for a short time
     *  Yield: Halts Amplify (re)charge time for a short time
     *  Defenseless: Incapable of blocking for a short time but increase regular/perfect parry window
     *  Terror: Flips directional inputs for a short time
     *  Erratic: Flips Attack Buttons for a short time
     *  Slice: D.O.T. Stronger (Damage over time) after successful hit of last attack.
     *  Severed: Locks out meter gain for a short time
     */
}
[Serializable]
public enum DurationType
{
    Permenant,
    Timed,
}
[Serializable]
public enum ActiveState
{
    Active,
    Inactive,
}