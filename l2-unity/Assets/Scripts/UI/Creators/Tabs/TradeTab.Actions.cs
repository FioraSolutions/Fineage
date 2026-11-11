using System.Collections.Generic;
using UnityEngine.UIElements;
using static L2Slot;

public partial class TradeTab : AbstractTab, ITab
{
    public void OnClickLeftEvent(int position)
    {
        SelectSlot(position);
        SetEventOutside(position);
    }

    public ItemInstance GetSlotByPosition(int position)
    {
        if(ArrayUtils.IsValidIndexArray(_tradeSlots, position))
        {
            return _tradeSlots[position].GetItemInstance();
        }

        return null;
    }

    public void ClearAllSlots()
    {
        if (_tradeSlots == null) return;

   
        if (_selectedSlot != -1 && ArrayUtils.IsValidIndexArray(_tradeSlots, _selectedSlot))
        {
            _tradeSlots[_selectedSlot].UnSelect();
            _selectedSlot = -1;
        }

     
        for (int i = 0; i < _tradeSlots.Length; i++)
        {
            if (_tradeSlots[i] != null)
            {
                _tradeSlots[i].AssignEmpty();
            }
        }
    }

    public void SelectSlot(int slotPosition)
    {
        if (_selectedSlot != -1)
        {
            //It happens that the panel will become smaller and the index may be larger than the current panel, so you need to check
            if (ArrayUtils.IsValidIndexArray(_tradeSlots, _selectedSlot))
            {
                _tradeSlots[_selectedSlot].UnSelect();
            }

        }
        _tradeSlots[slotPosition].SetSelected();
        _selectedSlot = slotPosition;
    }

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
