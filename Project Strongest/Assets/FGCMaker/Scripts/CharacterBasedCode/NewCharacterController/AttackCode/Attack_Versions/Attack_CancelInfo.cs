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
    String_Normal_Attack = 2,
    Special_Attack = 3,
    Rekka_Input_Start = 4,
    Stance_Input_Start = 5,
    Rekka_Input_FollowUp = 6,
    Stance_Input_FollowUp = 7,
    Super_Attack = 8,
    Maximum_Attack = 9,
}