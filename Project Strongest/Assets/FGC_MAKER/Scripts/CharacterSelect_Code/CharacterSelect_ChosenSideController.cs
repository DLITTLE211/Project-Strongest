using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;
using UnityEngine.UI;

public class CharacterSelect_ChosenSideController : MonoBehaviour
{
    [SerializeField] private List<Transform> sides;
    [SerializeField] private GameObject chooseCharacterSelectObject;
    bool canSwitch;
    private void Start()
    {
        canSwitch = true;
    }
    public void UpdateControllerSide(ChooseSide_Object objectMoving, int direction, Callback failFunc) 
    {
        if (canSwitch)
        {
            if (direction == 1)
            {
                if ((objectMoving.sideIterator > sides.Count-1))
                {
                    objectMoving.sideIterator = sides.Count - 1;
                }
                else
                {
                    objectMoving.sideIterator++;
                }
            }
            if (direction == -1)
            {
                if ((objectMoving.sideIterator < 0))
                {
                    objectMoving.sideIterator = 0;
                }
                else
                {
                    objectMoving.sideIterator--;
                }
            }
            if (objectMoving.sideIterator == 0 || objectMoving.sideIterator == 2)
            {
                if (sides[objectMoving.sideIterator].transform.childCount > 0)
                {
                    objectMoving.sideIterator = 1;
                    failFunc();
                    return;
                }
            }
            switch (objectMoving.sideIterator) 
            {
                case 0:
                    objectMoving.SetImageP1(false);
                    break;
                case 1:
                    objectMoving.SetImageCPU(false);
                    break;
                case 2:
                    objectMoving.SetImageP2(false);
                    break;
            }
            objectMoving._object.transform.parent = sides[objectMoving.sideIterator];

            StartCoroutine(StartCoolDownTimer());
        }
    }
    public void CloseChooseSideMenu(Callback func) 
    {

        chooseCharacterSelectObject.SetActive(false);
        func();
    }
    IEnumerator StartCoolDownTimer() 
    {
        canSwitch = false;
           yield return new WaitForSeconds(0.5f);
        canSwitch = true;
    }
}
