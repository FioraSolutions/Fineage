using System;

public partial class MultiSellWindow : L2PopupWindow
{
    public ItemInstance GetItemByPosition(int position)
    {
        return _creatorWindow.GetActiveByPosition(position);
    }

    private List<Ingredient> GetIngredient(List<MultiSellData> data , int itemId , ItemInstance itemInstance , int position)
    {
        if (!IfMore1Item(data, itemId))
        {
            return  GetIngredient(data, itemInstance.ItemId);
        }
        else
        {
            return  GetIngredientByIndex(data, position);
        }
    }

    public void OnGeometryChanged(GeometryChangedEvent evt)
    {
        _contentPanel1.style.height = evt.newRect.height;
        _selectContainer.UnregisterCallback<GeometryChangedEvent>(OnGeometryChanged);
    }
}
