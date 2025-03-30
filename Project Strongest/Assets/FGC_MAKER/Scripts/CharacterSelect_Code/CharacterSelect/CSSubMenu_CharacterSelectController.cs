using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using FightingGame_FrameData;

public class CSSubMenu_CharacterSelectController : CharacterSelect_SubMenuBase
{
    [SerializeField] private List<Character_Profile> _activeProfiles;
    [SerializeField] private List<GameObject> activeCharacterSelectButtons;
    [SerializeField] private GameObject characterSelectButtonPrefab;
    [SerializeField] private GameObject characterSelectHolder;
    Vector3 standardButtonSize;
    Vector3 largeButtonSize;
    protected override void ActivateSubMenu()
    {
        standardButtonSize = new Vector3(0.7f, 0.7f, 0.7f);
        largeButtonSize = new Vector3(1f, 1f, 1f);
        characterSelectHolder.SetActive(true);
        AddCharacterSelectButtons();
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
        for (int i = 0; i < activeCharacterSelectButtons.Count; i++)
        {
            Vector3 selectButtonFirstSize = largeButtonSize;
            Sequence sizingSequence = DOTween.Sequence();
            sizingSequence.Append(activeCharacterSelectButtons[i].transform.DOScale(selectButtonFirstSize, 0.1f));
            sizingSequence.Append(activeCharacterSelectButtons[i].transform.DOScale(standardButtonSize, 0.05f));
            sizingSequence.Play();
            activeCharacterSelectButtons[i].GetComponent<CharacterSelect_Button>().SetPosition();
            yield return new WaitForSeconds(Base_FrameCode.ONE_FRAME*1.15f);
        }
        _topHeaderText.DOFade(1f, 0.15f);
        _bottomHeaderText.DOFade(1f, 0.15f).OnComplete(() =>
        {
            SetHeaderText(_topHeaderText, "Choose Your");
            SetHeaderText(_bottomHeaderText, "Character");
        });
    }
}
