using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

public partial class L2SlotManager
{
    private void DropItem()
    {
        InventorySlot slot = (InventorySlot)_draggedSlot;
        _slotTemp = slot;
        _pendingOp = PendingItemOp.Drop;
        _hasPendingDropPos = TryPickDropWorldPos(out _pendingDropWorldPos);
        if (!_hasPendingDropPos)
            Debug.LogWarning("DropItem: no ground under cursor, will fallback near player.");

        if (slot.Count > 1)
        {
            PauseAndShowQuantity(slot.Count);
            return;
        }

        SystemMessagePacketServices.Window.OnButtonOk += OkDrop;
        SystemMessagePacketServices.Window.OnButtonClosed += OnCancel;
        SystemMessagePacketServices.Window.ShowWindowDialogYesOrNot("Do you want to drop your " + slot.Name + "?");
    }

    private static bool TryPickDropWorldPos(out Vector3 worldPos)
    {
        worldPos = default;
        Camera cam = Camera.main;
        if (cam == null)
            return false;

        Ray ray = cam.ScreenPointToRay(PollInput.MousePosition);
        const float pickDistance = 10000f;
        World world = WorldApplyPacketServices.GameWorld;
        int groundMask = world != null ? world.GroundMask : 0;
        if (groundMask != 0 && Physics.Raycast(ray, out RaycastHit groundHit, pickDistance, groundMask))
        {
            worldPos = groundHit.point;
            return true;
        }

        if (!Physics.Raycast(ray, out RaycastHit hit, pickDistance))
            return false;

        worldPos = hit.point;
        return true;
    }

    private void DestroyItem()
    {
        // Destroy item logic
        InventorySlot slot = (InventorySlot)_draggedSlot;
        _slotTemp = slot;
        _pendingOp = PendingItemOp.Destroy;
        if (slot.Count > 1)
        {
            PauseAndShowQuantity(slot.Count);
        }
        else
        {
            SystemMessagePacketServices.Window.OnButtonOk += OkDestroy;
            SystemMessagePacketServices.Window.OnButtonClosed += OnCancel;
            SystemMessagePacketServices.Window.ShowWindowDialogYesOrNot("Do you want to destroy your " + slot.Name);
            
        }
    }

    private void OkDestroy()
    {
        InventoryPacketServices.Inventory.DestroyItem(_slotTemp.ObjectId, 1);
        CancelEvent();
    }

    private void OkDrop()
    {
        if (_slotTemp != null && InventoryPacketServices.Inventory != null)
            SendDrop(_slotTemp.ObjectId, 1);
        CancelEvent();
    }

    public void OnCancel()
    {
        CancelEvent();
    }

    private void CancelEvent()
    {
        SystemMessagePacketServices.Window.OnButtonOk -= OkDestroy;
        SystemMessagePacketServices.Window.OnButtonOk -= OkDrop;
        SystemMessagePacketServices.Window.OnButtonClosed -= OnCancel;
        _pendingOp = PendingItemOp.None;
        _hasPendingDropPos = false;
    }

    private void SendDrop(int objectId, int quantity)
    {
        Vector3? worldPos = _hasPendingDropPos ? _pendingDropWorldPos : (Vector3?)null;
        InventoryPacketServices.Inventory.DropItem(objectId, quantity, worldPos);
        _hasPendingDropPos = false;
    }

    private void PauseAndShowQuantity(int count)
    {
        QuantityInputPacketServices.Window.ShowWindow(count);
        QuantityInputPacketServices.Window.OnButtonOk += OnSelectedQuantity;
    }

    private void OnSelectedQuantity(string value)
    {
        int qty = int.Parse(value);
        if (_slotTemp != null && InventoryPacketServices.Inventory != null)
        {
            if (_pendingOp == PendingItemOp.Drop)
                SendDrop(_slotTemp.ObjectId, qty);
            else
                InventoryPacketServices.Inventory.DestroyItem(_slotTemp.ObjectId, qty);
        }
        QuantityInputPacketServices.Window.OnButtonOk -= OnSelectedQuantity;
        _pendingOp = PendingItemOp.None;
    }
}
