using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using FightingGame_FrameData;

public class MainGame_CameraController : MonoBehaviour
{
    [SerializeField] private float smoothTime;
    [SerializeField] private float minWidth;
    [SerializeField] private float maxWidth;
    [SerializeField] private float wallBoundBias;
    [SerializeField] private Canvas _screenSpaceCanvas;

    [SerializeField] private float size;
    private Vector3 velocity;
    [SerializeField] float zPos;

    [SerializeField] Transform cameraObjectHolder;
    [SerializeField] Camera orthoCamera, perspectiveBGCamera;
    [SerializeField] Transform[] playerCharacters;
    [SerializeField] Vector3 offset;
    Vector3 startPos;
    [SerializeField] private GameObject camera_LeftWall,camera_RightWall;
    [SerializeField] private HitPointCall cameraControlCalls;
    [SerializeField] private bool isTracking;
    IEnumerator ShakeRoutine;
    Tween cameraZoomTween;
    private void Start()
    {
        InitCameraInformation();
    }
    private void Update()
    {
        if (isTracking)
        {
            if (!checkCenterPoint())
            {
                ZoomCamera();
                CameraToPlayerCenter(orthoCamera);
                CameraToPlayerCenter(perspectiveBGCamera);
            }
        }
    }
    #region Function Summary
    /// <summary>
    /// Initializes Camera Settings for tracking Players Positions
    /// </summary>
    /// <returns></returns>
    #endregion
    public void InitCameraInformation()
    {
        size = GreatestDistance();
        minWidth = 1.65f;
        maxWidth = 2.05f;
        smoothTime = 0.215f;
    }
    public void CallCameraShake(float duration, int intensity)
    {
        isTracking = false;
        if (ShakeRoutine != null)
        {
            StopCoroutine(ShakeRoutine);
            ShakeRoutine = null;
        }
        startPos = cameraObjectHolder.localPosition;
        ShakeRoutine = ShakeCamera(duration, intensity);
        StartCoroutine(ShakeRoutine);

    }
    public void ToggleWallState(bool state) 
    {
        camera_LeftWall.SetActive(state);
        camera_RightWall.SetActive(state);
    }
    IEnumerator ShakeCamera(float duration, int intensity)
    {
        float durationInFrames = (duration / 2) * Time.smoothDeltaTime;
        while (durationInFrames > 0)
        {
            if (GameManager.instance.settingsController._isPause)
            {
                yield return new WaitForSeconds(Time.smoothDeltaTime);
            }
            else
            {
                Shake(intensity);
                durationInFrames -= Time.smoothDeltaTime;
                yield return new WaitForSeconds(Time.smoothDeltaTime);
                cameraObjectHolder.localPosition = startPos;
            }
        }
        ShakeRoutine = null;
        isTracking = true;
    }
    void Shake(int intensity)
    {
        float xRange = intensity * 0.0001f;
        float yRange = intensity * 0.0005f;
        float r_Xpos = Random.Range(-xRange, xRange);
        float r_Ypos = Random.Range(-yRange, yRange);
        float offSetX = cameraObjectHolder.localPosition.x + r_Xpos;
        float offSetY = cameraObjectHolder.localPosition.y + r_Ypos;
        cameraObjectHolder.localPosition = new Vector3(offSetX, offSetY, 0f);
    }
    #region Function Summary
    /// <summary>
    /// Adjusts Camera Zoom and positioning based on players positions
    /// </summary>
    /// <returns></returns>
    #endregion
    void ZoomCamera()
    {
        size = GreatestDistance();
        if (size >= maxWidth) { size = maxWidth; }
        if (size <= minWidth) { size = minWidth; }
        if (distanceCheckMax(GreatestDistance()))
        {
            FadeZoom(maxWidth);
        }
        if (distanceCheckMin(GreatestDistance()))
        {
            FadeZoom(minWidth);
        }
    }
    void FadeZoom(float newSize)
    {
        if (orthoCamera.orthographicSize != newSize && cameraZoomTween == null)
        {
            cameraZoomTween = orthoCamera.DOOrthoSize(newSize, 50f * Time.smoothDeltaTime);
            cameraZoomTween.Play();
            cameraZoomTween.OnComplete(() =>
            {
                cameraZoomTween = null;
            });
        }
    }
    #region Function Summary
    /// <summary>
    /// Returns if distance between players is between max bound
    /// </summary>
    /// <returns></returns>
    #endregion
    bool distanceCheckMax(float size)
    {
        bool check = size > 2.75f && size <= 4.75f;
        return check;
    }

    #region Function Summary
    /// <summary>
    ///  Returns if distance between players is between min bound
    /// </summary>
    /// <returns></returns>
    #endregion
    bool distanceCheckMin(float size)
    {
        bool check = size >= 0.45f && size <= 1.95f;
        return check;
    }

    #region Function Summary
    /// <summary>
    /// Determines center between Players and sets Camera Position to midpoint
    /// </summary>
    /// <returns></returns>
    #endregion
    void CameraToPlayerCenter(Camera cam)
    {
        cam.transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y, zPos);
        Vector3 centerPoint = centerBetweenPlayers();
        Vector3 newPos = centerPoint + offset;

        this.transform.position = Vector3.SmoothDamp(transform.position, newPos, ref velocity, smoothTime);
    }


    #region Function Summary
    /// <summary>
    /// Returns the distance between players
    /// </summary>
    /// <returns></returns>
    #endregion
    public float GreatestDistance()
    {
        Bounds greatestDistance = new Bounds(playerCharacters[0].position, Vector3.zero);
        for (int i = 0; i < playerCharacters.Length; i++)
        {
            greatestDistance.Encapsulate(playerCharacters[i].position);
        }
        return greatestDistance.size.x;
    }
    #region Function Summary
    /// <summary>
    /// Returns centerpoint between players
    /// </summary>
    /// <returns></returns>
    #endregion
    Vector3 centerBetweenPlayers()
    {
        if (playerCharacters.Length == 1)
        {
            return playerCharacters[0].position;
        }
        else
        {
            Bounds centerPoint = new Bounds(playerCharacters[0].position, Vector3.zero);
            for (int i = 0; i < playerCharacters.Length; i++)
            {
                centerPoint.Encapsulate(playerCharacters[i].position);
            }
            return centerPoint.center;
        }
    }
    #region Function Summary
    /// <summary>
    /// Checks if camera is equal to centerpoint of both players
    /// </summary>
    /// <returns></returns>
    #endregion
    bool checkCenterPoint()
    {
        Vector3 comparison = centerBetweenPlayers();
        bool checkCenterX = (Mathf.Round(this.transform.position.x * 10) / 0.001f == Mathf.Round(comparison.x * 10) / 0.001f);
        bool checkCenterY = (Mathf.Round(this.transform.position.y * 10) / 0.001f == Mathf.Round(comparison.y * 10) / 0.001f);
        bool checkCenterZ = (Mathf.Round(this.transform.position.z * 10) / 0.01f == Mathf.Round(comparison.z * 10) / 0.01f);
        return checkCenterX && checkCenterY && checkCenterZ;
    }
    public void ResetCanvas()
    {
        _screenSpaceCanvas.worldCamera = orthoCamera;
    }
    public Vector3 ReturnCameraPos() 
    {
        return cameraObjectHolder.position;
    }
}
