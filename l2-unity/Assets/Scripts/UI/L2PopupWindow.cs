using System;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class L2PopupWindow : L2Window
{
    protected void RegisterCloseWindowEvent(string closeButtonClass)
    {
        if (GetElementByClass(closeButtonClass) is Button closeButton)
            WireCloseButton(closeButton);
    }

    protected void RegisterCloseWindowEventByName(string closeButtonName)
    {
        if (GetElementById(closeButtonName) is Button closeButton)
            WireCloseButton(closeButton);
    }

    void WireCloseButton(Button closeButton)
    {
        closeButton.AddManipulator(new ButtonClickSoundManipulator(closeButton));
        closeButton.clicked += () =>
        {
            AudioManager.Instance.PlayUISound("window_close");
            HideWindow();
        };
    }

    public void RegisterClickWindowEvent(VisualElement windowEle, VisualElement dragEle)
    {
        if (windowEle != null)
        {
            windowEle.RegisterCallback<MouseDownEvent>(evt =>
            {

                BringToFront();
                ToolTipSimple.Instance.BringToFront();
            }, TrickleDown.TrickleDown);
        }

        if (dragEle != null)
        {
            dragEle.RegisterCallback<MouseDownEvent>(evt =>
            {
                BringToFront();
            }, TrickleDown.TrickleDown);
        }
    }

    public void UnRegisterClickWindowEvent(VisualElement windowEle, VisualElement dragEle)
    {
        if (windowEle != null)
        {
            windowEle.UnregisterCallback<MouseDownEvent>(evt =>
            {

                BringToFront();
                ToolTipSimple.Instance.BringToFront();
            }, TrickleDown.TrickleDown);
        }

        if (dragEle != null)
        {
            dragEle.UnregisterCallback<MouseDownEvent>(evt =>
            {
                BringToFront();
            }, TrickleDown.TrickleDown);
        }
    }
    public override void ShowWindow()
    {
        base.ShowWindow();
        BringToFront();
    }

    public override void HideWindow()
    {
        base.HideWindow();
    }

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
        catch (Exception ex)
        {
            // Debug.LogException(ex);
        }
    }
    public void RefreshOpacity(float opacity)
    {
        _windowEle.style.opacity = opacity;
    }


}
