using UnityEngine;
using UnityEngine.UIElements;

public partial class GearSlot : InventorySlot
{
    void UnequipWorn()
    {
        PlayerInventory inventory = InventoryPacketServices.Inventory;
        ItemInstance item = inventory != null ? inventory.GetItemEquip(ObjectId) : null;
        if (item == null)
        {
            AssignEmpty();
            return;
        }

        if (!ZoneCommands.IsConnected)
            return;

        ZoneCommands.Send(new UseItemCommand(ObjectId, 0));
    }

    private VisualElement CreateBlack(string name)
    {

        var overlay = new VisualElement();
        overlay.name = name;
        overlay.style.top = 0;
        overlay.style.left = 0;
        overlay.style.width = new Length(100, LengthUnit.Percent);
        overlay.style.height = new Length(100, LengthUnit.Percent);
        overlay.style.backgroundColor = new Color(0, 0, 0, 0.7f); // Черный цвет с 50% прозрачностью
        return overlay;
    }

    public void RemoveBlackOverlay()
    {

        var blackOverlay = _slotElement.Q<VisualElement>(_blackElementName);

        if (blackOverlay != null)
        {
            _slotElement.Remove(blackOverlay);
        }
    }

    public void AddBlackOverlay()
    {
        //_slotElement.style.backgroundImage = background;
        var blackOverlay = _slotElement.Q<VisualElement>(_blackElementName);

        if (blackOverlay == null)
        {
            _slotElement.Add(_blackElement);
        }

    }

    public new void AssignEmpty()
    {
        base.AssignEmpty();
        L2SlotIconView.Clear(_slotElement);
        if (SlotBg != null)
            L2SlotIconView.Clear(SlotBg);
    }
}
