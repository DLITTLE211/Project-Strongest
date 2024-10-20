public class State_BlockReact : BaseState
{
    bool inputIsCrouch;
    public State_BlockReact(Character_Base playerBase) : base(playerBase)
    { }
    public override void OnEnter()
    {
        if (_base._cHurtBox.triggerBox.huBType == HurtBoxType.BlockHigh) 
        {
            _base._cHurtBox.SetHurboxState(HurtBoxType.BlockHigh);
        }
        if (_base._cHurtBox.triggerBox.huBType == HurtBoxType.BlockLow)
        {
            _base._cHurtBox.SetHurboxState(HurtBoxType.BlockLow);
        }
    }
    public override void OnRecov()
    {
        base.OnRecov();
    }

    public override void OnExit()
    {
       
        base.OnExit();
    }
}
