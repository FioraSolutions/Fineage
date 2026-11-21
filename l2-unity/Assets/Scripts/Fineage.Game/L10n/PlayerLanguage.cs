using UnityEngine;

/// <summary>Account UI language, persisted locally and sent on Auth / EnterWorld.</summary>
public static class PlayerLanguage
{
    const string PrefKey = "opt.locale";

    public static string Tag { get; private set; } = PlayerLocale.Default;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void LoadSaved() => LoadSavedForRuntime();

    internal static void LoadSavedForRuntime()
    {
        Tag = PlayerLocale.Normalize(PlayerPrefs.GetString(PrefKey, PlayerLocale.Default));
        L10nRuntime.Tag = Tag;
    }

    public static void SetTag(string raw)
    {
        string next = PlayerLocale.Normalize(raw);
        if (Tag == next)
            return;
        Tag = next;
        L10nRuntime.Tag = Tag;
        PlayerPrefs.SetString(PrefKey, Tag);
        PlayerPrefs.Save();
        L10n.NotifyLocaleChanged();
    }

    public static string[] AvailableTags => L10n.AvailableLocales;
}