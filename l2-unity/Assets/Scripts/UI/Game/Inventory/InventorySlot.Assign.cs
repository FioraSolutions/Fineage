

public partial class InventorySlot
{
    public void AssignEmpty()
    {
        //_slotElement.AddToClassList("empty");
        _empty = true;
        _id = 0;
        _name = "Unkown";
        _description = "Unkown item.";
        _itemInstance = null;
        _product = null;
        _objectId = 0;
        _enchantLevel = 0;
        RefreshEnchantMark();
        if (_slotElement != null)
        {
            L2SlotIconView.PaintEmptyInventorySlot(_slotBg, _slotElement);
            if (_slotDragManipulator != null)
                _slotDragManipulator.enabled = false;
        }
    }

    public void RefreshPosition(int newPosition)
    {
        Position = newPosition;
    }

    public void AssignProduct(Product item)
    {
        _slotElement.RemoveFromClassList("empty");
        

        if (item != null)
        {
            _id = item.ItemId;
            _empty = false;
            _objectId = item.ObjId;
            _count = item.Count;
            _itemInstance = null;
        }
        else
        {
            _id = 0;
            _objectId = 0;
            _count = 0;
            _name = "Unkown";
            _description = "Unkown item.";
            _empty = true;
        }

        if (_slotElement != null)
        {
            if (_id > 0)
                L2SlotIconView.SetItemIcon(_slotBg, _id);
            else
                L2SlotIconView.PaintEmptyInventorySlot(_slotBg, _slotElement);
            // Shop/dealer products never use inventory/trade drag (floating DragIcon).
            if (_slotDragManipulator != null)
                _slotDragManipulator.enabled = false;
        }

        _product = item;
    }

    public object GetUseElement()
    {
        if(_product != null)
        {
            return _product;
        }

        return _itemInstance;
    }

    public void ManualHideToolTips()
    {
        ToolTipSimplePacketServices.Window.ManualHide();
    }

    public void AssignUniversal(object data)
    {
        if (data != null)
        {
            if (data.GetType() == typeof(Product))
            {
                AssignProduct((Product)data);
            }
            else if (data.GetType() == typeof(ItemInstance))
            {
                AssignItem((ItemInstance)data);
            }
        }
       
    }

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
