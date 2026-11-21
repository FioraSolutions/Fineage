using NUnit.Framework;
using UnityEngine;
using UnityEngine.Rendering;

public sealed class LoginLobbyPresentationEditModeTests
{
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
        RenderSettings.ambientIntensity = 0f;
        RenderSettings.ambientMode = AmbientMode.Flat;
        LobbyAccountRenderSettings.Apply();
        Assert.AreEqual(AmbientMode.Skybox, RenderSettings.ambientMode);
        Assert.AreEqual(0.6f, RenderSettings.ambientIntensity, 0.001f);
        Assert.IsNotNull(RenderSettings.skybox);
        LobbyAccountRenderSettings.Release();
    }
}