using System;
using System.Linq;
using UnityEngine.UIElements;
using static L2Slot;
using System.Collections;
using UnityEngine;

public partial class L2ClickableSlot : L2Slot
{
    public L2ClickableSlot(VisualElement slotElement, int position, SlotType type, bool leftMouseUp, bool rightMouseup) : base(slotElement, position, type)
    {
        _leftMouseUp = leftMouseUp;
        _rightMouseup = rightMouseup;
        RegisterClickableCallback();
    }

    protected void RegisterClickableCallback()
    {
        if (_slotElement == null)
        {
            return;
        }

        _slotElement.RegisterCallback<MouseDownEvent>(HandleSlotClickDown, TrickleDown.TrickleDown);
        _slotElement.RegisterCallback<MouseUpEvent>(HandleSlotClickUp, TrickleDown.TrickleDown);
    }

    public void UnregisterClickableCallback()
    {
        if (_slotElement == null)
        {
            return;
        }

        _slotElement.UnregisterCallback<MouseDownEvent>(HandleSlotClickDown, TrickleDown.TrickleDown);
        _slotElement.UnregisterCallback<MouseUpEvent>(HandleSlotClickUp, TrickleDown.TrickleDown);
    }
}
