using UnityEngine;

public sealed partial class ArmorVisualResolver
{
    public ArmorVisualResolver(ModelTable models, ItemTable items = null)
    {
        _models = models;
        _items = items;
    }

    public static ArmorVisualResolver FromTables()
    {
        return new ArmorVisualResolver(TablePacketServices.Models, TablePacketServices.Items);
    }

    public ArmorVisual Resolve(int itemId, CharacterRaceAnimation race)
    {
        ItemTable items = _items != null ? _items : TablePacketServices.Items;
        Armor armor = items != null ? items.GetArmor(itemId) : null;
        if (armor == null)
            return ArmorVisual.Missing(itemId, null, ItemSlot.none, "armor_not_in_item_table");

        return Resolve(armor, race);
    }
}
