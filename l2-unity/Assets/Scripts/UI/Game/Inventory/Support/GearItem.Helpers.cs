using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine.UIElements;

public partial class GearItem
{
    public void AssignImageAtIndeAlpha(ItemInstance item, int indexIcon)
    {
        _gearSlot.AssignItemAtIndexImage(item, indexIcon);
        _gearSlot.AddBlackOverlay();
    }

    public void AssignAlpha(ItemInstance item , float alpha)
    {
        _gearSlot.AssignItem(item , alpha);
        _gearSlot.AddBlackOverlay();
    }

    public void AssignEmpty()
    {
        _gearSlot.RemoveBlackOverlay();
        _gearSlot.AssignEmpty();
    }

    public bool IsEmptyGearSlot()
    {
        return _gearSlot.IsEmpty;
    }

    public int GetItemId()
    {
        return _gearSlot.ItemId;
    }

    public bool IsBow() => _gearSlot.GetUseElement() is ItemInstance item && item.IsBow();

    public bool IsArrow() => _gearSlot.GetUseElement() is ItemInstance item && item.IsArrow();

    public int GetObjectId()
    {
        return _gearSlot.ObjectId;
    }

    public VisualElement GetElement()
    {
        return _slotElement;
    }
}
