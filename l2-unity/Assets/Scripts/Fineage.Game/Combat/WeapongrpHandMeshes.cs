using System;
using System.Collections.Generic;

public static class WeapongrpHandMeshes
{
    public static void ApplyClassicMeshes(Weapongrp grp, string wpMeshValue, string textureValue)
    {
        if (!string.IsNullOrEmpty(wpMeshValue))
            ApplyMeshFields(grp, ParseClassicMeshList(wpMeshValue), null);
        if (!string.IsNullOrEmpty(textureValue))
            ApplyMeshFields(grp, null, ParseClassicMeshList(textureValue));
    }

    public static void ApplyInterludeMeshes(Weapongrp grp, string mesh0, string mesh1, string tex0, string tex1)
    {
        var meshes = new List<string>();
        if (!string.IsNullOrEmpty(mesh0))
            meshes.Add(NormalizeKey(mesh0));
        if (!string.IsNullOrEmpty(mesh1))
            meshes.Add(NormalizeKey(mesh1));

        var textures = new List<string>();
        if (!string.IsNullOrEmpty(tex0))
            textures.Add(NormalizeKey(tex0));
        if (!string.IsNullOrEmpty(tex1))
            textures.Add(NormalizeKey(tex1));

        ApplyMeshFields(grp, meshes, textures);
    }

    static void ApplyMeshFields(Weapongrp grp, IList<string> meshes, IList<string> textures)
    {
        if (meshes != null && meshes.Count > 0)
            grp.Model = meshes[0];
        if (meshes != null && meshes.Count > 1)
            grp.ModelLeft = meshes[1];
        if (textures != null && textures.Count > 0)
            grp.Texture = textures[0];
        if (textures != null && textures.Count > 1)
            grp.TextureLeft = textures[1];
    }

    public static IList<string> ParseClassicMeshList(string value)
    {
        var result = new List<string>();
        if (string.IsNullOrEmpty(value))
            return result;

        List<List<string>> multi = DatUtils.ParseMultiArray(value);
        if (multi == null || multi.Count == 0)
            return result;

        foreach (string entry in multi[0])
        {
            string key = NormalizeKey(entry);
            if (!string.IsNullOrEmpty(key))
                result.Add(key);
        }

        return result;
    }

    public static string ModelForHand(Weapongrp grp, bool leftHand)
    {
        if (grp == null)
            return null;
        if (leftHand && !string.IsNullOrEmpty(grp.ModelLeft))
            return grp.ModelLeft;
        return grp.Model;
    }

    public static bool ShouldMirrorLeftHand(Weapongrp grp)
    {
        if (grp == null || string.IsNullOrEmpty(grp.Model))
            return false;
        string left = ModelForHand(grp, leftHand: true);
        return string.Equals(left, grp.Model, StringComparison.Ordinal);
    }

    public static IEnumerable<string> EnumerateModelKeys(Weapongrp grp)
    {
        if (grp == null)
            yield break;
        if (!string.IsNullOrEmpty(grp.Model))
            yield return grp.Model;
        if (!string.IsNullOrEmpty(grp.ModelLeft) && !string.Equals(grp.ModelLeft, grp.Model, StringComparison.Ordinal))
            yield return grp.ModelLeft;
    }

    static string NormalizeKey(string raw)
    {
        if (string.IsNullOrEmpty(raw))
            return null;
        string trimmed = raw.Trim();
        if (trimmed.Length == 0)
            return null;
        if (trimmed.IndexOf('[') >= 0)
            return DatUtils.CleanupString(trimmed);
        return trimmed;
    }
}