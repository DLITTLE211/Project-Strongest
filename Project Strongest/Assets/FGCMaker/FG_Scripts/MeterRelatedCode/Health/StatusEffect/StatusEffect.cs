using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class StatusEffect : ScriptableObject
{
    public enum Effect_Amplify 
    {
        /*Permanent*/
        WillPower, 
        Iron, 
        Perfectionist, 
        /*Timed*/
        HeavyHitter, 
        Might,
        Focus,
        Bloodthirsty,
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
        Dizzy,
        Immobile, 
        Paralysis,
        Slashed,
        Fragile,
        /*Medium Afflictions*/
        Hollow,
        Ravaged,
        Shattered,
        Weakened,
        Yielded,
        /*Strong Afflictions*/
        Defenseless,
        Terror,
        Erratic,
        Sliced,
        Severed,
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
