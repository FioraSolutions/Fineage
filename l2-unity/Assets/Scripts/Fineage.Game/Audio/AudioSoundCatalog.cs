using System.Collections.Generic;

/// <summary>
/// Logical sound ids → exactly one backend + asset path (no FMOD↔Resources fallback chains).
/// </summary>
public static class AudioSoundCatalog
{
    public const string IdLoginIntro = "music/login_intro";
    public const string IdHitCritical = "sfx/hit_critical";
    public const string IdHitArmor = "sfx/hit_armor";

    static readonly Dictionary<string, AudioPlaySpec> Entries = Build();

    static Dictionary<string, AudioPlaySpec> Build()
    {
        var d = new Dictionary<string, AudioPlaySpec>();
        d[IdLoginIntro] = AudioPathResolver.SpecForFmodEvent(
            AudioMusicIds.LoginIntroFmodPath,
            AudioCategory.Music);
        d[IdHitCritical] = AudioPathResolver.SpecForFmodEvent(
            AudioPathResolver.FmodSkillSound("critical_hit"),
            AudioCategory.Sfx);
        d[IdHitArmor] = AudioPathResolver.SpecForFmodEvent(
            AudioPathResolver.FmodItemSound("armor_hit_underwear"),
            AudioCategory.Sfx);
        return d;
    }

    public static bool TryResolve(string soundId, out AudioPlaySpec spec)
    {
        if (string.IsNullOrEmpty(soundId))
        {
            spec = default;
            return false;
        }

        return Entries.TryGetValue(soundId, out spec);
    }

    public static string FormatUnknownIdError(string soundId) =>
        "AudioService: unknown catalog sound id=\"" + soundId + "\"";
}