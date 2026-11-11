using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static L2Slot;
using Debug = UnityEngine.Debug;
using VContainer;

public partial class DealerWindow
{
    private void SendServer(INetworkCommand packet)
    {
        ZoneCommands.Send(packet);
    }

    private void OkWear()
    {
        SendServer(new RequestPreviewItemCommand(_listId, _listSell));
        CancelEvent();
    }

    private void OnРЎancel()
    {
        _listSell.Clear();
        _listId = 0;
        CancelEvent();
    }

    private void CancelEvent()
    {
        SystemMessagePacketServices.Window.OnButtonOk -= OkWear;
        SystemMessagePacketServices.Window.OnButtonClosed -= OnРЎancel;
    }

    protected override IEnumerator BuildWindow(VisualElement root)
    {
        InitWindow(root);

        yield return new WaitForEndOfFrame();

        var dragArea = GetElementByClass("drag-area");

    }
}
