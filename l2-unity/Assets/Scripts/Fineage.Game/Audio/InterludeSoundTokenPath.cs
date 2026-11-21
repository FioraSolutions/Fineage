/// <summary>
/// Maps Interlude datapack tokens (ItemSound.foo, MonSound.bar, ChrSound.*) to FMOD event paths.
/// </summary>
public static class InterludeSoundTokenPath
{
    public const string ItemSoundPrefix = "event:/ItemSound/";
    public const string MonSoundPrefix = "event:/MonSound/";
    public const string SkillSoundPrefix = "event:/SkillSound/";
    public const string InterfaceSoundPrefix = "event:/InterfaceSound/";

    public static bool TryFromDatToken(string token, out string fmodEventPath)
    {
        fmodEventPath = null;
        if (string.IsNullOrWhiteSpace(token))
            return false;

        string cleaned = DatUtils.CleanupString(token);
        if (string.IsNullOrEmpty(cleaned))
            return false;

        int dot = cleaned.IndexOf('.');
        if (dot <= 0 || dot >= cleaned.Length - 1)
            return false;

        string bank = cleaned.Substring(0, dot);
        string stem = cleaned.Substring(dot + 1);
        if (string.IsNullOrEmpty(stem))
            return false;

        switch (bank.ToLowerInvariant())
        {
            case "itemsound":
                fmodEventPath = ItemSoundPrefix + stem;
                return true;
            case "monsound":
                fmodEventPath = MonSoundPrefix + stem;
                return true;
            case "skillsound":
                fmodEventPath = SkillSoundPrefix + stem;
                return true;
            case "interfacesound":
                fmodEventPath = InterfaceSoundPrefix + stem;
                return false;
            default:
                return false;
        }
    }

    public static bool IsUiOrInterfaceEvent(string fmodEventPath)
    {
        if (string.IsNullOrEmpty(fmodEventPath))
            return true;

        string lower = fmodEventPath.ToLowerInvariant();
        return lower.StartsWith(InterfaceSoundPrefix.ToLowerInvariant())
            || lower.Contains("/interface")
            || lower.Contains("/ui/");
    }

    public static bool IsCombatSfxEvent(string fmodEventPath)
    {
        if (string.IsNullOrEmpty(fmodEventPath))
            return false;
        if (IsUiOrInterfaceEvent(fmodEventPath))
            return false;

        string lower = fmodEventPath.ToLowerInvariant();
        return lower.StartsWith("event:/itemsound/")
            || lower.StartsWith("event:/monsound/")
            || lower.StartsWith("event:/skillsound/")
            || lower.StartsWith("event:/chrsound/");
    }

    public static string ResourcesPathFromFmodEvent(string fmodEventPath)
    {
        if (string.IsNullOrEmpty(fmodEventPath) || !fmodEventPath.StartsWith("event:/"))
            return null;

        string tail = fmodEventPath.Substring("event:/".Length);
        int slash = tail.IndexOf('/');
        if (slash <= 0)
            return null;

        string bank = tail.Substring(0, slash);
        string stem = tail.Substring(slash + 1);
        if (string.IsNullOrEmpty(stem))
            return null;

        switch (bank.ToLowerInvariant())
        {
            case "itemsound":
                return AudioPathResolver.ResourcesItemSoundRoot + stem;
            case "monsound":
                return AudioPathResolver.ResourcesMonSoundRoot + stem;
            case "skillsound":
                return AudioPathResolver.ResourcesSkillClip(stem);
            default:
                return null;
        }
    }
}