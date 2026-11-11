using System;
using System.Collections.Generic;
using UnityEngine;

public partial class ItemDropPresentationService
{
    Entity ResolveEntity(int objectId)
    {
        World world = GameWorld;
        if (objectId == 0 || world == null)
            return null;
        return world.GetEntityNoLockSync(objectId);
    }

    ItemDropVisualKind ResolveVisualKind(ItemEntity item)
    {
        if (item == null)
            return ItemDropVisualKind.Generic;

        int itemId = item.ItemId;
        if (_grp.IsAdenaDropVisual(itemId))
            return ItemDropVisualKind.Adena;

        ItemTable table = TablePacketServices.Items;
        if (table == null)
            return item.Stackable ? ItemDropVisualKind.EtcStackable : ItemDropVisualKind.Generic;
        if (table.GetWeapon(itemId) != null)
            return ItemDropVisualKind.Weapon;

        EtcItem etc = table.GetEtcItem(itemId);
        if (etc != null && etc.EtcItemgrp != null)
        {
            ConsumeCategory consume = etc.EtcItemgrp.ConsumeType;
            if (item.Stackable || consume == ConsumeCategory.Stackable)
                return ItemDropVisualKind.EtcStackable;
        }
        else if (item.Stackable)
        {
            return ItemDropVisualKind.EtcStackable;
        }

        if (table.GetArmor(itemId) != null)
            return ItemDropVisualKind.Armor;

        return ItemDropVisualKind.Generic;
    }

}
