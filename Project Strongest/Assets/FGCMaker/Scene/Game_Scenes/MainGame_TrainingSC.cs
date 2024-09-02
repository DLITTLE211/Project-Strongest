using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;

public class MainGame_TrainingSC : MainGame_SettingsController
{
    public Dictionary<int, Callback> teleportPositions;
    public TeleportPoint leftPos,centerPos,rightPos;
    public override void SetTeleportPositions() 
    {
        teleportPositions = new Dictionary<int, Callback>();
        teleportPositions.Add(1, TeleportLeftInverse);
        teleportPositions.Add(2, TeleportCenterInverse);
        teleportPositions.Add(3, TeleportRightInverse);
        teleportPositions.Add(4, TeleportLeft);
        teleportPositions.Add(5, TeleportCenter);
        teleportPositions.Add(6, TeleportRight);
        StartCoroutine(DelayGetTeleportPositions());
    }
    IEnumerator DelayGetTeleportPositions() 
    {
        yield return new WaitForSeconds(3 / 60f);
        leftPos = new TeleportPoint();
        centerPos = new TeleportPoint();
        rightPos = new TeleportPoint();
        leftPos.SetPositionPos("Left_TP");
        centerPos.SetPositionPos("Center_TP");
        rightPos.SetPositionPos("Right_TP");
    }
    public void SetPlayersPosition() 
    {
        Callback teleportFunction = null;
        if (teleportPositions.TryGetValue(mainPlayer.ReturnMovementInputs().Button_State.directionalInput, out teleportFunction)) 
        {
            teleportFunction();
        }
    }
    private async Task LandingCheck() 
    {
        while (!mainPlayer._cHurtBox.IsGrounded() && !secondaryPlayer._cHurtBox.IsGrounded()) 
        {
            await Task.Yield();
        }
    }
    async void TeleportLeft() 
    {
        await LandingCheck();
        mainPlayer.transform.position = leftPos._leftSidePos;
        secondaryPlayer.transform.position = leftPos._rightSidePos;

    }
    async void TeleportLeftInverse()
    {
        await LandingCheck();
        mainPlayer.transform.position = leftPos._rightSidePos;
        secondaryPlayer.transform.position = leftPos._leftSidePos;
    }
    async void TeleportRightInverse()
    {
        await LandingCheck();
        mainPlayer.transform.position = rightPos._rightSidePos;
        secondaryPlayer.transform.position = rightPos._leftSidePos;
    }
    async void TeleportRight()
    {
        await LandingCheck();
        mainPlayer.transform.position = rightPos._leftSidePos;
        secondaryPlayer.transform.position = rightPos._rightSidePos;
    }
    async void TeleportCenterInverse()
    {
        await LandingCheck();
        mainPlayer.transform.position = centerPos._rightSidePos;
        secondaryPlayer.transform.position = centerPos._leftSidePos;
    }
    async void TeleportCenter()
    {
        await LandingCheck();
        mainPlayer.transform.position = centerPos._leftSidePos;
        secondaryPlayer.transform.position = centerPos._rightSidePos;
    }
}

[Serializable]
public class TeleportPoint 
{
    public Transform _teleportPoint;
    public Vector3 _leftSidePos, _rightSidePos;
    public void SetPositionPos(string objectName)
    {
        _teleportPoint = GameObject.Find(objectName).GetComponent<Transform>();
           _leftSidePos = new Vector3(_teleportPoint.localPosition.x - 0.10f, _teleportPoint.localPosition.y,0);
        _rightSidePos = new Vector3(_teleportPoint.localPosition.x + 0.10f, _teleportPoint.localPosition.y, 0);
    }
}
