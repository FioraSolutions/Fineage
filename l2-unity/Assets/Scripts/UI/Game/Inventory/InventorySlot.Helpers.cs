using UnityEngine;
using UnityEngine.UIElements;

public partial class InventorySlot : L2DraggableSlot
{
    public void AssignItemAtIndexImage(ItemInstance item , int indexImage)
    {
        _slotElement.RemoveFromClassList("empty");

        AddDataItem(item);
        _itemInstance = item;
        _count = item.Count;
        _objectId = item.ObjectId;
        _id = item.ItemId;
        _remainingTime = item.RemainingTime;
        _enchantLevel = item.EnchantLevel;
        _empty = false;
        RefreshEnchantMark();

        if (_slotElement != null)
        {
            AddImageAtIndex(this , indexImage);
            AddTooltip(item);
            _slotDragManipulator.enabled = true;
        }
        else
        {
            // Debug.Log("Не критическая ошибка не смогли найти InventorySlot>SlotElement");
        }
    }

    void RefreshEnchantMark()
    {
        if (_slotElement == null)
            return;
        Label mark = _slotElement.Q<Label>("EnchantMark");
        if (_enchantLevel <= 0)
        {
            if (mark != null)
                mark.RemoveFromHierarchy();
            return;
        }

        if (mark == null)
        {
            mark = new Label { name = "EnchantMark", pickingMode = PickingMode.Ignore };
            mark.style.position = UnityEngine.UIElements.Position.Absolute;
            mark.style.left = 1;
            mark.style.top = 0;
            mark.style.fontSize = 10;
            mark.style.color = new StyleColor(new Color(1f, 0.92f, 0.45f));
            mark.style.unityFontStyleAndWeight = FontStyle.Bold;
            _slotElement.Add(mark);
        }

        mark.text = "+" + _enchantLevel;
    }
}
