using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using System.Globalization;

public partial class ToolTipsUtils
{
    public static string ConvertNumberToNormal(string wholeNumber)
    {
        return wholeNumber.Replace(",", ".");
    }

    public static VisualElement CloneOne(VisualTreeAsset vta)
    {
        var e = vta.CloneTree();
        return e.Children().First();
    }

    public static string[] GetUniquePosition(VisualElement ve)
    {
        return ve.name.Split('_');
    }

    public static SlotType DetectedClickPanel(int type)
    {
        switch (type)
        {
            case (int)SlotType.PriceBuy:
                return SlotType.PriceBuy;
            case (int)SlotType.PriceSell:
                return SlotType.PriceSell;
            case (int)SlotType.BuffPanel:
                return SlotType.BuffPanel;
            case (int)SlotType.Recipe:
                return SlotType.Recipe;
            case (int)SlotType.Enchant:
                return SlotType.Enchant;
            case (int)SlotType.Skill:
                return SlotType.Skill;
            case (int)SlotType.SkillBar:
                return SlotType.SkillBar;
            default:
                return SlotType.Other;
        }
    }
}
