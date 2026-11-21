using System.Collections.Generic;
using NUnit.Framework;

public sealed class LocalizationCatalogEditModeTests
{
    sealed class TestLog : ILocalizationLog
    {
        public int Fallbacks;
        public int MissingCount;

        public void Fallback(string key, string locale) => Fallbacks++;
        public void Missing(string key) => MissingCount++;
    }

  [Test]
  public void Get_returns_english_row()
    {
        var log = new TestLog();
        var catalog = LocalizationCatalog.FromTables(
            log,
            ("en", new Dictionary<string, string> { { "ui.login.log_in", "Log In" } }));
        Assert.AreEqual("Log In", catalog.Get("ui.login.log_in", "en"));
        Assert.AreEqual(0, log.MissingCount);
    }

    [Test]
  public void Get_logs_fallback_then_english_for_other_locale()
    {
        var log = new TestLog();
        var catalog = LocalizationCatalog.FromTables(
            log,
            ("en", new Dictionary<string, string> { { "chat.welcome", "Welcome, {0}." } }));
        Assert.AreEqual("Welcome, {0}.", catalog.Get("chat.welcome", "ru"));
        Assert.AreEqual(1, log.Fallbacks);
        Assert.AreEqual(0, log.MissingCount);
    }

    [Test]
  public void Get_logs_missing_and_returns_key()
    {
        var log = new TestLog();
        var catalog = LocalizationCatalog.Empty(log);
        Assert.AreEqual("ui.missing.key", catalog.Get("ui.missing.key", "en"));
        Assert.AreEqual(1, log.MissingCount);
    }

    [Test]
  public void Format_replaces_placeholders()
    {
        var log = new TestLog();
        var catalog = LocalizationCatalog.FromTables(
            log,
            ("en", new Dictionary<string, string> { { "combat.hit_target", "You hit {0} for {1} damage." } }));
        Assert.AreEqual(
            "You hit Goblin for 12 damage.",
            catalog.Format("combat.hit_target", "en", "Goblin", 12));
    }

    [Test]
  public void PlayerLocale_normalizes_invalid_to_en()
    {
        Assert.AreEqual("en", PlayerLocale.Normalize(null));
        Assert.AreEqual("en-us", PlayerLocale.Normalize("EN-US"));
        Assert.True(PlayerLocale.TryNormalize("pt-br", out string tag));
        Assert.AreEqual("pt-br", tag);
    }
}