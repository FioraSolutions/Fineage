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
}
