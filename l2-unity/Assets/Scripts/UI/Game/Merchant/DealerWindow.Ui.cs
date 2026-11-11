using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static L2Slot;
using Debug = UnityEngine.Debug;
using VContainer;

public partial class DealerWindow : L2PopupWindow
{
    protected override void LoadAssets()
    {
        _windowTemplate = LoadAsset("Data/UI/_Elements/Game/Merchant/DealerWindow");
        _inventorySlotTemplate = LoadAsset(L2InventoryUiAssets.Slot);
    }

    protected override void InitWindow(VisualElement root)
    {
        base.InitWindow(root);

        var dragArea = GetElementByClass("drag-area");
        _contentBuy = GetElementByClass("content-buy");
        _contentSell = GetElementByClass("content-sell");

        _price = (Label)GetElementById("PriceCount");
        _adena = (Label) GetElementById("AdenaCount");
        _currentWeight = (Label)GetElementById("CurrentWeight");

        _panelSellHeaderName = (Label)GetElementById("SellH1");
        _panelBuyHeaderName = (Label)GetElementById("BuyH1");

        var succesBtn = (Button)GetElementById("SuccessButton");
        succesBtn.RegisterCallback<ClickEvent>((evt) => HandleSuccessButtonClick());
        RegisterCloseWindowEventByName("CloseButton");
        _panelSellHeaderName.text = "Sell";
        _panelBuyHeaderName.text = "Buy";

        DragManipulator drag = new DragManipulator(dragArea, _windowEle);
        dragArea.AddManipulator(drag);

        RegisterCloseWindowEvent("btn-close-frame");
        RegisterClickWindowEvent(_windowEle, dragArea);
        _weiBar = GetElementById("WeightBg");
        _weiBarBg = GetElementById("WeightGauge");
        OnCenterScreen(root);
        CreateInitBuy();
        _windowName = (Label)GetElementById("windows-name-label");
        _productType = ProductType.NONE;
    }

    public void SetWindowName(string windowName)
    {
        _windowName.text = windowName;
    }

    public void SetHeaderNameBuyPanel(string headerName)
    {
        _panelBuyHeaderName.text = headerName;
    }

    public void SetProductType(ProductType type)
    {
        _productType = type;
    }
}
