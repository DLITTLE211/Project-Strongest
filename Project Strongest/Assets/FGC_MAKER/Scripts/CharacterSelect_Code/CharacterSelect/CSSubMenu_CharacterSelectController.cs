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
    [SerializeField] private CSSubMenu_AmplifyPageController _AmplifyPageController;
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
        objectHolder.SetActive(false);
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
        if(characterSelectHolder.transform.childCount > 0)
        {
            EnableControllerButtonNavigation(); 
            return; 
        }
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
            csButton.ClearPlayerCursorCount();
            csButton.HighlightSelection(null);
        }
        _topHeaderText.DOFade(1f, 0.15f);
        _bottomHeaderText.DOFade(1f, 0.15f).OnComplete(() =>
        {
            SetHeaderText(_topHeaderText, "Choose Your");
            SetHeaderText(_bottomHeaderText, "Character");
        });
        EnableControllerButtonNavigation();
    }
    public void EnableControllerButtonNavigation()
    {
        for (int i = 0; i < activeCharacterSelectButtons.Count; i++)
        {
            CharacterSelect_Button csButton = activeCharacterSelectButtons[i].GetComponent<CharacterSelect_Button>();
            csButton.HighlightSelection(null);
        }
        for (int i = 0; i < _playerCursors.Count; i++)
        {
            int buttonStartIndex = ((activeCharacterSelectButtons.Count / 2) + _playerCursors[i].ID) - 1;
            _playerCursors[i].ClearHighlightedButton();
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
    public ChosenCharacter GetLeftPlayerProfile()
    {
        CharacterSelect_Cursor curCursor = _playerCursors[0];
        if (curCursor.cursorPage.chosenCharacter != null)
        {
            int colorIndex = curCursor.cursorPage.colorSelectIndex;
            ChosenCharacter leftPlayerCharacter = new ChosenCharacter(curCursor.cursorPage.chosenCharacter, curCursor.cursorPage.chosenAmplifier, curCursor.ChosenPlayerSide, colorIndex, Character_SubStates.Controlled);

            return leftPlayerCharacter;
        }
        return RandomizeChoice(_characterSelect.player1, curCursor);
    }
    public ChosenCharacter GetRightPlayerProfile()
    {
        CharacterSelect_Cursor curCursor = _playerCursors[1];
        int colorIndex = curCursor.cursorPage.colorSelectIndex;
        if (_playerCursors[0].cursorPage.chosenCharacter == curCursor.cursorPage.chosenCharacter)
        {
            colorIndex = curCursor.cursorPage.colorSelectIndex != _playerCursors[0].cursorPage.colorSelectIndex ? curCursor.cursorPage.colorSelectIndex : curCursor.cursorPage.colorSelectIndex + 1;
            if (colorIndex > curCursor.cursorPage.chosenCharacter._characterSkins.ColorSets.Count - 1)
            {
                colorIndex = 0;
            }
        }
        if (curCursor.cursorPage.chosenCharacter != null)
        {
            ChosenCharacter rightPlayerCharacter = new ChosenCharacter(curCursor.cursorPage.chosenCharacter, curCursor.cursorPage.chosenAmplifier, curCursor.ChosenPlayerSide, colorIndex, Character_SubStates.Controlled);
            return rightPlayerCharacter;
        }
        return RandomizeChoice(_characterSelect.player2, curCursor);
    }
    public ChosenCharacter RandomizeChoice(ChooseSide_Object chosenSide, CharacterSelect_Cursor cursorObject)
    {
        int randomProfile = UnityEngine.Random.Range(0, _activeProfiles.Count - 1);
        int randomAmplifier = UnityEngine.Random.Range(0, _AmplifyPageController.ActiveAmplifiers.Count - 1);
        int colorIndex = _playerCursors[1].cursorPage.colorSelectIndex;
        if (_playerCursors[0].cursorPage.chosenCharacter == _activeProfiles[randomProfile])
        {
            colorIndex = cursorObject.cursorPage.colorSelectIndex != _playerCursors[0].cursorPage.colorSelectIndex ? cursorObject.cursorPage.colorSelectIndex : cursorObject.cursorPage.colorSelectIndex + 1;
            if (colorIndex >= _activeProfiles[randomProfile]._characterSkins.ColorSets.Count - 1)
            {
                colorIndex = 0;
            }
        }
        if (chosenSide.sideIterator == 1)
        {
            ChosenCharacter _randomizedCharacter = new ChosenCharacter(_activeProfiles[randomProfile], _AmplifyPageController.ActiveAmplifiers[randomAmplifier], -1, colorIndex);
            return _randomizedCharacter;
        }
        else
        {
            ChosenCharacter _randomizedCharacter = new ChosenCharacter(_activeProfiles[randomProfile], _AmplifyPageController.ActiveAmplifiers[randomAmplifier], cursorObject.ChosenPlayerSide, colorIndex, Character_SubStates.Controlled);
            return _randomizedCharacter;
        }
    }
}
