using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static L2Slot;
using Debug = UnityEngine.Debug;
using VContainer;

public partial class DealerWindow
{
    public VisualElement GetWeiBar()
    {
        return _weiBar;
    }

    public VisualElement GetWeiBarBg()
    {
        return _weiBarBg;
    }

    public void UpdatePrice(int allPrice)
    {
        _price.text = ToolTipsUtils.ConvertToPrice(allPrice);
    }

    public void UpdateDataWeight(int data)
    {
        _currentDataWeight = data;
    }

    public int GetCurrentDataWeight()
    {
        return _currentDataWeight;
    }

    public int GetMaxWeight()
    {
        return _maxWeight;
    }

    public void UpdateAdena(int adena)
    {
        _adena.text = ToolTipsUtils.ConvertToPrice(adena);
    }

    public void UpdateWeight(double weightPercent)
    {
        _currentWeight.text = weightPercent.ToString("0") + "%";
    }

    public void UpdateAllWeight(int currentWeight, int maxWeight)
    {
        _itemEvalutor.UpdateWeightBar(this , currentWeight, maxWeight);
    }

    public void UpdateDataMaxWeight(int data)
    {
        _maxWeight = data;
    }

    public InventorySlot[] GetInventorySlotsSell()
    {
        return _inventorySlotsSell;
    }

    public InventorySlot[] GetInventorySlotsBuy()
    {
        return _inventorySlotsBuy;
    }
}
