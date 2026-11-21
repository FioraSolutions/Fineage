using System.Collections.Generic;
using Fineage.Net.Contract;

public partial class RecipeItemMakeInfoDto
{
    int _manufacturerObjectId;
    long _shopPrice;
    bool _shopMode;

    public bool IsShopMode => _shopMode;
    public int ManufacturerObjectId => _manufacturerObjectId;
    public long ShopPrice => _shopPrice;

    public void SetShopContext(int manufacturerObjectId, long shopPrice)
    {
        _shopMode = true;
        _manufacturerObjectId = manufacturerObjectId;
        _shopPrice = shopPrice;
    }

    public void ApplyFromNet(RecipeItemMakeInfoNetMsg msg, RecipeData data, int bookDwarven, bool shopMode)
    {
        _recipeId = msg.RecipeId;
        _isDwarven = bookDwarven;
        _currentMp = msg.CurrentMp;
        _maxMp = msg.MaxMp;
        _status = msg.Result;
        _data = data;
        _shopMode = shopMode;
        _requiredItems = _requiredItems ?? new List<ItemInstance>();
        _requiredItems.Clear();

        RecipeMaterials[] materials = data.Materials;
        if (materials == null)
            return;

        for (int i = 0; i < materials.Length; i++)
        {
            RecipeMaterials material = materials[i];
            if (material == null || material.MaterialsMCnt <= 0)
                continue;

            var item = new ItemInstance(
                -1,
                material.MaterialsMId,
                ItemLocation.Void,
                i,
                material.MaterialsMCnt,
                ItemCategory.RequiredItem,
                false,
                ItemSlot.none,
                0,
                -1);
            item.SetItemRequiredType(true, material.MaterialsMCnt);
            _requiredItems.Add(item);
        }
    }
}