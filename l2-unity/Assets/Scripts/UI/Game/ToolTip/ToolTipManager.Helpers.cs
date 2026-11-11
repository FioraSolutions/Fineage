using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UIElements;
using static L2Slot;

public partial class ToolTipManager
{
    public Product FindProductInBuyList(int positionIndex)
    {
        if (IsIndexValid(_buyProduct, positionIndex))
        {
            if(_buyProduct[positionIndex] != null)
            {
                return _buyProduct[positionIndex];
            }
            
        }
        return null;
    }

    private bool IsIndexValid(List<Product> list, int index)
    {
        if(list != null)
        {
            return index >= 0 && index < list.Count;
        }
        return false;
    }

    public void RegisterSimple(SlotType type , VisualElement list , int position)
    {
        ToolTipSimplePacketServices.Window.RegisterCallback(type , list , position);
    }

    public void EventLeftClickSlot(VisualElement slotElement)
    {
        ToolTipSimplePacketServices.Window.EventLeftClickSlot(slotElement);
    }

    public IDataTips GetProductText(object obj)
    {
        return new SimpleToolTipData(obj);
    }
}
