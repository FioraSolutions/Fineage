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

    public void SetInventorySlotsSell(InventorySlot[] inventorySlotSell)
    {
        _inventorySlotsSell = inventorySlotSell;
    }

    public void SetInventorySlotsBuy(InventorySlot[] inventorySlotBuy)
    {
        _inventorySlotsBuy = inventorySlotBuy;
    }

    public VisualTreeAsset GetInventorySlotTemplate()
    {
        return _inventorySlotTemplate;
    }

    public VisualElement GetContentSell()
    {
        return _contentSell;
    }

    public void SetTabSell(InventoryTab tabSell)
    {
        _tabSell = tabSell;
    }

    public void SetTabBuy(InventoryTab tabBuy)
    {
        _tabBuy = tabBuy;
    }

    public InventoryTab GetTabBuy()
    {
        return _tabBuy;
    }

    public InventoryTab GetTabSell()
    {
        return _tabSell;
    }

    public List<Product> GetListSell()
    {
        return _listSell;
    }

    public List<Product> GetListBuy()
    {
        return _listBuy;
    }

    public void SetListBuy(List<Product> listBuy)
    {
        _listBuy = listBuy;
    }

    public void SetListSell(List<Product> listSell)
    {
        _listSell = listSell;
    }

    public void UpdateDataForm(int adena, double weightPercent, int currentWeight, int maxWeight)
    {
        _itemEvalutor.UpdateDataForm(adena, weightPercent, currentWeight, maxWeight);
    }

 

}
