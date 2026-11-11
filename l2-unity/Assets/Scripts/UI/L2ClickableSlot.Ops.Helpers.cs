using System;
using System.Linq;
using static L2Slot;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public partial class L2ClickableSlot : L2Slot
{
    protected virtual void UnsetActive()
    {
        _slotElement.RemoveFromClassList("active");
    }

    protected virtual void HandleLeftClick() { }

    protected virtual void HandleRightClick() { }

    protected virtual void HandleMiddleClick() { }
}
