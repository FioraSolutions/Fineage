using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>Static entry for UI and gameplay strings on the client.</summary>
public static class L10n
{
    static LocalizationCatalog _catalog;

    public static event Action LocaleChanged;

    public static string[] AvailableLocales { get; private set; } = { PlayerLocale.Default };

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Boot()
    {
        PlayerLanguage.LoadSavedForRuntime();
        AvailableLocales = ParseLocaleList(Resources.Load<TextAsset>("Data/l10n/locales"));
        _catalog = LocalizationCatalog.LoadFromResources(UnityLocalizationLog.Instance);
        L10nRuntime.Catalog = _catalog;
        L10nRuntime.Tag = PlayerLanguage.Tag;
    }

    static string[] ParseLocaleList(TextAsset manifest)
    {
        if (manifest == null || string.IsNullOrWhiteSpace(manifest.text))
            return new[] { PlayerLocale.Default };
        string trimmed = manifest.text.Trim();
        if (!trimmed.StartsWith("[", StringComparison.Ordinal))
            return new[] { PlayerLocale.Default };

        var locales = new List<string>();
        int i = 1;
        while (i < trimmed.Length)
        {
            while (i < trimmed.Length && trimmed[i] != '"' && trimmed[i] != ']')
                i++;
            if (i >= trimmed.Length || trimmed[i] == ']')
                break;
            i++;
            int start = i;
            while (i < trimmed.Length && trimmed[i] != '"')
                i++;
            if (i > start)
                locales.Add(trimmed.Substring(start, i - start));
            i++;
        }

        return locales.Count > 0 ? locales.ToArray() : new[] { PlayerLocale.Default };
    }

    static LocalizationCatalog Catalog
    {
        get
        {
            if (_catalog == null)
            {
                _catalog = LocalizationCatalog.LoadFromResources(UnityLocalizationLog.Instance);
                L10nRuntime.Catalog = _catalog;
            }

            return _catalog;
        }
    }

    public static string Get(string key)
    {
        L10nRuntime.Tag = PlayerLanguage.Tag;
        return Catalog.Get(key, PlayerLanguage.Tag);
    }

    public static string Format(string key, params object[] args) =>
        Catalog.Format(key, PlayerLanguage.Tag, args);

    public static string Resolve(string key, string englishFallback) =>
        Catalog.Resolve(key, PlayerLanguage.Tag, englishFallback);

    internal static void NotifyLocaleChanged() => LocaleChanged?.Invoke();
}