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

    /// <summary>
    /// Interlude client inventory semantics: a single left click only selects the slot and shows the
    /// item description (the base handler calls <c>SelectSlot</c>, which raises
    /// <c>ToolTipSimple.EventLeftClickSlot</c>). The double click — two clicks inside
    /// <c>InventorySlot</c>'s 0.3s window — routes to <see cref="UseItem"/> → <see cref="UnequipWorn"/>,
    /// and the right click unequips immediately.
    ///
    /// <para>
    /// This used to call <c>UnequipWorn()</c> on every left click, so inspecting a worn item
    /// silently took it off instead of showing what it was.
    /// </para>
    /// </summary>
    protected override void HandleLeftClick()
    {
        // A completed drag is not a click: the drop already equipped/unequipped it.
        if (_slotDragManipulator != null && (_slotDragManipulator.dragged || _slotDragManipulator.DragCompleted))
            return;
        base.HandleLeftClick();
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

}
