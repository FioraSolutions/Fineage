using System.Collections.Generic;
using System.IO;

namespace Fineage.Client.Domain.Tests;

/// <summary>Loads the shipped English client catalog for domain tests.</summary>
static class L10nTestCatalog
{
    sealed class SilentLog : ILocalizationLog
    {
        public void Fallback(string key, string locale) { }
        public void Missing(string key) { }
    }

    static bool _loaded;

    public static void EnsureLoaded()
    {
        if (_loaded)
            return;
        string path = Path.GetFullPath(Path.Combine(
            AppContext.BaseDirectory,
            "..", "..", "..", "..", "..",
            "l2-unity", "Assets", "Resources", "Data", "l10n", "en", "strings.json"));
        if (!File.Exists(path))
            throw new FileNotFoundException("Client strings.json not found for tests.", path);
        var table = L10nStringsJson.Parse(File.ReadAllText(path));
        L10nRuntime.Catalog = LocalizationCatalog.FromTables(new SilentLog(), ("en", table));
        L10nRuntime.Tag = PlayerLocale.Default;
        _loaded = true;
    }
}