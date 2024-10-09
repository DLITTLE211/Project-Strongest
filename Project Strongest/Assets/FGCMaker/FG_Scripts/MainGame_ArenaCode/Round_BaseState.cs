using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;
using TMPro;

public class Round_BaseState 
{
    public MainGame_RoundSystemController _rSystem;
    public virtual void OnEnter()
    {
    }
    public virtual void OnExit() 
    {
    }
    public virtual void Update() {}
    public virtual void OnGamePause(bool state) { }
    public Round_BaseState(MainGame_RoundSystemController rSystem)
    {
        _rSystem = rSystem;
    }

    public void PulseAndSetText(TMP_Text textObject, string message) 
    {
        textObject.gameObject.SetActive(true);
        textObject.DOFade(0f, 0f);
        textObject.transform.DOScale(0f, 0f).OnComplete(() =>
        {
            textObject.DOFade(1f, 0.3f);
            textObject.text = message;
            textObject.transform.DOScale(1.15f, 0.3f).OnComplete(() =>
            {
                textObject.transform.DOScale(1f, 0.3f);
            });
        });
    }
}
