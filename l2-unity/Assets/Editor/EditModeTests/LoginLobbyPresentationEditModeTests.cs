using NUnit.Framework;
using UnityEngine;
using UnityEngine.Rendering;

public sealed class LoginLobbyPresentationEditModeTests
{
    [TearDown]
    public void TearDown()
    {
        LobbyAccountRenderSettings.Release();
    }

    [Test]
    public void CharSelect_is_account_lobby_camera()
    {
        var go = new GameObject("CharSelect");
        var cam = go.AddComponent<Camera>();
        Assert.IsTrue(LoginLobbyPresentation.IsAccountLobbyCamera(cam));
        Object.DestroyImmediate(go);
    }

    [Test]
    public void Race_child_camera_is_account_lobby()
    {
        var human = new GameObject("Human");
        var male = new GameObject("Male");
        male.transform.SetParent(human.transform);
        var cam = male.AddComponent<Camera>();
        Assert.IsTrue(LoginLobbyPresentation.IsAccountLobbyCamera(cam));
        Object.DestroyImmediate(human);
    }

    [Test]
    public void Account_lobby_applies_render_settings()
    {
        LobbyAccountRenderSettings.Release();
        RenderSettings.ambientIntensity = 0f;
        RenderSettings.ambientMode = AmbientMode.Flat;
        var profile = LobbyAccountRenderSettings.LoadProfile();
        Assert.IsNotNull(profile);
        LobbyAccountRenderSettings.Apply();
        Assert.IsTrue(LobbyAccountRenderSettings.IsApplied);
        Assert.AreEqual(profile.ambientMode, LobbyAccountRenderSettings.LastAppliedAmbientModeForTests);
        Assert.AreEqual(profile.ambientIntensity, RenderSettings.ambientIntensity, 0.001f);
        Assert.IsNotNull(RenderSettings.skybox);
        LobbyAccountRenderSettings.Release();
    }

    [Test]
    public void Account_profile_exposes_actor_world_hours_for_lut()
    {
        var profile = LobbyAccountRenderSettings.LoadProfile();
        Assert.IsNotNull(profile);
        Assert.GreaterOrEqual(profile.actorWorldHours, 0f);
        Assert.Less(profile.actorWorldHours, 24f);
        Assert.Greater(profile.skyExposure, 0.5f);
    }
}