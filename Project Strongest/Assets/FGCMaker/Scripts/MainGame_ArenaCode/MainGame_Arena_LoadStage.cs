using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

public class MainGame_Arena_LoadStage : MonoBehaviour
{
    [SerializeField] private Transform _stageLoadingLocation;
    [SerializeField] private Player_SideManager _sideManager;
    public void LoadStage(Stage_StageAsset stageAsset) 
    {
        GameObject newStage = Instantiate(stageAsset.stagePrefab, _stageLoadingLocation.transform);
        newStage.gameObject.transform.localPosition = Vector3.zero;
        newStage.gameObject.transform.localRotation = Quaternion.identity;
        newStage.gameObject.transform.localScale = Vector3.one;
        _sideManager.LeftWall.WallTransform = newStage.GetComponent<Stage_StageAsset>().LeftWall;
        _sideManager.RightWall.WallTransform = newStage.GetComponent<Stage_StageAsset>().RightWall;
        _sideManager.LeftWall.wallIgnore = newStage.GetComponent<Stage_StageAsset>().LeftWall.GetComponent<StageWallColliderIgnore>();
        _sideManager.RightWall.wallIgnore = newStage.GetComponent<Stage_StageAsset>().RightWall.GetComponent<StageWallColliderIgnore>();
    }
}
