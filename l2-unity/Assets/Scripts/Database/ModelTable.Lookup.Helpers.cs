using System;
using System.Collections.Generic;
using UnityEngine;

public partial class ModelTable : AbstractCache
{
    private void AddArmorMaterials(Armorgrp grp, int raceIndex, L2Armor l2Armor, ref int materialCount)
    {
        string texture = grp.FirstTexture[raceIndex];
        if (l2Armor.materials.ContainsKey(texture))
            return;

        if (NeedsExtraArmorModels(l2Armor, grp.AllModels[raceIndex], grp.BodyPart))
        {
            l2Armor.allModels = LoadAllArmorModels(grp.AllModels[raceIndex]);
            // Debug.Log("Reloading all models for " + l2Armor.baseModel.name + ", loading " + l2Armor.allModels.Length + " models");
        }

        Material material = LoadArmorMaterial(texture);
        if (material == null)
            return;

        Material[] fullSet = LoadAllArmorMaterials(grp.AllTextures[raceIndex]);
        if (grp.BodyPart == ItemSlot.fullarmor)
        {
            // Keep at least the chest material when legs texture is None.
            if (fullSet == null || fullSet.Length == 0 || fullSet[0] == null)
                fullSet = new[] { material };
            else if (fullSet[0] == null)
                fullSet[0] = material;
            l2Armor.allMaterials[texture] = fullSet;
        }

        l2Armor.materials[texture] = material;
        materialCount++;
    }

    private static bool NeedsExtraArmorModels(L2Armor l2Armor, List<string> models, ItemSlot slot)
    {
        if (l2Armor.allModels == null || l2Armor.allModels.Length == 0)
            return models != null && models.Count > 0;
        return l2Armor.allModels[0] == null ||
               (models != null && l2Armor.allModels.Length < models.Count && slot == ItemSlot.fullarmor);
    }

    private void CacheNpcs()
    {
        _npcs = new Dictionary<string, L2Npc>();
        int success = 0;

        foreach (KeyValuePair<int, Npcgrp> pair in TablePacketServices.NpcGrps.Npcgrps)
        {
            string mesh = pair.Value.Mesh;
            if (_npcs.ContainsKey(mesh))
                continue;

            GameObject npc = LoadNpc(mesh);
            if (npc == null)
                continue;

            Dictionary<string, Material[]> materials = new Dictionary<string, Material[]>();
            materials[mesh] = LoadAllMaterials(new List<string>(pair.Value.Materials));
            _npcs[mesh] = new L2Npc(npc, materials);
            success++;
        }

        // Debug.Log("Loaded " + success + " npc model(s).");
    }

    /// <summary>
    /// Race fallbacks ask for a mesh no npcgrp row uses (Dark Elf teleporter → Human in the
    /// same package). <see cref="GetNpc"/> only sees the boot cache, so load the prefab once.
    /// </summary>
    public GameObject GetOrLoadNpc(string meshname)
    {
        GameObject cached = GetNpc(meshname);
        if (cached != null)
            return cached;
        if (string.IsNullOrEmpty(meshname) || _npcs == null)
            return null;

        GameObject loaded = LoadNpc(meshname);
        if (loaded == null)
            return null;

        _npcs[meshname] = new L2Npc(loaded, new Dictionary<string, Material[]>());
        return GetNpc(meshname);
    }
}
