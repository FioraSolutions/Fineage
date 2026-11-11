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

    private string GetValidTextureName(Armor armor, CharacterRaceAnimation raceId)
    {
        byte raceIndex = (byte)raceId;
        string texture = null;
        string[] textureArray = armor.Armorgrp.FirstTexture;
        if (textureArray != null && raceIndex < textureArray.Length)
            texture = textureArray[raceIndex];

        // Onepiece robes: FirstTexture used to be wrongly parsed as "None" (legs slot).
        // Fall back to the first real entry in AllTextures for this race.
        if (IsMissingArmorToken(texture) &&
            armor.Armorgrp.AllTextures != null &&
            raceIndex < armor.Armorgrp.AllTextures.Count)
        {
            List<string> all = armor.Armorgrp.AllTextures[raceIndex];
            if (all != null)
            {
                for (int i = 0; i < all.Count; i++)
                {
                    if (!IsMissingArmorToken(all[i]))
                    {
                        texture = all[i];
                        break;
                    }
                }
            }
        }

        return IsMissingArmorToken(texture) ? null : texture;
    }

    static bool IsMissingArmorToken(string token)
    {
        return string.IsNullOrEmpty(token) ||
               string.Equals(token, "None", StringComparison.OrdinalIgnoreCase);
    }

    private bool TryGetMaterials(L2Armor l2Armor, string textureName, out Material firstMaterial, out Material[] allMaterials)
    {
        firstMaterial = null;
        allMaterials = null;

        if (l2Armor.materials == null)
            l2Armor.materials = new Dictionary<string, Material>();

        l2Armor.materials.TryGetValue(textureName, out firstMaterial);
        if (firstMaterial == null)
        {
            firstMaterial = LoadArmorMaterial(textureName);
            if (firstMaterial != null)
                l2Armor.materials[textureName] = firstMaterial;
        }

        if (l2Armor.allMaterials != null)
            l2Armor.allMaterials.TryGetValue(textureName, out allMaterials);

        if (firstMaterial == null)
        {
            // Debug.LogWarning($"Can't find armor material for model: {textureName} in ModelTable");
            return false;
        }

        // Onepiece robes ship texture={t00;[None]} — cache may omit allMaterials when the
        // first LoadArmorMaterial failed earlier. Synthesize so EquipFullArmor still dresses.
        if (allMaterials == null || allMaterials.Length == 0)
        {
            allMaterials = new[] { firstMaterial };
            if (l2Armor.allMaterials == null)
                l2Armor.allMaterials = new Dictionary<string, Material[]>();
            l2Armor.allMaterials[textureName] = allMaterials;
        }

        return true;
    }

    protected Material LoadArmorMaterial(string texture)
    {
        if (string.IsNullOrEmpty(texture))
            return null;
        if (string.Equals(texture, "None", StringComparison.OrdinalIgnoreCase))
            return null;

        string[] parts = texture.Split('.');
        if (parts.Length < 2)
            return null;

        string folder = parts[0];
        string file = parts[1];
        // Dat packs use lowercase packages (mmagic.mmagic_m007_t90_u); Resources
        // folders are PascalCase (MMagic/Materials/MMagic_m007_t90_u). Unity
        // Resources.Load is case-sensitive even on macOS APFS.
        string canonFolder = CanonicalSysTexturePackage(folder);
        string canonFile = file;
        if (!string.IsNullOrEmpty(folder) &&
            file.StartsWith(folder, StringComparison.OrdinalIgnoreCase))
            canonFile = canonFolder + file.Substring(folder.Length);

        string[] folders =
        {
            folder,
            canonFolder,
            folder.Replace("NPCs", "Npcs"),
            folder.Replace("Npcs", "NPCs")
        };
        string[] files = { file, canonFile };

        for (int f = 0; f < folders.Length; f++)
        {
            string fold = folders[f];
            if (string.IsNullOrEmpty(fold))
                continue;
            for (int n = 0; n < files.Length; n++)
            {
                string asset = files[n];
                if (string.IsNullOrEmpty(asset))
                    continue;
                string[] candidates =
                {
                    $"Data/SysTextures/{fold}/Materials/{asset}",
                    $"Data/SysTextures/{fold}/Materials/{fold}.{asset}",
                    $"Data/SysTextures/{fold}/Materials/{texture}",
                    // Fresh NPC imports often drop .mat next to the TGA, not under Materials/.
                    $"Data/SysTextures/{fold}/{asset}",
                    $"Data/SysTextures/{fold}/{texture}"
                };

                for (int i = 0; i < candidates.Length; i++)
                {
                    Material material = Resources.Load<Material>(candidates[i]);
                    if (material != null)
                        return material;
                }
            }
        }

        return null;
    }
}
