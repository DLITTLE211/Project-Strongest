using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Combo List", menuName = "Movement Tools")]
public class ExtraMoveAsset : ScriptableObject
{
    public List<ExtraMovementControl> moveControls;

    public void CallActionInEMC(MovementInput move, Character_Base _base) 
    {
        for (int i = 0; i < moveControls.Count; i++) 
        {
            if (moveControls[i] == move) 
            {
                moveControls[i].CallAction(move, _base);
                break;
            }
            else { continue; }
        }
    }
}