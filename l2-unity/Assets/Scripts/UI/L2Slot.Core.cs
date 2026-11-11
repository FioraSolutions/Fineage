using System;
using UnityEngine;
using UnityEngine.UIElements;

public partial class L2Slot
{
    public L2Slot(VisualElement slotElement)
    {
        _slotElement = slotElement;
        _slotElement.AddToClassList("dragged");
        _slotBg = _slotElement.Q<VisualElement>(null, "slot-bg");
        _slotDisabled = _slotElement.Q<VisualElement>(null, "slot-disabled");

        _position = -1;
        _id = -1;
        _name = "";
        _description = "";
        _icon = "";
        _slotType = SlotType.Other;
    }
}
