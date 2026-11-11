using UnityEngine;
using System;

public sealed partial class ItemDropPrefabLoader
{
    public ItemDropPrefabLoader(ItemDropGrpCatalog grp, ModelTable models)
    {
        _grp = grp;
        _models = models;
    }

    public GameObject Resolve(int itemId)
    {
        ModelTable models = _models != null ? _models : TablePacketServices.Models;
        Abstractgrp grp = _grp.ResolveGrp(itemId);
        if (grp != null && !string.IsNullOrEmpty(grp.DropModel) &&
            !grp.DropModel.Equals("None", System.StringComparison.OrdinalIgnoreCase))
        {
            GameObject drop = models.GetOrLoadModel(grp.DropModel);
            if (drop != null)
                return drop;

            GameObject coinFallback = LoadCoinDropFallback(models, grp.DropModel);
            if (coinFallback != null)
                return coinFallback;

            GameObject propFallback = LoadDropPropFallback(models, grp.DropModel);
            if (propFallback != null)
                return propFallback;
        }

        string equipModel = _grp.ResolveEquipModel(grp);
        if (!string.IsNullOrEmpty(equipModel))
        {
            GameObject equip = models.GetOrLoadModel(equipModel);
            if (equip != null)
                return equip;
        }

        if (_grp.IsHerb(itemId))
            return models.GetOrLoadModel(PotionFallbackModel);

        return null;
    }

    public static bool IsCoinPrefab(GameObject prefab)
    {
        string name = prefab.name;
        return name.IndexOf("coin00", System.StringComparison.OrdinalIgnoreCase) >= 0 ||
               name.IndexOf("coin01", System.StringComparison.OrdinalIgnoreCase) >= 0;
    }
}
