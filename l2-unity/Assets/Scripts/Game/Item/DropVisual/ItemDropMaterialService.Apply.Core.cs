using UnityEngine;

public sealed partial class ItemDropMaterialService
{
    public void ApplyCoin(GameObject visual)
    {
        Texture2D tex0 = Resources.Load<Texture2D>(CoinTex0Resource);
        Texture2D tex1 = Resources.Load<Texture2D>(CoinTex1Resource);
        ApplyDropMaskedTextures(visual, tex0, tex1);
    }

    public void ApplyDropItems(GameObject visual, int itemId, Abstractgrp grp)
    {
        Material source = LoadGrpMaterial(grp != null ? grp.DropTexture : null);
        if (source == null)
            source = LoadArmorDropMaterial(itemId, grp);
        if (source != null)
        {
            ApplyClonedMaterials(visual, source);
            return;
        }

        Texture2D albedo = LoadGrpAlbedo(grp != null ? grp.DropTexture : null);
        if (albedo == null)
            albedo = LoadArmorDropAlbedo(itemId, grp);
        if (albedo == null)
            albedo = LoadItemIcon(grp);
        ApplyUrpLit(visual, albedo);
    }

    public void ApplyPropFallback(GameObject visual, Abstractgrp grp)
    {
        Texture2D icon = LoadItemIcon(grp);
        if (icon == null)
            icon = Resources.Load<Texture2D>(CoinTex0Resource);
        ApplyUrpLit(visual, icon);
    }
}
