using UnityEngine;

public abstract partial class AbstractServerItem
{
    public void SetItemId(int itemId)
    {
        this.itemId = itemId;
    }

    public Armorgrp GetArmor()
    {
        return TablePacketServices.ArmorGrps.GetArmor(itemId);
    }

    public Weapongrp GetWeapon()
    {
        return TablePacketServices.WeaponGrps.GetWeapon(itemId);
    }

    public EtcItemgrp GetEtcItem()
    {
        return TablePacketServices.EtcItemGrps.GetEtcItem(itemId);
    }

    public ConsumeCategory GetConsumeCategory()
    {
        EtcItemgrp etcgrp = TablePacketServices.EtcItemGrps.GetEtcItem(itemId);
        if (etcgrp != null) return etcgrp.ConsumeType;
        return ConsumeCategory.Normal;
    }

    public string GetTypeAccessoriesName()
    {
        Armorgrp armor = TablePacketServices.ArmorGrps.GetArmor(itemId);

        if (armor != null)
        {
            if (armor.BodyPart == ItemSlot.neck)
            {
                return "Necklace";
            }
            else if (armor.BodyPart == ItemSlot.rear | armor.BodyPart == ItemSlot.lear)
            {
                return "Earring";
            }
            else if (armor.BodyPart == ItemSlot.rfinger | armor.BodyPart == ItemSlot.lfinger)
            {
                return "Ring";
            }
        }


        return "";
    }

    public void SetMultiSell(bool data)
    {
        _isMultisellData = data;
    }

    public string GetItemDescription()
    {
        ItemName item = TablePacketServices.ItemNames.GetItemName(itemId);
        if (string.IsNullOrEmpty(item.Description) && !_isMultisellData) return "Not Found description";
        return item.Description;
    }

    public ItemName[] GetSets()
    {
        ItemName item = TablePacketServices.ItemNames.GetItemName(itemId);
        return item.GetSetsName();
    }
}
