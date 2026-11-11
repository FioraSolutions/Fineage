using System;
using System.Collections.Generic;
using UnityEngine;

public partial class ItemTable
{
    public void Initialize()
    {
        TablePacketServices.Bind(items: this);
        FillDataToLoad();
    }

    private void OnDestroy()
    {
        _weapons.Clear();
        _armors.Clear();
        _instance = null;
    }

    public void CacheItems()
    {
        CacheWeapons();
        CacheArmors();
        CacheEtcItems();
    }

    private void FillDataToLoad()
    {
        weaponsToLoad = new List<int> { 1, 2, 3, 4, 5, 6, 7, 10, 14, 89, 102, 129, 177, 188, 156, 275, 2369, 2370, 5284, 20 };
        armorsToLoad = new List<int> { NAKED_CHEST, NAKED_LEGS, NAKED_GLOVES, NAKED_BOOTS, 425, 461, 1146, 1147, 45, 118, 112, 116, 26};
        itemsToLoad = new List<int>() { 1835, 3947, 2509, 57, 736 };
    }

    public bool ShouldLoadItem(int id)
    {
        if (_loadAll) return true;

        FillDataToLoad(); //TODO: Remove
        if (weaponsToLoad.Count == 0 && armorsToLoad.Count == 0 && itemsToLoad.Count == 0)
        {
            return true;
        }

        if (weaponsToLoad.Contains(id))
        {
            return true;
        }
        if (armorsToLoad.Contains(id))
        {
            return true;
        }
        if (itemsToLoad.Contains(id))
        {
            return true;
        }

        return false;
    }

    private void CacheWeapons()
    {
        foreach (KeyValuePair<int, Weapongrp> kvp in TablePacketServices.WeaponGrps.WeaponGrps)
        {
            if (_armors.ContainsKey(kvp.Key))
            {
                continue;
            }

            Weapongrp weapongrp = kvp.Value;
            ItemName itemName = TablePacketServices.ItemNames.GetItemName(kvp.Key);
            ItemStatData itemStatData = TablePacketServices.ItemStats.GetItemStatData(kvp.Key);

            Weapon weapon = new Weapon(kvp.Key, itemName, itemStatData, weapongrp);

            if (!_weapons.ContainsKey(kvp.Key))
            {
                _weapons.Add(kvp.Key, weapon);
            }

        }
    }
}
