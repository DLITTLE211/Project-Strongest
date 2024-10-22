using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public class Character_MobilityOption : IMobilityOption
{
    [SerializeField] private string mobilityOptionName;
    private Character_Base _curBase;
    public Character_Base CurBase { get { return _curBase; } }

    [SerializeField] private bool _requiresCharge;
    [SerializeField] private Character_InputTimer_Mobility _mobTimer;
    MovementType _movementType;
    public FrameData frameData;
    public MobilityOption_Anim _animInformation;

    public void SetStarterInformation(Character_Base _base)
    {
        throw new NotImplementedException();
    }
    public void SetMobilityTimer()
    {
        _mobTimer = _curBase._cMobiltyTimer;
    }
    public void PerformMobilityAction(Callback SendOnSucess)
    {
        if (_curBase._cHurtBox.IsGrounded() == true && _curBase._cForce.CanSendForce())
        {
            if (_curBase._cAnimator._lastMovementState == lastMovementState.nullified)
            {
                _curBase._aManager.ResetMoveHierarchy();
                _curBase._cAnimator.SetActivatedInput(this);
            }
        }
    }
    public MovementType GetMovementType()
    {
        return _movementType;
    }
}
[Serializable]
public class MobilityOption_Anim 
{
    public AnimationClip _animationClip;
    public float _animLength;
}