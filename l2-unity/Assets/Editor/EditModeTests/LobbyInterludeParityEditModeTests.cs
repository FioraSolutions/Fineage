using System.IO;
using NUnit.Framework;
using UnityEngine;

public sealed class LobbyInterludeParityEditModeTests
{
    const string CharSelectUxml =
        "Assets/Resources/Data/UI/_Elements/Login/CharSelectWindow.uxml";
    const string LoginForestMeta =
        "Assets/Resources/Data/Maps/l2_lobby/Meta/LoginForestActors.json";

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

    [Test]
    public void Lobby_map_contract_points_at_interlude_lobby_unr_pipeline()
    {
        Assert.AreEqual("l2_lobby", LobbyMapInterludeContract.AdditiveLobbySceneName);
        Assert.AreEqual("Lobby.unr", LobbyMapInterludeContract.RetailMapName);
        string meta = File.ReadAllText(LoginForestMeta);
        StringAssert.Contains("Lobby.unr", meta);
    }
}