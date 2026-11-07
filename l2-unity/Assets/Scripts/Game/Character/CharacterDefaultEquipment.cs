using System;
using UnityEngine;
using static ModelTable;

public class CharacterDefaultEquipment
{

    public static void EquipStarterGear(UserGear gear, PlayerAppearance appearance)
    {
        gear.SyncEquippedArmor(appearance);
        // Same path as Entity.EquipAllWeapons / world spawn — dual fist / lrhand
        // only equips both meshes when SyncEquippedWeapons sees WeaponType.fist.
        gear.SyncEquippedWeapons(appearance.RHand, appearance.LHand);
    }



    public static int[] GetDefaultItemsByItemSlot(ItemSlot slot)
    {
        return slot switch
        {
            ItemSlot.chest => new int[] { ItemTable.NAKED_CHEST },
            ItemSlot.legs => new int[] { ItemTable.NAKED_LEGS },
            ItemSlot.gloves => new int[] { ItemTable.NAKED_GLOVES },
            ItemSlot.feet => new int[] { ItemTable.NAKED_BOOTS },
            ItemSlot.fullarmor => new int[] { ItemTable.NAKED_CHEST , ItemTable.NAKED_LEGS },
            _ => new int[0] // Return empty array for unknown slots
        };
    }


    public static Armor[] GetDefaultArmorByItemSlot(ItemSlot slot)
    {

        int[] itemIds = GetDefaultItemsByItemSlot(slot);


        if (itemIds == null || itemIds.Length == 0)
        {
            return Array.Empty<Armor>();
        }


        Armor[] armors = new Armor[itemIds.Length];
        for (int i = 0; i < itemIds.Length; i++)
        {
            int itemId = itemIds[i];
            armors[i] = ItemTable.Instance.GetArmor(itemId);
        }


        return armors ?? Array.Empty<Armor>();
    }


}
