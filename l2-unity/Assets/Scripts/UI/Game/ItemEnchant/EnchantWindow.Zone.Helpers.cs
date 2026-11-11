using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using static L2Slot;
using VContainer;

public partial class EnchantWindow
{
    public override void HideWindow()
    {
        if (_isWindowHidden)
            return;

        bool cancel = _zoneWire && !_closingFromServer;
        _zoneWire = false;
        _awaitingRoll = false;
        _scrollObjectId = 0;
        _selectItem = null;
        base.HideWindow();
        if (!cancel)
            return;

        IZoneTransport game = ZoneCommands.Transport;
        if (game != null)
            ZoneCommands.Send(new RequestEnchantItemCommand(-1));
    }

    void WireFrameClose()
    {
        if (_frameCloseWired || _windowEle == null)
            return;
        VisualElement frameClose = GetElementByClass("btn-close-frame");
        if (frameClose == null)
            return;
        frameClose.RegisterCallback<MouseUpEvent>(evt => OnCancel(null));
        _frameCloseWired = true;
    }

    void SetWindowTitle(string title)
    {
        if (_windowEle == null)
            return;
        Label label = _windowEle.Q<Label>("windows-name-label");
        if (label != null)
            label.text = title;
    }

    private void RequestEnchantItem(int objectId)
    {
        //if (_selectItem != null)
        //{
            ZoneCommands.Send(new RequestEnchantItemCommand(objectId));
        //}
    }
}
