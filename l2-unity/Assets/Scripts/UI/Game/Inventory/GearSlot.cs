using UnityEngine;
using UnityEngine.UIElements;

public class GearSlot : InventorySlot
{
    private VisualElement _blackElement;
    private string _blackElementName = "backOverlay";

    public GearSlot(int position, VisualElement slotElement, InventoryGearTab tab, SlotType slotType) : base(position, slotElement, tab, slotType, false, true)
    {
        _blackElement = CreateBlack(_blackElementName);
    }

    protected override void HandleLeftClick()
    {
        if (_slotDragManipulator != null && _slotDragManipulator.dragged)
            return;
        if (TryGiveToEnchant())
            return;
        UnequipWorn();
    }

    protected override void HandleRightClick()
    {
        if (TryGiveToEnchant())
            return;
        UnequipWorn();
    }

    public override void UseItem()
    {
        UnequipWorn();
    }

    void UnequipWorn()
    {
        PlayerInventory inventory = PlayerInventory.Instance;
        ItemInstance item = inventory != null ? inventory.GetItemEquip(ObjectId) : null;
        if (item == null)
        {
            AssignEmpty();
            return;
        }

        GameClient game = InventoryPacketServices.Game;
        if (game == null)
            return;

        if (FineageWireConfig.IsZoneV2)
        {
            InventoryGearTab gearTab = _currentTab as InventoryGearTab;
            ItemSlot cell = gearTab != null ? gearTab.CellSlot(_position) : ItemSlot.none;
            int slot = ZoneV2Paperdoll.UnequipIndex(ZoneV2Paperdoll.IndexForGearCell(cell.ToString()), item.BodyPart.ToString());
            if (slot >= 0)
            {
                game.Send(new UnequipItemCommand(slot));
                return;
            }
        }

        game.Send(new UseItemCommand(ObjectId, 0));
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

}
