#if UNITY_5_3_OR_NEWER
using System;
using System.Collections.Generic;
using UnityEngine;

public sealed partial class LocalizationCatalog
{
    public static LocalizationCatalog LoadFromResources(ILocalizationLog log)
    {
        var tables = new Dictionary<string, Dictionary<string, string>>(StringComparer.Ordinal);
        TextAsset manifest = Resources.Load<TextAsset>("Data/l10n/locales");
        string[] locales = ParseLocaleManifest(manifest);
        foreach (string locale in locales)
        {
            if (!PlayerLocale.TryNormalize(locale, out string tag))
                continue;
            TextAsset strings = Resources.Load<TextAsset>($"Data/l10n/{tag}/strings");
            if (strings == null)
            {
                Debug.LogError($"event=l10n_missing key=locale-table locale={tag}");
                continue;
            }

            Dictionary<string, string> table;
            try
            {
                table = L10nStringsJson.Parse(strings.text);
            }
            catch (Exception ex)
            {
                Debug.LogError($"event=l10n_parse_error locale={tag} error={ex.Message}");
                table = new Dictionary<string, string>(StringComparer.Ordinal);
            }

            tables[tag] = table;
        }

        if (!tables.ContainsKey(PlayerLocale.Default))
            Debug.LogError($"event=l10n_missing key=locale-table locale={PlayerLocale.Default}");

        return new LocalizationCatalog(tables, log);
    }

    static string[] ParseLocaleManifest(TextAsset manifest)
    {
        if (manifest == null || string.IsNullOrWhiteSpace(manifest.text))
            return new[] { PlayerLocale.Default };

        string trimmed = manifest.text.Trim();
        if (trimmed.StartsWith("[", StringComparison.Ordinal))
        {
            var list = new List<string>();
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
                {
                    if (trimmed[i] == '\\')
                        i += 2;
                    else
                        i++;
                }

                if (i > start)
                    list.Add(trimmed.Substring(start, i - start));
                i++;
            }

            return list.Count > 0 ? list.ToArray() : new[] { PlayerLocale.Default };
        }

        return trimmed.Split(new[] { ',', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
    }
}
#endif