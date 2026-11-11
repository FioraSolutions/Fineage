using UnityEngine;

public sealed partial class ItemDropMaterialService
{
    Material LoadArmorDropMaterial(int itemId, Abstractgrp grp)
    {
        Armorgrp armor = ResolveArmorgrp(itemId, grp);
        if (armor == null || armor.FirstTexture == null)
            return LoadGrpMaterial(grp != null ? grp.DropTexture : null);

        int magic = (int)CharacterRaceAnimation.MMagic;
        if (magic >= 0 && magic < armor.FirstTexture.Length)
        {
            Material mat = LoadGrpMaterial(armor.FirstTexture[magic]);
            if (mat != null)
                return mat;
        }

        for (int i = 0; i < armor.FirstTexture.Length; i++)
        {
            Material mat = LoadGrpMaterial(armor.FirstTexture[i]);
            if (mat != null)
                return mat;
        }

        return LoadGrpMaterial(grp != null ? grp.DropTexture : null);
    }

    Armorgrp ResolveArmorgrp(int itemId, Abstractgrp grp)
    {
        Armorgrp armor = grp as Armorgrp;
        if (armor != null)
            return armor;

        ItemTable items = _items != null ? _items : TablePacketServices.Items;
        if (items == null)
            return null;
        Armor item = items.GetArmor(itemId);
        return item != null ? item.Armorgrp : null;
    }
}
