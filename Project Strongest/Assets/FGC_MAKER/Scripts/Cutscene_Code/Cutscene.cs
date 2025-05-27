using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Cutscene : MonoBehaviour
{
    public List<SingleScene> cutscene;
    [SerializeField]
    private int cutsceneIterator;

    private void Start()
    {
        SetupCutscene();
        //Messenger.AddListener(Events.PlayCutscene, playScene);
        Messenger.AddListener(Events.SceneComplete, UpdateSceneIterator);
        DOTween.Init();
    }

    public void SetupCutscene() 
    {
        for(int i = 0; i < cutscene.Count; i++) 
        {
            cutscene[i].SetCameraTransform();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            playScene();
        }
    }
    public void UpdateSceneIterator()
    {
        cutscene[cutsceneIterator].scenePlaying = false;
        cutscene[cutsceneIterator].sceneComplete = true;
        cutsceneIterator++;
        
    }
    public void playScene() 
    {
        if (cutsceneIterator == 0)
        {
            if (cutscene[cutsceneIterator].scenePlaying == false)
            {
                cutscene[cutsceneIterator].PlayScene(cutscene[cutsceneIterator].sceneType);
            }
        }
        else 
        {
            if (cutscene[cutsceneIterator - 1].sceneComplete == true)
            {
                cutscene[cutsceneIterator].PlayScene(cutscene[cutsceneIterator].sceneType);
            }
        }
    }
}
