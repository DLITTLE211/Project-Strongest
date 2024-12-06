using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using FightingGame_FrameData;
using TMPro;
using System;

public class Character_AmplifyController : MonoBehaviour
{
    public Character_AmplifyOption _amplifyAnimation;
    [SerializeField] private Character_Base _base;
    [SerializeField] private Amplifiers chosenAmplifier;
    [SerializeField] private Slider _amplifySlider;
    [SerializeField] private TMP_Text _amplifyText;
    IEnumerator AmplifyRoutine;
    Tween drainRoutine;
    float frameCount = 0;
    public bool allowFill;
    public void SetChosenAmplifier(Amplifiers _chosenAmplifier)
    {   
        if (_chosenAmplifier != null)
        {
            _amplifyAnimation.SetStarterInformation(_base, _base.characterProfile.AmplifyAnim);
            _amplifySlider.value = 0f;
            chosenAmplifier = _chosenAmplifier;
            chosenAmplifier._base = _base;
            _amplifyText.text = $"{_chosenAmplifier.amplifier.ToString()}";
            allowFill = true;
            _chosenAmplifier.SetFillVariables();
            _amplifySlider.targetGraphic.color = chosenAmplifier.meterColor;
        }
    }
    public void KillCurrentRoutine()
    {
        if (AmplifyRoutine != null)
        {
            StopCoroutine(AmplifyRoutine);
            AmplifyRoutine = null;
        }
        frameCount = 0;
    }
    public void ActivateAmplify()
    {
        if (chosenAmplifier.durationType == DurationType.Permenant)
        {
            return;
        }
        if (allowFill)
        {
            return;
        }
        _base._cAnimator.CountUpNegativeFrames();
        ActivateAmplifyAnim(_amplifyAnimation);
    }
    public void CheckAmplifyActiveCall(Attack_BaseProperties currentAttack = null) 
    {
        if(chosenAmplifier.buffType == BuffType.PassiveBuff) 
        {
            return;
        }
        if (chosenAmplifier.durationType == DurationType.Permenant)
        {
            return;
        }
        if (chosenAmplifier.currentState == ActiveState.Inactive)
        {
            return;
        }
        chosenAmplifier.ActivateEffect(currentAttack);
    }
    public void ActivateAmplifyAnim(Character_AmplifyOption _currentAction)
    {
        KillCurrentRoutine();
        _currentAction.PerformAction(); 
        AmplifyRoutine = PlayAnimSequence(_currentAction);
        StartCoroutine(AmplifyRoutine);
    }
    IEnumerator PlayAnimSequence(Character_AmplifyOption _currentAction)
    {
        _currentAction.CurBase._cHurtBox.SetHurboxState();
        _currentAction.activated = false;
        float waitTime = Base_FrameCode.ONE_FRAME;

        float endingFrame = _currentAction.recoveryAmount + _currentAction.activationPoint;
        float endingPoint = endingFrame * waitTime;
        while (frameCount < endingPoint)
        {
            #region Mobility Anim Checks
            float hitPoint = waitTime * _currentAction.activationPoint;
            if (frameCount >= hitPoint && !_currentAction.activated)
            {
                _currentAction.activated = true;
            }
            frameCount += waitTime;
            yield return new WaitForSeconds(waitTime);
            #endregion
        }
        DrainMeter();
    }
    private void Update()
    {
        if (chosenAmplifier != null)
        {
            if (allowFill)
            {
                AllowFillMeter();
            }
        }
    }
    public void AllowFillMeter() 
    {
        if(_amplifySlider.value == _amplifySlider.maxValue) 
        {
            allowFill = false;
            return;
        }
        IState currenState = _base._cStateMachine._playerState.current.State;
        if(currenState == _base._cStateMachine.hitStateRef) 
        {
            return;
        }
        _amplifySlider.value += chosenAmplifier.fillRateInFrames;
    }
    public void DrainMeter()
    {
        if (chosenAmplifier.durationType != DurationType.Permenant)
        {
            chosenAmplifier.currentState = ActiveState.Active;
            if (drainRoutine != null) 
            {
                drainRoutine.Kill();
                drainRoutine = null;
            }
            drainRoutine = _amplifySlider.DOValue(0f, chosenAmplifier.activeDuration);
            drainRoutine.OnComplete(() => 
            { 
                allowFill = true;
                chosenAmplifier.currentState = ActiveState.Inactive;
            });
            drainRoutine.Play();
        }
    }
}
[Serializable]
public class Character_AmplifyOption : IBlockOption
{
    private Character_Base _curBase;
    public Character_Base CurBase { get { return _curBase; } }
    public int activationPoint;
    public int recoveryAmount;
    public bool activated;
    public AmplifyAnim _animInformation;
    public void SetStarterInformation(Character_Base _base, AnimationClip blockAnimHash)
    {
        _curBase = _base;
        _animInformation._animationClip = blockAnimHash;
        _animInformation.SetAnimInformation(blockAnimHash);
    }
    public void PerformAction()
    {
        _curBase._cAnimator.PlayNextAnimation(_animInformation._animHash, 2 * Base_FrameCode.ONE_FRAME);
    }
}
[Serializable]
public class AmplifyAnim : MobilityOption_Anim{}