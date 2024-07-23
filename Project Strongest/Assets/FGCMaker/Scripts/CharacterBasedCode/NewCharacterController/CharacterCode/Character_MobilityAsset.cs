using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Combo List", menuName = "Mobility Options")]
public class Character_MobilityAsset : ScriptableObject
{
    [SerializeField]private List<Character_Mobility> mobilityOptions;
    float frameCount;
    List<CustomCallback> customMobilityCallBacks;
    public List<Character_Mobility> MobilityOptions 
    {
        get 
        {
            return mobilityOptions.Select(mobilityOption => mobilityOption.Clone()).ToList();
        }
    }

    public void CallMobilityAction(Character_Mobility _mobOption)
    {
        _mobOption.baseCharacter._cForce.HandleExtraMovement(_mobOption);
    }
    public bool ReturnActiveMove() 
    {
        for (int i = 0; i < mobilityOptions.Count; i++) 
        {
            if (mobilityOptions[i].activeMove == true) 
            {
                return true;
            }
        }
        return false;
    }
    public Character_Mobility GetCharacterMobility(int i) 
    {
        return mobilityOptions[i];
    }

    public void OnDestroy()
    {
        for (int i = 0; i < mobilityOptions.Count; i++)
        {
           mobilityOptions[i].ClearAnimatorAndBase();
        }
    }
    public IEnumerator TickMobilityAnimation(Character_Mobility inputToActivate, MobilityAnimation anim, float totalWaitTime, Callback endFunc)
    {
        float waitTime = 1f / 60f;
        frameCount = 0;

        inputToActivate.baseCharacter._cAnimator.PlayNextAnimation(Animator.StringToHash(anim.animName[0]), 0.25f);
        while (frameCount <= anim.animLength[0])
        {
            #region Mobility Anim Checks
            for (int i = 0; i < anim.frameData._extraPoints.Count; i++)
            {
                ExtraFrameHitPoints newHitPoint = anim.frameData._extraPoints[i];
                if (frameCount >= waitTime * newHitPoint.hitFramePoints)
                {
                    if (newHitPoint.call == HitPointCall.ActivateMobilityAction && newHitPoint.hitFrameBool == false)
                    {
                        newHitPoint.hitFrameBool = true;
                        inputToActivate.baseCharacter.ApplyForceOnCustomCallback(anim.MobilityCallbacks[0], inputToActivate);
                        anim.MobilityCallbacks.RemoveAt(0);
                    }
                    else 
                    {
                        if (newHitPoint.hitFrameBool == false)
                        {
                            newHitPoint.hitFrameBool = true;
                            inputToActivate.baseCharacter.ApplyForceOnCustomCallback(anim.MobilityCallbacks[0]);
                            //Messenger.Broadcast<CustomCallback>(Events.CustomCallback, anim.MobilityCallbacks[0]);
                            anim.MobilityCallbacks.RemoveAt(0);
                        }
                    }
                }
            }
            frameCount += waitTime;
            yield return new WaitForSeconds(waitTime);
            #endregion
        }
        //endFunc();
    }
}

public interface IMobility 
{
    public void TurnInputsToString(Character_Base _base);
    public void SetAnims(Character_Animator animator);
    public bool CheckMovement(Character_ButtonInput movement, Character_Base curBase, bool superPropAvaiable);
    public void PlayAnimation(Character_Mobility mobilityAnim, Character_Base curBase);
    public bool ContinueCombo(Character_ButtonInput movement, Character_Base curBase, bool superPropAvaiable);
    public bool IsCorrectInput(Character_ButtonInput newInput,int curInput);
    public void ResetOnSuccess();
    public void ResetCurrentInput();
}