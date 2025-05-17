using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine.EventSystems;

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
        _pauseMenu.SetActive(false);
        _pauseMenu.GetComponent<TrainingMenu_Controller>().SetupTrainingButtons(_eventSystem);
        _eventSystem.firstSelectedGameObject = null;

        _pauseMenu.GetComponent<TrainingMenu_Controller>().SetP1MoveListInformation(mainPlayer.comboList3_0, mainPlayer.characterProfile.CharacterName);
        _pauseMenu.GetComponent<TrainingMenu_Controller>().SetP2MoveListInformation(secondaryPlayer.comboList3_0, secondaryPlayer.characterProfile.CharacterName);
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
    public override void TogglePauseMenu()
    {
        base.TogglePauseMenu();
        if (_pauseMenu.activeInHierarchy)
        {
            _eventSystem.SetSelectedGameObject(_pauseMenu.GetComponent<TrainingMenu_Controller>().ReturnTopButton().gameObject);
        }
        else 
        {
            _eventSystem.firstSelectedGameObject = null; 
            List<bool> backupSet = new List<bool> { false, false, false, false, false };
            UI_DisplaySet p1Set = _pauseMenu.GetComponent<TrainingMenu_Controller>().ReturnPlayer1DisplaySettings();
            List<bool> p1UI_StateData = p1Set != null ? p1Set.ReturnBooleanStatesForObject() : backupSet;
            SetObjectState(mainPlayer, p1UI_StateData);

            UI_DisplaySet p2Set = _pauseMenu.GetComponent<TrainingMenu_Controller>().ReturnPlayer2DisplaySettings();
            List<bool> p2UI_StateData = p2Set != null ? p2Set.ReturnBooleanStatesForObject() : backupSet;
            SetObjectState(secondaryPlayer, p2UI_StateData);
        }
    }
    void SetObjectState(Character_Base player, List<bool> states)
    {
        player.widget.SetObjectState(states[0]);
        player._timer.SetObjectState(states[1]);
        player._cDamageCalculator.SetObjectState(states[2]);
        player._aFrameDataMeter.SetObjectState(states[3]);
        player._cHitController.SetObjectState(states[4]);
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
        List<float> stunHealhList = _pauseMenu.GetComponent<TrainingMenu_Controller>().ReturnHealthValues();

        Character_Base firstCharacter = GameManager.instance.players.totalPlayers[0];
        firstCharacter._cHealth.SetHealthAndStunOnSceneReset(stunHealhList[0], stunHealhList[1]);

        Character_Base secondCharacter = GameManager.instance.players.totalPlayers[1];
        secondCharacter._cHealth.SetHealthAndStunOnSceneReset(stunHealhList[2], stunHealhList[3]);
    }
    private async Task LandingCheck() 
    {
        await mainPlayer.ResetPlayerOnTeleport();
        await secondaryPlayer.ResetPlayerOnTeleport();

        mainPlayer._cStateMachine.idleStateRef.OnEnter();
        secondaryPlayer._cStateMachine.idleStateRef.OnEnter();
    }
    IEnumerator TeleportTweenController(Vector3 pos1, Vector3 pos2)
    {
        GameManager.instance._hitstopController.CameraController.ToggleWallState(false);
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
        GameManager.instance._hitstopController.CameraController.ToggleWallState(true);
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
