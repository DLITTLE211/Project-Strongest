using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "StageSelect/Stage")]
public class Stage_StageAsset : ScriptableObject
{
    public string stageName;
    public Sprite stageImage;
    public GameObject stagePrefab;
    public GameObject LeftWall,RightWall;

}
