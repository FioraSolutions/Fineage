public partial class L2ClickableSlot : L2Slot
{
    private void HandleSlotClickDown(MouseDownEvent evt)
    {
        if (evt.button == 0)
        {
            SetActive();

            if (!_leftMouseUp)
            {
                HandleLeftClick();
            }
        }
        else if (evt.button == 1)
        {
            if (!_rightMouseup)
            {
                HandleRightClick();
            }
        }
        else
        {
            HandleMiddleClick();
        }
    }

    private void HandleSlotClickUp(MouseUpEvent evt)
    {
        UnsetActive();

        // Hover manipulator can be cleared (e.g. skillbar reassign) while click handlers remain.
        if (_hoverManipulator != null && !_hoverManipulator.Hovering)
        {
            return;
        }

        if (evt.button == 0)
        {
            if (_leftMouseUp)
            {
                HandleLeftClick();
            }
        }
        else if (evt.button == 1)
        {
            if (_rightMouseup)
            {
                HandleRightClick();
            }
        }
        else
        {
            HandleMiddleClick();
        }
    }

    protected virtual void SetActive()
    {
        _slotElement.AddToClassList("active");
    }
}
