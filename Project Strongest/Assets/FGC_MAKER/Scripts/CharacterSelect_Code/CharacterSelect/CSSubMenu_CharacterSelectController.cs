using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using FightingGame_FrameData;
using UnityEngine.EventSystems;
using Rewired;

public class CSSubMenu_CharacterSelectController : CharacterSelect_SubMenuBase
{
    public List<CharacterSelect_Cursor> _playerCursors;
    [SerializeField] private EventSystem _eventSystem;
    [SerializeField] private List<Character_Profile> _activeProfiles;
    [SerializeField] private List<GameObject> activeCharacterSelectButtons;
    [SerializeField] private GameObject characterSelectButtonPrefab;
    [SerializeField] private GameObject characterSelectHolder;
    Vector3 standardButtonSize;
    Vector3 largeButtonSize;
    private void OnEnable()
    {
        objectHolder.SetActive(false);
    }
    public void Activate()
    {
        objectHolder.SetActive(true);
        ActivateSubMenu();
    }
    protected override void ActivateSubMenu()
    {
        standardButtonSize = new Vector3(0.7f, 0.7f, 0.7f);
        largeButtonSize = new Vector3(0.85f, 0.85f, 0.85f);
        characterSelectHolder.SetActive(true);
        AddCharacterSelectButtons();
    }
    public void Deactivate()
    {
        DeactivateSubMenu();
    }
    protected override void DeactivateSubMenu()
    {
        characterSelectHolder.SetActive(false);
        _topHeaderText.DOFade(0f, 0.15f);
        _bottomHeaderText.DOFade(0f, 0.15f);
    }
    private void Update()
    {
        if (allowBase) 
        {
            base.OnUpdate();
        }
    }
    public void AddCharacterSelectButtons()
    {
        for (int i = 0; i < _activeProfiles.Count; i++)
        {
            GameObject selectButton = Instantiate(characterSelectButtonPrefab, characterSelectHolder.transform);
            selectButton.gameObject.transform.localPosition = new Vector3(1, 1, 1);
            selectButton.gameObject.transform.localRotation = Quaternion.identity;
            selectButton.gameObject.transform.localScale = standardButtonSize;
            Button characterIconImage = selectButton.GetComponentInChildren<Button>();
            characterIconImage.image.sprite = _activeProfiles[i].CharacterSelectIcon;
            characterIconImage.image.DOFade(1f, 0f);
            if (_activeProfiles[i].characterModel != null) 
            {
                characterIconImage.image.color = Color.white;
                characterIconImage.interactable = true;
            }
            else 
            {
                characterIconImage.image.color = Color.black;
                characterIconImage.interactable = false;
            }
            selectButton.name = $"{_activeProfiles[i].CharacterName}_CSButton_{i}";
            GameObject _selectButtonInfo = selectButton;
            _selectButtonInfo.GetComponent<CharacterSelect_Button>().characterProfile = _activeProfiles[i];
            activeCharacterSelectButtons.Add(_selectButtonInfo);
        }
        StartCoroutine(CascadeScaleSelectButtons());
    }
    IEnumerator CascadeScaleSelectButtons()
    {
        _topHeaderText.text = "";
        _bottomHeaderText.text = "";
        for (int i = 0; i < activeCharacterSelectButtons.Count; i++)
        {
            Vector3 selectButtonFirstSize = largeButtonSize;
            Sequence sizingSequence = DOTween.Sequence();
            Transform buttonTransform = activeCharacterSelectButtons[i].transform;
            sizingSequence.Append(buttonTransform.DOScale(selectButtonFirstSize, 0.1f));
            sizingSequence.Append(buttonTransform.DOScale(standardButtonSize, 0.05f));
            sizingSequence.Play();
            CharacterSelect_Button csButton = activeCharacterSelectButtons[i].GetComponent<CharacterSelect_Button>();
            csButton.SetPosition();
            yield return new WaitForSeconds(Base_FrameCode.ONE_FRAME*1.15f);
        }
        for(int i = 0; i < activeCharacterSelectButtons.Count; i++)
        {
            CharacterSelect_Button csButton = activeCharacterSelectButtons[i].GetComponent<CharacterSelect_Button>();
            csButton.SetNavTransforms();
            csButton.HighlightSelection(null);
        }
        _topHeaderText.DOFade(1f, 0.15f);
        _bottomHeaderText.DOFade(1f, 0.15f).OnComplete(() =>
        {
            SetHeaderText(_topHeaderText, "Choose Your");
            SetHeaderText(_bottomHeaderText, "Character");
        });
        for (int i = 0; i < _playerCursors.Count; i++)
        {
            int buttonStartIndex = ((activeCharacterSelectButtons.Count / 2) + _playerCursors[i].ID) - 1;
            CharacterSelect_Button currentButton = activeCharacterSelectButtons[buttonStartIndex].GetComponent<CharacterSelect_Button>();
            if (_playerCursors[i].isConnected)
            {
                _playerCursors[i].AllowInitialChange();
                _playerCursors[i].SetHighlightedButton(currentButton);
                currentButton.HighlightSelection(_playerCursors[i]);
            }
        }
    }
    public void SetPlayerControllers()
    {
        if (ReInput.controllers.GetJoystickNames().Length <= 0)
        {
            return;
        }
        else
        {
            for(int i = 0; i < ReInput.controllers.GetJoystickNames().Length; i++) 
            {
                CharacterSelect_Cursor curCursor = _playerCursors[i];
                if (curCursor.curPlayer == null) 
                {
                    SetCharacterSelectCursorState(curCursor, i);
                }
                else 
                {
                    curCursor.UnlockCharacterChoice();
                    curCursor.isConnected = true;
                    curCursor.cursorPage.ClearInfo();
                }
            }
        }
    }
    void SetCharacterSelectCursorState(CharacterSelect_Cursor player, int ID)
    {
        player.curPlayer = ReInput.players.GetPlayer(_characterSelect.players.UsedID.Item1[ID]);
        player.ID = ID;
        player.curPlayer.controllers.AddController(ControllerType.Joystick, _characterSelect.players.UsedID.Item1[ID], true);
        player.curPlayer.controllers.maps.LoadMap(ControllerType.Joystick, _characterSelect.players.UsedID.Item1[ID], $"UI_CanvasController", $"TestPlayer{_characterSelect.players.UsedID.Item1[ID]}");
        if (_characterSelect.currentSet.gameMode == GameMode.Training)
        {
            player.cursorObject.SetActive(true);
            player.cursorText.text = $"{_characterSelect.players.UsedID.Item1[ID] + 1}";

        }
        player.isConnected = true;
    }
}
