using UnityEngine;

/// <summary>
/// Single entry for body-armor visuals: item + race → slot + mesh/material parts.
/// ModelTable stays the Resources cache; this type only shapes the result for gear.
/// </summary>
public sealed class ArmorVisualResolver
{
    readonly ModelTable _models;
    readonly ItemTable _items;

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

    public ArmorVisual Resolve(Armor armor, CharacterRaceAnimation race)
    {
        if (armor == null)
            return ArmorVisual.Missing(0, null, ItemSlot.none, "armor_null");

        ItemSlot slot = ResolveBodySlot(armor);
        if (!IsBodySlot(slot))
            return ArmorVisual.Missing(armor.Id, armor, slot, "not_body_slot");

        if (_models == null)
            return ArmorVisual.Missing(armor.Id, armor, slot, "model_table_null");

        if (!_models.TryGetBodyArmorParts(armor, race, slot, out var cachedParts) ||
            cachedParts == null ||
            cachedParts.Length == 0)
            return ArmorVisual.Missing(armor.Id, armor, slot, "mesh_or_material");

        var parts = new ArmorVisualPart[cachedParts.Length];
        for (int i = 0; i < cachedParts.Length; i++)
        {
            AbstractGetCache.BodyArmorMeshPart cached = cachedParts[i];
            parts[i] = new ArmorVisualPart(cached.Mesh, cached.Material, cached.PartSlot);
        }

        return ArmorVisual.Ok(armor, slot, parts);
    }

    public static ItemSlot ResolveBodySlot(Armor armor)
    {
        ItemSlot body = armor != null && armor.Armorgrp != null
            ? armor.Armorgrp.BodyPart
            : ItemSlot.none;
        return ArmorBodySlots.Normalize(body);
    }

    public static bool IsBodySlot(ItemSlot slot)
    {
        return ArmorBodySlots.IsBody(slot);
    }
}
