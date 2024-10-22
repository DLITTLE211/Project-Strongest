using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_InputTimer_Mobility : Character_InputTimer
{
    [SerializeField] private Character_Base _base;
    #region Function Summary
    /// <summary>
    /// Sets timer to starting value and sends broadcast to reset mobility based input checks
    /// </summary>
    /// <returns></returns>
    #endregion
    public void ResetTimer()
    {
        FrameCountTimer = StartFrameCountTimer;
        CheckForInput = false;
        _base._cComboDetection.ResetMobilityString();
    }
    #region Function Summary
    /// <summary>
    /// Sets starting values for this timer
    /// </summary>
    /// <returns></returns>
    #endregion
    public void SetStartingValues()
    {
        StartFrameCountTimer = 0.25f;
        FrameCountTimer = StartFrameCountTimer;
        CheckForInput = false;
    }
    #region Function Summary
    /// <summary>
    /// On successful input, sets timer to starting value
    /// </summary>
    /// <returns></returns>
    #endregion
    public void ResetTimerSuccess()
    {
        FrameCountTimer = StartFrameCountTimer;
    }
    // Update is called once per frame
    private void Update()
    {
        TimerTickDown();
    }
    #region Function Summary
    /// <summary>
    /// When CheckForInput is true, has the timer count down
    /// </summary>
    /// <returns></returns>
    #endregion
    public void TimerTickDown()
    {
        if (CheckForInput)
        {
            CountDownTimer();
        }
    }

    #region Function Summary
    /// <summary>
    /// Count down function for timer
    /// </summary>
    /// <returns></returns>
    #endregion
    public void CountDownTimer()
    {
        if (FrameCountTimer <= -1 / 60f)
        {
            ResetTimer();
        }
        else
        {
            FrameCountTimer -= 1 / 60f;
        }
    }
}
