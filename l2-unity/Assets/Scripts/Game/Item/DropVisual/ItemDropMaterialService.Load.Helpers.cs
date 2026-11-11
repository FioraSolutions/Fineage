using UnityEngine;
using System;

public sealed partial class ItemDropMaterialService
{
    static Material LoadGrpMaterial(string grpName)
    {
        if (string.IsNullOrEmpty(grpName) ||
            grpName.Equals("None", System.StringComparison.OrdinalIgnoreCase))
            return null;

        string[] parts = grpName.Split('.');
        string pkg = parts.Length >= 2 ? parts[0] : string.Empty;
        string asset = parts.Length >= 2 ? parts[1] : parts[0];
        if (string.IsNullOrEmpty(asset))
            return null;

        // Same casing issue as armor equip: dat says mmagic.mmagic_*; disk is MMagic/MMagic_*.
        string canonPkg = CanonicalDropPackage(pkg);
        string canonAsset = asset;
        if (!string.IsNullOrEmpty(pkg) &&
            asset.StartsWith(pkg, System.StringComparison.OrdinalIgnoreCase))
            canonAsset = canonPkg + asset.Substring(pkg.Length);

        string[] pkgs = string.IsNullOrEmpty(pkg)
            ? new[] { string.Empty }
            : new[] { pkg, canonPkg };
        string[] assets = { asset, canonAsset };

        for (int p = 0; p < pkgs.Length; p++)
        {
            string folder = pkgs[p];
            if (string.IsNullOrEmpty(folder))
                continue;
            for (int a = 0; a < assets.Length; a++)
            {
                string leaf = assets[a];
                if (string.IsNullOrEmpty(leaf))
                    continue;
                Material mat = Resources.Load<Material>("Data/SysTextures/" + folder + "/Materials/" + leaf);
                if (mat != null)
                    return mat;
            }
        }

        for (int a = 0; a < assets.Length; a++)
        {
            string leaf = assets[a];
            if (string.IsNullOrEmpty(leaf))
                continue;
            Material dropTex = Resources.Load<Material>("Data/SysTextures/DropItemsTex/Materials/" + leaf);
            if (dropTex != null)
                return dropTex;
            Material magic = Resources.Load<Material>("Data/SysTextures/MMagic/Materials/" + leaf);
            if (magic != null)
                return magic;
            Material fighter = Resources.Load<Material>("Data/SysTextures/MFighter/Materials/" + leaf);
            if (fighter != null)
                return fighter;
        }

        return null;
    }

    static string CanonicalDropPackage(string folder)
    {
        if (string.IsNullOrEmpty(folder))
            return folder;
        switch (folder.ToLowerInvariant())
        {
            case "mmagic": return "MMagic";
            case "fmagic": return "FMagic";
            case "mfighter": return "MFighter";
            case "ffighter": return "FFighter";
            case "melf": return "MElf";
            case "felf": return "FElf";
            case "mdarkelf": return "MDarkElf";
            case "fdarkelf": return "FDarkElf";
            case "mdwarf": return "MDwarf";
            case "fdwarf": return "FDwarf";
            case "morc": return "MOrc";
            case "forc": return "FOrc";
            case "mshaman": return "MShaman";
            case "fshaman": return "FShaman";
            case "dropitemstex": return "DropItemsTex";
            case "lineagenpcstex": return "LineageNpcsTex";
            default:
                return char.ToUpperInvariant(folder[0]) + folder.Substring(1);
        }
    }

    static Texture2D LoadItemIcon(Abstractgrp grp)
    {
        if (grp == null || string.IsNullOrEmpty(grp.Icon))
            return null;

        string name = grp.Icon;
        int dot = name.LastIndexOf('.');
        if (dot >= 0 && dot < name.Length - 1)
            name = name.Substring(dot + 1);

        Texture2D tex = Resources.Load<Texture2D>("Data/SysTextures/Icon/" + name);
        if (tex != null)
            return tex;
        return Resources.Load<Texture2D>("Data/SysTextures/Icon/" + name + " 1");
    }
}
