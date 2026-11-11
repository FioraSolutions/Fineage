public partial class ItemsType
{
    public static ItemCategory ParceCategory(int type2)
    {
        if (type2 == 0)
        {
            return ItemCategory.Weapon;
        }
        else if (type2 == 1)
        {
            return ItemCategory.ShieldArmor;
        }
        else if (type2 == 2)
        {
            return ItemCategory.Jewel;
        }
        else if (type2 == 3)
        {
            return ItemCategory.Quest;
        }
        else if (type2 == 4)
        {
            return ItemCategory.Adena;
        }
        else if (type2 == 5)
        {
            return ItemCategory.Item;
        }

        return ItemCategory.Item;
    }
}
