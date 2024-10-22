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
    bool animRunning = false;
    public List<Character_Mobility> MobilityOptions 
    {
        get 
        {
            return mobilityOptions.Select(mobilityOption => mobilityOption.Clone()).ToList();
        }
    }

    public void CallMobilityAction(Character_Mobility _mobOption)
    {
        //_mobOption.baseCharacter._cForce.HandleExtraMovement(_mobOption);
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
    public IEnumerator TickMobilityAnimation(Character_Mobility inputToActivate)
    {
        if (!animRunning)
        {
            float waitTime = 1f / 60f;
            frameCount = 0;
            animRunning = true;
            inputToActivate.baseCharacter._cAnimator.PlayNextAnimation(Animator.StringToHash(inputToActivate.mobilityAnim.animName[0]), 0.25f, true);
            while (frameCount <= inputToActivate.mobilityAnim.animLength[0])
            {
                #region Mobility Anim Checks
                if (inputToActivate.mobilityAnim._customMobilityCallBacks.Count > 0)
                {
                    CustomCallback callback = inputToActivate.mobilityAnim._customMobilityCallBacks[0];
                    ExtraFrameHitPoints newHitPoint = inputToActivate.mobilityAnim.frameData._extraPoints[0];
                    float hitPoint = waitTime * callback.timeStamp;
                    if (frameCount >= hitPoint && !callback.funcBool)
                    {
                        newHitPoint.hitFrameBool = true;
                        callback.funcBool = true;
                        //inputToActivate.baseCharacter.ApplyForceOnCustomCallback(inputToActivate.mobilityAnim._customMobilityCallBacks[0], inputToActivate);
                        inputToActivate.mobilityAnim._customMobilityCallBacks.RemoveAt(0);
                    }
                }
                frameCount += waitTime;
                yield return new WaitForSeconds(waitTime);
                #endregion
            }
            animRunning = false;
        }
        else
        {
            animRunning = false;
            inputToActivate.baseCharacter._cAnimator.NullifyMobilityOption();
        }
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