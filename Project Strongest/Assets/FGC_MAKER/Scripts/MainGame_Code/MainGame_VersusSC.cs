using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.UI;

public class MainGame_VersusSC : MainGame_SettingsController
{
    public TeleportPoint centerPos;
    Sequence coverTweenSequence;
    private bool teleporting;
    [SerializeField] private Image _trainingCoverImage;
    private float teleportTime;

    public override void SetTeleportPositions(float teleportTime = 3 / 60f)
    {
        _pauseMenu.SetActive(false);
        _pauseMenu.GetComponent<VersusMenu_Controller>().SetupVersusButtons(_eventSystem);
        _eventSystem.firstSelectedGameObject = null;

        _pauseMenu.GetComponent<VersusMenu_Controller>().SetP1MoveListInformation(mainPlayer.comboList3_0, mainPlayer.characterProfile.CharacterName, mainPlayer._side);
        _pauseMenu.GetComponent<VersusMenu_Controller>().SetP2MoveListInformation(secondaryPlayer.comboList3_0, secondaryPlayer.characterProfile.CharacterName, secondaryPlayer._side);
        StartCoroutine(DelayGetTeleportPositions(teleportTime));
    }
    IEnumerator DelayGetTeleportPositions(float _teleportTime = 3 / 60f)
    {
        teleportTime = _teleportTime;
        yield return new WaitForSeconds(teleportTime);
        centerPos = new TeleportPoint();
        centerPos.SetPositionPos("Center_TP");
        if (_trainingCoverImage == null)
        {
            _trainingCoverImage = GameObject.Find("Versus_ImageCover").GetComponent<Image>();
        }
        StartCoroutine(TeleportTweenController(centerPos._leftSidePos, centerPos._rightSidePos));
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
        coverTweenSequence.Append(_trainingCoverImage.DOFade(1f, teleportTime));
        coverTweenSequence.OnComplete(() =>
        {
            mainPlayer.transform.position = pos1;
            secondaryPlayer.transform.position = pos2;
            teleported = true;
        });
        coverTweenSequence = null;

        yield return new WaitUntil(() => teleported);
        yield return new WaitForSeconds(teleportTime);
        _trainingCoverImage.DOFade(0f, teleportTime);
        teleporting = false;
        GameManager.instance._hitstopController.CameraController.ToggleWallState(true);
    }
    public override void TogglePauseMenu()
    {
        base.TogglePauseMenu();
        if (_pauseMenu.activeInHierarchy)
        {
            _eventSystem.SetSelectedGameObject(_pauseMenu.GetComponent<VersusMenu_Controller>().ReturnTopButton().gameObject);
        }
        else
        {
            _eventSystem.firstSelectedGameObject = null;
        }
        GameManager.instance.RoundSystemController.StateMachine.GetCurrentState().OnGamePause(!_pauseMenu.activeInHierarchy);
    }
}
