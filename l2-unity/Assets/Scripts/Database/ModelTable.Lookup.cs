using System;
using System.Collections.Generic;
using UnityEngine;

public partial class ModelTable : AbstractCache
{
    private void CacheEtcItems()
    {
        _etcItems = LoadItemModels(
            TablePacketServices.Items.EtcItems,
            etc => etc.EtcItemgrp != null ? etc.EtcItemgrp.Model : null,
            "EtcItemgrp");
    }

    private void CacheArmors()
    {
        _armors = new Dictionary<string, L2Armor>();
        int materialCount = 0;

        foreach (KeyValuePair<int, Armor> pair in TablePacketServices.Items.Armors)
        {
            Armorgrp grp = pair.Value.Armorgrp;
            if (grp.BodyPart == ItemSlot.alldress)
                continue;

            for (int race = 0; race < RACE_COUNT; race++)
            {
                string model = grp.FirstModel[race];
                if (string.IsNullOrEmpty(model))
                {
                    // Debug.LogWarning("Model string is null for race " + (CharacterRaceAnimation)race + " in armor " + pair.Key);
                    continue;
                }

                L2Armor l2Armor = GetOrCreateArmor(model, grp.AllModels[race]);
                if (l2Armor == null || l2Armor.baseModel == null)
                    continue;

                AddArmorMaterials(grp, race, l2Armor, ref materialCount);
            }
        }

        // Debug.Log("Successfully loaded " + _armors.Count + " armor model(s).");
        // Debug.Log("Successfully loaded " + materialCount + " armor material(s).");
    }

    private L2Armor GetOrCreateArmor(string model, List<string> extraModels)
    {
        L2Armor existing;
        if (_armors.TryGetValue(model, out existing))
            return existing;

        L2Armor created = new L2Armor
        {
            baseModel = LoadArmorModel(model),
            allModels = LoadAllArmorModels(extraModels)
        };

        if (created.baseModel == null)
            return created;

        created.materials = new Dictionary<string, Material>();
        created.allMaterials = new Dictionary<string, Material[]>();
        _armors[model] = created;
        return created;
    }

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
}
