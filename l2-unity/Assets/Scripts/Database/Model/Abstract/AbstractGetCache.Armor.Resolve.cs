using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VContainer;

public abstract partial class AbstractGetCache
{
    public bool TryGetBodyArmorParts(
        Armor armor,
        CharacterRaceAnimation raceId,
        ItemSlot bodySlot,
        out BodyArmorMeshPart[] parts)
    {
        parts = Array.Empty<BodyArmorMeshPart>();
        L2ArmorPiece piece = GetArmorPiece(armor, raceId);
        if (piece == null)
            return false;

        parts = BuildBodyArmorParts(bodySlot, piece);
        return parts.Length > 0;
    }

    L2ArmorPiece GetArmorPiece(Armor armor, CharacterRaceAnimation raceId)
    {
        if (armor == null)
            return null;

        if (armor.Armorgrp == null || armor.Armorgrp.FirstModel == null)
            return null;

        byte raceIndex = (byte)raceId;
        if (raceIndex >= armor.Armorgrp.FirstModel.Length)
            return null;

        string model = armor.Armorgrp.FirstModel[raceIndex];
        if (string.IsNullOrEmpty(model))
            return null;

        if (!_armors.TryGetValue(model, out var l2Armor))
            return null;

        if (l2Armor.baseModel == null)
            return null;

        string textureName = GetValidTextureName(armor, raceId);
        if (textureName == null)
            return null;

        if (!TryGetMaterials(l2Armor, textureName, out var firstMaterial, out var allMaterials))
            return null;

        return new L2ArmorPiece(l2Armor.baseModel, firstMaterial, l2Armor.allModels, allMaterials);
    }

    static BodyArmorMeshPart[] BuildBodyArmorParts(ItemSlot bodySlot, L2ArmorPiece piece)
    {
        if (bodySlot == ItemSlot.fullarmor)
            return BuildFullArmorParts(piece);

        if (piece.baseArmorModel == null || piece.material == null)
            return Array.Empty<BodyArmorMeshPart>();

        return new[]
        {
            new BodyArmorMeshPart(piece.baseArmorModel, piece.material, bodySlot)
        };
    }

    static BodyArmorMeshPart[] BuildFullArmorParts(L2ArmorPiece piece)
    {
        GameObject[] models = piece.baseAllModels;
        if (models == null || models.Length == 0)
        {
            if (piece.baseArmorModel == null)
                return Array.Empty<BodyArmorMeshPart>();
            models = new[] { piece.baseArmorModel };
        }

        Material[] materials = piece.allMaterials;
        var list = new List<BodyArmorMeshPart>(models.Length);
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
            list.Add(new BodyArmorMeshPart(mesh, mat, partSlot));
        }

        return list.ToArray();
    }
}
