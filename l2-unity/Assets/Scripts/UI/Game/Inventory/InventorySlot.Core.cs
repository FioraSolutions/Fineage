using UnityEngine;
using UnityEngine.UIElements;

public partial class InventorySlot : L2DraggableSlot
{
    public InventorySlot(int position, VisualElement slotElement, L2Tab tab, SlotType slotType , bool rightMouseup, bool leftMouseUp = false)
    : base(position, slotElement, slotType, leftMouseUp, rightMouseup)
    {
        _currentTab = tab;
        _empty = true;

        if (_slotClickSoundManipulator == null)
        {
            _slotClickSoundManipulator = new SlotClickSoundManipulator(_slotElement);
            _slotElement.AddManipulator(_slotClickSoundManipulator);
        }
       
    }

    public InventorySlot(int position, VisualElement slotElement, SlotType slotType)
    : base(position, slotElement, slotType, true, false)
    {
        _empty = true;
    }
}
