public partial class AssignData
{
    public void RefreshData(ItemInstance item)
    {
        _itemInstance = item;
    }

    public void RefreshDataItem(ItemInstance item)
    {
        if (item.ItemData != null)
        {
            _itemData = item.ItemData;
        }
    }

    public void ResetData()
    {
        _itemInstance = null;
        _itemData = null;
    }

    public int GetCount()
    {
        if (_itemInstance == null) return 0;
        return _itemInstance.Count;
    }

    public int GetObjectId()
    {
        if (_itemInstance == null) return 0;
        return _itemInstance.ObjectId;
    }

    public int GetEnchantLevel()
    {
        if (_itemInstance == null) return 0;
        return _itemInstance.EnchantLevel;
    }
}
