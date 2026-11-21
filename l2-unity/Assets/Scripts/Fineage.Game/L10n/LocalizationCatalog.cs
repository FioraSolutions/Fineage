using System;
using System.Collections.Generic;
using System.Globalization;

/// <summary>
/// Client string catalog: same lookup rules as the backend
/// <c>Fineage.Game.L10n.LocalizationCatalog</c>.
/// </summary>
public sealed partial class LocalizationCatalog
{
    readonly Dictionary<string, Dictionary<string, string>> _tables;
    readonly ILocalizationLog _log;

    LocalizationCatalog(Dictionary<string, Dictionary<string, string>> tables, ILocalizationLog log)
    {
        _tables = tables;
        _log = log ?? throw new ArgumentNullException(nameof(log));
    }

    public static LocalizationCatalog Empty(ILocalizationLog log) =>
        new LocalizationCatalog(new Dictionary<string, Dictionary<string, string>>(StringComparer.Ordinal), log);

    public static LocalizationCatalog FromTables(
        ILocalizationLog log,
        params (string Locale, Dictionary<string, string> Entries)[] tables)
    {
        var built = new Dictionary<string, Dictionary<string, string>>(StringComparer.Ordinal);
        foreach ((string locale, Dictionary<string, string> entries) in tables)
        {
            if (!PlayerLocale.TryNormalize(locale, out string tag))
                throw new ArgumentException($"'{locale}' is not a locale tag.", nameof(tables));
            var copy = new Dictionary<string, string>(StringComparer.Ordinal);
            foreach (var pair in entries)
                copy[pair.Key] = pair.Value ?? "";
            built[tag] = copy;
        }

        return new LocalizationCatalog(built, log);
    }

    public bool TryExact(string locale, string key, out string value)
    {
        value = "";
        if (string.IsNullOrEmpty(key) || !PlayerLocale.TryNormalize(locale, out string tag))
            return false;
        return _tables.TryGetValue(tag, out Dictionary<string, string> table)
               && table.TryGetValue(key, out value);
    }

    public string Get(string key, string locale) => Resolve(key, locale, english: null);

    public string Resolve(string key, string locale, string english)
    {
        if (string.IsNullOrEmpty(key))
        {
            _log.Missing(key ?? "");
            return english ?? "";
        }

        string tag = PlayerLocale.Normalize(locale);
        if (TryExact(tag, key, out string hit))
            return hit;

        if (!PlayerLocale.IsDefault(tag))
        {
            _log.Fallback(key, tag);
            if (TryExact(PlayerLocale.Default, key, out string fallback))
                return fallback;
            if (!string.IsNullOrEmpty(english))
                return english;
        }
        else if (!string.IsNullOrEmpty(english))
        {
            return english;
        }

        _log.Missing(key);
        return key;
    }

    public string Format(string key, string locale, params object[] args)
    {
        string template = Get(key, locale);
        if (args == null || args.Length == 0)
            return template;
        return string.Format(CultureInfo.InvariantCulture, template, args);
    }
}