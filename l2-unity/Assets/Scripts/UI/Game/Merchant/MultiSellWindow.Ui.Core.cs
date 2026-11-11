using System;

public partial class MultiSellWindow : L2PopupWindow
{
    public void AddData(List<ItemInstance> allItems , MultiSellListDto listMultisell)
    {
        if(allItems.Count > 0 && listMultisell != null)
        {
            _listMultisell = listMultisell;
            _listId = listMultisell.GetListId();
            _creatorWindow.AddData(allItems);
        }

        _creatorWindow.SetClickActiveTab(0);
        _entryId = 1;
    }

    public override void ShowWindow()
    {
        _userInput.value = "1";
        base.ShowWindow();
    }

    public void OnEventLeftClick(int itemId , ItemCategory category, int position)
    {
        ItemInstance itemInstance = new ItemInstance(0, itemId, ItemLocation.Trade, 0, 1, category, false, ItemSlot.none, 0, 999);
        itemInstance.SetMultiSell(true);
        
        _selectContainer = _toolTips.GetContainer(itemInstance);

        if(_selectContainer != null)
        {
            _contentPanel1.Clear();
            _toolTips.UseItem(itemInstance, _selectContainer);

            _contentPanel1.Add(_selectContainer);
            _selectContainer.RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);

            _contentPanel2.Clear();

            List<MultiSellData> data = _listMultisell.GetMultiSell();
            List<Ingredient> ingredients = null;
            ingredients = GetIngredient(data  , itemId, itemInstance, position);

            if (ingredients != null)
            {
                _toolTips.AddIngredient(ingredients, _contentPanel2);
            }

        }
        else
        {
            _contentPanel1.Clear();
            _contentPanel2.Clear();
            _entryId = -1;
        }
    }
}
