using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

public class MainGame_Arena_LoadStage : MonoBehaviour
{
    [SerializeField] private Transform _stageLoadingLocation;
    public void LoadStage(Stage_StageAsset stageAsset) 
    {
        GameObject selectButton = Instantiate(stageAsset.stagePrefab, _stageLoadingLocation.transform);
        selectButton.gameObject.transform.localPosition = Vector3.zero;
        selectButton.gameObject.transform.localRotation = Quaternion.identity;
        selectButton.gameObject.transform.localScale = Vector3.one;
    }
}
