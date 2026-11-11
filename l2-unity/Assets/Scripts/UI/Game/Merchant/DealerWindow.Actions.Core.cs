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
    public void EventDoubleClick(VisualElement slotElement)
    {
        string[] ids = ToolTipsUtils.GetUniquePosition(slotElement);
        int position = Int32.Parse(ids[0]);
        int type = Int32.Parse(ids[1]);
        SlotType slot = ToolTipsUtils.DetectedClickPanel(type);
        MoveРЎellElsePriceType(slot , _listBuy , position);
        RefreshToolTips(slotElement , slot);
    }

    public void RefreshToolTips(VisualElement slotElement , SlotType slot)
    {
        if(SlotType.PriceBuy == slot)
        {
            ToolTipManager.GetInstance().EventLeftClickSlot(slotElement);
        }
        
    }

    private void CreateInitBuy()
    {
        _tabBuy = new InventoryTab();
        _contentBuy.Clear();
        _tabBuy.Initialize(_windowEle, _contentBuy, _contentBuy , false , false);
        _inventorySlotsBuy = _shopCellCreator.CreateEmptyBuy(_tabBuy, _defaultCount, _contentBuy, GetInventorySlotTemplate());
    }

    private void HandleSuccessButtonClick()
    {
        // Snapshot before HideWindow — cart/listId must survive any close side-effects.
        int listId = _listId;
        List<Product> cart = _listSell != null ? new List<Product>(_listSell) : new List<Product>();
        ProductType productType = _productType;
        bool modifySell = _isModifySell;

        HideWindow();

        if (!modifySell)
        {
            switch (productType)
            {
                case ProductType.BUY:
                    if (cart.Count == 0)
                    {
                        Debug.LogWarning("[Shop] Success with empty cart listId=" + listId);
                        break;
                    }
                    SendServer(new RequestBuyItemCommand(listId, cart));
                    break;

                case ProductType.WEAR:
                    SystemMessagePacketServices.Window.OnButtonOk += OkWear;
                    SystemMessagePacketServices.Window.OnButtonClosed += OnРЎancel;
                    SystemMessagePacketServices.Window.ShowWindowDialogYesOrNot("The try-on state lasts only 5 seconds. When the character's state changes, it can be canceled.");
                    break;
                case ProductType.BUY_SEED:
                    // Debug.LogWarning("РќРµ СЂРµР°Р»РёР·РѕРІР°РЅ РїР°РєРµС‚ РѕС‚РїСЂР°РІРєР° BUY_SEED ");
                    break;
            }
        }
        else
        {
            switch (_productType)
            {
                case ProductType.SELL:
                    SendServer(new RequestSellItemCommand(_listId, _listSell));
                    break;
                case ProductType.WHDepositList:
                    SendServer(new SendWarehouseDepositListCommand(_listSell));
                    break;
                case ProductType.WHWithdrawList:
                    SendServer(new SendWarehouseWithdrawListCommand(_listSell));
                    break;
                case ProductType.PackageSendableList:
                    int playerObjectId = _listId;
                    SendServer(new RequestPackageSendCommand(playerObjectId , _listSell));
                    break;
                case ProductType.BUY_SEED:
                    // Debug.LogWarning("РќРµ СЂРµР°Р»РёР·РѕРІР°РЅ РїР°РєРµС‚ РѕС‚РїСЂР°РІРєР° BUY_SEED ");
                    break;
            }
        }

    }
}
