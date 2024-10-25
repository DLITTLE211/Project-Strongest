using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FightingGame_FrameData;
using System;

public class Character_BlockHandler : MonoBehaviour
{
    [SerializeField] private Character_BlockOption StandBlock;
    [SerializeField] private Character_BlockOption CrouchBlock;
    public void SetBlockAnimationData(Character_Base _base) 
    {
        StandBlock.SetStarterInformation(_base, _base.characterProfile.StandBlockAnim);
        CrouchBlock.SetStarterInformation(_base, _base.characterProfile.CrouchBlockAnim);
    }
}
[Serializable]
public class Character_BlockOption : IBlockOption
{
    private Character_Base _curBase;
    public Character_Base CurBase { get { return _curBase; } }
    public FrameData frameData;
    public BlockOptionAnim _animInformation;
    public void SetStarterInformation(Character_Base _base, AnimationClip blockAnimHash)
    {
        _curBase = _base;
        _animInformation.SetAnimInformation(blockAnimHash);
    }
    public void PerformMobilityAction()
    {
        //TODO play Block Animation
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