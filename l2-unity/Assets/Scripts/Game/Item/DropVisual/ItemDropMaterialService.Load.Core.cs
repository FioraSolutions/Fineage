using UnityEngine;
using System.Collections.Generic;
using System;

public sealed partial class ItemDropMaterialService
{
    static Texture2D LoadGrpAlbedo(string grpName)
    {
        if (string.IsNullOrEmpty(grpName) ||
            grpName.Equals("None", System.StringComparison.OrdinalIgnoreCase))
            return null;

        string[] parts = grpName.Split('.');
        string pkg = parts.Length >= 2 ? parts[0] : string.Empty;
        string asset = parts.Length >= 2 ? parts[1] : parts[0];
        if (string.IsNullOrEmpty(asset))
            return null;

        string[] folders =
        {
            string.IsNullOrEmpty(pkg) ? null : "Data/SysTextures/" + pkg + "/",
            "Data/SysTextures/DropItemsTex/",
            "Data/SysTextures/MMagic/",
            "Data/SysTextures/MFighter/"
        };

        string[] names = BuildAlbedoNames(asset);
        for (int i = 0; i < folders.Length; i++)
        {
            if (string.IsNullOrEmpty(folders[i]))
                continue;
            for (int n = 0; n < names.Length; n++)
            {
                Texture2D tex = Resources.Load<Texture2D>(folders[i] + names[n]);
                if (tex != null)
                    return tex;
            }
        }

        return null;
    }

    static string[] BuildAlbedoNames(string asset)
    {
        var names = new System.Collections.Generic.List<string>(6);
        names.Add(asset);
        names.Add(asset + "_sh");
        names.Add(asset + "_sp");
        names.Add(asset + "_ori");
        if (asset.EndsWith("_t00", System.StringComparison.OrdinalIgnoreCase))
        {
            string stem = asset.Substring(0, asset.Length - 4);
            names.Add(stem);
            names.Add(stem + "_sh");
            names.Add(stem + "_ori");
            names.Add(stem + "_sp");
        }
        return names.ToArray();
    }
}
