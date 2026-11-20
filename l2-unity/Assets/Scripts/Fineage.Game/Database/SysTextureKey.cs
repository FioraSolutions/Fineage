/// <summary>
/// Split an npcgrp/armor texture token <c>package.asset</c> or
/// <c>package.group.asset</c> into the SysTextures folder and the leaf asset name.
/// Interlude is usually two parts; Classic rows sometimes insert a group.
/// Always use the last segment as the file stem — never the middle group.
/// </summary>
public static class SysTextureKey
{
    public static bool TrySplit(string texture, out string package, out string asset)
    {
        package = null;
        asset = null;
        if (string.IsNullOrEmpty(texture))
            return false;
        int first = texture.IndexOf('.');
        if (first <= 0 || first == texture.Length - 1)
            return false;
        package = texture.Substring(0, first);
        int last = texture.LastIndexOf('.');
        asset = texture.Substring(last + 1);
        return !string.IsNullOrEmpty(asset);
    }
}
