using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public class Attack_CancelInfo 
{
    public Cancel_State cancelFrom;
    public Cancel_State cancelTo;
}
[Serializable]
public enum Cancel_State 
{
    NotCancellable = 0,
    Normal_Attack = 1,
    String_Normal_Start = 2,
    String_Normal_FollowUp = 3,
    Special_Attack = 4,
    Rekka_Input_Start = 5,
    Stance_Input_Start = 6,
    Rekka_Input_FollowUp = 7,
    Stance_Input_FollowUp = 8,
    Super_Attack = 9,
    Maximum_Attack = 10,
}