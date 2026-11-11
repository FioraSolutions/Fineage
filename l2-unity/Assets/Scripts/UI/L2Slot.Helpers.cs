using System;
using UnityEngine;
using UnityEngine.UIElements;

public partial class L2Slot
{
    public L2Slot(VisualElement slotElement, int position, SlotType type)
    {
        _slotElement = slotElement;
        _position = position;
        _slotType = type;

        if (slotElement == null)
        {
            return;
        }

        _slotBg = _slotElement.Q<VisualElement>(null, "slot-bg");
        _slotDisabled = _slotElement.Q<VisualElement>(null, "slot-disabled");

        if (slotElement != null)
        {
            if(type != SlotType.Trash)
            {
                ToolTipManager.GetInstance().RegisterSimple(type, _slotElement, position);
            }
           
        }

        if (_hoverManipulator == null)
        {
            _hoverManipulator = new SlotHoverDetectManipulator(_slotElement, this);
            _slotElement.AddManipulator(_hoverManipulator);
        }

    }

    public virtual void ClearManipulators()
    {
        if (_slotElement == null)
        {
            return;
        }

        if (_tooltipManipulator != null)
        {
            _tooltipManipulator.Clear();
            _slotElement.RemoveManipulator(_tooltipManipulator);
            _tooltipManipulator = null;
        }

        if (_hoverManipulator != null)
        {
            _slotElement.RemoveManipulator(_hoverManipulator);
            _hoverManipulator = null;
        }
    }
}
