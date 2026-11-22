using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using VContainer;

public partial class PlayerInventory
{
    private void SetupMessageWindow()
    {
        SystemMessagePacketServices.Window.OnButtonOk += OkUse;
        SystemMessagePacketServices.Window.OnButtonClosed += OnCancel;
        SystemMessagePacketServices.Window.ShowWindowDialogYesOrNot("Component registration is not reversible, do you want to continue?");
    }

    private bool TryGetItem(int objectId, out ItemInstance item)
    {
        item = null;
        if (!Book.Contains(objectId))
        {
            Debug.LogError("[Inventory] use objectId " + objectId + " is not in the inventory");
            return false;
        }
        item = FindByAllInventory(objectId);
        if (item != null)
            return true;
        Debug.LogError("[Inventory] use objectId " + objectId + " is in the book but has no view");
        return false;
    }

    private void OkUse()
    {
        SendUseItemPacket(_objectIdEtcType);
        CleanupMessageWindow();
    }

    private void OnCancel()
    {
        _objectIdEtcType = -1;
        CleanupMessageWindow();
    }

    private void CleanupMessageWindow()
    {
        SystemMessageUtils.CancelEvent(SystemMessagePacketServices.Window, OkUse, OnCancel);
    }

    public void DestroyItem(int objectId, int quantity)
    {
        if (!Book.Contains(objectId))
        {
            Debug.LogError("[Inventory] destroy objectId " + objectId + " is not in the inventory");
            return;
        }

        IZoneTransport game = ZoneCommands.Transport;
        if (game == null)
        {
            Debug.LogError("[Inventory] destroy objectId " + objectId + " was not sent; zone transport is down");
            return;
        }
        ZoneCommands.Send(new RequestDestroyItemCommand(objectId, quantity));
    }

    public void DropItem(int objectId, int quantity, Vector3? dropWorldPos = null)
    {
        if (!Book.Contains(objectId))
        {
            Debug.LogError("[Inventory] drop objectId " + objectId + " is not in the inventory");
            return;
        }

        IZoneTransport game = ZoneCommands.Transport;
        if (game == null || DomainPacketServices.LocalPlayer == null)
            return;

        Transform player = DomainPacketServices.LocalPlayer.transform;
        Vector3 dropPos = dropWorldPos ?? player.position + player.forward * 0.6f;

        Vector3 l2 = VectorUtils.ConvertPosUnityToL2j(dropPos);
        // Debug.Log($"DropItem obj={objectId} qty={quantity} world={dropPos} l2=({(int)l2.x},{(int)l2.y},{(int)l2.z})");
        ZoneCommands.Send(new RequestDropItemCommand(
            objectId,
            quantity,
            (int)l2.x,
            (int)l2.y,
            (int)l2.z));
    }

    public bool IsContaineInventory(int objectId)
    {
        return Book.Contains(objectId);
    }
}
