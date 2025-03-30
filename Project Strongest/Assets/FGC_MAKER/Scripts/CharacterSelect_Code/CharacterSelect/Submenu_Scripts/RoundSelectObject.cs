using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;
using System;

public class RoundSelectObject : MonoBehaviour
{
    [SerializeField] private Transform _roundSelectTransform;
    [SerializeField] private Image leftImage,rightImage;
    [SerializeField] private TMP_Text _roundCountText;
    [SerializeField] private float activatedHeight, deactivatedHeight;
    public bool toggleReady;
    public void Activate() 
    {
        _roundSelectTransform.DOLocalMoveY(activatedHeight,0.25f).OnStart(() => FadeAssets(1f));
    }
    public void Deactivate()
    {
        _roundSelectTransform.DOLocalMoveY(deactivatedHeight, 0.25f).OnStart(() => FadeAssets(0f));
    }

    private void FadeAssets(float value) 
    {
        if(value == 0f) 
        { toggleReady = false; }

        leftImage.DOFade(value, 0.15f);
        rightImage.DOFade(value, 0.15f);
        _roundCountText.DOFade(value, 0.15f).OnComplete(() => { toggleReady = value == 1f; });
    }
    public void SetRoundText(string fullMessage) 
    {
        _roundCountText.text = SpriteToTextColorUtility.AppendSpriteName(fullMessage, _roundCountText.color);
    }
}
