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
    IEnumerator TeleportTweenController(Vector3 pos1, Vector3 pos2)
    {
        bool teleported = false;
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
            teleported = true;
        });
        coverTweenSequence = null;

        yield return new WaitUntil(() => teleported);
        yield return new WaitForSeconds(0.25f);
        _trainingCoverImage.DOFade(0f, 0.15f);
        teleporting = false;
    }
    async void TeleportLeft() 
    {
        await LandingCheck();
        StartCoroutine(TeleportTweenController(leftPos._leftSidePos, leftPos._rightSidePos));
    }
    async void TeleportLeftInverse()
    {
        await LandingCheck();
        StartCoroutine(TeleportTweenController(leftPos._rightSidePos, leftPos._leftSidePos));
    }
    async void TeleportRightInverse()
    {
        await LandingCheck();
        StartCoroutine(TeleportTweenController(rightPos._rightSidePos, rightPos._leftSidePos));
    }
    async void TeleportRight()
    {
        await LandingCheck();
        StartCoroutine(TeleportTweenController(rightPos._leftSidePos, rightPos._rightSidePos));
    }
    async void TeleportCenterInverse()
    {
        await LandingCheck();
        StartCoroutine(TeleportTweenController(centerPos._rightSidePos, centerPos._leftSidePos));
    }
    async void TeleportCenter()
    {
        await LandingCheck();
        StartCoroutine(TeleportTweenController(centerPos._leftSidePos, centerPos._rightSidePos));
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
