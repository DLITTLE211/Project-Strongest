using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelect_CharacterButton : Button
{
    public bool isHovered()
    {
        return currentSelectionState == SelectionState.Highlighted;
    }
}
