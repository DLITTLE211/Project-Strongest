using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Player_SideManager : MonoBehaviour
{
    public WallIdentification LeftWall, RightWall;
    public Player_SideRecognition _p1Position;
    public Player_SideRecognition _p2Position;
    public HitPointCall sideCall;

    private void Start()
    {
        StartCoroutine(OnStartSideSwitch());
    }
    IEnumerator OnStartSideSwitch() 
    {
        yield return new WaitUntil(() => _p1Position.thisPosition.ModelTransform != null);
        yield return new WaitUntil(() => _p2Position.thisPosition.ModelTransform != null);
        SetStartingFaceState();
    }
    public void ForceSideSwitch(CustomCallback callback = null)
    {
        if (_p1Position._base.callSource) 
        {
            SwapPlayerSide(_p2Position._base);
        }
        else if (_p2Position._base.callSource)
        {
            SwapPlayerSide(_p1Position._base);
        }
        CheckPositionState();
    }
    void SwapPlayerSide(Character_Base target)
    {
        float newXPos = 0;
        if (target.pSide.thisPosition._directionFacing == Character_Face_Direction.FacingLeft)
        {
            newXPos = target.opponentPlayer.myRb.position.x - 1f;
        }
        else 
        {
            newXPos = target.opponentPlayer.myRb.position.x + 1f;
        }
        target.myRb.position = new Vector3(newXPos, target.myRb.position.y, target.myRb.position.z);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T)) 
        {
            ForceSideSwitch();
        }
    }
    public void CheckPlayerPositions() 
    {
        if (_p1Position != null && _p2Position != null)
        {
            if (_p1Position.gameObject.activeSelf)
            {
                _p1Position.thisPosition.UpdatePlayerFacingDirection(LeftWall.WallTransform, RightWall.WallTransform);
            }
            if (_p2Position.gameObject.activeSelf)
            {
                _p2Position.thisPosition.UpdatePlayerFacingDirection(LeftWall.WallTransform, RightWall.WallTransform);
            }
            if (_p1Position.gameObject.activeSelf && _p2Position.gameObject.activeSelf)
            {
                CheckPositionState();
            }
        }
    }
    public void SetStartingFaceState()
    {
        _p1Position.thisPosition.SetFacingState(Character_Face_Direction.FacingRight);
        _p2Position.thisPosition.SetFacingState(Character_Face_Direction.FacingLeft);
    }
    public void UpdateFaceState(bool state)
    {
        if (state)
        {
            if (!CorrectLeftPlayerFacing(state))
            {
                _p1Position.thisPosition.SetFacingState(Character_Face_Direction.FacingRight);
                _p2Position.thisPosition.SetFacingState(Character_Face_Direction.FacingLeft);
            }
        }
        else
        {
            if (!CorrectLeftPlayerFacing(state))
            {
                _p1Position.thisPosition.SetFacingState(Character_Face_Direction.FacingLeft);
                _p2Position.thisPosition.SetFacingState(Character_Face_Direction.FacingRight);
            }
        }
    }
    public void CheckPositionState()
    {
        if (ReturnPlayerLeftDistance() && ReturnPlayerRightDistance())
        {
            UpdateFaceState(true);
        }
        else
        {
            UpdateFaceState(false);
        }
    }
    #region Bool Check Position
    bool ReturnPlayerLeftDistance()
    {
        return _p1Position.thisPosition.LW_Distance < _p2Position.thisPosition.LW_Distance;
    }
    bool ReturnPlayerRightDistance()
    {
        return _p1Position.thisPosition.RW_Distance > _p2Position.thisPosition.RW_Distance;
    }
    bool CorrectLeftPlayerFacing(bool state)
    {
        if (_p1Position.thisPosition._directionFacing == _p2Position.thisPosition._directionFacing) 
        {
            return false;
        }
        else 
        {
            if (state)
            {
                if (_p1Position.thisPosition._directionFacing == Character_Face_Direction.FacingLeft && _p2Position.thisPosition._directionFacing != Character_Face_Direction.FacingLeft)
                {
                    return false;
                }
            }
            else 
            {
                if (_p1Position.thisPosition._directionFacing == Character_Face_Direction.FacingRight && _p2Position.thisPosition._directionFacing != Character_Face_Direction.FacingRight)
                {
                    return false;
                }
            }
        }
        return true;
    }
    #endregion
}
[Serializable]
public class WallIdentification 
{
    public Transform WallTransform;
    public StageWallColliderIgnore wallIgnore;
}