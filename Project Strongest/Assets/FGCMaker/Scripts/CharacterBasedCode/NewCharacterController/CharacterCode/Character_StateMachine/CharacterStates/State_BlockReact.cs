public class State_BlockReact : BaseState
{
    bool inputIsCrouch;
    public State_BlockReact(Character_Base playerBase) : base(playerBase)
    { }
    public override void OnEnter()
    {
        DebugMessageHandler.instance.DisplayErrorMessage(1, "Enter Block React State");
    }
    public override void OnRecov()
    {
        base.OnRecov();
    }

    public override void OnExit()
    {
        if (!_base._cHitController.ReturnStandBlock())
        {
            _base._cHurtBox.SetHurboxState(HurtBoxType.BlockHigh);
        }
        if (!_base._cHitController.ReturnCrouchBlock())
        {
            _base._cHurtBox.SetHurboxState(HurtBoxType.BlockLow);
        }
        base.OnExit();
    }
}
