using System;
using System.IO;
using NUnit.Framework;
using UnityEngine;

public sealed class LobbyInterludeParityEditModeTests
{
    const string CharSelectUxml =
        "Assets/Resources/Data/UI/_Elements/Login/CharSelectWindow.uxml";
    const string LobbyT3d =
        "Assets/Resources/Data/Maps/l2_lobby/Meta/l2_lobby.t3d";

    [Test]
    public void Account_environment_profile_loads_from_resources()
    {
        var profile = LobbyAccountRenderSettings.LoadProfile();
        Assert.IsNotNull(profile, "LobbyAccountEnvironmentProfile must live under Resources/Settings");
        Assert.Greater(profile.ambientIntensity, 0.5f);
        Assert.AreEqual("Data/Effects/skybox/L2Sky", profile.skyboxResourcePath);
    }

    [Test]
    public void Char_select_uxml_uses_interlude_skin_not_df_chronicle()
    {
        string text = File.ReadAllText(CharSelectUxml);
        StringAssert.Contains("il-char-select", text);
        StringAssert.Contains("Status/Interlude/Status_Interlude_Ps_HpBar", text);
        Assert.IsFalse(text.Contains("Gauge_DF_Large"), "Char select must not reference Gracia DF gauges");
        Assert.IsFalse(text.Contains("Windows_DF"), "Char select chrome belongs in USS under il-char-select");
    }

    const string StyleSheet =
        "Assets/Resources/Data/UI/_Elements/L2StyleSheet.uss";

    [Test]
    public void Char_select_stylesheet_scopes_bar_labels_and_row_height()
    {
        string uss = File.ReadAllText(StyleSheet);
        StringAssert.Contains(".il-char-select .char-select-row", uss);
        StringAssert.Contains("height: auto", uss);
        StringAssert.Contains(".il-char-select .bar-label-numbers", uss);
        StringAssert.Contains("letter-spacing: 0", uss);
        Assert.IsFalse(
            uss.Contains(".bar-label-numbers {\n}"),
            "Empty global .bar-label-numbers blocks reset HUD gauge label layout");
    }

    [Test]
    public void Char_select_detail_has_one_l10n_row_per_stat_not_duplicate_gauge_names()
    {
        string uxml = File.ReadAllText(CharSelectUxml);
        Assert.AreEqual(1, CountOccurrences(uxml, "ui.char_select.mp"));
        Assert.AreEqual(1, CountOccurrences(uxml, "ui.char_select.exp"));
        Assert.AreEqual(1, CountOccurrences(uxml, "ui.char_select.hp"));
    }

    static int CountOccurrences(string text, string needle)
    {
        int count = 0;
        int index = 0;
        while ((index = text.IndexOf(needle, index, StringComparison.Ordinal)) >= 0)
        {
            count++;
            index += needle.Length;
        }

        return count;
    }

    [Test]
    public void Lobby_map_contract_points_at_interlude_lobby_unr_pipeline()
    {
        Assert.AreEqual("l2_lobby", LobbyMapInterludeContract.AdditiveLobbySceneName);
        Assert.AreEqual("Lobby.unr", LobbyMapInterludeContract.RetailMapName);
        string t3d = File.ReadAllText(LobbyT3d);
        StringAssert.Contains("Class=Camera", t3d);
        StringAssert.Contains("Camera25", t3d);
    }
}