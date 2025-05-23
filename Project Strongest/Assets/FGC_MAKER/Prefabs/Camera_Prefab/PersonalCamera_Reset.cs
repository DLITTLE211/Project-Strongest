using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonalCamera_Reset : MonoBehaviour
{
    public Character_SubStateController_Base _subStateController;
    public void ResestCameraData() 
    {
        _subStateController.ResetCameraCanvas();
    }
}
