using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public abstract partial class AbstractButtonEvents
{
    protected void UnregisterInsideCallBackAllButtons()
    {
        Unregister(_insideButtons, _insideCallbacks, _isInsideButtonsSubscribed);
    }

    protected void UnregisterCallBackButton(ref bool isSubscrible , Button button , EventCallback<ClickEvent> callback)
    {
        if (isSubscrible)
        {
            if (button != null & callback != null) button?.UnregisterCallback(callback);
            isSubscrible = false;
        }
    }

    protected EventCallback<ClickEvent> SubscribeOnButton(ref bool isSubscrible , Button button , EventCallback<ClickEvent> callback)
    {
        if (button != null && !isSubscrible)
        {
                button.RegisterCallback(callback);
                isSubscrible = true;
        }

        return callback;
    }

    private void Register(Button[] buttons, EventCallback<ClickEvent>[] callbacks, bool[] isButtonsSubscribed)
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            EventCallback<ClickEvent> callback = callbacks[i];

            if (callback != null)
            {
                Button button = buttons[i];
                bool isSubscrible = isButtonsSubscribed[i] = false;
                SubscribeOnButton(ref isSubscrible, button, callback);
            }

        }
    }

    private void Unregister(Button[] buttons, EventCallback<ClickEvent>[] callbacks, bool[] isButtonsSubscribed)
    {
        if (buttons != null)
        {
            for (int i = 0; i < buttons.Length; i++)
            {
                EventCallback<ClickEvent> callback = callbacks[i];
                Button button = buttons[i];

                if (callback != null & button != null)
                {
                    var isSubscrible = isButtonsSubscribed[i];
                    UnregisterCallBackButton(ref isSubscrible, button, callback);
                }

            }
        }
    }

    private bool GetStatus(bool[] _isButtonsSubscribed, int index)
    {
        if (_isButtonsSubscribed != null)
        {
            if (ArrayUtils.IsValidIndexArray(_isButtonsSubscribed, index)) return _isButtonsSubscribed[index];
        }

        return false;
    }
}
