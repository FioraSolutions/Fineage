
using UnityEngine;
using UnityEngine.UIElements;

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
}
