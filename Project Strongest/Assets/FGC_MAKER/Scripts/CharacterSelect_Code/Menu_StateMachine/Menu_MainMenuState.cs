using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using DG.Tweening;
using System.Threading.Tasks;

public class Menu_MainMenuState : Menu_BaseState
{
    [SerializeField] private GameObject _mainMenuHolder;
    [SerializeField] private CharacterSelect_Setup _characterSelect;
    [SerializeField] private MenuButtonHolder FirstMenuButtonLayer;
    [SerializeField] private Image _backgroundImage, whiteFillerImage;
    [SerializeField] private EventSystem _eventSystem;
    [SerializeField] private TMP_Text _titleText, _versionText;
    IEnumerator closingRoutine;
    // Start is called before the first frame update
    public override void OnEnter()
    {
        _backgroundImage.DOColor(Color.black, 0f);
        _mainMenuHolder.SetActive(true);
        ActivateMainMenu();
        if(closingRoutine != null) 
        {
            StopCoroutine(closingRoutine);
            closingRoutine = null;
        }
    }
    IEnumerator DelayCloseMainMenu() 
    {
        DeactivateMainMenu();
        yield return new WaitForSeconds(1.35f);
        _mainMenuHolder.SetActive(false);
        Character_AvailableID _players = Menu_Manager.instance.players;
        GameModeSet _gameModeSet = Menu_Manager.currentMode;
        _characterSelect.SetUpCharacterSelectScreen(_players, _gameModeSet);
    }
    public override void OnExit()
    {
        closingRoutine = DelayCloseMainMenu();
        StartCoroutine(closingRoutine);

    }
    public override void OnUpdate()
    {

    }
    void ActivateMainMenu() 
    {
        FirstMenuButtonLayer.SetImageObject(_backgroundImage);
        FirstMenuButtonLayer.EnableButtons();
        FirstMenuButtonLayer.SlideHolderIn(SetActiveButton);
        _titleText.DOFade(255f, 1.5f);
        _versionText.DOFade(255f, 1.5f);
        Transform _titleTextTransform = _titleText.transform;
        float moveUpPos = _titleTextTransform.localPosition.y - 225f;
        _titleTextTransform.DOLocalMoveY(moveUpPos, 1.5f).SetEase(Ease.InOutBack);
        _backgroundImage.DOFade(1f, 1.5f);
        whiteFillerImage.DOFade(1f, 1.5f);
    }
    void DeactivateMainMenu()
    {
        FirstMenuButtonLayer.DisableButtons();
        FirstMenuButtonLayer.DisableButtons();
        _titleText.DOFade(0, 1.5f);
        _versionText.DOFade(0, 1.5f);
        FirstMenuButtonLayer.SlideHolderOut();
        Transform _titleTextTransform = _titleText.transform;
        float moveUpPos = _titleTextTransform.localPosition.y + 225f;
        _titleTextTransform.DOLocalMoveY(moveUpPos, 1.5f);
        _backgroundImage.DOFade(0, 1.5f);
        whiteFillerImage.DOFade(0, 1.5f);
    }
    void SetActiveButton() 
    {
        _eventSystem.firstSelectedGameObject = FirstMenuButtonLayer.buttonList[0].gameObject;
        FirstMenuButtonLayer.buttonList[0].Select();
    }
}
