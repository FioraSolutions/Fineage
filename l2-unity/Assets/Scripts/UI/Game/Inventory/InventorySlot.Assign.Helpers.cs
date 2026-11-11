
using UnityEngine;
using UnityEngine.UIElements;

public partial class InventorySlot
{
    public void AssignItem(ItemInstance item)
    {
        // A shortcut can point at an objectId the bag no longer holds (stack used up before the bar
        // refreshed). Rendering an empty cell is right; dereferencing the missing row is not.
        if (item == null)
        {
            AssignEmpty();
            return;
        }

        _slotElement.RemoveFromClassList("empty");

        AddDataItem(item);
        _itemInstance = item;
        _count = item.Count;
        _objectId = item.ObjectId;
        _enchantLevel = item.EnchantLevel;
        _id = item.ItemId;
        _remainingTime = item.RemainingTime;
        _empty = false;
        RefreshEnchantMark();

        if (_slotElement != null)
        {
            AddImage(this);
            AddTooltip(item);
            _slotDragManipulator.enabled = true;
        }
        else
        {
            // Debug.Log("Не критическая ошибка не смогли найти InventorySlot>SlotElement");
        }
    }

    public void AssignItem(ItemInstance item , float alpha)
    {
        _slotElement.RemoveFromClassList("empty");

        AddDataItem(item);
        _itemInstance = item;
        _count = item.Count;
        _objectId = item.ObjectId;
        _id = item.ItemId;
        _remainingTime = item.RemainingTime;
        _empty = false;
        _enchantLevel = item.EnchantLevel;
        RefreshEnchantMark();

        if (_slotElement != null)
        {
            AddImageAlpha(this, alpha);
            AddTooltip(item);
            _slotDragManipulator.enabled = true;
        }
        else
        {
            // Debug.Log("Не критическая ошибка не смогли найти InventorySlot>SlotElement");
        }
    }

    private void AddDataItem(ItemInstance item)
    {
        if (item.ItemData != null)
        {
            _id = item.ItemData.Id;
            _name = NetEnchant.WithLevel(item.ItemData.ItemName.DisplayName, item.EnchantLevel);
            _description = item.ItemData.ItemName.Description;
            _icon = item.ItemData.Icon;
            _objectId = item.ObjectId;

            
            _itemCategory = item.Category;
        }
        else
        {
            // Debug.LogWarning($"Item data is null for item {item.ItemId}.");
            _id = 0;
            _name = "Unkown";
            _description = "Unkown item.";
            _icon = "";
            _objectId = -1;
            _itemCategory = ItemCategory.Item;
        }

    }

    private void AddImage(InventorySlot slot)
    {
        if (SlotBg == null && _slotElement == null)
            return;

        // Paint on SlotBg so gear icons sit in Inventory_Back silhouette frames
        // (root paint sat top-left / floated off the wells).
        if (_slotElement != null)
            L2SlotIconView.Clear(_slotElement);
        L2SlotIconView.SetInventoryStyleItemIcon(
            SlotBg,
            _slotElement,
            _id,
            paintOnRootElement: SlotBg == null);
    }

    private void AddImageAtIndex(InventorySlot slot, int indexIamge)
    {
        if (SlotBg == null && _slotElement == null)
            return;

        if (_slotElement != null)
            L2SlotIconView.Clear(_slotElement);
        L2SlotIconView.SetInventoryStyleItemIconAtIndex(
            SlotBg,
            _slotElement,
            _id,
            indexIamge,
            paintOnRootElement: SlotBg == null);
    }

    private void AddImageAlpha(InventorySlot slot, float alpha)
    {
        AddImage(slot);
    }

    private void AddTooltip(ItemInstance item)
    {
        if (_tooltipManipulator != null)
            _tooltipManipulator.SetText(L2ItemSlotTooltip.Format(_name, _count, item.Category));
    }
}
