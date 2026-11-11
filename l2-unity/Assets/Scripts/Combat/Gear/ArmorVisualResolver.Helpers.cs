using UnityEngine;

public sealed partial class ArmorVisualResolver
{
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
