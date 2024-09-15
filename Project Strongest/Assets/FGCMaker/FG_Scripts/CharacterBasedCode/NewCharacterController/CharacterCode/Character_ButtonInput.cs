using UnityEngine;
using Rewired;

[System.Serializable]
public class Character_ButtonInput 
{
    [SerializeField] internal ActionElementMap Button_Element;
    [SerializeField] internal ButtonStateMachine Button_State;
    [SerializeField] private int actionID;
    private string element_Name,controller_ButtonName;
    public string Button_Name;
    public void SetButton(ActionElementMap action)
    {
        element_Name = action.actionDescriptiveName;
        controller_ButtonName = action.elementIdentifierName;
        actionID = action.actionId;
    }
    public Character_ButtonInput(ActionElementMap newElement) 
    {
        Button_Element = newElement;
        Button_State = new ButtonStateMachine();
    }
    public bool CheckAndSetButtonIdentifier(InputAction newIdentifier) 
    {
        int new_ID = newIdentifier.id;
        int element_ID = Button_Element.actionId;
        if (element_ID == new_ID) 
        {
            Button_Name = newIdentifier.name;
            return true;
        }
        return false;
    }
    public void TryAddButton(string newIdentifier)
    {
        Button_Name = newIdentifier;
    }
}
