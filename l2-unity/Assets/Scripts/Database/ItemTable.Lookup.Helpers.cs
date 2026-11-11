using System;
using System.Collections.Generic;
using UnityEngine;

public partial class ItemTable
{
    public Armor GetArmor(int id)
    {
        Armor armor;
        _armors.TryGetValue(id, out armor);

        return armor;
    }

    public EtcItem GetEtcItem(int id)
    {
        EtcItem item;
        _etcItems.TryGetValue(id, out item);

        return item;
    }

    public AbstractItem GetItem(int id)
    {
        if (_weapons.TryGetValue(id, out Weapon weapon))
        {
            return weapon;
        }
        if (_armors.TryGetValue(id, out Armor armor))
        {
            return armor;
        }
        if (_etcItems.TryGetValue(id, out EtcItem etcItem))
        {
            return etcItem;
        }

        return null;
    }
}
