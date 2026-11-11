using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine.UIElements;

public partial class GearItem
{
    public GearItem(VisualElement slotElement , ItemSlot itemSlot)
    {
        _slotElement = slotElement;
        _itemSlot = itemSlot;

    }

    public ItemSlot GetSlotType()
    {
        return _itemSlot;
    }

    public ItemSlot GetItemType()
    {
        if(_gearSlot.ItemInstance != null)
        {
            return _gearSlot.ItemInstance.BodyPart;
        }
        return ItemSlot.none;
    }

    public void SetGearSlot(GearSlot gearSlot)
    {
        _gearSlot = gearSlot;
    }

    public GearSlot PeekGearSlot() => _gearSlot;

    public void SetSelectGear()
    {
        _gearSlot.SetSelected();
    }

    public void UnSelectGear()
    {
        _gearSlot.UnSelect();
    }

    public void Assign(ItemInstance item)
    {
        _gearSlot.AssignItem(item);
    }

    public void AssignImageAtIndex(ItemInstance item , int indexIcon)
    {
        _gearSlot.AssignItemAtIndexImage(item , indexIcon);
    }
}
