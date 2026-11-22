using System.Collections.Generic;

/// <summary>
/// Maps missing FMOD/Resources ItemSound and MonSound stems to clips present under Resources.
/// </summary>
public static class InterludeSoundResourcesAlias
{
    static readonly Dictionary<string, string> StemToResourcesStem = new Dictionary<string, string>
    {
        { "sword_small_3", "armor_underwear_2" },
        { "sword_great_4", "armor_underwear_3" },
        { "public_sword_shing_8", "armor_underwear_4" },
        { "sword_small_1", "armor_underwear_1" },
        { "sword_small_2", "armor_underwear_2" },
        { "sword_small_7", "armor_underwear_3" },
        { "sword_great_1", "armor_underwear_1" },
        { "public_sword_shing_4", "armor_underwear_4" },
    };

    static readonly Dictionary<string, string> MonsterStemToResourcesStem = new Dictionary<string, string>
    {
        { "dryad/death", "fox/death" },
    };

    public static bool TryResolveItemResourcesPath(string resourcesPath, out string resolvedPath)
    {
        resolvedPath = resourcesPath;
        if (string.IsNullOrEmpty(resourcesPath))
            return false;

        string root = AudioPathResolver.ResourcesItemSoundRoot;
        if (!resourcesPath.StartsWith(root))
            return false;

        string stem = resourcesPath.Substring(root.Length);
        if (StemToResourcesStem.TryGetValue(stem, out string alias))
        {
            resolvedPath = root + alias;
            return true;
        }

        return false;
    }

    public static bool TryResolveMonsterResourcesPath(string resourcesPath, out string resolvedPath)
    {
        resolvedPath = resourcesPath;
        if (string.IsNullOrEmpty(resourcesPath))
            return false;

        string root = AudioPathResolver.ResourcesMonSoundRoot;
        if (!resourcesPath.StartsWith(root))
            return false;

        string tail = resourcesPath.Substring(root.Length);
        if (MonsterStemToResourcesStem.TryGetValue(tail, out string alias))
        {
            resolvedPath = root + alias;
            return true;
        }

        return false;
    }
}