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




    private static DealerWindow _instance;
    private VisualElement _contentBuy;
    private VisualElement _contentSell;
    private VisualTreeAsset _inventorySlotTemplate;
    private int _defaultCount = 24;
    private Label _adena;
    private Label _price;
    private Label _currentWeight;
    private List<Product> _listBuy;
    private List<Product> _listSell;
    private InventoryTab _tabSell;
    private InventoryTab _tabBuy;
    private InventorySlot[] _inventorySlotsBuy;
    private InventorySlot[] _inventorySlotsSell;
    private VisualElement _weiBar;
    private VisualElement _weiBarBg;
    private int _currentDataWeight = 0;
    private int _maxWeight = 0;
    private ProductType _productType;

    private bool _isModifySell = false;

    public VisualTreeAsset InventorySlotTemplate { get { return _inventorySlotTemplate; } }
    private ShopItemEvaluator _itemEvalutor;
    private ShopInventoryController _shopCellCreator;
    private TradeItemMover _tradeItemMover;

    private Label _panelSellHeaderName;
    private Label _panelBuyHeaderName;
    private int _listId;
    private Label _windowName;
    [Obsolete("Use [Inject] / PacketServices instead")]
    public static DealerWindow Instance
    {
        get { return _instance; }
    }







   public void UpdateBuyData(List<Product>listBuy ,bool  isModifySell , int listId)
   {
        _listId = listId;
        _listBuy = listBuy;
        _isModifySell = isModifySell;
        // Cart starts empty; never leave a null list (OK would send no RequestBuyItem lines).
        _listSell = new List<Product>();

        //UnityEngine.Debug.Log("Server size list buy " + listBuy.Count);

        _shopCellCreator.ClearSell(_tabSell);
        _shopCellCreator.CreateSell(listBuy, _tabSell, _contentSell , _windowEle);
        _shopCellCreator.ClearBuy(_inventorySlotsBuy, _tabBuy);
        _shopCellCreator.CreateBuy(listBuy, _tabBuy, _contentBuy, _windowEle);
        _itemEvalutor.UpdateAllPrice(_listSell);
   }

   public void SetHeaderNameSellPanel(string headerName)
   {
        _panelSellHeaderName.text = headerName;
   }




}
