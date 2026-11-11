public partial class L2ClickableSlot : L2Slot
{
    public void SetSelected()
    {
        // Debug.Log($"Slot {_position} {Id}selected.");

        if (slotFrame == null) slotFrame = _slotElement.Q(className: "slot-frame");

        if (slotFrame != null)
        {
            slotFrame.style.display = DisplayStyle.Flex;
        }

        ToolTipManager.GetInstance().EventLeftClickSlot(_slotElement);
        EventLeftDoubleClick(_slotElement);

    }

    private void EventLeftDoubleClick(VisualElement slotElement)
    {
        if (Time.time - clickTime < doubleClickThreshold)
        {
            switch (_slotType)
            {
                case SlotType.Recipe:
                    CraftPacketServices.RecipeBook.EventDoubleClick(slotElement);
                    break;
                case SlotType.PriceBuy:
                case SlotType.PriceSell:
                    MerchantPacketServices.Dealer.EventDoubleClick(slotElement);
                    break;
            }
        }
        clickTime = Time.time;
    }

    public void UnSelect()
    {
        // Debug.Log($"Slot {_position} unselected.");
        if(slotFrame == null ) slotFrame = _slotElement.Q(className: "slot-frame");

        if (slotFrame != null)
        {
            slotFrame.style.display = DisplayStyle.None;
        }
        
    }
}
