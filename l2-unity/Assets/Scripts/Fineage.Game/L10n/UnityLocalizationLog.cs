using UnityEngine;

public sealed class UnityLocalizationLog : ILocalizationLog
{
    public static readonly UnityLocalizationLog Instance = new UnityLocalizationLog();

    public void Fallback(string key, string locale)
    {
        Debug.LogWarning($"event=l10n_fallback key={key} locale={locale} fallback={PlayerLocale.Default}");
    }

    public void Missing(string key)
    {
        Debug.LogError($"event=l10n_missing key={key ?? ""} locale={PlayerLocale.Default}");
    }
}