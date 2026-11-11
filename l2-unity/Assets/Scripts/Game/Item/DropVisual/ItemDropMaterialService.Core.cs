using UnityEngine;

public sealed partial class ItemDropMaterialService
{
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
}
