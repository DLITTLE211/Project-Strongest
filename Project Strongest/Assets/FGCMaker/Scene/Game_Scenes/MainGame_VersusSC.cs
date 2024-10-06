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

    public override void SetTeleportPositions()
    {
        _pauseMenu.SetActive(false);
        _pauseMenu.GetComponent<VersusMenu_Controller>().SetupVersusButtons(_eventSystem);
        _eventSystem.firstSelectedGameObject = null;

        _pauseMenu.GetComponent<VersusMenu_Controller>().SetP1MoveListInformation(mainPlayer.comboList3_0, mainPlayer.characterProfile.CharacterName);
        _pauseMenu.GetComponent<VersusMenu_Controller>().SetP2MoveListInformation(secondaryPlayer.comboList3_0, secondaryPlayer.characterProfile.CharacterName);
        StartCoroutine(DelayGetTeleportPositions());
    }
    IEnumerator DelayGetTeleportPositions()
    {
        yield return new WaitForSeconds(3 / 60f);
        centerPos = new TeleportPoint();
        centerPos.SetPositionPos("Center_TP");
        _trainingCoverImage = GameObject.Find("Versus_ImageCover").GetComponent<Image>();
        StartCoroutine(TeleportTweenController(centerPos._leftSidePos, centerPos._rightSidePos));
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
