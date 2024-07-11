using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class StatusEffect : ScriptableObject
{
    public float effectNumber;
    public Image effectImage,effectImageFrame;
    public Sprite effectImageSprite, effectFrameSprite;
    public enum PositiveStatusEffect {Fury, Might, WillPower, Focus};
    /*
     * Fury: Outright increase to Raw Damage
     * Might: Increase to blockstun on attacks
     * Willpower: Decrease to stun damage taken
     * Focus: Increase meter gain 
     */
    public enum NegativeStatusEffect {Dizzy, Paralysis, Shatter, Cut};
    /*
    * Dizzy: Increases rate of Stun Damage taken.
    * Paralysis: Limits movement capabilites (Dashes/Jumping).
    * Shatter: Decrease to damage output.
    * Cut: Damage over time after attack.
    */
}
