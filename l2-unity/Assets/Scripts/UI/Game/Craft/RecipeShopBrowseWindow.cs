using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Fineage.Net.Contract;

public sealed class RecipeShopBrowseWindow : L2PopupWindow
{
    static RecipeShopBrowseWindow _instance;
    ICreatorTradeTab _creatorWindow;
    List<RecipeShopOfferDto> _offers = new List<RecipeShopOfferDto>();
    int _manufacturerObjectId;
    Label _adenaLabel;
    Label _mpLabel;

    public static RecipeShopBrowseWindow Instance => _instance;
    public bool IsShown { get; private set; }

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            _creatorWindow = new CreatorTradingWindows();
            ManufacturePacketServices.Bind(browse: this);
        }
        else
        {
            Destroy(this);
        }
    }

    void Start() => ManufacturePacketServices.Bind(browse: this);

    protected override void LoadAssets()
    {
        _windowTemplate = LoadAsset("Data/UI/_Elements/Game/Craft/RecipeBookWindow");
        _creatorWindow.LoadAsset(LoadAsset);
    }

    protected override IEnumerator BuildWindow(VisualElement root)
    {
        InitWindow(root);
        var dragArea = GetElementByClass("drag-area");
        dragArea.AddManipulator(new DragManipulator(dragArea, _windowEle));

        _creatorWindow.InitTradeTabs(new[] { "ALL" });
        _creatorWindow.CreateTradeTabs(GetElementById("InventoryTabView"), SlotType.Recipe, true);

        _adenaLabel = (Label)GetElementById("sizeLabel");
        _mpLabel = _adenaLabel;

        RegisterCloseWindowEvent("btn-close-frame");
        RegisterClickWindowEvent(_windowEle, dragArea);
        OnCenterScreen(_root);
        yield return new WaitForEndOfFrame();
    }

    public override void ShowWindow()
    {
        base.ShowWindow();
        IsShown = true;
    }

    public override void HideWindow()
    {
        base.HideWindow();
        IsShown = false;
    }

    public void OpenList(int manufacturerObjectId, int curMp, int maxMp, long buyerAdena, List<RecipeShopOfferDto> offers)
    {
        _manufacturerObjectId = manufacturerObjectId;
        _offers = offers ?? new List<RecipeShopOfferDto>();
        if (_mpLabel != null)
            _mpLabel.text = $"MP {curMp}/{maxMp}  Adena {buyerAdena}";

        var items = new List<ItemInstance>();
        for (int i = 0; i < _offers.Count; i++)
        {
            RecipeShopOfferDto offer = _offers[i];
            var recipe = new RecipeInstance(offer.RecipeData, offer.Position);
            items.Add(recipe.GetItemInstance());
        }

        _creatorWindow.ClearSlots(items);
        _creatorWindow.AddData(items);
        ShowWindow();
    }

    public void EventDoubleClick(VisualElement slotElement)
    {
        string[] ids = ToolTipsUtils.GetUniquePosition(slotElement);
        int position = int.Parse(ids[0]);
        RecipeShopOfferDto offer = _offers.Find(o => o.Position == position + 1);
        if (offer == null)
        {
            FineageLog.Error(FineageLogCategory.Net, $"RecipeShopBrowse: no offer at position {position}");
            return;
        }

        NetCraftSession.ActiveShopPrice = offer.Price;
        ZoneCommands.Send(new RequestRecipeShopMakeInfoCommand(_manufacturerObjectId, offer.RecipeId));
    }

    public RecipeShopOfferDto GetOfferByPosition(int position) =>
        _offers.Find(o => o.Position == position + 1);
}