using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using FightingGame_FrameData;

public class Character_MobilityOptions : MonoBehaviour 
{
    [SerializeField] private List<Character_MobilityOption> _mobilityOptions;
    public List<Character_MobilityOption> Mobility { get { return _mobilityOptions; } }
    bool animRunning;
    float frameCount;
    IEnumerator MobilityRoutine;
    List<CustomCallback> callbackList;
    public void CallMobilityAction(Character_MobilityOption _mobOption)
    {
        _mobOption.CurBase._cForce.HandleExtraMovement(_mobOption);
    }
    public void ActivateMobilityOption(Character_MobilityOption _currentAction) 
    {
        KillCurrentRoutine();
        animRunning = true;
        SetCustomCallbacks(_currentAction);
         _currentAction.CurBase._cAnimator.PlayNextAnimation(_currentAction._animInformation._animHash, 0.25f, true);
         MobilityRoutine = PlayAnimSequence(_currentAction);
        StartCoroutine(MobilityRoutine);
    }
    IEnumerator PlayAnimSequence(Character_MobilityOption _currentAction)
    {
        float waitTime = Base_FrameCode.ONE_FRAME;
        while (frameCount <= _currentAction._animInformation._animLength)
        {
            #region Mobility Anim Checks
            if (callbackList.Count > 0)
            {
                CustomCallback callback = callbackList[0];
                ExtraFrameHitPoints newHitPoint = _currentAction.frameData._extraPoints[0];
                float hitPoint = waitTime * callback.timeStamp;
                if (frameCount >= hitPoint && !callback.funcBool)
                {
                    newHitPoint.hitFrameBool = true;
                    callback.funcBool = true;
                    _currentAction.CurBase.ApplyForceOnCustomCallback(callbackList[0], _currentAction);
                    callbackList.RemoveAt(0);
                }
            }
            frameCount += waitTime;
            yield return new WaitForSeconds(waitTime);
            #endregion
        }
    }
    void SetCustomCallbacks(Character_MobilityOption _currentAction)
    {
        callbackList = new List<CustomCallback>();
        for (int i = 0; i < _currentAction.frameData._extraPoints.Count; i++)
        {
            _currentAction.frameData._extraPoints[i].hitFrameBool = false;
            CustomCallback customCallback = new CustomCallback(
                _currentAction.frameData._extraPoints[i].call,
                _currentAction.frameData._extraPoints[i].hitFramePoints,
                _currentAction.frameData._extraPoints[i].hitFrameBool,
                _currentAction.frameData._extraPoints[i].camPos,
                _currentAction.frameData._extraPoints[i].camRotation,
                _currentAction.frameData._extraPoints[i].Force,
                _currentAction.frameData._extraPoints[i].projectileSpeed,
                _currentAction.frameData._extraPoints[i].snapMovement,
                _currentAction.frameData._extraPoints[i].customDamage,
                _currentAction.frameData._extraPoints[i].awaitEnum);
            callbackList.Add(customCallback);
        }
    }
    public void KillCurrentRoutine() 
    {
        if ( MobilityRoutine != null) 
        {
            StopCoroutine(MobilityRoutine);
            MobilityRoutine = null;
        }
        frameCount = 0;
        animRunning = false;
    }
}
interface IMobilityOption
{
    void PerformMobilityAction(Callback SendOnSucess);
    void SetStarterInformation(Character_Base _base);
    MovementType GetMovementType();
    void SetMobilityTimer();
}