using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VContainer;

public abstract partial class AbstractGetCache
{
    public GameObject GetEtcItem(string model)
    {
        if (!_etcItems.ContainsKey(model))
        {
            // Debug.LogWarning($"Can't find weapon model {model} in ModelTable");
            return null;
        }

        GameObject go = _etcItems[model];
        if (go == null)
        {
            // Debug.LogWarning($"Can't find weapon model {model} in ModelTable");
            return null;
        }

        return go;
    }

    public GameObject TryGetCachedItemModel(string model)
    {
        if (string.IsNullOrEmpty(model))
            return null;
        if (_weapons != null && _weapons.TryGetValue(model, out GameObject weapon) && weapon != null)
            return weapon;
        if (_etcItems != null && _etcItems.TryGetValue(model, out GameObject etc) && etc != null)
            return etc;
        if (_loadedModels != null && _loadedModels.TryGetValue(model, out GameObject extra) && extra != null)
            return extra;
        return null;
    }

    public GameObject GetOrLoadModel(string model)
    {
        if (string.IsNullOrEmpty(model) ||
            model.Equals("None", StringComparison.OrdinalIgnoreCase))
            return null;

        GameObject cached = TryGetCachedItemModel(model);
        if (cached != null)
            return cached;

        GameObject loaded = LoadUkxPrefab(model);
        if (loaded == null)
            return null;

        if (_loadedModels == null)
            _loadedModels = new Dictionary<string, GameObject>();
        _loadedModels[model] = loaded;
        return loaded;
    }

    public GameObject GetDropPrefab(int itemId)
    {
        if (App.GameContainer != null)
            return App.GameContainer.Resolve<ItemDropPrefabLoader>().Resolve(itemId);

        Abstractgrp grp = ResolveDropGrp(itemId);
        if (grp != null && !string.IsNullOrEmpty(grp.DropModel))
        {
            GameObject drop = GetOrLoadModel(grp.DropModel);
            if (drop != null)
                return drop;
        }

        string equip = grp is Weapongrp weapon ? weapon.Model
            : grp is EtcItemgrp etc ? etc.Model
            : null;
        return GetOrLoadModel(equip);
    }

    static Abstractgrp ResolveDropGrp(int itemId)
    {
        ItemTable items = TablePacketServices.Items;
        if (items == null)
            return null;
        Weapon weapon = items.GetWeapon(itemId);
        if (weapon != null && weapon.Weapongrp != null)
            return weapon.Weapongrp;
        Armor armor = items.GetArmor(itemId);
        if (armor != null && armor.Armorgrp != null)
            return armor.Armorgrp;
        EtcItem etc = items.GetEtcItem(itemId);
        return etc != null ? etc.EtcItemgrp : null;
    }
}
