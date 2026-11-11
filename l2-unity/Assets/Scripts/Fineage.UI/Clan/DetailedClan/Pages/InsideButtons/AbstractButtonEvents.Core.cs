using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public abstract partial class AbstractButtonEvents
{
    protected bool GetStatusCallBack(int index){

        return GetStatus(_isButtonsSubscribed, index);
    } 

    protected bool GetStatusInsideCallBack(int index)
    {
        return GetStatus(_isInsideButtonsSubscribed, index);
    }

    protected void RegisterCallBackAllButtons(Button[] buttons , EventCallback<ClickEvent>[] callbacks)
    {
        _buttons = buttons;
        _callbacks = callbacks;
        _isButtonsSubscribed = new bool[_buttons.Length];

        Register(_buttons, _callbacks, _isButtonsSubscribed);
    }

    protected void RegisterInsideCallBackAllButtons(Button[] buttons, EventCallback<ClickEvent>[] callbacks)
    {

        ClearDataInside();

        _insideButtons = buttons;
        _insideCallbacks = callbacks;
        _isInsideButtonsSubscribed = new bool[_buttons.Length];

        Register(_insideButtons, _insideCallbacks, _isInsideButtonsSubscribed);

    }

    private void ClearDataInside()
    {
        _insideButtons = null;
        _insideCallbacks = null;
        _isInsideButtonsSubscribed = null;
    }

    protected void UnregisterCallBackAllButtons()
    {
        Unregister(_buttons, _callbacks, _isButtonsSubscribed);
    }
}
