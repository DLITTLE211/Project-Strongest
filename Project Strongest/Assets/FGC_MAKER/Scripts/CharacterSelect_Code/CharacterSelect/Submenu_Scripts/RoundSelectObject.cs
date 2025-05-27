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
    public void Activate(Callback func) 
    {
        _roundSelectTransform.DOLocalMoveY(activatedHeight,0.35f).OnStart(() => FadeAssets(1f, func));
    }
    public void Deactivate()
    {
        _roundSelectTransform.DOLocalMoveY(deactivatedHeight, 0.25f).OnStart(() => FadeAssets(0f));
    }

    private void FadeAssets(float value, Callback func = null) 
    {
        leftImage.DOFade(value, 0.15f);
        rightImage.DOFade(value, 0.15f);
        _roundCountText.DOFade(value, 0.15f).OnComplete(() => 
        {
            if(func != null) { func(); }
        });
    }
    public void SetRoundText(string fullMessage) 
    {
        _roundCountText.text = SpriteToTextColorUtility.AppendSpriteName(fullMessage.ToUpper(), _roundCountText.color);
    }
}
