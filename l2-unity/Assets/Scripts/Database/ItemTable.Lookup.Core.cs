using System;
using System.Collections.Generic;
using UnityEngine;

public partial class ItemTable
{
    private void CacheArmors()
    {
        foreach (KeyValuePair<int, Armorgrp> kvp in TablePacketServices.ArmorGrps.ArmorGrps)
        {
            if (_armors.ContainsKey(kvp.Key))
            {
                continue;
            }

            if(kvp.Key == 356)
            {
                // Debug.Log("");
            }

            Armorgrp armorgrp = kvp.Value;
            ItemName itemName = TablePacketServices.ItemNames.GetItemName(kvp.Key);
            ItemStatData itemStatData = TablePacketServices.ItemStats.GetItemStatData(kvp.Key);

            Armor armor = new Armor(kvp.Key, itemName, itemStatData, armorgrp);
            _armors.Add(kvp.Key, armor);
        }
    }

    private void CacheEtcItems()
    {
        foreach (KeyValuePair<int, EtcItemgrp> kvp in TablePacketServices.EtcItemGrps.EtcItemGrps)
        {
            if (kvp.Key == 7832)
            {
                // Debug.Log("");
            }

            if (_etcItems.ContainsKey(kvp.Key))
            {
                continue;
            }

            EtcItemgrp itemgrp = kvp.Value;
            ItemName itemName = TablePacketServices.ItemNames.GetItemName(kvp.Key);
            ItemStatData itemStatData = TablePacketServices.ItemStats.GetItemStatData(kvp.Key);

            EtcItem item = new EtcItem(kvp.Key, itemName, itemStatData, itemgrp);
            _etcItems.Add(kvp.Key, item);
        }
    }

    public Weapon GetWeapon(int id)
    {
        Weapon weapon;
        _weapons.TryGetValue(id, out weapon);

        return weapon;
    }
}
