using System;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

[Serializable]
public class Attack_BaseProperties
{
    [Header("_______ATTACK IDENTIFICATION INFO______")]
    public string _attackName;
    [SerializeField] protected Character_InputTimer_Attacks _cTimer;
    public Character_InputTimer_Attacks InputTimer 
    { 
        get { return _cTimer; } 
        set { _cTimer = value; } 
    }
    #region Attack Damage Numbers
    [Space(20)]
    [Header("_______DAMAGE NUMBERS______")]
    public float rawAttackDamage;
    public float rawChipDamage;
    public float counterHitDamageMult;

    public Attack_StunValues attackMainStunValues;
    [Space(20)]
    [Range(1, 20)] public int hitstopValue;
    [Range(1, 50)] public int blockStunValue;
    [Range(1, 200)] public int hitstunValue;
    public HitLevel hitLevel;
    [SerializeField] public List<int> attackHashes;
    [Space(20)]
    #endregion

    [Header("_____REQUIREMENTS/RESTRICTIONS______")]
    [Range(0, 3)] public int _meterRequirement;
    [Range(0, 30)] public int _meterAwardedOnHit;
    [Range(-10, 40)] public int attackScalingPercent;
    public bool dashCancelable, JumpCancelable;

    #region MoveType Properties
    [Space(20)]
    [Header("_______ATTACK PROPERTY INFORMATION_______")]
    public AirAttackInfo _airInfo;
    public Attack_CancelInfo cancelProperty;
    public MoveType _moveType;
    public AttackHandler_Attack AttackAnims;
    public bool hitConnected;
    #endregion

    #region KnockBack/KnockDown Variables
    [Space(20)]
    [Header("______KNOCKDOWN INFORMATION______")]
    public Horizontal_KnockBack lateralKBP; // Lateral KnockBack Properties
    public Vertical_KnockBack verticalKBP; // Vertical KnockBack Properties
    public Attack_KnockDown KnockDown; // Vertical KnockBack Properties
    #endregion
    public void SetAttackAnims(Character_Animator animator)
    {
        hitConnected = false;
        attackHashes = new List<int>();
        AttackAnims.SetAttackAnim(animator);
        attackHashes.Add(Animator.StringToHash(AttackAnims.animName));
        SetChipDamageValue();
    }
    public void SetChipDamageValue()
    {
        rawChipDamage = (rawAttackDamage / 10f);
    }

}
[Serializable]
public class Attack_StunValues
{
    [Range(1, 100)] public int hitstunValue;
    [Range(0, 50)] public int blockStunValue;
    [Range(0, 20)] public int hitstopValue;
    [Range(0, 10)] public int blockStopValue;
}

[Serializable]
public class CustomDamageField 
{
    public float rawAttackDamage;
    public float counterHitDamageMult;

    public Attack_StunValues customDamageFieldStunValues;
    [Space(20)]
    [Range(1, 100)] public int hitstopValue;
    [Range(1, 200)] public int hitstunValue;
    public HitLevel hitLevel;
    public bool isFinalAttack;
    public Horizontal_KnockBack lateralKBP; // Lateral KnockBack Properties
    public Vertical_KnockBack verticalKBP; // Vertical KnockBack Properties
    public Attack_KnockDown KnockDown; // Vertical KnockBack Properties
}
[Serializable]
public class Attack_Input 
{
    public string attackString;
    public char[] attackStringArray;
    public void turnStringToArray() 
    {
        attackStringArray = attackString.ToCharArray();
    }
    public Attack_Input(string _attackString, char[] _stringArray) 
    {
        attackString = _attackString;
        attackStringArray = _stringArray;
    }
}
[Serializable]
public class Vertical_KnockBack
{
    public Attack_KnockBack_Vertical verticalKBP;
    [Range(0f, 50f)] public int Value;
}
[Serializable]
public class Horizontal_KnockBack
{
    public Attack_KnockBack_Lateral lateralKBP;
    [Range(0f, 50f)] public int Value;
}
[Serializable]
public enum Attack_KnockBack_Lateral
{
    No_KB,
    Crumple,
    Slight_KB,
    Medium_KB,
    Heavy_KB,
    FullForceWallBounce,
}
[Serializable]
public enum Attack_KnockBack_Vertical
{
    No_KUD,
    Slight_KU,
    Medium_KU,
    Heavy_KU,
    Slight_KD,
    Medium_KD,
    Heavy_KD,
    FullForceGroundBounce,
}
[Serializable, Flags]
public enum HitLevel
{
    SlightKnockback = 2,
    MediumKnockback = 4,
    SoaringHit = 8,
    Crumple = 16,
    Spiral = 32,
    StandardBlock = 64,
    GuardBreak = 128,
    Scorpion = 256,

}
/*
 * SKD = Soft KnockDown
 * HKD = Hard KnockDown
 */
[Serializable, Flags]
public enum Attack_KnockDown
{
    NONE = 2,
    SKD = 4,
    HKD = 8,
}
[Serializable]
public enum AirAttackInfo
{
    GroundOnly,
    AirOk,
    AirOnly
}