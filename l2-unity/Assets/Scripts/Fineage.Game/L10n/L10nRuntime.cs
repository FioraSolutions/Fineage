/// <summary>Engine-free l10n state shared by <see cref="L10n"/> and <see cref="NetSystemLines"/>.</summary>
public static class L10nRuntime
{
    public static LocalizationCatalog Catalog = LocalizationCatalog.Empty(NullLog.Instance);

    public static string Tag { get; set; } = PlayerLocale.Default;

    public static string Get(string key) => Catalog.Get(key, Tag);

    sealed class NullLog : ILocalizationLog
    {
        public static readonly NullLog Instance = new NullLog();
        public void Fallback(string key, string locale) { }
        public void Missing(string key) { }
    }
}