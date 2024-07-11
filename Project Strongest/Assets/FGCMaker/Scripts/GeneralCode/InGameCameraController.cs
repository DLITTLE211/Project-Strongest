using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class InGameCameraController : MonoBehaviour
{
    [SerializeField] private float smoothTime;
    [SerializeField] private float minWidth;
    [SerializeField] private float maxWidth;
    [SerializeField] private float wallBoundBias;

    private float size;
    private Vector3 velocity;
    [SerializeField] float zPos;

    [SerializeField] Camera orthoCamera, perspectiveBGCamera;
    [SerializeField] Transform[] playerCharacters;
    [SerializeField] Vector3 offset;


    private void Start()
    {
        InitCameraInformation();
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
        minWidth = 1.35f;
        maxWidth = 1.65f;
        smoothTime = 0.215f;
    }
    private void LateUpdate()
    {
        if (!checkCenterPoint())
        {
            ZoomCamera();
            CameraToPlayerCenter(orthoCamera);
            CameraToPlayerCenter(perspectiveBGCamera);
        }
    }
    #region Function Summary
    /// <summary>
    /// Adjusts Camera Zoom and positioning based on players positions
    /// </summary>
    /// <returns></returns>
    #endregion
    void ZoomCamera()
    {
        if (GreatestDistance() >= (minWidth - 1f) && GreatestDistance() <= (maxWidth + 2f))
        {
            float lastVal = ((int)(GreatestDistance() * 10) / 10f);
            float intBound = ((int)(size * 10) / 10f);
            if (distanceCheckMax(GreatestDistance()))
            {
                if (intBound != lastVal)
                {
                    size = GreatestDistance();
                    orthoCamera.DOOrthoSize(maxWidth, 2f);
                }
            }
            if (distanceCheckMin(GreatestDistance()))
            {
                if (intBound != lastVal)
                {
                    size = GreatestDistance();
                    orthoCamera.DOOrthoSize(minWidth, 2f);
                }
            }
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
        bool check = size >= 3.15f && size <= 3.75f;
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
        bool check = size >= 2.55f && size <= 3.15f;
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

        this.transform.position = Vector3.SmoothDamp(transform.position, newPos,ref velocity, smoothTime);
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


}
