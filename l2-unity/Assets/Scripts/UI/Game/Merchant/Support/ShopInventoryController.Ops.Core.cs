using System.Collections.Generic;
using UnityEngine.UIElements;
using static L2Slot;

public partial class ShopInventoryController
{
    public SlotType DetectedClickPanel(int type)
    {
        if (type == (int)SlotType.PriceBuy)
            return SlotType.PriceBuy;
        if (type == (int)SlotType.PriceSell)
            return SlotType.PriceSell;
        return SlotType.Other;
    }

    public List<Product> AddProduct(List<Product> listSell, Product source)
    {
        if (listSell == null)
            return new List<Product> { source };

        bool isUpdate = false;
        if (source.GetTypeItem() == EnumType2.TYPE2_OTHER)
            isUpdate = UpdateList(listSell, source, plus: true);

        if (!isUpdate)
            listSell.Add(source);

        return listSell;
    }

    public List<Product> RemoveProduct(List<Product> listSell, Product source, bool isRemoval)
    {
        if (listSell == null)
            return new List<Product> { source };

        bool isUpdate = false;
        if (source.GetTypeItem() == EnumType2.TYPE2_OTHER && !isRemoval)
            isUpdate = UpdateList(listSell, source, plus: false);

        if (!isUpdate || isRemoval || source.Count == 0)
            listSell.Remove(source);

        return listSell;
    }
}
