using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CSSubMenu_CharacterSelectController : CharacterSelect_SubMenuBase
{
    [SerializeField] private List<Character_Profile> _activeProfiles;
    [SerializeField] private List<GameObject> activeCharacterSelectButtons;
    [SerializeField] private GameObject characterSelectButtonPrefab;
    [SerializeField] private GameObject characterSelectHolder;
    protected override void ActivateSubMenu()
    {
        AddCharacterSelectButtons();
    }
    protected override void DeactivateSubMenu()
    {

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
            selectButton.gameObject.transform.localScale = new Vector3(0.85f, 0.85f, 0.85f);
            selectButton.GetComponentInChildren<Button>().image.sprite = _activeProfiles[i].CharacterSelectIcon;
            selectButton.GetComponentInChildren<Button>().interactable = true;
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
            yield return new WaitForSeconds(0.05f);
            Vector3 selectButtonFirstSize = new Vector3(1.15f, 1.15f, 1.15f);
            activeCharacterSelectButtons[i].transform.DOScale(selectButtonFirstSize, 0.15f);
            yield return new WaitForSeconds(0.025f);
            activeCharacterSelectButtons[i].transform.DOScale(new Vector3(0.85f, 0.85f, 0.85f), 0.15f);
            activeCharacterSelectButtons[i].GetComponent<CharacterSelect_Button>().SetPosition();
        }
    }
}
