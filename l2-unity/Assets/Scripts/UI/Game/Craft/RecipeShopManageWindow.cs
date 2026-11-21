using System.Collections;
using System.Collections.Generic;
using Fineage.Net.Contract;
using UnityEngine;
using UnityEngine.UIElements;

public sealed class RecipeShopManageWindow : L2PopupWindow
{
    static RecipeShopManageWindow _instance;
    readonly List<RecipeShopRowNet> _rows = new List<RecipeShopRowNet>();
    TextField _messageField;
    ScrollView _rowList;
    bool _pickingRecipes;

    public static RecipeShopManageWindow Instance => _instance;
    public bool IsPickingRecipes => _pickingRecipes;

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            ManufacturePacketServices.Bind(manage: this);
        }
        else
        {
            Destroy(this);
        }
    }

    void Start() => ManufacturePacketServices.Bind(manage: this);

    protected override void LoadAssets()
    {
        _windowTemplate = LoadAsset("Data/UI/_Elements/Game/ShowList/ShowListWindow");
    }

    protected override IEnumerator BuildWindow(VisualElement root)
    {
        InitWindow(root);
        _messageField = new TextField { name = "ShopMessage" };
        _messageField.value = "Craft";
        _rowList = new ScrollView();
        var body = GetElementById("BodyBox");
        if (body != null)
        {
            body.Add(_messageField);
            body.Add(_rowList);
        }

        var startBtn = GetElementById("StartButton");
        if (startBtn != null)
            startBtn.RegisterCallback<ClickEvent>(_ => OnStartShop());
        var cancelBtn = GetElementById("CancelButton");
        if (cancelBtn != null)
            cancelBtn.RegisterCallback<ClickEvent>(_ => OnStopShop());

        RegisterCloseWindowEvent("btn-close-frame");
        OnCenterScreen(_root);
        yield return new WaitForEndOfFrame();
    }

    public void OpenForDwarvenManufacture()
    {
        _rows.Clear();
        RefreshRowList();
        _pickingRecipes = true;
        ZoneCommands.Send(new RequestRecipeBookOpenCommand(1));
        ShowWindow();
    }

    public void OpenForCommonManufacture()
    {
        _rows.Clear();
        RefreshRowList();
        _pickingRecipes = true;
        ZoneCommands.Send(new RequestRecipeBookOpenCommand(0));
        ShowWindow();
    }

    public void AddRecipeFromBook(int recipeId, long defaultPrice = 1)
    {
        if (_rows.Exists(r => r.RecipeId == recipeId))
            return;
        _rows.Add(new RecipeShopRowNet { RecipeId = recipeId, Price = defaultPrice });
        RefreshRowList();
    }

    void RefreshRowList()
    {
        if (_rowList == null)
            return;
        _rowList.Clear();
        for (int i = 0; i < _rows.Count; i++)
        {
            RecipeShopRowNet row = _rows[i];
            RecipeData data = TablePacketServices.Recipes.GetRecipeData(row.RecipeId);
            string label = data != null ? data.Alias : $"recipe:{row.RecipeId}";
            var rowBox = new VisualElement();
            rowBox.style.flexDirection = FlexDirection.Row;
            var name = new Label(label);
            var price = new TextField { value = row.Price.ToString() };
            price.style.width = 100;
            int index = i;
            price.RegisterValueChangedCallback(evt =>
            {
                if (long.TryParse(evt.newValue, out long parsed) && parsed >= 0)
                    _rows[index].Price = parsed;
            });
            rowBox.Add(name);
            rowBox.Add(price);
            _rowList.Add(rowBox);
        }
    }

    void OnStartShop()
    {
        string message = _messageField != null ? _messageField.value : "";
        ZoneCommands.Send(new RequestSetRecipeShopCommand(message, _rows.ToArray()));
        _pickingRecipes = false;
    }

    void OnStopShop()
    {
        ZoneCommands.Send(new RequestSetRecipeShopCommand("", System.Array.Empty<RecipeShopRowNet>()));
        ZoneCommands.Send(new RequestPrivateStoreQuitCommand());
        _pickingRecipes = false;
        HideWindow();
    }
}