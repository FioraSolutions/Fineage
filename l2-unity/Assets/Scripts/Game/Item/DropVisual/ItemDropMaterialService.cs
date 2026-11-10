using UnityEngine;

public sealed partial class ItemDropMaterialService
{
    const string CoinTex0Resource = "Data/SysTextures/LineageEffectsTextures/cbui24";
    const string CoinTex1Resource = "Data/SysTextures/LineageEffectsTextures/cbui25";
    const string DropMaskedShaderName = "L2/Items/DropMeshMasked";

    readonly ItemTable _items;

    public ItemDropMaterialService(ItemTable items)
    {
        _items = items;
    }







    Texture2D LoadArmorDropAlbedo(int itemId, Abstractgrp grp)
    {
        Armorgrp armor = ResolveArmorgrp(itemId, grp);
        if (armor != null && armor.FirstTexture != null)
        {
            int magic = (int)CharacterRaceAnimation.MMagic;
            if (magic >= 0 && magic < armor.FirstTexture.Length)
            {
                Texture2D magicTex = LoadGrpAlbedo(armor.FirstTexture[magic]);
                if (magicTex != null)
                    return magicTex;
            }

            for (int i = 0; i < armor.FirstTexture.Length; i++)
            {
                Texture2D tex = LoadGrpAlbedo(armor.FirstTexture[i]);
                if (tex != null)
                    return tex;
            }
        }

        Texture2D dropTex = LoadGrpAlbedo(grp != null ? grp.DropTexture : null);
        if (dropTex != null)
            return dropTex;
        return LoadItemIcon(grp);
    }

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
