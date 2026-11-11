using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VContainer;

public abstract partial class AbstractGetCache
{
    protected static string CanonicalSysTexturePackage(string folder)
    {
        if (string.IsNullOrEmpty(folder))
            return folder;
        switch (folder.ToLowerInvariant())
        {
            case "mmagic": return "MMagic";
            case "fmagic": return "FMagic";
            case "mfighter": return "MFighter";
            case "ffighter": return "FFighter";
            case "melf": return "MElf";
            case "felf": return "FElf";
            case "mdarkelf": return "MDarkElf";
            case "fdarkelf": return "FDarkElf";
            case "mdwarf": return "MDwarf";
            case "fdwarf": return "FDwarf";
            case "morc": return "MOrc";
            case "forc": return "FOrc";
            case "mshaman": return "MShaman";
            case "fshaman": return "FShaman";
            case "dropitemstex": return "DropItemsTex";
            case "lineagenpcstex": return "LineageNpcsTex";
            case "lineageweaponstex": return "LineageWeaponsTex";
            default:
                return char.ToUpperInvariant(folder[0]) + folder.Substring(1);
        }
    }

    protected GameObject LoadUkxPrefab(string model)
    {
        if (string.IsNullOrEmpty(model))
            return null;

        string[] parts = model.Split('.');
        if (parts.Length < 2)
            return null;

        string pkg = parts[0];
        string asset = parts[1];

        GameObject go = Resources.Load<GameObject>($"Data/Animations/{pkg}/{asset}");
        if (go != null)
            return go;

        go = Resources.Load<GameObject>($"Data/Animations/{pkg}/{asset}/{asset}");
        if (go != null)
            return go;

        go = Resources.Load<GameObject>($"Data/StaticMeshes/{pkg}/{asset}");
        if (go != null)
            return go;

        return Resources.Load<GameObject>($"Data/StaticMeshes/{pkg}/{asset}/{asset}");
    }

    public GameObject GetWeaponById(int itemId)
    {
        Weapon weapon = TablePacketServices.Items.GetWeapon(itemId);
        if (weapon == null)
        {
            // Debug.LogWarning($"Can't find weapon {itemId} in ItemTable");
        }

        return GetWeapon(weapon.Weapongrp.Model);
    }

    public GameObject GetEtcById(int itemId)
    {
        EtcItem etcItem = TablePacketServices.Items.GetEtcItem(itemId);
        if (etcItem == null)
        {
            // Debug.LogWarning($"Can't find etcItem  {itemId} in ItemTable");
        }

        return GetEtcItem(etcItem.EtcItemgrp.Model);
    }

    public GameObject GetWeapon(string model)
    {
        if (!_weapons.ContainsKey(model))
        {
            // Debug.LogWarning($"Can't find weapon model {model} in ModelTable");
            return null;
        }

        GameObject go = _weapons[model];
        if (go == null)
        {
            // Debug.LogWarning($"Can't find weapon model {model} in ModelTable");
            return null;
        }

        return go;
    }

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
