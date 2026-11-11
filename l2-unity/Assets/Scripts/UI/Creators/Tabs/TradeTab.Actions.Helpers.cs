using System.Collections.Generic;
using UnityEngine.UIElements;
using static L2Slot;

public partial class TradeTab : AbstractTab, ITab
{
    private void SetEventOutside(int slotPosition)
    {
        if (slotPosition != -1)
        {
            TradingSlot slot = _tradeSlots[slotPosition];
            if (slot != null)
            {
                OnEventLeftClick(slot.GetItemId(), slot.GetItemCategory(), slotPosition);
                //EventLeftClick?.Invoke(slot.GetItemId() , slot.GetItemCategory() , slotPosition);
            }
            
        }
    }

    private TradingSlot CreateTradeSlot(TradingSlotModel model)
    {
        return new TradingSlot(model);
    }

    public VisualElement GetContentElement()
    {
        return null;
    }

    public string GetTabName()
    {
        return _tabName;
    }
}
