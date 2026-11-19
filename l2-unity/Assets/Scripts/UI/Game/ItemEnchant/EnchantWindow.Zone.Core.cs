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
        AudioPacketServices.Audio.PlayUISound("window_open");
        if (_textLabel != null)
            _textLabel.text = "Select an item to enchant.";
        SetWindowTitle("Enchant");
    }

    public void ApplyZoneResult(NetEnchantResult result)
    {
        _awaitingRoll = false;
        NetEnchant.ResultAnim anim = NetEnchant.AnimOf(result.Result);
        if (anim == NetEnchant.ResultAnim.Close || anim == NetEnchant.ResultAnim.None)
        {
            CloseFromServer();
            return;
        }

        _pendingCloseFromServer = true;
        StartEnchantAnim(anim == NetEnchant.ResultAnim.Success);
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
