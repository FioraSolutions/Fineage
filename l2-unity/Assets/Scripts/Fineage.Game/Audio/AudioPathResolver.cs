/// <summary>
/// Data-driven FMOD event paths and Resources clip paths — one pipeline, no per-sound switches.
/// </summary>
public static class AudioPathResolver
{
    public const string ResourcesSkillSoundRoot = "Data/Sounds/SkillSound/";
    public const string ResourcesSkillVoiceRoot = "Data/Sounds/SkillVoice/";
    public const string ResourcesItemSoundRoot = "Data/Sounds/ItemSound/";
    public const string ResourcesMusicRoot = "Data/Sounds/Music/";
    public const string ResourcesMonSoundRoot = "Data/Sounds/MonSound/";

    public static string FmodChrSound(CharacterRaceSound race, CharacterSoundEvent soundEvent) =>
        FmodChrSoundBank(race.ToString(), soundEvent);

    public static string FmodChrSoundBank(string bank, CharacterSoundEvent soundEvent) =>
        "event:/ChrSound/" + bank + "/" + bank + "_" + soundEvent;

    public static string FmodItemSound(string soundName) =>
        "event:/ItemSound/" + soundName;

    public static string FmodItemSound(ItemSoundEvent itemSoundEvent) =>
        FmodItemSound(itemSoundEvent.ToString());

    public static string FmodInterfaceSound(string soundName) =>
        "event:/InterfaceSound/" + soundName;

    public static string FmodStepSound(string eventKey) =>
        "event:/StepSound/" + eventKey;

    public static string FmodMonsterSound(string npcClassName, string eventKey) =>
        "event:/MonSound/" + NormalizeMonsterSoundFolder(npcClassName) + "/" + eventKey;

    /// <summary>Strip <c>LineageMonster.</c> and summon/sm suffixes for MonSound folder names.</summary>
    public static string NormalizeMonsterSoundFolder(string npcClassOrName)
    {
        if (string.IsNullOrEmpty(npcClassOrName))
            return npcClassOrName;

        string name = npcClassOrName.Trim();
        int dot = name.IndexOf('.');
        if (dot >= 0 && dot < name.Length - 1)
            name = name.Substring(dot + 1);

        name = name.ToLowerInvariant();
        if (name.EndsWith("_sm"))
            name = name.Substring(0, name.Length - 3);
        return name;
    }

    public static string FmodSkillSound(string clipStem) =>
        "event:/SkillSound/" + clipStem;

    public static string ResourcesSkillClip(string clipStem) =>
        ResourcesSkillSoundRoot + clipStem;

    public static string ResourcesVoiceClip(string clipStem) =>
        ResourcesSkillVoiceRoot + clipStem;

    public static string ResourcesMonClip(string npcFolder, string clipStem) =>
        ResourcesMonSoundRoot + npcFolder + "/" + clipStem;

    public static string StepEventKey(string surfaceTag, bool running)
    {
        if (!running)
            return "default_walk";

        string tag = string.IsNullOrEmpty(surfaceTag) ? "" : surfaceTag.ToLowerInvariant();
        switch (tag)
        {
            case "dirt":
            case "stone":
            case "wood":
            case "grass":
                return tag + "_run";
            default:
                return "default_run";
        }
    }

    /// <summary>
    /// Maps a Resources path under <c>Data/Sounds/</c> to category + backend (Resources-only pipeline).
    /// </summary>
    public static AudioPlaySpec SpecForResourcesPath(string resourcesPath)
    {
        if (string.IsNullOrEmpty(resourcesPath))
            return new AudioPlaySpec(AudioCategory.Sfx, AudioSourceKind.ResourcesClip, resourcesPath);

        if (resourcesPath.StartsWith(ResourcesSkillVoiceRoot))
            return new AudioPlaySpec(AudioCategory.Voice, AudioSourceKind.ResourcesClip, resourcesPath);

        if (resourcesPath.StartsWith(ResourcesMusicRoot))
            return new AudioPlaySpec(AudioCategory.Music, AudioSourceKind.ResourcesClip, resourcesPath);

        if (resourcesPath.StartsWith(ResourcesSkillSoundRoot))
            return new AudioPlaySpec(AudioCategory.Sfx, AudioSourceKind.ResourcesClip, resourcesPath);

        if (resourcesPath.StartsWith(ResourcesItemSoundRoot))
            return new AudioPlaySpec(AudioCategory.Sfx, AudioSourceKind.ResourcesClip, resourcesPath);

        if (resourcesPath.StartsWith(ResourcesMonSoundRoot))
            return new AudioPlaySpec(AudioCategory.Sfx, AudioSourceKind.ResourcesClip, resourcesPath);

        return new AudioPlaySpec(AudioCategory.Sfx, AudioSourceKind.ResourcesClip, resourcesPath);
    }

    public static AudioPlaySpec SpecForFmodEvent(string eventPath, AudioCategory category) =>
        new AudioPlaySpec(category, AudioSourceKind.FmodStudioEvent, eventPath);
}