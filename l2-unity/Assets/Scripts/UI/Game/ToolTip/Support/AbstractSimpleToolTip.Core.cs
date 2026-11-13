using UnityEngine;
using UnityEngine.UIElements;

public partial class AbstractSimpleToolTip
{
    public string GetName(bool hideCount = false)
    {

        if (hideCount)
        {
            return _name;
        }

        if (_product != null)
        {
            if (_product.Count > 1) _name = _name + " (" + _product.Count + ")";
        }
        else if (_itemInstance != null)
        {
            if (_itemInstance.Category == ItemCategory.Adena)
            {
                if (_itemInstance.Count > 1) _name = _name + " (" + ToolTipsUtils.ConvertToPrice(_itemInstance.Count) + ")";
            }
            else
            {
                if (_itemInstance.Count > 1) _name = _name + " (" + _itemInstance.Count + ")";
            }

        }

        return _name;
    }

    public Texture2D GetGradeTexture()
    {
        if (_product != null)
        {
            return _product.GradeIcon();
        }
        else if (_itemInstance != null)
        {
            return _itemInstance.GetGradeTexture();
        }
        return null;
    }

    public ItemName[] GetSets()
    {
        if (_product != null)
        {
            return _product.GetSets();
        }
        else if (_itemInstance != null)
        {
            return _itemInstance.GetSets();
        }
        return null;

    }

    public ItemSets[] GetSetsEffect()
    {
        if (_product != null)
        {
            return _product.GetSetsEffects();
        }
        else if (_itemInstance != null)
        {
            return _itemInstance.GetSetsEffects();
        }
        return null;

    }
}
