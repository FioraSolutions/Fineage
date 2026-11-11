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
}
