
using UnityEngine;

public abstract partial class AbstractServerItem
{
    public ItemSets[] GetSetsEffects()
    {
        ItemName item = TablePacketServices.ItemNames.GetItemName(itemId);
        return item.GetSetsEffect();
    }

    public string GetTypeArmorName()
    {
        Armorgrp armor = TablePacketServices.ArmorGrps.GetArmor(itemId);

        if (armor != null)
        {
            // Debug.Log("Body part test > " + armor.BodyPart);
            if (armor.BodyPart == ItemSlot.legs)
            {
                if (armor.ArmorType == ArmorType.light) return "Lower Body / Light";
                if (armor.ArmorType == ArmorType.heavy) return "Lower Body / Heavy";

            }
            else if (armor.BodyPart == ItemSlot.chest)
            {
                if (armor.ArmorType == ArmorType.light) return "Upper Body / Light";
                if (armor.ArmorType == ArmorType.heavy) return "Upper Body / Heavy";
            }
            else if (armor.BodyPart == ItemSlot.boots)
            {
                return "Boots";
            }
            else if (armor.BodyPart == ItemSlot.feet)
            {
                return "Boots";
            }
            else if (armor.BodyPart == ItemSlot.head)
            {
                return "Helmet";
            }
            else if (armor.BodyPart == ItemSlot.fullarmor)
            {
                return "Fullbody / Heavy";
            }
            else if (armor.BodyPart == ItemSlot.gloves)
            {
                return "Gloves";
            }
        }


        return "";
    }

    public string GetTypeWeaponName()
    {
        Weapongrp weapon = TablePacketServices.WeaponGrps.GetWeapon(itemId);

        if (weapon != null && weapon.WeaponType == WeaponType.shield)
            return "Shield";


        return "";
    }

    public bool IsBow() => TablePacketServices.WeaponGrps.GetWeapon(itemId)?.WeaponType == WeaponType.bow;
}
