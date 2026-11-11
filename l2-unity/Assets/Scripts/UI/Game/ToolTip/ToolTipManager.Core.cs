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
    public static ToolTipManager GetInstance()
    {
        if (_instance == null)
            _instance = new ToolTipManager();
        return _instance;
    }

    public void SetSellData(List<Product> sellProduct)
    {
        //if(_sellProduct != null) _sellProduct.Clear();
        _sellProduct = sellProduct;
    }

    public Product FindProductInSellList(int positionIndex)
    {

        if(IsIndexValid(_sellProduct, positionIndex))
        {
            return _sellProduct[positionIndex];
        }
        return null;
    }

    public void SetBuyData(List<Product> buyProduct)
    {
        _buyProduct = buyProduct;
    }
}
