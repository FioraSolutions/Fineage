using System.Collections.Generic;
using UnityEngine;
using static ModelTable;

/// <summary>
/// Single entry for body-armor visuals: item + race → slot + mesh/material parts.
/// ModelTable stays the Resources cache; this type only resolves and shapes the result.
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

        L2ArmorPiece piece = _models.GetArmorPiece(armor, race);
        if (piece == null)
            return ArmorVisual.Missing(armor.Id, armor, slot, "mesh_or_material");

        ArmorVisualPart[] parts = BuildParts(slot, piece);
        if (parts.Length == 0)
            return ArmorVisual.Missing(armor.Id, armor, slot, "no_mesh_parts");

        return ArmorVisual.Ok(armor, slot, parts, piece);
    }

    public static ItemSlot ResolveBodySlot(Armor armor)
    {
        ItemSlot body = armor != null && armor.Armorgrp != null
            ? armor.Armorgrp.BodyPart
            : ItemSlot.none;
        return ArmorDresserModel.GetExtendedArmorPart(body);
    }

    public static bool IsBodySlot(ItemSlot slot)
    {
        return slot == ItemSlot.fullarmor
            || slot == ItemSlot.chest
            || slot == ItemSlot.legs
            || slot == ItemSlot.gloves
            || slot == ItemSlot.feet;
    }

    static ArmorVisualPart[] BuildParts(ItemSlot slot, L2ArmorPiece piece)
    {
        if (slot == ItemSlot.fullarmor)
            return BuildFullArmorParts(piece);

        if (piece.baseArmorModel == null || piece.material == null)
            return System.Array.Empty<ArmorVisualPart>();

        return new[]
        {
            new ArmorVisualPart(piece.baseArmorModel, piece.material, slot)
        };
    }

    static ArmorVisualPart[] BuildFullArmorParts(L2ArmorPiece piece)
    {
        GameObject[] models = piece.baseAllModels;
        if (models == null || models.Length == 0)
        {
            if (piece.baseArmorModel == null)
                return System.Array.Empty<ArmorVisualPart>();
            models = new[] { piece.baseArmorModel };
        }

        Material[] materials = piece.allMaterials;
        var list = new List<ArmorVisualPart>(models.Length);
        for (int i = 0; i < models.Length; i++)
        {
            GameObject mesh = models[i];
            if (mesh == null)
                continue;

            Material mat = null;
            if (materials != null && i < materials.Length)
                mat = materials[i];
            if (mat == null && materials != null && materials.Length > 0)
                mat = materials[0];
            if (mat == null)
                mat = piece.material;
            if (mat == null)
                continue;

            ItemSlot partSlot = i == 0 ? ItemSlot.chest : ItemSlot.legs;
            list.Add(new ArmorVisualPart(mesh, mat, partSlot));
        }

        return list.ToArray();
    }
}
