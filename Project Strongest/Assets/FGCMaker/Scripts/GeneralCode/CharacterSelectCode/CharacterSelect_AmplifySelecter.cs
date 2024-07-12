using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

[System.Serializable]
public class CharacterSelect_AmplifySelecter :MonoBehaviour
{
    public TMP_Text chosenAmplifierText;
    public Image LeftBumperImage;
    public Image RightBumperImage;
    private List<Amplifiers> totalAmplifiers;
    public Amplifiers chosenAmplifier;
    int curAmplifier;
    public void GetListOfAmplifiers(List<Amplifiers> _totalAmplifiers)
    {
        totalAmplifiers = _totalAmplifiers;
        curAmplifier = 0;
        SetInfo(totalAmplifiers[curAmplifier]);
    }
    public void UpdateInfoDown()
    {
        if (curAmplifier <= 0)
        {
            curAmplifier = totalAmplifiers.Count - 1;
        }
        else { curAmplifier--; }
        SetInfo(totalAmplifiers[curAmplifier]);
    }
    public void UpdateInfoUp()
    {
        if (curAmplifier >= totalAmplifiers.Count - 1)
        {
            curAmplifier = 0;
        }
        else { curAmplifier++; }
        SetInfo(totalAmplifiers[curAmplifier]);
    }

    public void SetInfo(Amplifiers curAmplifier)
    {
        chosenAmplifier = curAmplifier;
        chosenAmplifierText.text = curAmplifier.amplifier.ToString();
        chosenAmplifier = curAmplifier;
    }

    public void SetAmplifyInfo()
    {
        chosenAmplifierText.DOFade(1f, 0f);
        LeftBumperImage.DOFade(1f, 0f);
        RightBumperImage.DOFade(1f, 0f);
    }
    public void ClearAmplifyInfo()
    {
        chosenAmplifierText.DOFade(0f, 1.5f);
        LeftBumperImage.DOFade(0f, 1.5f);
        RightBumperImage.DOFade(0f, 1.5f);
    }
}
