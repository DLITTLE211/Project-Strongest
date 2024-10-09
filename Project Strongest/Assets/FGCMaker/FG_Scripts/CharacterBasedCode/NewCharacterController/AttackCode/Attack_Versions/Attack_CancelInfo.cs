using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public class Attack_CancelInfo 
{
    public Cancel_State cancelFrom;
    public Cancel_State CurrentLevel;
    public Cancel_State cancelTo;
}
[Serializable]
public enum Cancel_State 
{
    NotCancellable = 0,
    Light_Normal_Attack = 1,
    Heavy_Normal_Attack = 2,
    Command_Normal_Attack = 3,
    Light_String_Normal_Start = 4,
    Light_String_Normal_FollowUp = 5,
    Heavy_String_Normal_Start = 6,
    Heavy_String_Normal_FollowUp = 7,
    Special_Attack = 8,
    Rekka_Input_Start = 9,
    Stance_Input_Start = 10,
    Rekka_Input_FollowUp = 11,
    Stance_Input_FollowUp = 12,
    Super_Attack = 13,
    Maximum_Attack = 14,
    Parry =15,
}