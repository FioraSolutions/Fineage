using System;
using UnityEngine.UIElements;

public class L2DraggableSlot : L2ClickableSlot
{
    protected SlotDragManipulator _slotDragManipulator;

    public L2DraggableSlot(int position, VisualElement slotElement, SlotType slotType, bool leftMouseUp, bool rightMouseup)
    : base(slotElement, position, slotType, leftMouseUp, rightMouseup)
    {
        if (slotElement == null)
        {
            return;
        }

        // Merchant DealerWindow (PriceSell/PriceBuy) is cart click/double-click — not inventory/trade drag.
        // Leaving drag enabled shows the floating DragIcon ghost that "flies out" of the shop.
        CreateDragManipulator(ref _slotDragManipulator, ref slotElement, AllowsSlotDrag(slotType));
    }

    public L2DraggableSlot(int position, VisualElement slotElement, SlotType slotType , bool isDragged)
         : base(slotElement, position, slotType, true, false)
    {
        if (slotElement == null)
        {
            return;
        }

        CreateDragManipulator(ref _slotDragManipulator, ref slotElement , isDragged);
    }

    static bool AllowsSlotDrag(SlotType slotType) =>
        slotType != SlotType.PriceBuy
        && slotType != SlotType.PriceSell
        && slotType != SlotType.Trade;

    private void CreateDragManipulator(ref SlotDragManipulator slotDragManipulator, ref VisualElement slotElement , bool isDragged)
    {
        if (slotDragManipulator == null)
        {
            slotDragManipulator = new SlotDragManipulator(slotElement, this , isDragged);
            slotElement.AddManipulator(slotDragManipulator);
        }
    }

    public override void ClearManipulators()
    {
        base.ClearManipulators();

        if (_slotDragManipulator != null)
        {
            _slotElement.RemoveManipulator(_slotDragManipulator);
            _slotDragManipulator = null;
        }
    }
}