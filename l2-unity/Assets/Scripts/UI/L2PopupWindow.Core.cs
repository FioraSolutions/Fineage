using System;
using UnityEngine;
using UnityEngine.UIElements;

public abstract partial class L2PopupWindow : L2Window
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
            AudioPacketServices.Audio.PlayUISound("window_close");
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
                ToolTipSimplePacketServices.Window.BringToFront();
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
                ToolTipSimplePacketServices.Window.BringToFront();
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
}
