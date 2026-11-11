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
