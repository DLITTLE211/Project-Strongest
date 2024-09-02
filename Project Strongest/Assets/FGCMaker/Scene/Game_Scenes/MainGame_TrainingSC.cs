using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using System.Threading.Tasks;
using DG.Tweening;

public class MainGame_TrainingSC : MainGame_SettingsController
{
    public Dictionary<int, Callback> teleportPositions;
    public TeleportPoint leftPos,centerPos,rightPos;
    [SerializeField] private Image _trainingCoverImage;
    Sequence coverTweenSequence;
    private bool teleporting;
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
        _trainingCoverImage = GameObject.Find("Training_ImageCover").GetComponent<Image>();
    }
    public override void SetPlayersPosition() 
    {
        Callback teleportFunction = null;
        if (!teleporting)
        {
            if (teleportPositions.TryGetValue(mainPlayer.ReturnMovementInputs().Button_State.directionalInput, out teleportFunction))
            {
                teleportFunction();
            }
        }
    }
    private async Task LandingCheck() 
    {
        while (!mainPlayer._cHurtBox.IsGrounded() && !secondaryPlayer._cHurtBox.IsGrounded()) 
        {
            await Task.Yield();
        }
    }
    void TeleportTweenController(Vector3 pos1, Vector3 pos2)
    {
        if (coverTweenSequence != null)
        {
            coverTweenSequence.Kill();
        }
        teleporting = true;
        coverTweenSequence = DOTween.Sequence();
        coverTweenSequence.Append(_trainingCoverImage.DOFade(1f, 0.15f));
        coverTweenSequence.OnComplete(() =>
        {
            mainPlayer.transform.position = pos1;
            secondaryPlayer.transform.position = pos2;
            _trainingCoverImage.DOFade(0f, 0.15f);
        });
        coverTweenSequence = null;
        teleporting = false;
    }
    async void TeleportLeft() 
    {
        await LandingCheck();
        TeleportTweenController(leftPos._leftSidePos, leftPos._rightSidePos);
       /* coverTweenSequence = DOTween.Sequence();
        coverTweenSequence.Append(_trainingCoverImage.DOFade(255f, 0.35f));
        coverTweenSequence.OnComplete(() =>
        {
            mainPlayer.transform.position = leftPos._leftSidePos;
            secondaryPlayer.transform.position = leftPos._rightSidePos;
        });
        coverTweenSequence.Append(_trainingCoverImage.DOFade(0f, 0.35f));
        coverTweenSequence = null;*/
    }
    async void TeleportLeftInverse()
    {
        await LandingCheck();
        TeleportTweenController(leftPos._rightSidePos, leftPos._leftSidePos);
        //mainPlayer.transform.position = leftPos._rightSidePos;
        //secondaryPlayer.transform.position = leftPos._leftSidePos;
    }
    async void TeleportRightInverse()
    {
        await LandingCheck();
        TeleportTweenController(rightPos._rightSidePos, rightPos._leftSidePos);
        //mainPlayer.transform.position = rightPos._rightSidePos;
        //secondaryPlayer.transform.position = rightPos._leftSidePos;
    }
    async void TeleportRight()
    {
        await LandingCheck();
        TeleportTweenController(rightPos._leftSidePos, rightPos._rightSidePos);
        //mainPlayer.transform.position = rightPos._leftSidePos;
        //secondaryPlayer.transform.position = rightPos._rightSidePos;
    }
    async void TeleportCenterInverse()
    {
        await LandingCheck();
        TeleportTweenController(centerPos._rightSidePos, centerPos._leftSidePos);
        //mainPlayer.transform.position = centerPos._rightSidePos;
        //secondaryPlayer.transform.position = centerPos._leftSidePos;
    }
    async void TeleportCenter()
    {
        await LandingCheck();
        TeleportTweenController(centerPos._leftSidePos, centerPos._rightSidePos);
        //mainPlayer.transform.position = centerPos._leftSidePos;
        //secondaryPlayer.transform.position = centerPos._rightSidePos;
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
        _leftSidePos = new Vector3(_teleportPoint.position.x - 1.10f, _teleportPoint.position.y,0);
        _rightSidePos = new Vector3(_teleportPoint.position.x + 1.10f, _teleportPoint.position.y, 0);
    }
}
