using FightingGame_FrameData;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
public class Character_InputDetection : MonoBehaviour
{
    [SerializeField] private Character_Base _base;
    [SerializeField] private Dictionary<Vector2, int> moveAxes;
    private int lastNum;
    public bool isTerrified;
    private void Start()
    {
        SetupMoveAxes();
    }
    void SetupMoveAxes()
    {
        lastNum = 5;
        moveAxes = new Dictionary<Vector2, int>();
        int startingValue = 1;
        for (int yAxis = -1; yAxis < 2; yAxis++) 
        {
            for (int xAxis = -1; xAxis < 2; xAxis++)
            {
                moveAxes.Add(new Vector2(xAxis,yAxis),startingValue);
                startingValue++;
            }
        }
    }
    public void ReceiveInput()
    {
        if(_base._subState != Character_SubStates.Controlled) { return; }

        try
        {
            _base.xVal = _base.player.GetAxisRaw(_base.moveAxes[4].Button_Element.actionId);
            _base.yVal = _base.player.GetAxisRaw(_base.moveAxes[5].Button_Element.actionId);
        }
        catch (ArgumentOutOfRangeException) { return; }
     
        int _terrifierMultiplier = isTerrified ? -1 : 1;
        _base.xVal = (_base.xVal >= _base.controllerYield.positiveXYield) ? 1 : ((_base.xVal <= -_base.controllerYield.negativeXYield) ? -1 : 0);
        _base.yVal = (_base.yVal >= _base.controllerYield.positiveYYield) ? 1 : ((_base.yVal <= -_base.controllerYield.negativeXYield) ? -1 : 0);
        _base.numpadVector.x = _base.xVal * _terrifierMultiplier;
        _base.numpadVector.y = _base.yVal * _terrifierMultiplier;

        ConvertVector(_base.numpadVector);
    }
    async void ConvertVector(Vector2 moveAxis)
    {
        int _outputValue = -1;
        if (moveAxes.TryGetValue(moveAxis, out _outputValue)) 
        {
            _base.numpadValue = _outputValue;
            _base.widget.GetAxisLocation(_base.numpadValue);
            _base._timer.UpdateInputLogger(_base.moveAxes[0]);
            if (lastNum != _base.numpadValue)
            {
                lastNum = _base.numpadValue;
            }
            await SendDirectionInputToCombo();
        }
    }
    async Task SendDirectionInputToCombo() 
    {
        float TwoFrameDelay = (2 * Base_FrameCode.ONE_FRAME);
        int delayInMS = (int)(TwoFrameDelay * 1000f);
        await Task.Delay(TimeSpan.FromSeconds(TwoFrameDelay));
        _base.ReturnInputValue(_base.numpadValue);
    }
}
