using System;

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
