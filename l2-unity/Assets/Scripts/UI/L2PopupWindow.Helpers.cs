using System;
using UnityEngine;
using UnityEngine.UIElements;

public abstract partial class L2PopupWindow : L2Window
{
    public override void OnlyHideWindow()
    {
        base.OnlyHideWindow();
    }

    public override void BringToFront()
    {
        _windowEle.BringToFront();
    }

    public override void SendToBack()
    {
        _windowEle.SendToBack();
    }

    public void ShowWindowToCenter()
    {
        base.ShowWindow();
        OnCenterScreen(_root);
    }

    public void ShowWindowToCenterAndBringToFront()
    {
        base.ShowWindow();
        OnCenterScreen(_root);
        _windowEle.BringToFront();
    }

    public void ReplaceWindow(VisualTreeAsset template)
    {
        try
        {
            _root.Remove(_windowEle);
            _windowEle.RemoveManipulator(_mouseOverDetection);
            UnRegisterClickWindowEvent(_windowEle, null);

            _windowEle = template.Instantiate()[0];
            _mouseOverDetection = new MouseOverDetectionManipulator(_windowEle);
            _windowEle.AddManipulator(_mouseOverDetection);

            DisableEventOnOver(_windowEle);
            RegisterClickWindowEvent(_windowEle, null);

            if (_isWindowHidden)
            {
                _mouseOverDetection.Disable();
            }

            _isReg = false;
            _root.Add(_windowEle);
        }
        catch (Exception)
        {
            // Debug.LogException(ex);
        }
    }

    public void RefreshOpacity(float opacity)
    {
        _windowEle.style.opacity = opacity;
    }
}
