using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public partial class L2GameUI
{
    public void WindowOpened(L2PopupWindow popupWindow)
    {
        _openedWindows.Add(popupWindow);
    }

    public void WindowClosed(L2PopupWindow popupWindow)
    {
        _openedWindows.Remove(popupWindow);
    }

    public void RegisterWorldInputBlocker(L2PopupWindow popupWindow)
    {
        if (popupWindow == null || _worldInputBlockers.Contains(popupWindow))
            return;
        _worldInputBlockers.Add(popupWindow);
    }

    public void UnregisterWorldInputBlocker(L2PopupWindow popupWindow)
    {
        if (popupWindow == null)
            return;
        _worldInputBlockers.Remove(popupWindow);
    }

    public bool IsPointerOverBlockingPopup()
    {
        Vector2 mouse = PollInput.MousePosition;
        for (int i = _worldInputBlockers.Count - 1; i >= 0; i--)
        {
            L2PopupWindow popup = _worldInputBlockers[i];
            if (popup == null)
            {
                _worldInputBlockers.RemoveAt(i);
                continue;
            }

            if (popup.IsWindowVisible && popup.ContainsScreenPoint(mouse))
                return true;
        }

        return false;
    }

    public bool BlocksWorldPointerInput()
    {
        return MouseOverUI || IsPointerOverPanel() || IsPointerOverBlockingPopup();
    }

    public void OnGUI()
    {
        if (_mouseEnabled)
        {
            UnityEngine.Cursor.visible = true;
            UnityEngine.Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            UnityEngine.Cursor.visible = false;
            UnityEngine.Cursor.lockState = CursorLockMode.Confined;
        }
    }
}
