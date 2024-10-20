using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[System.Serializable]
public class SingleScene
{
    public Camera sceneCamera;
    public enum SceneType 
    { 
        Idle,
        SweepLeft, 
        SweepRight, 
        ZoomIn, 
        ZoomOut, 
        Circle, 
        Rising, 
        Lowering 
    }
    public SceneType sceneType;

    public Vector3 defaultPosition;
    public Transform focalPoint;
    public Transform playerTarget;
    public Transform cameraTransform;
    public Vector3 cameraEndPosition;
    public Vector3 setCamPos;
    public Quaternion startCamRotation;
    public float startZoom;
    public float targetRotation;

    public float panSpeed,spinSpeed;
    public bool sceneComplete,scenePlaying;
    public void PlayScene(SceneType type)
    {
        switch (type)
        {
            case SceneType.Idle:
                //Do Nothing
                break;
            case SceneType.SweepLeft:
                PanLeft(-1);
                break;
            case SceneType.SweepRight:
                PanLeft(1);
                break;
            case SceneType.ZoomIn:
                ZoomIn(-1);
                break;
            case SceneType.ZoomOut:
                ZoomIn(1);
                break;
            case SceneType.Circle:
                Rotate(targetRotation);
                break;
            case SceneType.Rising:
                PanUp(1);
                break;
            case SceneType.Lowering:
                PanUp(-1);
                break;
            default:
                DebugMessageHandler.instance.DisplayErrorMessage(3, $"Invalid Scene Type {type}... cancelling action");
                break;
        }
    }
    public void SetCameraTransform() 
    {
        cameraTransform = sceneCamera.transform;
        defaultPosition = new Vector3(sceneCamera.transform.position.x, sceneCamera.transform.position.y, sceneCamera.transform.position.z);
        startCamRotation = Quaternion.Euler(cameraTransform.rotation.x, cameraTransform.rotation.y, cameraTransform.rotation.z);
        startZoom = sceneCamera.fieldOfView;
    }
    public void SetCameraPosition(Vector3 position, int xOffset = 0, int yOffset = 0) 
    {
        cameraTransform.position = new Vector3 (position.x + xOffset, position.y + yOffset, position.z);
    }
  
    public void SetCameraRotation(Quaternion rotation)
    {
        cameraTransform.localRotation = Quaternion.Euler(rotation.x,rotation.y,rotation.z);
    }

    #region Left/Right Panning
    public bool CheckCameraXPosition(float multCheck)
    {
        if (multCheck == 1)
        {
            return cameraTransform.position.x >= cameraEndPosition.x;
        }
        else
        {
            return cameraTransform.position.x <= cameraEndPosition.x;
        }
    }
    public void PanLeft(float multiplier) 
    {
        switch (multiplier) 
        {
            case -1:
                if (!scenePlaying)
                {
                    scenePlaying = true;
                    SetcamPosRight();
                    SetCameraPosition(setCamPos);
                }
                if (!CheckCameraXPosition(multiplier))
                {
                    cameraTransform.DOMoveX(cameraEndPosition.x,panSpeed).OnComplete(() =>
                    {
                        Messenger.Broadcast(Events.SceneComplete);
                    });
                }
                break;
            case 1:
                if (!scenePlaying)
                {
                    scenePlaying = true;
                    SetcamPosLeft();
                    SetCameraPosition(setCamPos);
                }
                if (!CheckCameraXPosition(multiplier))
                {
                    cameraTransform.DOMoveX(cameraEndPosition.x, multiplier * panSpeed).OnComplete(() =>
                    {
                        Messenger.Broadcast(Events.SceneComplete);
                    });
                    
                }
                break;
        }
   
    }
    void SetcamPosLeft() 
    {
        setCamPos = new Vector3 (playerTarget.position.x - 2, 1.33f, cameraTransform.position.z);
        cameraEndPosition = new Vector3(playerTarget.position.x + 2, 1.33f, cameraTransform.position.z);
    }
    void SetcamPosRight()
    {
        setCamPos = new Vector3(playerTarget.position.x + 2, 1.33f, cameraTransform.position.z);
        cameraEndPosition = new Vector3(playerTarget.position.x - 2, 1.33f, cameraTransform.position.z);
    }
    #endregion

    #region Up/Down Panning
    public bool CheckCameraYPosition(float multCheck)
    {
        if (multCheck == 1)
        {
            return cameraTransform.position.y >= cameraEndPosition.y;
        }
        else
        {
            return cameraTransform.position.y <= cameraEndPosition.y;
        }
    }
    public void PanUp(float multiplier)
    {
        switch (multiplier)
        {
            case -1:
                if (!scenePlaying)
                {
                    scenePlaying = true;
                    SetcamPosDown();
                    SetCameraPosition(setCamPos);
                }
                if (!CheckCameraYPosition(multiplier))
                {
                    cameraTransform.DOMoveY(cameraEndPosition.y, panSpeed).OnComplete(() =>
                    {
                        Messenger.Broadcast(Events.SceneComplete);
                    });
                }
                break;
            case 1:
                if (!scenePlaying)
                {
                    scenePlaying = true;
                    SetcamPosUp();
                    SetCameraPosition(setCamPos);
                }
                if (!CheckCameraYPosition(multiplier))
                {
                    cameraTransform.DOMoveY(cameraEndPosition.y, multiplier * panSpeed).OnComplete(() =>
                    {
                        Messenger.Broadcast(Events.SceneComplete);
                    });
                }
                break;
        }

    }
    void SetcamPosUp()
    {
        setCamPos = new Vector3(0, playerTarget.position.y - 2, cameraTransform.position.z);
        cameraEndPosition = new Vector3(0, playerTarget.position.y + 2, cameraTransform.position.z);
    }
    void SetcamPosDown()
    {
        setCamPos = new Vector3(0, playerTarget.position.y + 2, cameraTransform.position.z);
        cameraEndPosition = new Vector3(0, playerTarget.position.y - 2, cameraTransform.position.z);
    }
    #endregion

    #region Zoom Shot
    public bool CheckCameraZoom(float multCheck)
    {
        if (multCheck == 1)
        {
            return sceneCamera.fieldOfView <= startZoom;
        }
        else
        {
            return sceneCamera.fieldOfView <= sceneCamera.fieldOfView - 15;
        }
    }
    public void ZoomIn(float multiplier)
    {
        switch (multiplier)
        {
            case -1:
                //Zoom In
                if (!scenePlaying)
                {
                    scenePlaying = true;
                    SetStartZoom();
                }
                if (!CheckCameraZoom(multiplier))
                {
                    sceneCamera.DOFieldOfView(sceneCamera.fieldOfView - 15, panSpeed).OnComplete(() =>
                    {
                        scenePlaying = false;
                        ZoomIn(1);
                    });
                }
                break;
            case 1:
                //Zoom Out
                if (!scenePlaying)
                {
                    scenePlaying = true;
                }
                if (CheckCameraZoom(multiplier))
                {
                    sceneCamera.DOFieldOfView(startZoom, panSpeed + 0.5f).OnComplete(() =>
                    {
                        Messenger.Broadcast(Events.SceneComplete);
                    });
                }
                break;
        }

    }
    void SetStartZoom()
    {
        sceneCamera.fieldOfView = startZoom;
    }

    #endregion

    #region Rotating Shot
    public bool CheckCameraRotation(float targetRotation)
    {
        return cameraTransform.rotation.y == targetRotation;
    }
    public void Rotate(float targetRotation)
    {
        //Rotate To Y angle
        if (!scenePlaying)
        {
            scenePlaying = true;
            SetRotation();
        }
        if (!CheckCameraRotation(targetRotation))
        {
            cameraTransform.DORotate(new Vector3(0, targetRotation, 0), panSpeed).OnComplete(() =>
            {
                Messenger.Broadcast(Events.SceneComplete);
            });
        }
    }
    void SetRotation()
    {
        cameraTransform.rotation = startCamRotation;
    }
    #endregion
}
