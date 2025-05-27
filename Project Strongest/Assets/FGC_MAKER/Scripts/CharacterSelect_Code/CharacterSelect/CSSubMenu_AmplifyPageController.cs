using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;
using DG.Tweening;

public class CSSubMenu_AmplifyPageController : CharacterSelect_SubMenuBase
{
    [SerializeField] private List<Amplifiers> _activeAmplifiers;
    public List<Amplifiers> ActiveAmplifiers { get { return _activeAmplifiers; } }
    [SerializeField] private List<AmplifyController_Object> amplifierObjects;
    private void OnEnable()
    {
        objectHolder.SetActive(false);
        for (int i = 0; i < amplifierObjects.Count; i++) 
        {
            amplifierObjects[i].DeactivateInstant();
        }
    }

    public void Activate() 
    {
        objectHolder.SetActive(true);
        ActivateSubMenu();
    }
    public void Deactivate()
    {
        DeactivateSubMenu();
    }
    protected override void ActivateSubMenu()
    {
        for (int i = 0; i < amplifierObjects.Count; i++) 
        {
            amplifierObjects[i].gameObject.SetActive(true);
            amplifierObjects[i].Activate();
            amplifierObjects[i].UpdateAmplifier(_activeAmplifiers[0], 0, true);
        }
        _topHeaderText.DOFade(1f, 0.15f);
        _bottomHeaderText.DOFade(1f, 0.15f).OnComplete(() => 
        {
            SetHeaderText(_topHeaderText,"Select Your");
            SetHeaderText(_bottomHeaderText, "Amplifier");
        });
    }
    protected override void DeactivateSubMenu()
    {
        for (int i = 0; i < amplifierObjects.Count; i++)
        {
            amplifierObjects[i].Deactivate();
        }
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
    public void CyclePlayerAmplifierUp(int playerIndex = 0)
    {
        if (amplifierObjects[playerIndex].AmplifierIndex >= _activeAmplifiers.Count - 1)
        {
            if (amplifierObjects[playerIndex].ToggleReady)
            {
                amplifierObjects[playerIndex].AmplifierIndex = 0;
                amplifierObjects[playerIndex].UpdateAmplifier(_activeAmplifiers[amplifierObjects[playerIndex].AmplifierIndex], 0);
                return;
            }
        }
        if (amplifierObjects[playerIndex].ToggleReady)
        {
            amplifierObjects[playerIndex].AmplifierIndex++;
            amplifierObjects[playerIndex].UpdateAmplifier(_activeAmplifiers[amplifierObjects[playerIndex].AmplifierIndex], amplifierObjects[playerIndex].AmplifierIndex);
        }
    }
    public void CyclePlayerAmplifierDown(int playerIndex = 0)
    {
        if (amplifierObjects[playerIndex].AmplifierIndex <= 0)
        {
            if (amplifierObjects[playerIndex].ToggleReady)
            {
                amplifierObjects[playerIndex].AmplifierIndex = _activeAmplifiers.Count - 1;
                amplifierObjects[playerIndex].UpdateAmplifier(_activeAmplifiers[amplifierObjects[playerIndex].AmplifierIndex], _activeAmplifiers.Count - 1);
                return;
            }
        }
        if (amplifierObjects[playerIndex].ToggleReady)
        {
            amplifierObjects[playerIndex].AmplifierIndex--;
            amplifierObjects[playerIndex].UpdateAmplifier(_activeAmplifiers[amplifierObjects[playerIndex].AmplifierIndex], amplifierObjects[playerIndex].AmplifierIndex);
        }
    }
}
