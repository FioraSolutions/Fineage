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
}
