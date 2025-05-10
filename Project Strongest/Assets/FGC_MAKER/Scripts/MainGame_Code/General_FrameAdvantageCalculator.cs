using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Threading.Tasks;

public class General_FrameAdvantageCalculator : MonoBehaviour
{
    IEnumerator WaitingRoutine;
    float frameDifference;
    public float FrameDifference { get { return frameDifference; } }
    public void ReturnFrameDifference(Character_Base _hitPlayer, FrameData _frameData)
    {
        frameDifference = 0;
        if (WaitingRoutine != null) 
        {
            StopCoroutine(WaitingRoutine);
            WaitingRoutine = null;
        }
        WaitingRoutine = WaitForPlayersIdle(_hitPlayer, _frameData);
        StartCoroutine(WaitingRoutine);
    }
    IEnumerator WaitForPlayersIdle(Character_Base _hitPlayer, FrameData _frameData) 
    {
        List<IState> acceptableState = new List<IState>() 
        {
            _hitPlayer._cStateMachine.idleStateRef,
            _hitPlayer._cStateMachine.secondIdleState,
            _hitPlayer._cStateMachine.moveStateRef,
            _hitPlayer._cStateMachine.crouchStateRef,
            _hitPlayer._cStateMachine.standBlockRef,
            _hitPlayer._cStateMachine.crouchBlockRef,
        };
        while ((!acceptableState.Contains(_hitPlayer._cStateMachine._playerState.current.State)) && (!acceptableState.Contains(_hitPlayer.opponentPlayer._cStateMachine._playerState.current.State)))
        {
            yield return new WaitForEndOfFrame();
        }

        frameDifference = -(_hitPlayer.opponentPlayer._aFrameDataMeter.TrueFrameCount - _hitPlayer._aFrameDataMeter.TrueFrameCount);
        _hitPlayer.opponentPlayer._aFrameDataMeter.SetFrameInformation(_frameData);
    }
}