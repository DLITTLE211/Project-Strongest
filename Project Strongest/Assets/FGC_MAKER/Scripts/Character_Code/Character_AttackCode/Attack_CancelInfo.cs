using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public class Attack_CancelInfo 
{
    public Cancel_State CurrentLevel;
    public Cancel_State nextAvailableAttackRoute;
}
[Serializable,Flags]
public enum Cancel_State 
{
    NotCancellable = 1 << 0,
    Parry = 1 << 1,
    Throw = 1 << 2,
    Light_Normal_Attack = 1 << 3,
    Heavy_Normal_Attack = 1 << 4,
    Command_Normal_Attack_Cancelable = 1 << 5,
    Command_Normal_Attack_Noncancelable = 1 << 6,
    Light_String_Normal_Start = 1 << 7,
    Light_String_Normal_FollowUp = 1 << 8,
    Heavy_String_Normal_Start = 1 << 9,
    Heavy_String_Normal_FollowUp = 1 << 10,
    Special_Attack = 1 << 11,
    Rekka_Input_Start = 1 << 12,
    Stance_Input_Start = 1 << 13,
    Rekka_Input_FollowUp = 1 << 14,
    Stance_Input_FollowUp = 1 << 15,
    Super_Attack = 1 << 16,
    Maximum_Attack = 1 << 17,
}