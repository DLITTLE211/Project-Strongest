using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public class Attack_CancelInfo 
{
    //public Cancel_State cancelFrom;
    public Cancel_State CurrentLevel;
    public Cancel_State nextAvailableAttackRoute;
}
[Serializable,Flags]
public enum Cancel_State 
{
    NotCancellable = 1 << 0,
    Light_Normal_Attack = 1 << 1,
    Heavy_Normal_Attack = 1 << 2,
    Command_Normal_Attack = 1 << 3,
    Light_String_Normal_Start = 1 << 4,
    Light_String_Normal_FollowUp = 1 << 5,
    Heavy_String_Normal_Start = 1 << 6,
    Heavy_String_Normal_FollowUp = 1 << 7,
    Special_Attack = 1 << 8,
    Rekka_Input_Start = 1 << 9,
    Stance_Input_Start = 1 << 10,
    Rekka_Input_FollowUp = 1 << 11,
    Stance_Input_FollowUp = 1 << 12,
    Super_Attack = 1 << 13,
    Maximum_Attack = 1 << 14,
    Throw = 1 << 15,
    Parry = 1 << 16,
}