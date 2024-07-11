using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Combo List", menuName = "Mobility Options")]
public class Character_MobilityAsset : ScriptableObject
{
    [SerializeField]private List<Character_Mobility> mobilityOptions;
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