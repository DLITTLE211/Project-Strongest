using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Player_SideManager : MonoBehaviour
{
    public Transform LeftWall, RightWall;
    public Player_SideRecognition _p1Position;
    public Player_SideRecognition _p2Position;

    private void Start()
    {
        SetStartingFaceState();
    }
    private void Update()
    {
        if (_p1Position.gameObject.activeSelf)
        {
            _p1Position.thisPosition.UpdatePlayerFacingDirection(LeftWall, RightWall);
        }
        if (_p2Position.gameObject.activeSelf)
        {
            _p2Position.thisPosition.UpdatePlayerFacingDirection(LeftWall, RightWall);
        }
        if (_p1Position.gameObject.activeSelf && _p2Position.gameObject.activeSelf)
        {
            CheckPositionState();
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
            if (_p1Position.thisPosition._directionFacing != Character_Face_Direction.FacingRight)
            {
                _p1Position.thisPosition.SetFacingState(Character_Face_Direction.FacingRight);
                _p2Position.thisPosition.SetFacingState(Character_Face_Direction.FacingLeft);
            }
        }
        else
        {
            if (_p1Position.thisPosition._directionFacing != Character_Face_Direction.FacingLeft)
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
    #endregion
}
