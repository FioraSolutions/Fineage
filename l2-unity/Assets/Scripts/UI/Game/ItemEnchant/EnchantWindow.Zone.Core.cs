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
}
