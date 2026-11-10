using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

public partial class L2SlotManager
{
    private void DestroyRecipe()
    {
        if (_hoverSlot == null)
        {
            return;
        }

        SystemMessagePacketServices.Window.OnButtonOk += OkDestroyRecipe;
        SystemMessagePacketServices.Window.OnButtonClosed += OnРЎancelRecipe;
        SystemMessagePacketServices.Window.ShowWindowDialogYesOrNot("Are you sure you want to destroy the recipe?");

        int position = _hoverSlot.Position;
        TradingSlot sourceSlot = (TradingSlot)_draggedSlot;
        _recipeId = CraftPacketServices.RecipeBook.GetRecipeIdByPosition(sourceSlot.Position);

    }

    private void OkDestroyRecipe()
    {
        ZoneCommands.Send(new RequestRecipeBookDestroyCommand(_recipeId));
        CleanupMessageWindow();
    }

    private void OnРЎancelRecipe()
    {

        CleanupMessageWindow();
    }

    private void CleanupMessageWindow()
    {
        SystemMessageUtils.CancelEvent(SystemMessagePacketServices.Window, OkDestroyRecipe, OnРЎancelRecipe);
    }


    private void HandleSkillbarDrag()
    {
        if (SkillbarPacketServices.Skillbar.Locked)
        {
            return;
        }

        if (_hoverSlot == null)
        {
            if (!SystemUIPacketServices.Ui.MouseOverUI)
            {
                RemoveSkillbarSlot();
            }

            return;
        }

        switch (_hoverSlot.Type)
        {
            case SlotType.SkillBar:
                MoveSkillbarSlot();
                break;
            case SlotType.Enchant:
                int oldSlot = _draggedSlot.Id;
                int newSlot = _hoverSlot.Id;
                // Debug.LogWarning($"Moving skillbar enchant from slot {oldSlot} to slot {newSlot}.");
                break;
            default:
                if (!SystemUIPacketServices.Ui.MouseOverUI)
                {
                    RemoveSkillbarSlot();
                }
                break;
        }
    }

    #region SlotVerification
    private bool IsValidDrag() => _draggedSlot != null && (_hoverSlot != null || !SystemUIPacketServices.Ui.MouseOverUI);

    private bool IsSameSlot() => _draggedSlot != null && _hoverSlot != null && _draggedSlot.Position == _hoverSlot.Position && _draggedSlot.Type == _hoverSlot.Type;

    private bool IsItemGear(ItemCategory category)
    {
        return category == ItemCategory.Weapon
            || category == ItemCategory.ShieldArmor
            || category == ItemCategory.Jewel;
    }

    #endregion

    #region DragActions

    private void Unequip()
    {
        // Unequip logic
        GearSlot slot = (GearSlot)_draggedSlot;
        // Debug.Log($"Unequip {slot.Id}.");
        slot.UseItem();
    }

    private void Equip()
    {
        // Equip logic
        InventorySlot slot = (InventorySlot)_draggedSlot;
        // Debug.Log($"Equip {slot.Id}.");
        slot.UseItem();
    }

    private enum PendingItemOp
    {
        None,
        Destroy,
        Drop
    }

    private PendingItemOp _pendingOp = PendingItemOp.None;
    private Vector3 _pendingDropWorldPos;
    private bool _hasPendingDropPos;

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

    /// <summary>Mouse ray → world hit (same idea as ClickManager move pick).</summary>
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

    private InventorySlot _slotTemp = null;
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

    private void AddActionToSkillbar()
    {
        int actionId = ((ActionSlot)_draggedSlot).ActionId;
        int slot = _hoverSlot.Position;
        // Debug.LogWarning($"Add action ID {actionId} into skillbar slot {slot}.");

        ShortcutPacketServices.Shortcuts.AddShortcut(slot, actionId, Shortcut.TYPE_ACTION , 0);
    }

    private void ChangeItemOrder()
    {
        int fromSlot = _draggedSlot.Position;
        int toSlot = _hoverSlot.Position;

        // Debug.Log($"Moving item from slot {fromSlot} to slot {toSlot}.");

        InventoryPacketServices.Inventory.ChangeItemOrder(fromSlot, toSlot);
    }

    private void AddItemToSkillbar()
    {
        int itemId = ((InventorySlot)_draggedSlot).ObjectId;
        int slot = _hoverSlot.Position;
        // Debug.LogWarning($"Add item {itemId} to skillbar slot {slot}.");

        ShortcutPacketServices.Shortcuts.AddShortcut(slot, itemId, Shortcut.TYPE_ITEM, 0);
    }


    private void AddItemToEnchant()
    {
        int _objectId = ((InventorySlot)_draggedSlot).ObjectId;
        int slot = _hoverSlot.Position;
        // Debug.LogWarning($"Add _objectId {_objectId} to EnchantWindow slot {slot}.");
        ItemEnchantPacketServices.Enchant.AddEnchantItem(_objectId);
    }

    private void MoveSkillbarSlot()
    {
        int oldSlot = _draggedSlot.Position;
        int newSlot = _hoverSlot.Position;
        // Debug.LogWarning($"Moving skillbar shortcut from slot {oldSlot} to slot {newSlot}.");
        ShortcutPacketServices.Shortcuts.MoveShortcut(oldSlot, newSlot);
    }

    private void RemoveSkillbarSlot()
    {
        int oldSlot = _draggedSlot.Position;
        // Debug.LogWarning($"Renoving skillbar shortcut from slot {oldSlot}.");
        ShortcutPacketServices.Shortcuts.DeleteShortcut(oldSlot , true);
    }

    #endregion

    private void ResetSlots()
    {
        _draggedSlot = null;
        _hoverSlot = null;
    }

    public override void ShowWindow()
    {
        if (_windowEle == null)
            return;
        _windowEle.style.display = DisplayStyle.Flex;
        _windowEle.style.opacity = 1;
    }

    public override void HideWindow()
    {
        if (_windowEle == null)
            return;
        _windowEle.style.opacity = 0;
        _windowEle.style.display = DisplayStyle.None;
        _windowEle.style.left = -10000;
        _windowEle.style.top = -10000;
    }
}

