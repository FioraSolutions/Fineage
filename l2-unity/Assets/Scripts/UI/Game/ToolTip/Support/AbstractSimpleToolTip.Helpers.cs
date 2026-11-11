using UnityEngine;
using UnityEngine.UIElements;

public partial class AbstractSimpleToolTip
{
    public string GetPrice()
    {
        if (_product != null)
        {
            return _product.Price.ToString();
        }

        return "0";
    }

    public string GetDiscription()
    {
        return _description;
    }

    public string GetItemDiscription()
    {
        return _item_description;
    }

    public string GetIcon()
    {
        throw new System.NotImplementedException();
    }

    public int GetEnchant()
    {
        return _enchant;
    }
}
