using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FightingGame_FrameData;
using System;

public class Character_BlockHandler : MonoBehaviour
{
    [SerializeField] private Character_BlockOption StandBlock;
    [SerializeField] private Character_BlockOption CrouchBlock;
    IEnumerator BlockRoutine;
    float frameCount = 0;
    public void SetBlockAnimationData(Character_Base _base) 
    {
        StandBlock.SetStarterInformation(_base, _base.characterProfile.StandBlockAnim);
        CrouchBlock.SetStarterInformation(_base, _base.characterProfile.CrouchBlockAnim);
    }
    public void KillCurrentRoutine()
    {
        if (BlockRoutine != null)
        {
            StopCoroutine(BlockRoutine);
            BlockRoutine = null;
        }
        frameCount = 0;
    }
    public void ToggleBlockAnim(bool isStandBlock, bool isActivating) 
    {
        if (isActivating)
        {
            Callback ActivateCurrentBlock = isStandBlock == true ? () => ActivateBlockAnim(StandBlock, isStandBlock, isActivating) : () => ActivateBlockAnim(CrouchBlock, isStandBlock, isActivating);
            ActivateCurrentBlock();
        }
        else 
        {
            Callback DeactivateCurrentBlock = isStandBlock == true ? () => DeactivateBlockAnim(StandBlock, isStandBlock, isActivating) : () => DeactivateBlockAnim(CrouchBlock, isStandBlock, isActivating);
            DeactivateCurrentBlock();
        }
    }
    public void ActivateBlockAnim(Character_BlockOption _currentAction, bool isStandBlock, bool isActivating)
    {
        KillCurrentRoutine();
        _currentAction.CurBase._cAnimator.PlayNextAnimation(_currentAction._animInformation._animHash, 2 * Base_FrameCode.ONE_FRAME);
        BlockRoutine = PlayAnimSequence(_currentAction, isStandBlock, isActivating);
        StartCoroutine(BlockRoutine);
    }
    public void DeactivateBlockAnim(Character_BlockOption _currentAction, bool isStandBlock, bool isActivating)
    {
        KillCurrentRoutine();
       BlockRoutine = PlayAnimSequence(_currentAction, isStandBlock, isActivating);
        StartCoroutine(BlockRoutine);
    }
    IEnumerator PlayAnimSequence(Character_BlockOption _currentAction,bool isStandBlock ,bool isActivating)
    {
        if (!isActivating)
        {
            _currentAction.CurBase._cAnimator.SetCanTransitionIdle(false);
            _currentAction.CurBase.Deactivate();
        }
        _currentAction.CurBase._cHurtBox.SetHurboxState();
        _currentAction.activated = false;
        float waitTime = Base_FrameCode.ONE_FRAME;
        
        float endingFrame = isActivating ? _currentAction.activationPoint : _currentAction.recoveryAmount + _currentAction.activationPoint;
        float endingPoint = endingFrame * waitTime;
        while (frameCount <= endingPoint)
        {
            #region Mobility Anim Checks
            float hitPoint = waitTime * _currentAction.activationPoint;
            if (frameCount >= hitPoint && !_currentAction.activated)
            {
                _currentAction.activated = true;
                if (isActivating)
                {
                    _currentAction.CurBase._cAnimator.canBlock = true;
                    Callback SetBlockStyle = isStandBlock == true ?
                        () => _currentAction.CurBase._cHurtBox.SetHurboxState(HurtBoxType.BlockHigh) :
                        () => _currentAction.CurBase._cHurtBox.SetHurboxState(HurtBoxType.BlockLow);
                    SetBlockStyle();

                }
            }
            frameCount += waitTime;
            yield return new WaitForSeconds(waitTime);
            #endregion
        }
        if (!isActivating)
        {
            _currentAction.CurBase._cAnimator.SetCanTransitionIdle(true);
            _currentAction.CurBase.Activate();
        }
    }
}
[Serializable]
public class Character_BlockOption : IBlockOption
{
    private Character_Base _curBase;
    public Character_Base CurBase { get { return _curBase; } }
    public int activationPoint;
    public int recoveryAmount;
    public bool activated;
    public BlockOptionAnim _animInformation;
    public void SetStarterInformation(Character_Base _base, AnimationClip blockAnimHash)
    {
        _curBase = _base;
        _animInformation.SetAnimInformation(blockAnimHash);
    }
    public void PerformMobilityAction()
    {
        _curBase._cAnimator.PlayNextAnimation(_animInformation._animHash, 2 * (1 / 60f));
    }

}
interface IBlockOption
{
    void PerformMobilityAction();
    void SetStarterInformation(Character_Base _base, AnimationClip blockAnim);
}
[Serializable]
public class BlockOptionAnim : MobilityOption_Anim
{
 
}