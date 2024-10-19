using System;
using UnityEngine;
using System.Threading.Tasks;
public class Character_InputDetection : MonoBehaviour
{
    [SerializeField] private Character_Base _base;
    private int lastNum;
    private void Start()
    {
        lastNum = 5;
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
        //Will need to be revised
        _base.xVal = (_base.xVal >= _base.controllerYield.positiveXYield) ? 1 : ((_base.xVal <= -_base.controllerYield.negativeXYield) ? -1 : 0);
        _base.yVal = (_base.yVal >= _base.controllerYield.positiveYYield) ? 1 : ((_base.yVal <= -_base.controllerYield.negativeXYield) ? -1 : 0);
        _base.numpadVector.x = _base.xVal;
        _base.numpadVector.y = _base.yVal;
        ConvertVector(_base.numpadVector);
    }
    async void ConvertVector(Vector2 vector)
    {
        string directionalInput = ($"{vector.x},{vector.y}");
        switch (directionalInput)
        {
            case "-1,-1":
                _base.numpadValue = 1;
                break;
            case "0,-1":
                _base.numpadValue = 2;
                break;
            case "1,-1":
                _base.numpadValue = 3;
                break;
            case "-1,0":
                _base.numpadValue = 4;
                break;
            case "0,0":
                _base.numpadValue = 5;
                break;
            case "1,0":
                _base.numpadValue = 6;
                break;
            case "-1,1":
                _base.numpadValue = 7;
                break;
            case "0,1":
                _base.numpadValue = 8;
                break;
            case "1,1":
                _base.numpadValue = 9;
                break;
            default:
                break;
        }
        _base.widget.GetAxisLocation(_base.numpadValue);
        if (lastNum != _base.numpadValue)
        {
            lastNum = _base.numpadValue;
            _base._timer.UpdateInputLogger(_base.moveAxes[0]);
        }
        await SendDirectionInputToCombo();
    }
    async Task SendDirectionInputToCombo() 
    {
        float TwoFrameDelay = (2 * (1 / 60f));
        int delayInMS = (int)(TwoFrameDelay * 1000f);
        await Task.Delay(delayInMS);
        _base.ReturnInputValue(_base.numpadValue);
    }
}
