using UnityEditor.Build.Content;
using static UnityEditor.Progress;
using UnityEngine;

public partial class AssignData
{
    public int GetItemId()
    {
        if (_itemInstance == null) return 0;
        return _itemInstance.ItemId;
    }

    public ItemInstance GetItemInstance()
    {
        return _itemInstance;
    }

    public string GetName()
    {
        if (_itemData == null) return "";
        return _itemData.ItemName.DisplayName;
    }

    public string Description()
    {
        if (_itemData == null) return "";
        return _itemData.ItemName.Description;
    }

    public string GetIcon()
    {
        if (_itemData == null) return "";
        return _itemData.Icon;
    }

    public ItemCategory GetItemCategory()
    {
        if (_itemInstance == null) return ItemCategory.None;
        return _itemInstance.Category;
    }
}
