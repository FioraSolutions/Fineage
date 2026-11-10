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
    public void BeginZoneSelect(ItemInstance scroll)
    {
        _zoneWire = true;
        _scrollObjectId = scroll != null ? scroll.ObjectId : 0;
        _selectItem = null;
        if (_enchant1 == null || scroll == null)
            return;

        ShowWindow(scroll);
        if (_textLabel != null)
            _textLabel.text = "Select an item to enchant.";
        SetWindowTitle("Enchant");
    }

    public void ApplyZoneResult(NetEnchantResult result)
    {
        NetEnchant.View view = NetEnchant.ViewOf(result);
        if (view.RemoveItem || view.ShowEnchant || view.CloseWindow)
            CloseFromServer();
    }

    public void AbortZone()
    {
        if (!_zoneWire)
            return;
        CloseFromServer();
    }

    void CloseFromServer()
    {
        _closingFromServer = true;
        HideWindow();
        _closingFromServer = false;
    }

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
