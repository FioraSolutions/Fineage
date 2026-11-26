

using NUnit.Framework.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public abstract partial class L2Window
{
    public bool IsWindowVisible => _windowEle != null && !_isWindowHidden;

    public bool ContainsScreenPoint(Vector2 screenPosition)
    {
        if (_windowEle == null || _isWindowHidden)
            return false;

        return L2UiPointer.PanelBoundsContain(_windowEle, screenPosition);
    }

    public virtual void HideWindow()
    {
        try
        {
            _isWindowHidden = true;
            _windowEle.style.display = DisplayStyle.None;
            _mouseOverDetection.Disable();
            TooltipLifecycle.HideIfOwnerClosed(_windowEle);
        }
        catch (Exception)
        {
            // Debug.LogException(ex);
        }

    }

    public virtual void OnlyHideWindow()
    {
        _windowEle.style.display = DisplayStyle.None;
        TooltipLifecycle.HideIfOwnerClosed(_windowEle);
    }

    public virtual void ShowWindow()
    {
        try
        {
            _isWindowHidden = false;
            _windowEle.style.display = DisplayStyle.Flex;
            _mouseOverDetection.Enable();
        }
        catch (Exception)
        {
            // Debug.LogException(ex);
        }

    }

    public virtual void ToggleHideWindow()
    {
        if (_isWindowHidden)
        {
            ShowWindow();
        }
        else
        {
            HideWindow();
        }
    }

    public virtual void BringToFront()
    {
    }

    public virtual void SendToBack()
    {
    }

}
